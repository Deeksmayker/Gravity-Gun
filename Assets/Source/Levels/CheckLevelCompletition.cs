using UnityEngine;
using UnityEngine.UI;

public class CheckLevelCompletition : MonoBehaviour{
    [SerializeField] private string levelId;
    
    private void Start(){
        var level = GameManager.Instance.GetLevelById(levelId);
        if (level == null){
            Debug.LogWarning("no such level");
            return;
        }   
        
        if (level.Completed){
            if (TryGetComponent<Activator>(out var activator)){
                activator.SetState(true);
            }
            if (TryGetComponent<Button>(out var button)){
                button.interactable = true;
            }
        } else{
            if (TryGetComponent<Button>(out var button)){
                button.interactable = false;
            }
        }
    }
}
