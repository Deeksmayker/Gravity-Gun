using UnityEngine;

public class StikyWall : MonoBehaviour{
    private void OnCollisionEnter(Collision col){
        if (col.gameObject.TryGetComponent<Rigidbody>(out var rb)){
            var velocity = rb.velocity;
            velocity.x = 0;
            velocity.z = 0;
            rb.velocity = velocity;
        }
    }

    private void OnCollisionStay(Collision col){
        if (col.gameObject.TryGetComponent<Rigidbody>(out var rb)){
            rb.velocity *= (1f-Time.fixedDeltaTime*7);
        }
    }
}
