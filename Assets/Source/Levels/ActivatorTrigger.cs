using UnityEngine;

public class ActivatorTrigger : MonoBehaviour{
    private void OnTriggerEnter(Collider col){
        GetComponent<Activator>().SetState(true);
    }

    private void OnTriggerExit(Collider col){
        GetComponent<Activator>().SetState(false);
    }
}
