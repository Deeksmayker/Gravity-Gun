using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class GameManager : MonoBehaviour{
    public static GameManager Instance;
    
    [SerializeField] private TitlePanel titlePanel;

    private LevelSettings[] _levels = new LevelSettings[]{
        new LevelSettings(){Id = "LIMBO", Name = "Где всё началось", Completed = true},
        new LevelSettings(){Id = "0-1", Name = "Первый шаг"},
        new LevelSettings(){Id = "0-2", Name = "Дверь"},
        new LevelSettings(){Id = "0-3", Name = "Вертикаль"},
        new LevelSettings(){Id = "1-1", Name = "Суть гравитации"},
        new LevelSettings(){Id = "1-2", Name = "E = mc2"},
        new LevelSettings(){Id = "1-3", Name = "Дорогой друг"},
        new LevelSettings(){Id = "2-1", Name = "Яма"},
        new LevelSettings(){Id = "2-2", Name = "Шаг по воздуху"},
        new LevelSettings(){Id = "2-3", Name = "Лифт к небу"},
        new LevelSettings(){Id = "3-1", Name = "Последняя мечта"}
    };

    private int _levelIndex;
    
    private bool _gameCompleted;
    private bool _gameJustStarted = true;    
    private bool _gameJustCompleted;

    public float Volume {get; private set;} = 1;
    public float Sensitivity {get; private set;} = 0.1f;
    public float LevelTime = 0;
    
    private void Awake(){
        if (Instance != null && Instance != this){
            Instance.LevelTime = 0;
            Destroy(this);
            return;
        }

        Instance = this;
        _levelIndex = SceneManager.GetActiveScene().buildIndex;
        LoadGame();
        
        var delay = 1f;
        
        if (GetCurrentLevelSettings().Id == "LIMBO" && !_gameJustCompleted){
            delay = _gameJustStarted ? 12f : 0.1f;
            titlePanel.gameObject.SetActive(true);
            titlePanel.SetDelay(delay);
            if (_gameJustStarted){
                Invoke(nameof(TypeIntroText), 1f);
            }
            
            Invoke(nameof(TypeTutorial), delay + 10f);
            
            _gameJustStarted = false;
        }
            
        if (_gameJustCompleted){
            delay = 16f;
            titlePanel.gameObject.SetActive(true);
            titlePanel.SetDelay(delay);
            Invoke(nameof(TypeEndCredits), 1f);
            _gameJustCompleted = false;
        }
        
        Invoke(nameof(TypeLevelName), delay);
    }
    
    private void Update(){
        LevelTime += Time.deltaTime;
    }

    public void LoadNextLevel(){
        if (_levels[_levelIndex].Id == "2-3"){
            _gameCompleted = true;
            _gameJustCompleted = true;
            _levelIndex = 0;
        } else{
            _levelIndex++;
        }
        
        SaveGame();
    
        SceneManager.LoadScene(_levels[_levelIndex].Id);
    }
    
    public void LoadLevelById(string id){
        SaveGame();
        for (int i = 0; i < _levels.Length; i++){
            if (_levels[i].Id == id){
                SceneManager.LoadScene(_levels[i].Id);
                _levelIndex = i;
                break;
            }
            if (i == _levels.Length-1){
                //@TODO need to log on screen error
            }
        }
    }
    
    public LevelSettings GetLevelById(string id){
        for (int i = 0; i < _levels.Length; i++){
            if (_levels[i].Id == id){
                return _levels[i];
            }
        }
        
        return null;
    }

    public LevelSettings GetCurrentLevelSettings(){
        if (_levelIndex >= _levels.Length) return _levels[_levelIndex-1];
        return _levels[_levelIndex];
    }
    
    public void SetCurrentLevelCompleted(){
        GetCurrentLevelSettings().Completed = true;
        SaveGame();
    }   
    
    public int GetCurrentLevelIndex(){
        return _levelIndex;
    }
    
    public void ReloadLevel(){
        SceneManager.LoadScene(_levelIndex);
    }
    
    public void ResetPhysics(){
        FindObjectOfType<ManekenController>().ResetDelay();
        var objects = FindObjectsOfType<PhysicsObject>();
        foreach (var obj in objects){
            obj.ResetPosition();
        }
    }
    
    public void SetVolume(float value){
        Volume = value;
        AudioListener.volume =  Volume;
    }
    
    public void LoadLimbo(){
        SaveGame();
        LoadLevelById("LIMBO");
    }
    
    public void QuitGame(){
        SaveGame();
        Application.Quit();
    }
    
    public bool IsGameCompleted(){
        return _gameCompleted;
    }
    
    public void SaveGame(){
        for (int i = 0; i < _levels.Length; i++){
            if (_levels[i].Completed){
                PlayerPrefs.SetInt(_levels[i].Id, 1);
            }
        }
        if (_gameCompleted){
            PlayerPrefs.SetInt("GameCompleted", 1);
        } 
        if (!_gameJustStarted){
            PlayerPrefs.SetInt("GameJustStarted", 0);
        }
        if (_gameJustCompleted){
            PlayerPrefs.SetInt("GameJustCompleted", 1);
        } else{
            PlayerPrefs.SetInt("GameJustCompleted", 0);
        }
        
        PlayerPrefs.SetInt("Language", (int)Languages.CurrentLanguage);
        PlayerPrefs.SetFloat("Volume", Volume);
        PlayerPrefs.SetFloat("Sensitivity", Sensitivity);
        
        PlayerPrefs.Save();
    }
    
    public void LoadGame(){
        for (int i = 0; i < _levels.Length; i++){
            if (PlayerPrefs.HasKey(_levels[i].Id) && PlayerPrefs.GetInt(_levels[i].Id) == 1){
                _levels[i].Completed = true;
            }
        }
        
        if (PlayerPrefs.HasKey("GameCompleted")){
            _gameCompleted = true;
        }
        if (PlayerPrefs.HasKey("GameJustStarted")){
            _gameJustStarted = false;
        }
        if (PlayerPrefs.HasKey("GameJustCompleted")){
            _gameJustCompleted = PlayerPrefs.GetInt("GameJustCompleted") == 1;
        }
        
        if (PlayerPrefs.HasKey("Volume")){
            SetVolume(PlayerPrefs.GetFloat("Volume"));
        }
        if (PlayerPrefs.HasKey("Sensitivity")){
            Sensitivity = PlayerPrefs.GetFloat("Sensitivity");
            SetSensitivity(Sensitivity);
        }
        if (PlayerPrefs.HasKey("Language")){
            SetLanguageDirect(PlayerPrefs.GetInt("Language"));
        }
    }
    
    public void DeleteSaves(){
        //PlayerPrefs.DeleteAll();
        PlayerPrefs.DeleteKey("GameJustStarted");
        LoadGame();
        //SaveGame();
        //PlayerPrefs.Save();
        LoadLimbo();
    }
    
    private void TypeLevelName(){
        var currentLevel = GameManager.Instance.GetCurrentLevelSettings();
        UiLogger.Instance.LogLong(currentLevel.Id, 0.5f, 0f, 8f);
        if (currentLevel.Id == "LIMBO" && IsGameCompleted()){
            UiLogger.Instance.LogLong(Languages.GetTextByTag("LIMBO1"), 0.05f, 3f, 7f);
        } else{
            UiLogger.Instance.LogLong(Languages.GetTextByTag(currentLevel.Id), 0.05f, 3f, 7f);
        }
    }
    
    private void TypeTutorial(){
        UiLogger.Instance.LogLong(Languages.GetTextByTag("Tutorial1"), 0.03f, 0f, 20f);
        UiLogger.Instance.LogLong(Languages.GetTextByTag("TutorialLevels"), 0.03f, 2f, 20f);
        UiLogger.Instance.LogLong(Languages.GetTextByTag("Tutorial2"), 0.03f, 2f, 20f);
        UiLogger.Instance.LogLong(Languages.GetTextByTag("Tutorial3"), 0.03f, 5f, 25f);
        UiLogger.Instance.LogLong(Languages.GetTextByTag("TutorialLock"), 0.03f, 10f, 30f);
    }

    
    private void TypeIntroText(){
        UiLogger.Instance.BigLog("Gravity Gun", 0.05f, 1.5f, 8f);
        UiLogger.Instance.BigLog("by", 0.05f, 3f, 9f);
        UiLogger.Instance.BigLog("DT", 0.2f, 4f, 10f);
        
        if (IsGameCompleted()){
            PlayerSound.Instance.PlaySound(PlayerSound.Instance.GetClip("ClairDeLune"), 0.01f, 1, 400);
        }
    } 
    
    private void TypeEndCredits(){
        UiLogger.Instance.BigLog(Languages.GetTextByTag("End1"), 0.05f, 1.5f, 8f);
        UiLogger.Instance.BigLog(Languages.GetTextByTag("End2"), 0.05f, 4f, 8f);
        UiLogger.Instance.BigLog(Languages.GetTextByTag("End3"), 0.05f, 9f, 16f);
        
        PlayerSound.Instance.PlaySound(PlayerSound.Instance.GetClip("ClairDeLune"), 0.03f, 1, 400);
    }
    
    public event Action OnLanguageChanged;
    
    public void SetLanguage(string language){
        switch (language){
            case "ru":
                Languages.CurrentLanguage = Language.Russian;
                break;
            case "eng":
                Languages.CurrentLanguage = Language.English;
                break;
            case "turk":
                Languages.CurrentLanguage = Language.Turkish;
                break;
        }

        OnLanguageChanged?.Invoke();
    }
    
    public void SetLanguageDirect(int lang){
        Languages.CurrentLanguage = (Language)lang;
        OnLanguageChanged?.Invoke();
    }
    
    public void SetSensitivity(float value){
        Sensitivity = value;
        FindObjectOfType<CameraLook>().SetSensitivity(Sensitivity);
    }
}

public class LevelSettings{
    public string        Id;
    public string      Name;
    public bool   Completed;
}
