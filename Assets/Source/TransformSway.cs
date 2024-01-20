using UnityEngine;
using UnityEngine.InputSystem;

public class TransformSway : MonoBehaviour
{
    [Header("Common")] [SerializeField] private Vector2 force = Vector2.one;
    [SerializeField, Min(0f)] private float multiplier = 5f;
    [SerializeField] private bool inverseX;
    [SerializeField] private bool inverseY;

    [Header("Clamp")] [SerializeField] private Vector2 minMaxX;
    [SerializeField] private Vector2 minMaxY;

    private const string MouseX = "Mouse X";
    private const string MouseY = "Mouse Y";

    protected float AdditionalX;
    protected float AdditionalY;

    private float _mouseX, _mouseY;
    private float _velocityY;
    private float _walkProgress;

    private PlayerInput _playerInput;
    private PlayerPhysicsController _playerMover;

    private void Awake()
    {
        _playerInput = GetComponentInParent<PlayerInput>();
        _playerMover = FindObjectOfType<PlayerPhysicsController>();
    }

    private void LateUpdate()
    {
        PerformTransformSway();
        MakeFootstepSway();
    }

    private void PerformTransformSway()
    {    
        var deltaTime = Time.deltaTime;
        var inverseSwayX = inverseX ? -1f : 1f;
        var inverseSwayY = inverseY ? -1f : 1f;

        var input = _playerInput.actions["Look"].ReadValue<Vector2>();
        _mouseX = input.x * inverseSwayX;
        _mouseY = input.y * inverseSwayY;

        OnSwayPerforming(deltaTime);

        var currentX = _mouseY * force.y;
        var currentY = _mouseX * force.x;

        var endEulerAngleX = Mathf.Clamp(currentX + AdditionalX, minMaxX.x, minMaxX.y);
        var endEulerAngleY = Mathf.Clamp(currentY + AdditionalY, minMaxY.x, minMaxY.y);

        var moment = deltaTime * multiplier;
        var localEulerAngles = transform.localEulerAngles;

        localEulerAngles.x = Mathf.LerpAngle(localEulerAngles.x, endEulerAngleX, moment * moment);
        localEulerAngles.y = Mathf.LerpAngle(localEulerAngles.y, endEulerAngleY, moment * moment);
        localEulerAngles.z = 0f;

        transform.localEulerAngles = localEulerAngles;
    }
    
    private void MakeFootstepSway(){
        var progress = _playerMover.GetWalkProgress01();   
        if (progress < _walkProgress) _walkProgress = 1f - progress;
        else _walkProgress = progress;
        var sinX = Mathf.Sin(_walkProgress * Mathf.PI*0.5f);
        var sinY = Mathf.Sin(_walkProgress-0.5f * Mathf.PI);
        var maxX = 0.02f;
        var maxY = 0.03f;
        
        transform.localPosition = new Vector3(sinX * maxX - maxX/2, sinY * maxY - maxY * 0.5f, 0);
    }

    protected virtual void OnSwayPerforming(in float deltaTime)
    {
    }
}
