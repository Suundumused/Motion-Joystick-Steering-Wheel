import os
import sys
import plyvel
import locale
import ctypes


available_languages = ('en', 'de', 'es', 'it', 'ja', 'ru', 'zh', 'pt', 'hi', 'fr')

class CurrentLocalization:
    def __init__(self, db_path: str):
        self.db = plyvel.DB(db_path)
        
        try:
            self.locale = self.available_locale(locale.windows_locale[ctypes.windll.kernel32.GetUserDefaultUILanguage()].split('_')[0].lower()) + '_'
        except:
            self.locale = available_languages[0] + '_'
    
    @staticmethod
    def available_locale(locale:str) -> str:
        return locale if locale in available_languages else available_languages[0]
    
    def return_translations(self) -> dict:
        _dict = {}
        
        for key, value in self.db:
            key: str = key.decode('utf-8')
            
            if not key.startswith(self.locale):
                continue
            
            _dict[key[key.find('_') + 1:]] = value.decode()
            
        self.db.close()
        return _dict
        
        
texts = CurrentLocalization(os.path.join(os.path.dirname(sys.executable) if getattr(sys, 'frozen', False) else os.path.dirname(os.path.abspath(sys.argv[0])), 'IO', 'Localization', 'Localizations')).return_translations()