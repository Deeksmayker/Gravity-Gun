using UnityEngine;

public class ResetBasePositionTrigger : MonoBehaviour{
    private void OnTriggerEnter(Collider col){
        if (col.TryGetComponent<PhysicsObject>(out var obj)){
            obj.SetBasePosition(transform.position);
        }
    }
}
