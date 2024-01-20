using UnityEngine;

public class MovingPlatform : MonoBehaviour{
    [SerializeField] private bool inheritVelocity;

    private Vector3 _oldPos;
    private Vector3 _deltaPosition;

    private void Awake(){
        _oldPos = transform.position;
    }

    private void FixedUpdate(){
        _deltaPosition = transform.position - _oldPos;

        _oldPos = transform.position;
    }

    public Vector3 GetDeltaMovement(){
        return _deltaPosition;
    }

    public bool NeedToInheritVelocity() => inheritVelocity;
}
