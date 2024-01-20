using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    [System.Serializable]
    public class MovementSettings
    {
        public float MaxSpeed;
        public float Acceleration;
        public float Deceleration;

        public MovementSettings(float maxSpeed, float accel, float decel)
        {
            MaxSpeed = maxSpeed;
            Acceleration = accel;
            Deceleration = decel;
        }
    }

    [Header("Movement")]
    [SerializeField] private float m_Friction = 6;
    [SerializeField] private float m_Gravity = 20;
    [SerializeField] private float m_JumpForce = 8;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius;
    [Tooltip("Automatically jump when holding jump button")]
    [SerializeField] private bool m_AutoBunnyHop = false;
    [Tooltip("How precise air control is")]
    [SerializeField] private float m_AirControl = 0.3f;
    [SerializeField] private MovementSettings m_GroundSettings = new MovementSettings(7, 14, 10);
    [SerializeField] private MovementSettings m_AirSettings = new MovementSettings(7, 2, 2);
    [SerializeField] private MovementSettings m_StrafeSettings = new MovementSettings(1, 50, 50);

    /// <summary>
    /// Returns player's current speed.
    /// </summary>
    public float Speed { get { return _ch.velocity.magnitude; } }

    private Vector3 m_MoveDirectionNorm = Vector3.zero;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _currentUpNormal;

    private const float c_verticalVelocityToBreakSlopeMovement = 10;

    // Used to queue the next jump just before hitting the ground.
    private bool m_JumpQueued = false;

    private bool _isGrounded = true;
    private bool _contactedWithEnvThisFrame;
    private bool _previousContactedWithEnv;

    private Vector3 m_MoveInput;
    private Transform m_Tran;

    private CharacterController _ch;

    public event Action OnLanding;
    public event Action<Vector3> OnBounce;
    public event Action OnJump;

    private void Awake()
    {
        m_Tran = transform;
        _ch = GetComponent<CharacterController>();
    }

    private void Update()
    {
        /* if (IsGrounded())
         {
         }*/

        var newGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, Layers.Environment);

        if (newGrounded && _currentUpNormal.y > 0.5f && !IsGrounded())
            OnLanding?.Invoke();

        _isGrounded = newGrounded && _currentUpNormal.y > 0.5f;

        // Set movement state.
        if (IsGrounded())
        {
            GroundMove();
        }
        else
        {
            AirMove();
        }

        if (_previousContactedWithEnv && Physics.Raycast(groundCheckPoint.position, Vector3.down, out var hit, 10, Layers.Environment) && hit.normal.y < 0.99f)
        {
            var projectVelocity = Vector3.ProjectOnPlane(_velocity, _currentUpNormal).normalized;
            var magnitude = _velocity.magnitude;
            if (_velocity.y - (projectVelocity * magnitude).y < c_verticalVelocityToBreakSlopeMovement)
            {
                _velocity = projectVelocity * _velocity.magnitude;
                _ch.Move(Vector3.down * 0.1f);
            }
        }

        // Move the character.
        _ch.Move(_velocity * Time.deltaTime);

        if (_contactedWithEnvThisFrame)
        {
            _contactedWithEnvThisFrame = false;
            _previousContactedWithEnv = true;
        }
        else if (_previousContactedWithEnv)
        {
            _previousContactedWithEnv = false;
        }
    }

    // Handle air movement.
    private void AirMove()
    {
        float accel;

        var wishdir = new Vector3(m_MoveInput.x, 0, m_MoveInput.z);
        wishdir = m_Tran.TransformDirection(wishdir);

        float wishspeed = wishdir.magnitude;
        wishspeed *= m_AirSettings.MaxSpeed;

        wishdir.Normalize();
        m_MoveDirectionNorm = wishdir;

        // CPM Air control.
        float wishspeed2 = wishspeed;
        if (Vector3.Dot(_velocity, wishdir) < 0)
        {
            accel = m_AirSettings.Deceleration;
        }
        else
        {
            accel = m_AirSettings.Acceleration;
        }

        // If the player is ONLY strafing left or right
        if (m_MoveInput.x != 0)
        {
            if (wishspeed > m_StrafeSettings.MaxSpeed)
            {
                wishspeed = m_StrafeSettings.MaxSpeed;
            }

            accel = m_StrafeSettings.Acceleration;
        }

        Accelerate(wishdir, wishspeed, accel);
        if (m_AirControl > 0)
        {
            AirControl(wishdir, wishspeed2);
        }

        // Apply gravity
        _velocity.y -= m_Gravity * Time.deltaTime;
    }

    // Air control occurs when the player is in the air, it allows players to move side 
    // to side much faster rather than being 'sluggish' when it comes to cornering.
    private void AirControl(Vector3 targetDir, float targetSpeed)
    {
        // Only control air movement when moving forward or backward.
        if (Mathf.Abs(m_MoveInput.z) < 0.001)
        {
            return;
        }

        float zSpeed = _velocity.y;
        _velocity.y = 0;
        /* Next two lines are equivalent to idTech's VectorNormalize() */
        float speed = _velocity.magnitude;
        _velocity.Normalize();

        float dot = Vector3.Dot(_velocity, targetDir);
        float k = 32;
        k *= m_AirControl * dot * dot * Time.deltaTime;

        // Change direction while slowing down.
        if (dot > 0)
        {
            _velocity.x *= speed + targetDir.x * k;
            _velocity.y *= speed + targetDir.y * k;
            _velocity.z *= speed + targetDir.z * k;

            _velocity.Normalize();
            m_MoveDirectionNorm = _velocity;
        }

        _velocity.x *= speed;
        _velocity.y = zSpeed; // Note this line
        _velocity.z *= speed;
    }

    // Handle ground movement.
    private void GroundMove()
    {
        // Do not apply friction if the player is queueing up the next jump
        if (!m_JumpQueued)
        {
            ApplyFriction(1.0f);
        }
        else
        {
            ApplyFriction(0);
        }

        var wishdir = new Vector3(m_MoveInput.x, 0, m_MoveInput.z);
        wishdir = m_Tran.TransformDirection(wishdir);
        wishdir.Normalize();
        m_MoveDirectionNorm = wishdir;

        var wishspeed = wishdir.magnitude;
        wishspeed *= m_GroundSettings.MaxSpeed;

        Accelerate(wishdir, wishspeed, m_GroundSettings.Acceleration);

        // Reset the gravity velocity
        if (_velocity.y <= 0)
            _velocity.y = -.1f;

        if (m_JumpQueued)
        {
            Jump();
        }
    }

    private void ApplyFriction(float t)
    {
        // Equivalent to VectorCopy();
        Vector3 vec = _velocity;
        vec.y = 0;
        float speed = vec.magnitude;
        float drop = 0;

        // Only apply friction when grounded.
        if (IsGrounded())
        {
            float control = speed < m_GroundSettings.Deceleration ? m_GroundSettings.Deceleration : speed;
            drop = control * m_Friction * Time.deltaTime * t;
        }

        float newSpeed = speed - drop;
        if (newSpeed < 0)
        {
            newSpeed = 0;
        }

        if (speed > 0)
        {
            newSpeed /= speed;
        }

        _velocity.x *= newSpeed;
        // playerVelocity.y *= newSpeed;
        _velocity.z *= newSpeed;
    }

    // Calculates acceleration based on desired speed and direction.
    private void Accelerate(Vector3 targetDir, float targetSpeed, float accel)
    {
        float currentspeed = Vector3.Dot(_velocity.normalized, targetDir);
        currentspeed *= _velocity.magnitude;
        float addspeed = targetSpeed - currentspeed;
        if (addspeed <= 0)
        {
            return;
        }

        float accelspeed = accel * Time.deltaTime * targetSpeed;
        if (accelspeed > addspeed)
        {
            accelspeed = addspeed;
        }

        _velocity.x += accelspeed * targetDir.x;
        _velocity.z += accelspeed * targetDir.z;
    }

    private void Jump()
    {
        _velocity.y = m_JumpForce;
        m_JumpQueued = false;

        OnJump?.Invoke();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y > 0)
            _currentUpNormal = hit.normal;

        if (Mathf.Pow(2, hit.gameObject.layer) == (int)Layers.Environment)
        {
            _previousContactedWithEnv = true;
            _contactedWithEnvThisFrame = true;
        }


        var needToResolveWallCollision = hit.gameObject.layer is not 8 && hit.normal.y <= 0.5f && !IsGrounded() && GetHorizontalSpeed() > 20
            && !Physics.Raycast(transform.position, Vector3.down, 5, Layers.Environment)
            && !(Vector3.Dot(hit.normal, _velocity.normalized) < -0.5f);

        if (needToResolveWallCollision && Vector3.Dot(hit.normal, _velocity.normalized) <= -0.95f)
        {
            SetVelocity(Vector3.Reflect(GetVelocity(), hit.normal) * 0.9f);
            OnBounce?.Invoke(hit.normal);
        }

        else if (needToResolveWallCollision)
        {
            AddVelocity(hit.normal * (-Vector3.Dot(_velocity.normalized, hit.normal) * _velocity.magnitude));
        }
    }

    public void SetInput(Vector2 input)
    {
        m_MoveInput = new Vector3(input.x, 0, input.y);
    }

    public void SetJumpInput(bool value)
    {
        m_JumpQueued = value;
    }

    public void SetVerticalVelocity(float velocity)
    {
        _velocity.y = velocity;
    }

    public void SetMaxSpeed(float value)
    {
        //targetHorizontalSpeed = value;
    }

    public void AddVerticalVelocity(float addedVelocity)
    {
        _velocity.y += addedVelocity;

        _velocity.y = Mathf.Clamp(_velocity.y, -200, 100);
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        _velocity = newVelocity;
    }

    public void SetHorizontalVelocity(Vector3 newVelocity)
    {
        _velocity.x = newVelocity.x;
        _velocity.z = newVelocity.z;
    }

    public void AddVelocity(Vector3 addedVelocityVector)
    {
        _velocity += addedVelocityVector;
    }

    public void AddOrSetVelocity(Vector3 addedVeloctiy)
    {
        _velocity = Vector3.RotateTowards(_velocity, addedVeloctiy, 100, 100) + addedVeloctiy;
    }

    public void SetInputResponse(bool value)
    {
        //_isResponseToInput = value;
    }

    [ContextMenu("Recalculate ground checker position")]
    public void RecalculateGroundCheckerPosition()
    {
        /*var localPos = groundCheckPoint.localPosition;
        groundCheckPoint.localPosition = new Vector3(localPos.x,
            _groundCheckerHeightRelatedPosition * _ch.height, localPos.z);*/
    }

    public float GetVelocityMagnitude()
    {
        return _ch.velocity.magnitude;
    }

    public Vector3 GetVelocity()
    {
        return _velocity;
    }

    public Vector2 GetHorizontalInput()
    {
        return m_MoveInput;
    }

    public float GetHorizontalSpeed()
    {
        return new Vector3(_velocity.x, 0, _velocity.z).magnitude;
    }

    public bool IsGrounded()
    {
        return _isGrounded;
    }

    public void AddForce(Vector3 force)
    {
        _velocity += force;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }
}
