using UnityEngine;

public class LevelLoad : MonoBehaviour{
    [SerializeField] private bool enable;
    [SerializeField] private bool disable;
    [SerializeField] private bool destroy;

    private void OnEnable(){
        GetComponent<ActivateTarget>().OnChangeState += HandleActivation;

        if (enable) gameObject.SetActive(false);
    }

    private void HandleActivation(bool state){
        if (enable)
            gameObject.SetActive(true);
        else if (disable)
            gameObject.SetActive(false);
        else if (destroy)
            Destroy(gameObject);
    }

    private void OnDestroy(){
        GetComponent<ActivateTarget>().OnChangeState -= HandleActivation;
    }
}
