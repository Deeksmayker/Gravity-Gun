using UnityEngine;

public class EnemyWalking : MonoBehaviour
{
    [SerializeField] private bool needToMove = true;
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float rotationSpeed = 10;
    [SerializeField] private float maxSpeed = 20;

    [SerializeField] private Rigidbody leftLegGround, rightLegGround;
    [SerializeField] private Transform leftLegHeel, rightLegHeel, leftLegTip, rightLegTip;
    [SerializeField] private float groundCheckRadius = 0.5f;

    [SerializeField] private Rigidbody head;
    [SerializeField] private Rigidbody body;
    [SerializeField] private Rigidbody leftHand;
    [SerializeField] private Rigidbody rightHand;
    [SerializeField] private Rigidbody leftLeg;
    [SerializeField] private Rigidbody rightLeg;

    [SerializeField] private float headPushUpForce;
    [SerializeField] private float bodyPushUpForce;
    [SerializeField] private float handsPushUpForce;
    [SerializeField] private float legsPushUpForce;

    private float _timeOnGround;

    private PlayerLocator _playerLocator;

    private void Awake()
    {
        _playerLocator = GetComponent<PlayerLocator>();
    }

    private void FixedUpdate()
    {
        if (!OnGround())
        {
            _timeOnGround = Mathf.Clamp01(_timeOnGround -= Time.fixedDeltaTime);
            return;
        }
        _timeOnGround = Mathf.Clamp01(_timeOnGround + Time.fixedDeltaTime);
        AddUpForce();
        if (needToMove)
            Move();
    }

    private void AddUpForce()
    {
        var upForceMultiplier = _timeOnGround;

        head.AddForce(Vector3.up * headPushUpForce * upForceMultiplier, ForceMode.Acceleration);
        body.AddForce(Vector3.up * bodyPushUpForce * upForceMultiplier, ForceMode.Acceleration);
        leftHand.AddForce(Vector3.up * handsPushUpForce * upForceMultiplier, ForceMode.Acceleration);
        rightHand.AddForce(Vector3.up * handsPushUpForce * upForceMultiplier, ForceMode.Acceleration);
        leftLeg.AddForce(Vector3.up * legsPushUpForce * upForceMultiplier, ForceMode.Acceleration);
        rightLeg.AddForce(Vector3.up * legsPushUpForce * upForceMultiplier, ForceMode.Acceleration);

        //leftLegGround.MoveRotation(Quaternion.SlerpUnclamped(leftLegGround.rotation, Quaternion.identity, rotationSpeed * 0.2f));
        //rightLegGround.MoveRotation(Quaternion.SlerpUnclamped(rightLegGround.rotation, Quaternion.identity, rotationSpeed * 0.2f));
        leftLegGround.AddForce(Vector3.down * 20);
        leftLegGround.AddForce(Vector3.down * 20);
    }

    private void Move()
    {
        if (!_playerLocator.IsPlayerVisible()) return;
        body.velocity *= 0.9f;
        body.AddForce(moveSpeed * _playerLocator.GetDirectionToPlayerNorm(), ForceMode.Force);
        body.MoveRotation(Quaternion.Slerp(body.rotation, Quaternion.LookRotation(_playerLocator.GetDirectionToPlayerNorm()), rotationSpeed));
        body.velocity = Vector3.ClampMagnitude(body.velocity, maxSpeed);
    }

    public bool OnGround()
    {
        return Physics.CheckCapsule(rightLegHeel.position, rightLegTip.position, groundCheckRadius, Layers.Environment)
            || Physics.CheckCapsule(leftLegHeel.position, leftLegTip.position, groundCheckRadius, Layers.Environment);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(leftLegHeel.position, groundCheckRadius);
        Gizmos.DrawWireSphere(leftLegTip.position, groundCheckRadius);
        Gizmos.DrawWireSphere(rightLegHeel.position, groundCheckRadius);
        Gizmos.DrawWireSphere(rightLegTip.position, groundCheckRadius);
    }
}
