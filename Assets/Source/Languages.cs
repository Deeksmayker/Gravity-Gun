using System.Collections.Generic;


public enum Language{
    Russian,
    English,
    Turkish
}


public static class Languages{
    
    public static Language CurrentLanguage = Language.Russian;
    
    public static string GetTextByTag(string tag){
        if (!_textsTable.ContainsKey(tag)) {
            return "ERROR WRONG TAG FIX NOW";
        }
        
        switch (CurrentLanguage){
            case Language.Russian:
                return _textsTable[tag].russianText;
            case Language.English:
                return _textsTable[tag].englishText;
            case Language.Turkish:
                return _textsTable[tag].turkishText;
        }
        
        return _textsTable[tag].russianText;
    }
    
    private static Dictionary<string, LanguageText> _textsTable = new (){
        {"LIMBO", new LanguageText()
            { russianText = "Где всё началось", 
              englishText = "Where it all began",
              turkishText = "Her şeyin başladığı yer"}
        },
        {"LIMBO1", new LanguageText()
            { russianText = "Где всё закончилось", 
              englishText = "Where it all ended",
              turkishText = "Her şeyin bittiği yer"}
        },
        {"0-1", new LanguageText()
            { russianText = "Первый шаг", 
              englishText = "First step",
              turkishText = "İlk adım"}
        },
        {"0-2", new LanguageText()
            { russianText = "Дверь", 
              englishText = "Door",
              turkishText = "Kapı"}
        },
        {"0-3", new LanguageText()
            { russianText = "Вертикаль", 
              englishText = "The Vertical",
              turkishText = "Dikey"}
        },
        {"1-1", new LanguageText()
            { russianText = "Суть гравитации", 
              englishText = "The essence of gravity",
              turkishText = "Yerçekiminin özü"}
        },
        {"1-2", new LanguageText()
            { russianText = "E = mc2", 
              englishText = "E = mc2",
              turkishText = "E = mc2"}
        },
        {"1-3", new LanguageText()
            { russianText = "Дорогой друг", 
              englishText = "Dear friend",
              turkishText = "Sevgili dostum"}
        },
        {"2-1", new LanguageText()
            { russianText = "Яма", 
              englishText = "Pit",
              turkishText = "Çukur"}
        },
        {"2-2", new LanguageText()
            { russianText = "Шаг по воздуху", 
              englishText = "Step in the air",
              turkishText = "Havada bir adım"}
        },
        {"2-3", new LanguageText()
            { russianText = "Лифт к небу", 
              englishText = "Sky elevator",
              turkishText = "Gökyüzüne kaldırın"}
        },
        {"3-1", new LanguageText()
            { russianText = "Последняя мечта", 
              englishText = "Last dream",
              turkishText = "Son bir rüya"}
        },
        {"Sound", new LanguageText()
            { russianText = "Звук", 
              englishText = "Sound",
              turkishText = "Ses"}
        },
        {"Sensitivity", new LanguageText()
            { russianText = "Чувствительность", 
              englishText = "Sensitivity",
              turkishText = "Hassasiyet"}
        },
        {"Restart", new LanguageText()
            { russianText = "По новой", 
              englishText = "Restart",
              turkishText = "Yeni"}
        },
        {"Physics", new LanguageText()
            { russianText = "Сброс физики", 
              englishText = "Reset physics",
              turkishText = "Fiziği sıfırla"}
        },
        {"Quit", new LanguageText()
            { russianText = "Уйти", 
              englishText = "Quit",
              turkishText = "Çık dışarı."}
        },
        {"Limbo", new LanguageText()
            { russianText = "В Лимбо", 
              englishText = "To Limbo",
              turkishText = "Araf'ta"}
        },
        {"End1", new LanguageText()
            { russianText = "На этом пока всё!", 
              englishText = "That's all for now!",
              turkishText = "Şimdilik bu kadar!"}
        },
        {"End2", new LanguageText()
            { russianText = "Спасибо за игру", 
              englishText = "Thanks for playing",
              turkishText = "Oynadığınız için teşekkürler"}
        },
        {"End3", new LanguageText()
            { russianText = "Теперь меню создания доступно везде...", 
              englishText = "The creation menu is now available everywhere...",
              turkishText = "Yaratım menüsü artık her yerde mevcut..."}
        },
        {"Tutorial1", new LanguageText()
            { russianText = "Хватать объекты - ЛКМ", 
              englishText = "Grab objects - LMB",
              turkishText = "Nesneleri yakala - LMB"}
        },
        {"TutorialLevels", new LanguageText()
            { russianText = "Уровни в комнатах",
              englishText = "The levels are in the rooms",
              turkishText = "Seviyeler odaların içinde"}
        },
        {"Tutorial2", new LanguageText()
            { russianText = "Меню создания - Q", 
              englishText = "Creation menu - Q",
              turkishText = "Oluşturma menüsü - Q"}
        },
        {"Tutorial3", new LanguageText()
            { russianText = "Удалить объект - Z", 
              englishText = "Delete object - Z",
              turkishText = "Nesneyi sil - Z"}
        },
        {"TutorialLock", new LanguageText()
            { russianText = "Остановить объект - ПКМ", 
              englishText = "Stop object - RMB",
              turkishText = "Durdurma nesnesi - RMB"}
        },
        {"Cubic", new LanguageText()
            { russianText = "Кубик", 
              englishText = "Box",
              turkishText = "Küp"}
        },
        {"CubicNG", new LanguageText()
            { russianText = "Кубик без гравитации", 
              englishText = "Box no gravity",
              turkishText = "Yerçekimsiz bir küp"}
        },
        {"Maneken", new LanguageText()
            { russianText = "Манекен", 
              englishText = "Maneken",
              turkishText = "Manken"}
        },
        {"ManekenNG", new LanguageText()
            { russianText = "Манекен без гравитации", 
              englishText = "Maneken no gravity",
              turkishText = "Yerçekimi olmayan bir manken"}
        },
        {"Stickman", new LanguageText()
            { russianText = "Стикман", 
              englishText = "Stickman",
              turkishText = "Çöp Adam"}
        },
        {"StickmanNOAI", new LanguageText()
            { russianText = "Стикман без ИИ", 
              englishText = "Stickman no AI",
              turkishText = "Yapay Zekasız Çöp Adam"}
        },
        {"Sphere", new LanguageText()
            { russianText = "Сфера", 
              englishText = "Sphere",
              turkishText = "Küre"}
        },
        {"Plank", new LanguageText()
            { russianText = "Балка", 
              englishText = "Plank",
              turkishText = "Kiriş"}
        },
        {"Big plank", new LanguageText()
            { russianText = "Большая балка", 
              englishText = "Big plank",
              turkishText = "Büyük kiriş"}
        },
        {"Grenade", new LanguageText()
            { russianText = "Граната", 
              englishText = "Grenade",
              turkishText = "El bombası"}
        },
        {"Automata", new LanguageText()
            { russianText = "Автомат", 
              englishText = "Machine-gun",
              turkishText = "Otomatik makine"}
        },
        {"Gravity", new LanguageText()
            { russianText = "Гравити-пушка", 
              englishText = "Gravity-gun",
              turkishText = "Yerçekimi tabancası"}
        },
        {"Undo", new LanguageText()
            { russianText = "Уничтожен", 
              englishText = "Deleted",
              turkishText = "Уничтожен"}
        },
        {"FUndo", new LanguageText()
            { russianText = "Никого в живых", 
              englishText = "No alive",
              turkishText = "Kimse hayatta değil."}
        },
        {"ManekenDown", new LanguageText()
            { russianText = "Он пал. Живых: ", 
              englishText = "He's down. Alive: ",
              turkishText = "Düştü. Yaşıyor: "}
        },
        {"вапва", new LanguageText()
            { russianText = "", 
              englishText = "",
              turkishText = ""}
        },
        {"авпав", new LanguageText()
            { russianText = "", 
              englishText = "",
              turkishText = ""}
        },

    };
}

public class LanguageText{
    public string russianText;
    public string englishText;
    public string turkishText;
}
