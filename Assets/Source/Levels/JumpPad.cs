using UnityEngine;

public class JumpPad : MonoBehaviour{
	[SerializeField] private float force;
	
	private AudioSource _source; 
	
	private void Awake(){
	   _source = GetComponent<AudioSource>();
	}

    private void OnTriggerEnter(Collider col){
        if (col.TryGetComponent<PlayerPhysicsController>(out var playerMover)){
            playerMover.SetAdditionalGravity(0);
            AnimationsHelper.Instance.ChangeFloat(playerMover.SetAdditionalGravity, 0, playerMover.GetBaseAdditionalGravity(), 1, 2);

            FindObjectOfType<CamFollow>().AddPositionSmooth(Vector3.down * 0.1f, Vector3.down * 0.2f, 0.1f);

            CameraHelper.Instance.ShakeCamera(Mathf.Lerp(0, 1, Mathf.InverseLerp(0, 100, force)));
            
            _source.Play();
        }

        if (col.TryGetComponent<Rigidbody>(out var rb)){
            if (col.GetComponent<PhysicsObject>()){
                rb.velocity = transform.up * force;
                _source.Play();
                return;
            }
            var velocity = rb.velocity;
            velocity.y = 0;
            velocity += transform.up * force;
            rb.velocity = velocity;
            
            _source.Play();
        }
	}

    private void OnTriggerStay(Collider col){
        if (col.TryGetComponent<Rigidbody>(out var rb)){
            if (col.GetComponent<PhysicsObject>()){
                rb.velocity = transform.up * force;
                return;
            }
            if (Vector3.Dot(rb.velocity.normalized, transform.up) * force < force * 0.9f){
                var velocity = rb.velocity;
                velocity.y = 0;
                velocity += transform.up * force;
                rb.velocity = velocity;
            }
        }
    }
    
    private void OnCollisionEnter(Collision col){
        if (col.gameObject.TryGetComponent<PlayerPhysicsController>(out var playerMover)){
            playerMover.SetAdditionalGravity(0);
            AnimationsHelper.Instance.ChangeFloat(playerMover.SetAdditionalGravity, 0, playerMover.GetBaseAdditionalGravity(), 1, 2);

            FindObjectOfType<CamFollow>().AddPositionSmooth(Vector3.down * 0.1f, Vector3.down * 0.2f, 0.1f);

            CameraHelper.Instance.ShakeCamera(Mathf.Lerp(0, 1, Mathf.InverseLerp(0, 100, force)));
        }

        if (col.gameObject.TryGetComponent<Rigidbody>(out var rb)){
            if (col.gameObject.GetComponent<PhysicsObject>()){
                rb.velocity = transform.up * force;
                return;
            }
            var velocity = rb.velocity;
            velocity.y = 0;
            velocity += transform.up * force;
            rb.velocity = velocity;
        }
	}

    private void OnCollisionStay(Collision col){
        if (col.gameObject.TryGetComponent<Rigidbody>(out var rb)){
            if (col.gameObject.GetComponent<PhysicsObject>()){
                rb.velocity = transform.up * force;
                return;
            }
            if (Vector3.Dot(rb.velocity.normalized, transform.up) * force < force * 0.9f){
                var velocity = rb.velocity;
                velocity.y = 0;
                velocity += transform.up * force;
                rb.velocity = velocity;
            }
        }
    }

}
