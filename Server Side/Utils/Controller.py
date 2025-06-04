import asyncio


class Smooth_Operator(object):
    def __init__(self, Variaveis, As_Gamepad):
        self.Variaveis = Variaveis
        self.gamepad = As_Gamepad
        
        self.loop_interval = 0.00694444444444444444444444444444
        
        self.WheelXValue = 0.0
        self.WheelYValue = 0.0
        
        self.filtered_valueX = None
        self.filtered_valueY = None
        
        self.active = True

    def smooth_Thread(self):
        asyncio.run(self.smoothWheel())


    async def smoothWheel(self):    
        while self.Variaveis.StartStop and self.active:            
            self.gamepad.left_joystick_float(self.XADJSensibility(), self.YADJSensibility())
            self.gamepad.update()
            
            await asyncio.sleep(self.loop_interval)
            
        self.gamepad.reset()
        del self.gamepad
                
    
    def XADJSensibility(self):     
        if self.filtered_valueX is None:
            self.filtered_valueX = self.WheelXValue
        else:
            self.filtered_valueX += (self.WheelXValue - self.filtered_valueX) * self.Variaveis.data['Smooth_Input']
        
        return max(min(self.filtered_valueX * self.Variaveis.Sensibility, 1.0), -1.0)

    def YADJSensibility(self):
        if self.filtered_valueY is None:
            self.filtered_valueY = self.WheelYValue
        else:
            self.filtered_valueY += (self.WheelYValue - self.filtered_valueY) * self.Variaveis.data['Smooth_Input']
        
        return max(min(self.filtered_valueY * self.Variaveis.Sensibility, 1.0), -1.0)