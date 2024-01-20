using System;
using UnityEngine;

public class GravityGun : MonoBehaviour, IFire
{	
    public float shootRadius = 0.5f;

	public float force = 600;
	public float damping = 6;
	
	private float _holdTime;

	private bool _input;
	private bool _holding;

    private bool _canHold;
    private bool _holdingActivatableObject;

	private Transform _jointTrans;
	private Transform _pointConnectedToTarget;
	private float _dragDepth;
	private float _rotationSpeed;

    private GameObject _objectInHold;

	private Weapon _weapon;
	private LineRenderer _lr;
	private Rigidbody _playerRb;
	private CameraLook _cameraLook;
	private AudioSource _audioSource;

	public event Action OnFire;

	private void Awake()
	{
		_weapon = GetComponent<Weapon>();
		_lr = GetComponentInChildren<LineRenderer>();
		_playerRb = GetComponentInParent<Rigidbody>();
		_audioSource = GetComponent<AudioSource>();
		_cameraLook = GetComponentInParent<CameraLook>();
	}

	public void SetInput(bool input)
	{
        if (!input) _canHold = true;

        if (!_canHold) return;

		if (input && !_jointTrans)
			HandleInputBegin(new Vector2(Screen.width / 2, Screen.height * 0.5f));
		else if (input && _input && _jointTrans)
		{
			HandleInput(new Vector2(Screen.width / 2, Screen.height * 0.5f));
		}
		else if (!input && _input && _jointTrans)
		{
			HandleInputEnd(new Vector2(Screen.width / 2, Screen.height * 0.5f));
		}

		_input = input;
	}
	
	private void LateUpdate()
	{
		if (!_holding){
		    _audioSource.volume = Mathf.Lerp(_audioSource.volume, 0, Time.deltaTime * 5f);
		    return;
		}
		
        //Picked max: 200		
   	   _rotationSpeed += _cameraLook.GetRotationSpeed();
   	   
   	   var speedProgress = Mathf.Lerp(0, 1, Mathf.InverseLerp(0, 200, _rotationSpeed));
   	   _audioSource.volume = Mathf.Lerp(0.001f, 0.1f, speedProgress);
   	   _audioSource.pitch = Mathf.Lerp(0.5f, 1.2f, speedProgress);
   	   
	   _rotationSpeed = Mathf.Lerp(_rotationSpeed, 0, Time.deltaTime * 10);
		
		if (_holding && !_pointConnectedToTarget){
		    StopHold();
		    return;
	    }
		DrawLineRenderer();
	}

	public void HandleInputBegin (Vector3 screenPosition)
	{
		var ray = Camera.main.ScreenPointToRay (screenPosition);
		RaycastHit hit;
        RaycastHit wallHit;

        Physics.Raycast(ray, out wallHit, 1000, Layers.GravityObstacle);
        if ((Physics.Raycast(ray, out hit, 1000, Layers.GravityTarget)
          || Physics.SphereCast(ray, shootRadius, out hit, 1000, Layers.GravityTarget))
          && Vector3.Distance(hit.point, transform.position) < Vector3.Distance(wallHit.point, transform.position))
		{
			if (hit.rigidbody)
			{
			    if (hit.transform.TryGetComponent<ExtandablePlatform>(out var platform)
			    && !platform.CanHoldWithGravity()) return;
				StartHoldObject(hit);

                if (hit.transform.gameObject.TryGetComponent<ActivateTarget>(out var activate) && activate.CanWithGravity()){
                    activate.SetState(true);
                }
                
                if (hit.rigidbody.constraints == (RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation)){
                    hit.rigidbody.constraints = RigidbodyConstraints.None;
                }

                _objectInHold = hit.transform.gameObject;
			}

		}
	}

	private void StartHoldObject(RaycastHit hit){
		//_playerSpring.connectedBody = hit.rigidbody;

		_holding = true;
		_dragDepth = CameraPlane.CameraToPointDepth (Camera.main, hit.point);
        _dragDepth = Mathf.Max(_dragDepth, 5);

		_jointTrans = AttachJoint (hit.rigidbody, hit.point);
		_pointConnectedToTarget = new GameObject("Point on target").transform;
		_pointConnectedToTarget.SetParent(hit.transform);
		_pointConnectedToTarget.position = hit.point;
		//hit.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		var physicsObjectController = _pointConnectedToTarget.GetComponentInParent<PhysicsObject>();
		if (physicsObjectController){
			physicsObjectController.EnableSmooth();
		}
		if (_pointConnectedToTarget.GetComponentInParent<ActivatableObject>()){
			_holdingActivatableObject = true;
		}
	}


	public void HandleInput (Vector3 screenPosition)
	{
		if (_jointTrans == null)
			return;
		
		if (_holding && !_pointConnectedToTarget || !_objectInHold){
		    StopHold();
		    return;
	    }
	    
	    if (Input.GetMouseButtonDown(1) && _objectInHold.GetComponent<Rigidbody>().constraints == RigidbodyConstraints.None){
	       _objectInHold.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
	       StopHold();
	    }
			
		_holdTime += Time.deltaTime;

		var endPos = transform.position + _weapon.GetDirectionTarget().forward * _dragDepth;
		var dirToEnd = endPos - transform.position;
		if (Physics.Raycast(transform.position, dirToEnd.normalized, out var hit, dirToEnd.magnitude, Layers.Environment)){
            _dragDepth = Mathf.Max((hit.point - transform.position).magnitude, 5);
        }

		var worldPos = Camera.main.ScreenToWorldPoint (screenPosition);
		_jointTrans.position = CameraPlane.ScreenToWorldPlanePoint (Camera.main, _dragDepth, screenPosition);
		
		if (_holdingActivatableObject && _holdTime > 1f){
		    if (Physics.Raycast(Camera.main.ScreenPointToRay(screenPosition), out var hit1, 50, Layers.Interactable | Layers.Environment | Layers.PhysicsInteractable) && hit1.transform.TryGetComponent<ObjectsCatcher>(out var catcher) && !catcher.IsActivated()){
		        catcher.Catch(_objectInHold.GetComponent<Rigidbody>());
		        StopHold();
		    }
		}
	}

	public void HandleInputEnd (Vector3 screenPosition)
	{
        if (_objectInHold && _objectInHold.TryGetComponent<ActivateTarget>(out var activate) && activate.CanWithGravity()){
            activate.SetState(false);
        }
        
		_lr.positionCount         = 0;
		_holding                  = false;
        _objectInHold             = null;
        _holdingActivatableObject = false;
        _holdTime                 = 0;
		Destroy(_jointTrans.gameObject);
        
		if (!_pointConnectedToTarget) return;

		var physicsObjectController = _pointConnectedToTarget.GetComponentInParent<PhysicsObject>();
		if (physicsObjectController)
			physicsObjectController.EnableSmoothOnTime(5);
        Destroy(_pointConnectedToTarget.gameObject);
	}

	Transform AttachJoint (Rigidbody rb, Vector3 attachmentPosition)
	{
		GameObject go = new GameObject("Attachment Point");
		go.transform.position = attachmentPosition;
		var newRb = go.AddComponent<Rigidbody> ();
		newRb.isKinematic = true;

		var joint = go.AddComponent<ConfigurableJoint> ();
		joint.connectedBody          = rb;
		joint.configuredInWorldSpace = true;
		
		joint.xDrive     = NewJointDrive (force, damping);
		joint.yDrive     = NewJointDrive (force, damping);
		joint.zDrive     = NewJointDrive (force, damping);
		joint.slerpDrive = NewJointDrive (force, damping);
		
		joint.rotationDriveMode = RotationDriveMode.Slerp;

		return go.transform;
	}

	private JointDrive NewJointDrive (float force, float damping)
	{
		JointDrive drive     = new JointDrive ();
		drive.mode           = JointDriveMode.Position;
		drive.positionSpring = force;
		drive.positionDamper = damping;
		drive.maximumForce   = Mathf.Infinity;
		return drive;
	}

	private void DrawLineRenderer()
	{
		var posCount       = 60;
		var middlePosition = transform.position + _weapon.GetDirectionTarget().forward * (_dragDepth / 2);
		var dirToMid       = middlePosition - transform.position;
		
		if (Physics.Raycast(transform.position, dirToMid.normalized,
		                    out var hit, dirToMid.magnitude, Layers.Environment | Layers.GravityTarget)){
			middlePosition = hit.point;
		}

		_lr.positionCount = posCount;
		_lr.SetPosition(0, _weapon.GetGunTipPos());
		var endPos = _pointConnectedToTarget.position;
		_lr.SetPosition(posCount - 1, endPos);

		for (var i = 1; i < posCount - 1; i++)
		{
			var t = Mathf.InverseLerp(0, posCount, i);

			var ab = Vector3.Lerp(_weapon.GetGunTipPos(), middlePosition, t);
			var bc = Vector3.Lerp(middlePosition, endPos, t);

			_lr.SetPosition(i, Vector3.Lerp(ab, bc, t));
		}
	}

    public void StopHold(){
        if (_jointTrans){
			HandleInputEnd(new Vector2(Screen.width / 2, Screen.height * 0.5f));
            _canHold = false; 
        }
    }
}
