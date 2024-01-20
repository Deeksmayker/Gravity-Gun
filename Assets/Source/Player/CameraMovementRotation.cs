using UnityEngine;

public class CameraMovementRotation : MonoBehaviour{
    [SerializeField] private float maxZRotation = 5f;
    [SerializeField] private float maxXRotation = 5f;

    [SerializeField] private float rotationSpeed = 5f;

    private PlayerInputHandler _input;

    private void Awake(){
        _input = GetComponentInParent<PlayerInputHandler>();
    }

    private void Update(){
        var moveInput = _input.GetGameVector2Input("Move");
        var wishZ = -moveInput.x * maxZRotation;
        var wishX = moveInput.y < 0 ? moveInput.y * maxXRotation : 0;

        var wishAngles = new Vector3(wishX, transform.localEulerAngles.y, wishZ);

        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(wishAngles), Time.deltaTime * rotationSpeed);
    }
}
