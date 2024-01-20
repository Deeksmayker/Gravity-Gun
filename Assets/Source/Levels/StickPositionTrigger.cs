using UnityEngine;

public class StickPositionTrigger : MonoBehaviour{
    [SerializeField] private int tag;

    private void OnTriggerEnter(Collider col){
        if (col.TryGetComponent<Stickable>(out var stickable) && tag == stickable.GetTag()){
            AnimationsHelper.Instance.RotateObject(col.transform, col.transform.rotation, transform.rotation, 0.5f, 0);
            var rb = col.gameObject.GetComponent<Rigidbody>();
            rb.velocity        = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            AnimationsHelper.Instance.AddMoveTask(col.transform, col.transform.position, transform.position, 0.5f, 0);

            FindObjectOfType<GravityGun>().StopHold();
        }
    }
}
