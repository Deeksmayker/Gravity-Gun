using UnityEngine;
using UnityEngine.UI;

public class GameUi : MonoBehaviour{
    [SerializeField] private GameObject spawnPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sensitivitySlider;
    
    private PlayerInputHandler _input;
    
    private void Awake(){
        _input = GetComponentInParent<PlayerInputHandler>();
    }
    
    private void Update(){
        if (GameManager.Instance.GetCurrentLevelIndex() == 0 || GameManager.Instance.IsGameCompleted()){
            if (_input.IsMenuButtonPressed("SpawnMenu")){
                TogglePanel(spawnPanel);
            }
        }
        
        if (_input.IsMenuButtonPressed("Settings")){
            TogglePanel(spawnPanel.activeSelf ? spawnPanel : settingsPanel);
        }
    }
    
    private void TogglePanel(GameObject panel){
        panel.SetActive(!panel.activeSelf);
        if (panel.activeSelf){
            volumeSlider.value = GameManager.Instance.Volume;
            sensitivitySlider.value = GameManager.Instance.Sensitivity;
            _input.SetResponseToInput(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } else{
            _input.SetResponseToInput(true);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
