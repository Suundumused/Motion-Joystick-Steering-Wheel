import os
import sys
import json
import time
import threading
import win32api
import subprocess
import asyncio
import ctypes
import customtkinter

from IO.Localization import texts

from concurrent.futures import ThreadPoolExecutor
from plyer import notification


def open_windows_ports(port:int):
    rule_name = f"Open UDP Port {port} for MotionJoy Server"
    
    result = subprocess.run([
        "powershell", "-Command",
        f"Get-NetFirewallRule -DisplayName '{rule_name}'"
    ], stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
    
    if result.returncode == 0 and rule_name in result.stdout:
        return
    
    subprocess.run([
        "powershell", "-Command",
        f"New-NetFirewallRule -DisplayName '{rule_name} In' "
        f"-Direction Inbound -Action Allow -Protocol UDP -LocalPort {port} -Profile Any"
    ], check=True)
    
    subprocess.run([
        "powershell", "-Command",
        f"New-NetFirewallRule -DisplayName '{rule_name} Out' "
        f"-Direction Outbound -Action Allow -Protocol UDP -LocalPort {port} -Profile Any"
    ], check=True)


def tkinter_thread(Variaveis):
    _ = UI(customtkinter.CTk(), Variaveis)
    
def NewWindow(Variaveis):
    if Variaveis.UI == None:
        tkinter_thread_thread = threading.Thread(target=tkinter_thread, args=(Variaveis, ))
        tkinter_thread_thread.start()


class UI:
    def msgbox(self, txt):
        win32api.MessageBox(0, f'{txt}', texts['alert'])       
           
    def on_sense_change(self, value):
       self.SensibilityLabelValue.configure(text = str(round(value * 100)))

       self.addr_schedule_save()
       
    def on_smooth_change(self, value):
       self.SmoothLabelValue.configure(text = str(round(value * 100)))

       self.addr_schedule_save()
    
    def addr_schedule_reset(self):
        if not self.schedule_save_task and not self.schedule_reset_task:
            _reset_task = threading.Thread(target=self.schedule_reset)
            _reset_task.start()
    
    def addr_schedule_save(self):
        if not self.schedule_save_task and not self.schedule_reset_task:
            _save_task = threading.Thread(target=self.schedule_save)
            _save_task.start()
            
    def schedule_save(self):
        self.schedule_save_task = True
        
        time.sleep(0.33)
        asyncio.run(self.SaveAll())
    
        self.schedule_save_task = False
        
    def schedule_reset(self):
        self.schedule_reset_task = True
        
        time.sleep(0.33)
        asyncio.run(self.openSaveFile(True))

        self.reset_state_controllers()
        
        self.schedule_reset_task = False
        
    def reset_state_controllers(self):
        self.IP.delete(0, "end")
        self.IP.insert(0, self.masterVariaveis.data['ServerIP'])
        
        self.PORT.delete(0, "end")
        self.PORT.insert(0, self.masterVariaveis.data['ServerPort'])
        
        self.slider.set(self.masterVariaveis.data['Sensibility'])
        self.switch_3.set(self.masterVariaveis.data['Smooth_Input'])
        
        self.SensibilityLabelValue.configure(text = str(round(self.masterVariaveis.data['Sensibility'] * 100)))
        self.SmoothLabelValue.configure(text = str(round(self.masterVariaveis.data['Smooth_Input'] * 100)))
        
        self.switch1_var = customtkinter.IntVar(value = self.masterVariaveis.data['Start_Hidden'])
        
    
    async def SaveAll(self):                
        try:                
            if os.path.exists(self.cw_root):
                pass
            else:
                os.makedirs(self.cw_root)
                
        except Exception as e:
            ctypes.windll.user32.MessageBoxW(0, repr(e), texts['error'], self.MB_ICONERROR | self.MB_OK)

        try:                        
            self.masterVariaveis.data['Smooth_Input'] = self.switch_3.get()
            self.masterVariaveis.data['Sensibility'] = self.slider.get()
            self.masterVariaveis.data['Start_Hidden'] = self.switch_1.get()
            
            self.masterVariaveis.data['ServerPort'] = self.PORT.get()
            
            if self.masterVariaveis.data['ServerIP'] != self.IP.get():
                self.masterVariaveis.data['ServerIP'] = self.IP.get()
                
                ctypes.windll.user32.MessageBoxW(0, texts['restart_desc'], texts['warning'], self.MB_ICONWARNING | self.MB_OK)
            
            self.masterVariaveis.Sensibility = self.masterVariaveis.data['Sensibility'] * 2
            self.masterVariaveis.alpha = 1 - self.masterVariaveis.data['Smooth_Input']
                
            self.data = self.masterVariaveis.data
            
            with ThreadPoolExecutor() as pool:
                await asyncio.get_running_loop().run_in_executor(
                    pool,
                    lambda: json.dump(self.data, open(self.cwd, 'w'), indent=4)
                )

        except Exception as e:
            ctypes.windll.user32.MessageBoxW(0, repr(e), texts['error'], self.MB_ICONERROR | self.MB_OK)
            
    async def openSaveFile(self, reset:bool=False):        
        try:                                        
            if os.path.exists(self.cw_root):
                pass
            else:
                os.makedirs(self.cw_root)
                
        except Exception as e:
            ctypes.windll.user32.MessageBoxW(0, repr(e), texts['error'], self.MB_ICONERROR | self.MB_OK)

        try:                
            if os.path.exists(self.cwd) and not reset:
                pass
            else:
                try:
                    if not reset:
                        open_windows_ports(3470)
                except Exception as e:
                    print(e)
                    
                self.data={
                    "Smooth_Input": 1.0,
                    "Sensibility": 1.0,
                    "Start_Hidden": 0,
                    "Show_Overlay": 0,
                    "ServerIP": "0.0.0.0",
                    "ServerPort": "3470"
                }
                    
                with ThreadPoolExecutor() as pool:
                    await asyncio.get_running_loop().run_in_executor(
                        pool,
                        lambda: json.dump(self.data, open(self.cwd, 'w'), indent=4)
                    )
                    
                self.masterVariaveis.data = self.data
                    
                self.masterVariaveis.Sensibility = self.masterVariaveis.data['Sensibility'] * 2
                self.masterVariaveis.alpha = 1 - self.masterVariaveis.data['Smooth_Input']
                
                return
            
            with ThreadPoolExecutor() as pool:
                self.masterVariaveis.data = await asyncio.get_running_loop().run_in_executor(
                    pool,
                    lambda: json.load(open(self.cwd, 'r'))
                )
                                    
            self.masterVariaveis.Sensibility = self.masterVariaveis.data['Sensibility'] * 2
            self.masterVariaveis.alpha = 1 - self.masterVariaveis.data['Smooth_Input']
                
        except Exception as e:
            ctypes.windll.user32.MessageBoxW(0, repr(e), texts['error'], self.MB_ICONERROR | self.MB_OK)
                
                
    def onClose(self):        
        self.masterVariaveis.UI = None
        self.root.destroy()
        self.masterVariaveis.sair()
        
    def __init__(self, root, Variaveis):
        self.MB_ICONERROR = 0x10
        self.MB_ICONWARNING = 0x30
        self.MB_OK = 0x0
        
        self.masterVariaveis = Variaveis
        
        self.schedule_save_task = False
        self.schedule_reset_task = False
        
        self.default_config_path_root = os.path.join('/', 'Documents', 'MotionJoystick', 'Settings')
        self.default_config_path = os.path.join('/', 'Documents', 'MotionJoystick', 'Settings', 'Settings.json')
        
        try:
            self.user_prof = os.path.expanduser(os.getenv('USERPROFILE'))
            
            self.cw_root = os.path.join(self.user_prof, self.default_config_path_root.lstrip('/\\'))
            self.cwd = os.path.join(self.user_prof, self.default_config_path.lstrip('/\\'))
            
        except Exception as e:
            ctypes.windll.user32.MessageBoxW(0, repr(e), texts['error'], self.MB_ICONERROR | self.MB_OK)
        
        asyncio.run(self.openSaveFile())
            
        if getattr(sys, 'frozen', False):  #-----ATUALIZADO-----
            # Executando como executable (PyInstaller)
            path = os.path.dirname(sys.executable)
        else:
            # Executando como  script .py
            path = os.path.dirname(os.path.abspath(sys.argv[0]))
                
        icon_path = os.path.join(path, 'icon', "icon.ico")
        
        customtkinter.set_default_color_theme("green")
        
        self.root = root
        self.root.protocol("WM_DELETE_WINDOW", lambda: self.onClose())
        self.root.title(f"{texts['title2']} V2.2.0")
        
        self.masterVariaveis.UI = self.root
        self.masterVariaveis.main_UI = self
        
        self.app_width = int(0.42 * self.root.winfo_screenwidth())
        self.app_height = int(0.90 * self.root.winfo_screenheight())
        
        self.screen_width = self.root.winfo_screenwidth()
        self.screen_height = self.root.winfo_screenheight()
        
        self.root.resizable(0,0)
        self.root.eval('tk::PlaceWindow . center')
        
        self.root.iconbitmap(icon_path)
                
        self.root.geometry('%dx%d+%d+%d' % (self.app_width, self.app_height, (self.root.winfo_screenwidth() / 3), 0))
        
        self.frame = customtkinter.CTkFrame(master=self.root)
        self.frame.pack(pady=(20,0), padx=60, fill="x", expand=False)
        
        self.title = customtkinter.CTkLabel(master = self.frame, text=texts['title2'], font = ("Roboto", 22))
        self.title.pack(pady=12, padx=10)
        
        self.title2 = customtkinter.CTkLabel(master = self.frame, text = f"{texts['listen_seto']} {self.masterVariaveis.MasterIP}:{self.masterVariaveis.data['ServerPort']}", font = ("Roboto", 14))
        self.title2.pack(pady=12, padx=10)
        
        self.IP = customtkinter.CTkEntry(master = self.frame, placeholder_text="0.0.0.0", justify="right")
        self.IP.pack(pady=12, padx=(10,1), side = "left", expand=True, fill="x")
                                
        self.IP.insert(0, self.masterVariaveis.data['ServerIP'])
        
        self.dott = customtkinter.CTkLabel(master = self.frame, text=":", font = ("Roboto", 18))
        self.dott.pack(pady=12, padx=1, side = "left")
        
        self.PORT = customtkinter.CTkEntry(master = self.frame, placeholder_text="3470")
        self.PORT.pack(pady=12, padx=(1,10), side = "right", expand=False, fill="x")
        
        self.PORT.insert(0, self.masterVariaveis.data['ServerPort'])
        
        self.frameC = self.frame = customtkinter.CTkFrame(master=self.root)
        self.frameC.pack(pady=(20,12), padx=60, fill="both", expand=True)
        
        self.subtitle = customtkinter.CTkLabel(master = self.frameC, text=texts['con_joys'], font = ("Roboto", 18))
        self.subtitle.pack(pady=12, padx=10)
        
        self.clientes_scroll = customtkinter.CTkScrollableFrame(master = self.frameC)
        self.clientes_scroll.pack(fill="both", expand=True)
        
        self.frame3 = customtkinter.CTkFrame(master=self.root)
        self.frame3.pack(pady=(0,12), padx=60, fill="both", expand=False)
        
        self.SenseLabel = customtkinter.CTkLabel(master = self.frame3, text=f"{texts['wheel_turn_range']} ", font = ("Roboto", 14))
        self.SenseLabel.pack(pady=1, padx=(6,1), side = "left")
        
        self.SensibilityLabelValue = customtkinter.CTkLabel(master = self.frame3, text=str(round(self.masterVariaveis.data['Sensibility'] * 100)), font = ("Roboto", 14))
        self.SensibilityLabelValue.pack(pady=1, padx=(6,1), side = "right")
        
        self.slider = customtkinter.CTkSlider(master=self.frame3, border_width=5.5, from_ = 0.67, to = 1.33, command=self.on_sense_change)
        self.slider.pack(pady=1, padx=(1,6), fill="x", expand=True, side = "right")
        self.slider.set(self.masterVariaveis.data['Sensibility'])
        
        self.frame45 = customtkinter.CTkFrame(master=self.root)
        self.frame45.pack(pady=(0,12), padx=60, fill="both", expand=False)
        
        self.frame4 = customtkinter.CTkFrame(master=self.root)
        self.frame4.pack(pady=(0,12), padx=60, fill="both", expand=False)
        
        self.switch1_var = customtkinter.IntVar(value = self.masterVariaveis.data['Start_Hidden'])
        self.switch_1 = customtkinter.CTkSwitch(master=self.frame4, text=texts['sthidden'],  onvalue=1, offvalue=0, variable = self.switch1_var, font = ("Roboto", 14), command=self.addr_schedule_save)
        self.switch_1.pack(pady=1, padx=6, fill="x", expand=True)
            
        self.SenseLabel3 = customtkinter.CTkLabel(master = self.frame45, text=f"{texts['wheel_direct_input']} ", font = ("Roboto", 14))
        self.SenseLabel3.pack(pady=1, padx=(6,1), side = "left")
        
        self.SmoothLabelValue = customtkinter.CTkLabel(master = self.frame45, text=str(round(self.masterVariaveis.data['Smooth_Input'] * 100)), font = ("Roboto", 14))
        self.SmoothLabelValue.pack(pady=1, padx=(6,1), side = "right")
        
        self.switch_3 = customtkinter.CTkSlider(master=self.frame45,  border_width=5.5, number_of_steps = 1000, from_ = 0.125, to = 1.0, command=self.on_smooth_change)
        self.switch_3.pack(pady=1, padx=(1,6), fill="x", expand=True, side = "right")
        self.switch_3.set(self.masterVariaveis.data['Smooth_Input'])
        
        self.frame39 = customtkinter.CTkFrame(master=self.root)
        self.frame39.pack(pady=(0,12), padx=60, fill="both", expand=False)
        
        self.button = customtkinter.CTkButton(master=self.frame39, border_width=0, corner_radius=8, text=texts['reset'], command = self.addr_schedule_reset)
        self.button.pack(pady=33, padx=1)
        
        if self.masterVariaveis.once == True and self.masterVariaveis.data['Start_Hidden'] == 1:            
            self.masterVariaveis.once = False
            self.masterVariaveis.UI = None
            self.root.destroy()
            
            notification.notify(
                    texts['attention'],
                    texts['attention_desc'],
                    
                    timeout=15,  # seconds
                )
        else:
            self.masterVariaveis.once = False
        try:
            root.mainloop()
        except Exception as es:
            print(es)