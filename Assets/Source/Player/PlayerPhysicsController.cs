using UnityEngine;

public class PlayerPhysicsController : MonoBehaviour{
    [SerializeField] private Transform rotationTarget;
	[SerializeField] private Transform groundCheckPoint;
	[SerializeField] private float groundCheckRadius = 0.5f;

	[SerializeField] private float speed;
    [SerializeField] private float maxSpeed;
	[SerializeField] private float friction;
	[SerializeField] private float airControlMultiplier;
    [SerializeField] private float additionalGravity;

    [SerializeField] private float coyoteTime = 0.3f;

	[SerializeField] private float jumpForce;
	[SerializeField] private float distanceForFootstep = 1;
	
	[SerializeField] private AudioSource flySource;
	
	private AudioClip _heavyLandingSound;
	private AudioClip _jumpSound;
	private AudioClip[] _footstepClips;

	private Vector3 _moveInput;
    private Vector3 _wishDir;
    private Vector3 _groundNormal;

    private Vector3 _lastPosition;
    private float _distanceWalked;
    
    private Vector3 _lastVelocity;

    private Vector3 _lastSafePosition;

    private float _baseAdditionalGravity;
    private float _currentAdditionalGravity;

    private float _notGroundedTime;

	private bool _isGrounded;
	private bool _canJump = true;

    private Transform _startParent;

	private Rigidbody _rb;
    private CamFollow _camFollow;
    
    private ParticleSystem _dustParticles;
    private ParticleSystem _bigDustParticles;

	private void Awake(){
		_rb = GetComponent<Rigidbody>();
        _camFollow = FindObjectOfType<CamFollow>();
        _startParent = transform.parent;

        _lastSafePosition = transform.position;

        _baseAdditionalGravity = additionalGravity;
        _currentAdditionalGravity = additionalGravity;
        
        _dustParticles = (Resources.Load(ResPath.PhysicsObjects + "DustParticlesBig") as GameObject).GetComponent<ParticleSystem>();
        _bigDustParticles = (Resources.Load(ResPath.PhysicsObjects + "DustParticlesBigger") as GameObject).GetComponent<ParticleSystem>();
        _heavyLandingSound = Resources.Load(ResPath.Audio + "HeavyLanding") as AudioClip;
	}
	
	private void Start(){
       _footstepClips = PlayerSound.Instance.GetAllClips("Footsteps");
       _jumpSound = PlayerSound.Instance.GetClip("Jump");
       
       _lastPosition = transform.position;
	}

	private void FixedUpdate(){
		var newGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, Layers.Standable);
        
        if (newGrounded && !IsGrounded()){
            var speed = _lastVelocity.magnitude;
            var speedPositionMultiplier = Mathf.Lerp(0, 5, Mathf.InverseLerp(0, 100, speed));
            _camFollow.AddPositionSmooth(Vector3.down * 0.1f * speedPositionMultiplier, Vector3.down * 0.2f * speedPositionMultiplier, 0.05f * speedPositionMultiplier);
        }

		_isGrounded = newGrounded;

        _wishDir = rotationTarget.forward * _moveInput.z + rotationTarget.right * _moveInput.x;
		
        if (OnSlope()){
            _rb.useGravity = false;
            var verticalVelocity = _rb.velocity.y;
            _rb.velocity *= (1f-Time.fixedDeltaTime*friction);
            _rb.AddForce(GetSlopeMoveDirection() * speed * 10f, ForceMode.Force);
            SpeedControl();
            _notGroundedTime = 0;
            
            if (Physics.Raycast(groundCheckPoint.position, Vector3.down, out var hit, 5f, Layers.Standable)
                && hit.rigidbody){
                hit.rigidbody.AddForceAtPosition(Vector3.down * hit.rigidbody.mass, hit.point);
            }
            
            CalculateFootsteps();
        }


        else if (IsGrounded()){
			//_rb.drag = groundDrag;
            _rb.useGravity = true;
			GroundMove();
            _notGroundedTime = 0;
            
            CalculateFootsteps();
		}
		else{ // In air
			//_rb.drag = 0;
            _rb.useGravity = true;
			AirMove();
            _notGroundedTime += Time.fixedDeltaTime;
            
		}

        var movingPlatform = OnMovingPlatform();
        if (movingPlatform){
            transform.parent = (movingPlatform.transform);
            var multiplier = movingPlatform.NeedToInheritVelocity() ? 180 : 6f;
            _rb.velocity += movingPlatform.GetDeltaMovement()*multiplier;
        }
        else{
            transform.parent = _startParent;
        }

        CheckSafePosition();
        MakeFlySound();

        
        _lastVelocity = _rb.velocity;
        _lastPosition = transform.position;
	}
	
	private void CalculateFootsteps(){
	   _distanceWalked += (transform.position - _lastPosition).magnitude;
	   _distanceWalked = Mathf.Clamp(_distanceWalked, 0, distanceForFootstep * 2);
	   if (_distanceWalked >= distanceForFootstep){
	       _distanceWalked -= distanceForFootstep;
	       PlayerSound.Instance.PlaySound(_footstepClips[Random.Range(0, _footstepClips.Length)], 0.1f
	       , Random.Range(1f, 1.5f));
	   }
	}

	private void GroundMove(){
        _rb.velocity *= (1f-Time.fixedDeltaTime*friction);
		_rb.AddForce(_wishDir * speed * 10f, ForceMode.Force);

		SpeedControl();
	}

	private void AirMove(){
        _rb.AddForce(Vector3.down * _currentAdditionalGravity);

        var dot = Vector3.Dot(GetHorizontalVelocity().normalized, _wishDir);
        if (dot > 0 && GetHorizontalSpeed() > maxSpeed){
            var velocity = _rb.velocity;
            var addedSpeed = speed * 10f * _wishDir * (1f-dot) * airControlMultiplier;
            // velocity.x *= (1f-dot);
            // velocity.z *= (1f-dot);
            // _rb.velocity = velocity;
            _rb.AddForce(addedSpeed, ForceMode.Force);
            return;
        } else if (dot >= 0){
            _rb.AddForce(_wishDir * speed * 3f, ForceMode.Force);
        } else if (GetHorizontalSpeed() > 4f){
            _rb.AddForce(_wishDir * speed * 3f, ForceMode.Force);
        } else{
            _rb.AddForce(_wishDir * speed * 0.5f, ForceMode.Force);
        }


		//_rb.AddForce(_wishDir * speed * 10f * airControlMultiplier, ForceMode.Force);
	}

	private void SpeedControl(){
        if (OnSlidingPanel()) return;

        if (_rb.velocity.magnitude > maxSpeed && _rb.velocity.magnitude < maxSpeed*2){
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, maxSpeed);
        }
	}

	public void SetInput(Vector2 horizontalInput){
		_moveInput = new Vector3(horizontalInput.x, 0, horizontalInput.y);
	}

	public void SetJumpInput(bool jumpInput){
		if (jumpInput && _canJump && (IsGrounded() || _notGroundedTime <= coyoteTime)){
            var velocity = _rb.velocity;
            velocity /= 2;
            velocity.y = jumpForce;
			_rb.velocity = velocity;
			
			PlayerSound.Instance.PlaySound(_jumpSound, 0.1f, Random.Range(1f, 1.5f), 1f);

            //AnimationsHelper.Instance.ChangeVector(FindObjectOfType<CamFollow>().AddPosition, Vector3.down * 0.1f, Vector3.down * 0.2f, 0.05f);

            // FindObjectOfType<CamFollow>().AddPosition(Vector3.down * 0.3f);
            _camFollow.AddPositionSmooth(Vector3.down * 0.1f, Vector3.down * 0.2f, 0.05f);

            _notGroundedTime = 10;
				
			_canJump = false;
			Invoke(nameof(ResetJump), 0.3f);
		}
	}

	private void ResetJump() => _canJump = true;

	public Vector3 GetVelocity() => _rb.velocity;
	public float GetHorizontalSpeed() => (new Vector3(_rb.velocity.x, 0, _rb.velocity.z)).magnitude;

	public bool IsGrounded(){
        if (Physics.Raycast(groundCheckPoint.position, Vector3.down, out var hit, 5, Layers.Standable)){
            _groundNormal = hit.normal;
            return _isGrounded && (hit.transform.tag == "Plank" ||  Vector3.Angle(_groundNormal, Vector3.up) < 30);
        }
        return _isGrounded;
    }

    public bool OnSlope(){
        if (IsGrounded() && Physics.Raycast(groundCheckPoint.position, Vector3.down, out var hit, 5, Layers.Standable) && Vector3.Angle(hit.normal, Vector3.up) > 0){
            _groundNormal = hit.normal;
            return hit.transform.tag == "Plank" || Vector3.Angle(_groundNormal, Vector3.up) < 30;
        }
        return false;
    }

    public Vector3 GetSlopeMoveDirection(){
        return Vector3.ProjectOnPlane(_wishDir, _groundNormal).normalized;
    }

    public MovingPlatform OnMovingPlatform(){
        if (IsGrounded() && Physics.Raycast(groundCheckPoint.position, Vector3.down, out var hit, 5, Layers.Standable)){
            return hit.transform.GetComponent<MovingPlatform>();
        }
        return null;
    }

    public bool OnSlidingPanel(){
        if (Physics.Raycast(groundCheckPoint.position, Vector3.down, out var hit, 5, Layers.Standable)){
            return hit.transform.GetComponent<SlidingPlane>();
        }
        return false;
    }

    private void CheckSafePosition(){
        if (IsGrounded() && _rb.velocity.magnitude < 6 && Physics.Raycast(groundCheckPoint.position, Vector3.down, out var hit, 5f, Layers.Environment)){
            _lastSafePosition = transform.position;
        }

    }

    public float GetBaseAdditionalGravity(){
        return _baseAdditionalGravity;
    }

    public void SetAdditionalGravity(float value){
        _currentAdditionalGravity = value;
    }

    public void ReturnToSafePosition(){
        _rb.velocity = Vector3.zero;
        _rb.MovePosition(_lastSafePosition);
    }
    
    public void MakeFlySound(){
        flySource.volume = Mathf.Lerp(flySource.volume, Mathf.Lerp(0, 0.5f, Mathf.InverseLerp(20, 100, _rb.velocity.magnitude)), Time.deltaTime * 20f);
        flySource.pitch = Mathf.Lerp(flySource.pitch, Mathf.Lerp(0.6f, 1.5f, Mathf.InverseLerp(20, 100, _rb.velocity.magnitude)), Time.deltaTime * 20f);
    }

    public Vector3 GetHorizontalVelocity() => new Vector3(_rb.velocity.x, 0, _rb.velocity.z);

    private void OnCollisionEnter(Collision col){
        var lastSpeed = _lastVelocity.magnitude;

        if (lastSpeed > 35){
            CameraHelper.Instance.ShakeCamera(Mathf.Lerp(0, 1, Mathf.InverseLerp(30, 60, lastSpeed)));
            Instantiate(_bigDustParticles, col.GetContact(0).point, Quaternion.identity);
            PlayerSound.Instance.PlaySound(_heavyLandingSound, 0.6f, Random.Range(0.3f, 0.8f));
        } else if (lastSpeed > 15){
            Instantiate(_dustParticles, col.GetContact(0).point, Quaternion.identity);
            PlayerSound.Instance.PlaySound(_heavyLandingSound, 0.2f, Random.Range(0.8f, 1.2f));
        }
    }
    
    public float GetWalkProgress01(){
        return _distanceWalked / distanceForFootstep;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }
}
