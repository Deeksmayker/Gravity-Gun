using UnityEngine;
using UnityEngine.SceneManagement;

public class DieTrigger : MonoBehaviour{
    [SerializeField] private bool startWithDelay;
    [SerializeField] private bool respawnPlayer = true;
    [SerializeField] private bool respawnObjects = true;

    private void OnTriggerEnter(Collider col){
        if (respawnObjects && col.gameObject.TryGetComponent<PhysicsObject>(out var obj)){
            obj.ResetPosition();
        }
        
        if (respawnPlayer && (!startWithDelay || GameManager.Instance.LevelTime > 5) && col.gameObject.TryGetComponent<PlayerUnit>(out var player)){
            player.GetComponent<PlayerPhysicsController>().ReturnToSafePosition();
        }
    }
}
