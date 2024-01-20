using UnityEngine;

public class LogTrigger : MonoBehaviour{
    [SerializeField] private string text;
    
    private void OnTriggerEnter(Collider col){
        if (col.GetComponentInParent<PlayerUnit>()){
            UiLogger.Instance.LogLong(text, 0.02f, 0, 3f);
        }
    }
}
