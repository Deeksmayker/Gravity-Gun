using UnityEngine;

public class BeegBox : MonoBehaviour{
    [SerializeField] private bool destroyOnCollision;
    
    private AudioClip _collisionSound;
    
    private void Start(){
        _collisionSound = PlayerSound.Instance.GetClip("Collision");
    }

    private void OnCollisionEnter(Collision col){
        PlayerSound.Instance.PlaySoundAtPosition(_collisionSound, transform.position, 2f, Random.Range(1.5f, 2f));
        if (destroyOnCollision){
            Destroy(gameObject, 1f);
        }
    }
}
