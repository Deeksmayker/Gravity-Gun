using UnityEngine;

public class PushPhysicsObjects : MonoBehaviour{
    private void OnCollisionStay(Collision col){
        return;
        if (col.gameObject.GetComponent<PhysicsObject>() && col.gameObject.TryGetComponent<Rigidbody>(out var rb)){
            rb.velocity += col.GetContact(0).normal * 10;
        }
    }
}
