using UnityEngine;

public class RotateSkyMaterial : MonoBehaviour{
    [SerializeField] private Material skyMaterial;
    
    private float _rotation;
    
    private PlayerPhysicsController _playerMover;
    
    private void Awake(){
        _playerMover = GetComponentInChildren<PlayerPhysicsController>();
    }
    
    private void Update(){
        var playerSpeedProgress = Mathf.Lerp(0, 1, Mathf.InverseLerp(20, 100, _playerMover.GetVelocity().magnitude));
        var speedMultiplier = Mathf.Lerp(0.3f, 30, playerSpeedProgress * playerSpeedProgress);
        _rotation += Time.deltaTime * speedMultiplier;
        _rotation %= 360;
        skyMaterial.SetFloat("_Rotation", _rotation);
    }
}
