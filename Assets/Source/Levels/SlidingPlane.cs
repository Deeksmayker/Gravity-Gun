using UnityEngine;

public class SlidingPlane : MonoBehaviour{
    [SerializeField] private float speedMultiplier = 1.01f;

    private void OnCollisionStay(Collision col){
        if (col.gameObject.TryGetComponent<Rigidbody>(out var rb)){
            rb.velocity = Vector3.ProjectOnPlane(rb.velocity * speedMultiplier, col.GetContact(0).normal);
        }
    }
}
