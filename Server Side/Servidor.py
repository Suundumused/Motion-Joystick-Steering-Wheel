import socket
import asyncio
import os
import sys
import ctypes
import threading
import platform
import time
import subprocess

import pystray

from msgpack import unpackb
from customtkinter import CTkLabel
from plyer import notification
from PIL import Image

from IO.Localization import texts
from UMG.UserInterface import tkinter_thread, NewWindow
from Utils.Controller import Smooth_Operator


if getattr(sys, 'frozen', False):
    # Executando como executable (PyInstaller)
    path = os.path.dirname(sys.executable)
else:
    # Executando como  script .py
    path = os.path.dirname(os.path.abspath(sys.argv[0]))

def install_drivers():
    MB_ICONERROR = 0x10
    MB_OK = 0x0
    
    bits = platform.architecture()[0]
    
    config_name = 'Driver'
    try:        
        if bits == '32bit':
            subprocess.run(
                [
                    "powershell",
                    "-Command",
                    f'Start-Process "msiexec.exe" -ArgumentList \'/i "{os.path.join(path, config_name, "DriverX86", "ViGEmBusSetup_x86.msi")}"\' -Verb RunAs -Wait'
                ],  
                check=True
            )
        elif bits == '64bit':
            subprocess.run(
                [
                    "powershell",
                    "-Command",
                    f'Start-Process "msiexec.exe" -ArgumentList \'/i "{os.path.join(path, config_name, "DriverX64", "ViGEmBusSetup_x64.msi")}"\' -Verb RunAs -Wait'
                ],
                check=True
            )
        else:
            ctypes.windll.user32.MessageBoxW(0, texts['arch'], texts['error'], MB_ICONERROR | MB_OK)
            
    except Exception as e:
        ctypes.windll.user32.MessageBoxW(0, str(e), texts['error'], MB_ICONERROR | MB_OK)
try:
    import vgamepad as vg
except:
    install_drivers()
    import vgamepad as vg
    
    
button_map = {
    '0': vg.XUSB_BUTTON.XUSB_GAMEPAD_LEFT_THUMB,
    '1': vg.XUSB_BUTTON.XUSB_GAMEPAD_RIGHT_THUMB,
    '11': vg.XUSB_BUTTON.XUSB_GAMEPAD_BACK,
    '12': vg.XUSB_BUTTON.XUSB_GAMEPAD_START,
    '13': vg.XUSB_BUTTON.XUSB_GAMEPAD_X,
    '14': vg.XUSB_BUTTON.XUSB_GAMEPAD_B,
    '15': vg.XUSB_BUTTON.XUSB_GAMEPAD_Y,
    '16': vg.XUSB_BUTTON.XUSB_GAMEPAD_A,
    '2': vg.XUSB_BUTTON.XUSB_GAMEPAD_RIGHT_SHOULDER,
    '3': vg.XUSB_BUTTON.XUSB_GAMEPAD_LEFT_SHOULDER,
    '4': vg.XUSB_BUTTON.XUSB_GAMEPAD_DPAD_LEFT,
    '5': vg.XUSB_BUTTON.XUSB_GAMEPAD_DPAD_RIGHT,
    '6': vg.XUSB_BUTTON.XUSB_GAMEPAD_DPAD_UP,
    '7': vg.XUSB_BUTTON.XUSB_GAMEPAD_DPAD_DOWN
}

class Variables:
    def __init__(self):
        self.once = True
        self.StartStop = True
        self.Sensibility = 1.0
        self.ServerIP = ""
        self.ServerPort = ""
        self.start_time = time.time()
        self.valor3 = 0.0
        
        self.main_UI = None
        self.client_manager = UDPClientManager(self) # Changed to UDPClientManager
                             
        self.accslider = None
        self.fov = False
        
        self.alpha = 0.875 #smooth rate 
        
        self.MasterIP = ""
        
        self.Socket = None
        self.UI = None
        self.Overlays = None
        self.ResCallSave = False
        
        self.data = {
            "Smooth_Input": 1.0,
            "Sensibility": 1.0,
            "Start_Hidden": 0,
            "Show_Overlay": 0,
            "ServerIP": "0.0.0.x",
            "ServerPort": "3470"
        }

    def sair(self):
        self.StartStop = False
        os._exit(0)
        
    def boolToFloat(self, valor):
        return 1.0 if valor else 0.0
        
    def acceleratorCor(self, valor):
        return max(0.0, valor)


# --- NEW: UDP Client Manager ---
class UDPClientManager:
    def __init__(self, Variaveis: Variables):
        self.clients = {}  # {client_id: {'gamepad': gamepad, 'ip': ip, 'last_seen': timestamp, 'joy_id_text': CTkLabel, 'smooth_handler': Smooth_Operator, 'smooth_thread': Thread}}
        self._lock = threading.Lock()
        
        self.Variaveis = Variaveis
        self.CLIENT_TIMEOUT = 5 # seconds without a packet until considered disconnected

    def add_or_update_client(self, client_id, ip):
        with self._lock:
            if client_id not in self.clients:
                try:
                    gamepad = vg.VX360Gamepad()
                    smooth_handler = Smooth_Operator(self.Variaveis, gamepad)
                    smooth_thread = threading.Thread(target=smooth_handler.smooth_Thread)
                    smooth_thread.daemon = True
                    smooth_thread.start()

                    client_text = CTkLabel(master=self.Variaveis.main_UI.clientes_scroll, text=f"Joystick {len(self.clients)} - {client_id}", font = ("Roboto", 14))
                    client_text.pack(pady=1, padx=(6,1))
                    
                    self.clients[client_id] = {
                        'gamepad': gamepad,
                        'ip': ip,
                        'last_seen': time.time(),
                        'joy_id_text': client_text,
                        'smooth_handler': smooth_handler,
                        'smooth_thread': smooth_thread,
                        'button_states': {
                                '0': False,  # LEFT_THUMB
                                '1': False,  # RIGHT_THUMB
                                '11': False, # BACK
                                '12': False, # START
                                '13': False, # X
                                '14': False, # B
                                '15': False, # Y
                                '16': False, # A
                                '2': False,  # RIGHT_SHOULDER
                                '3': False,  # LEFT_SHOULDER
                                '4': False,  # DPAD_LEFT
                                '5': False,  # DPAD_RIGHT
                                '6': False,  # DPAD_UP
                                '7': False   # DPAD_DOWN
                            }
                        }
                    notification.notify(
                        texts['connected'],
                        f"{texts['con_made']} {client_id}",
                        timeout=15,
                    )
                    os.system('cls')
                    print(f"{texts['listening_on']} {self.Variaveis.ServerIP}:{self.Variaveis.ServerPort}")
                    print(f"\n{texts['acept_con']} {client_id}\n{texts['con_running']}")

                except Exception as e:
                    print(f"{texts['gamepad_fail']} {client_id}: {e}")
                    notification.notify(
                        texts['gamepad_fail_title'],
                        f"{texts['gamepad_fail_desc']} {client_id}: {e}",
                        timeout=15,
                    )
                    return False
            else:
                self.clients[client_id]['last_seen'] = time.time() # Update last seen time
            return True

    def remove_client(self, client_id):
        with self._lock:
            if client_id in self.clients:
                client_info = self.clients[client_id]
                
                client_info['joy_id_text'].pack_forget()
                
                client_info['smooth_handler'].active = False
                if client_info['smooth_thread'].is_alive():
                    client_info['smooth_thread'].join(timeout=1) # Wait for thread to finish
                
                del client_info['gamepad']
                del self.clients[client_id]
                
                notification.notify(
                    texts['disconnected'],
                    f"{texts['disconnected_dedc']} {client_id}",
                    timeout=15,
                )
                os.system('cls')
                print(f"{texts['listening_on']} {self.Variaveis.ServerIP}:{self.Variaveis.ServerPort}")
                print(f"\n{texts['disconnected']}. {texts['work']}")

    def get_connected_ips(self):
        with self._lock:
            return [client['ip'] for client in self.clients.values()]

    def get_client_count(self):
        with self._lock:
            return len(self.clients)
            
    async def cleanup_inactive_clients(self):
        while self.Variaveis.StartStop:
            await asyncio.sleep(1) # Check every 1 seconds
            
            clients_to_remove = []
            
            with self._lock:
                for client_id, client_info in self.clients.items():
                    if time.time() - client_info['last_seen'] > self.CLIENT_TIMEOUT:
                        clients_to_remove.append(client_id)
                        
            for client_id in clients_to_remove:
                print(f"{texts['client']} {client_id} {texts['server_to']}")
                self.remove_client(client_id)


# --- NEW: UDP Protocol Class ---
class UDPProtocol(asyncio.DatagramProtocol):
    def __init__(self, Variaveis: Variables):
        self.Variaveis = Variaveis
        self.transport = None

    def connection_made(self, transport):
        self.transport = transport

    def datagram_received(self, data, addr):
        client_id = f"{addr[0]}:{addr[1]}"
        ip = addr[0]

        if not self.Variaveis.client_manager.add_or_update_client(client_id, ip):
            return # Failed to add/update client (e.g., gamepad creation failed)
        
        client_info = self.Variaveis.client_manager.clients[client_id]
        gamepad = client_info['gamepad']
        smooth_handler = client_info['smooth_handler']

        try:
            data = unpackb(data) 
    
            as_id = data[1]
            as_value = data[2]
            
            match data[0]: #tag  
                case 'a':
                    match as_id:
                        case 'w':                            
                            smooth_handler.WheelXValue = as_value[1]
                            smooth_handler.WheelYValue = as_value[0]
                            
                        case '17':
                            gamepad.right_trigger_float(self.Variaveis.acceleratorCor(as_value*2-1))
                            gamepad.left_trigger_float(self.Variaveis.acceleratorCor(-2*as_value+1))
                            gamepad.update()
                            
                        case 'rt':
                            gamepad.right_trigger_float(as_value)
                            gamepad.update()
                            
                        case 'lt':
                            gamepad.left_trigger_float(as_value)
                            gamepad.update()
                            
                        case 'rj':                            
                            gamepad.right_joystick_float(as_value[1], as_value[0])                            
                            gamepad.update()
                            
                case 'b':
                    if as_value:
                        if not client_info['button_states'][as_id]:
                            gamepad.press_button(button=button_map[as_id])
                            
                            client_info['button_states'][as_id] = as_value    
                            gamepad.update()                            
                    else:
                        if client_info['button_states'][as_id]:
                            gamepad.release_button(button=button_map[as_id])
                    
                            client_info['button_states'][as_id] = as_value    
                            gamepad.update()
                case 'p':
                    pass
                
                case 'c':
                    if as_id == 'rr':
                        smooth_handler.loop_interval = as_value
            
            if data[3]: #reliable flag
                self.transport.sendto(b'\x01', addr)

        except Exception as e:
            print(f"{texts['server_process_data']} {client_id}: {e}")
            notification.notify(
                f"{texts['server_err_from']} {client_id}",
                f"{e}",
                timeout=15,
            )


async def main(ServerAdd, Variaveis):
    MB_ICONERROR = 0x10
    MB_OK = 0x0
    
    while Variaveis.data['ServerIP'] == "0.0.0.x":
        await asyncio.sleep(0.01667) # Use asyncio.sleep instead of time.sleep in async functions

    try:
        server_ip = Variaveis.data['ServerIP']
        server_port = Variaveis.data['ServerPort']
        
        if server_ip == "0.0.0.0":
            try:
                s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
                s.connect(('10.255.255.255', 1))
                
                IP = s.getsockname()[0]
            except Exception as e:
                IP = '127.0.0.1'
                ctypes.windll.user32.MessageBoxW(0, str(e), texts['error'], MB_ICONERROR | MB_OK)
            finally:
                server_ip = IP
                Variaveis.MasterIP = server_ip
                
                s.close()
                
        Variaveis.ServerIP = server_ip
        Variaveis.ServerPort = server_port
        
        int_server_port = int(server_port)

        while Variaveis.once:
            await asyncio.sleep(0.01667)
        
        # --- UDP Server Initialization ---
        transport, _protocol = await asyncio.get_running_loop().create_datagram_endpoint(
            lambda: UDPProtocol(Variaveis),
            local_addr=(server_ip, int_server_port))
        
        print(f"{texts['listening_on']} {server_ip}:{server_port}")

        # Start the client cleanup task
        asyncio.create_task(Variaveis.client_manager.cleanup_inactive_clients())

        try:
            while Variaveis.StartStop:
                await asyncio.sleep(1) # Keep main loop alive, UDP protocol handles incoming data
        finally:
            transport.close()
            print("UDP Server stopped.")
            
    except Exception as e:
        ctypes.windll.user32.MessageBoxW(0, repr(e), texts['error'], MB_ICONERROR | MB_OK)
        
    finally:
        # Clean up any remaining clients if the server stops
        with Variaveis.client_manager._lock:
            for client_id in list(Variaveis.client_manager.clients.keys()): # Iterate over a copy
                Variaveis.client_manager.remove_client(client_id)


if __name__ == "__main__":        
    ctypes.windll.shell32.SetCurrentProcessExplicitAppUserModelID(texts['title'])
                
    icon_image = Image.open(os.path.join(path, 'icon', "icon2.ico"))
    
    Variaveis = Variables()
    tkinter_thread_thread = threading.Thread(target=tkinter_thread, args=(Variaveis, ))
    tkinter_thread_thread.start()
    
    MainMenu = pystray.MenuItem(texts['menu'], lambda: NewWindow(Variaveis))
    
    menu = (
        MainMenu,
        pystray.MenuItem(texts['exit'], Variaveis.sair),
    )
    icon = pystray.Icon("name", icon=icon_image, title=texts['title2'], menu=menu)
    
    tray_thread = threading.Thread(target=icon.run)
    tray_thread.daemon = True
    tray_thread.start()
    
    asyncio.run(main("0.0.0.0:3470", Variaveis))