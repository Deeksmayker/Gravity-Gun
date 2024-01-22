using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private bool autoJump;
    [SerializeField] private float jumpContinueInput;

    private float _jumpInputTimer;

    private bool _responseToInput = true;
	private bool _showConsole;

    private PlayerInput _input;

	private ConsoleController _console;

    private IFire _firing;
	private GravityGun _gravity;
    private PlayerController _mover;
	private PlayerPhysicsController _physicsMover;
    private SimpleCameraController _flyCam;

    private void Awake()
    {
        _input = GetComponentInChildren<PlayerInput>();
        _firing = GetComponentInChildren<IFire>();
        _gravity = GetComponentInChildren<GravityGun>();
        _mover = GetComponentInChildren<PlayerController>();
        _physicsMover = GetComponentInChildren<PlayerPhysicsController>();
		_console = GetComponentInChildren<ConsoleController>();
        _flyCam = FindObjectOfType<SimpleCameraController>();

        Application.targetFrameRate = 200;
    }

	private void OnEnable()
	{
		var weaponGiver = GetComponentInChildren<WeaponGiver>();
		if (weaponGiver){
			weaponGiver.OnWeaponChange += HandleWeaponChange;
		}
	}

    private void Update()
    {
		if (_console && _input.actions["Console"].WasPressedThisFrame()){
			_showConsole = !_showConsole;
			_console.SetConsoleToggle(_showConsole);
		}

		if (_console && _showConsole && _input.actions["Enter"].WasPressedThisFrame()){
			_console.SetEnterInput();
		}

        if (!_responseToInput || _showConsole){
            if (_physicsMover != null)
            {
                _physicsMover.SetInput(Vector2.zero);
            }
            return;
        }

		if (_firing != null)
		{
			_firing.SetInput(_input.actions["Fire"].IsInProgress());
		}

        if (_mover != null)
        {
            _mover.SetInput(_input.actions["Move"].ReadValue<Vector2>());
        }

        if (_physicsMover != null)
        {
            _physicsMover.SetInput(_input.actions["Move"].ReadValue<Vector2>());
        }

        if (_mover != null)
        {
            if (autoJump)
            {
                _mover.SetJumpInput(_input.actions["Jump"].IsInProgress());
            }

            else
            {
                var jumpInput = _input.actions["Jump"].WasPressedThisFrame();
                if (jumpInput)
                    _jumpInputTimer = jumpContinueInput;

                var needToJump = jumpInput || _jumpInputTimer > 0;

                _mover.SetJumpInput(needToJump);

                _jumpInputTimer -= Time.deltaTime;
            } 
        }

        if (_physicsMover != null)
        {
            if (autoJump)
            {
                _physicsMover.SetJumpInput(_input.actions["Jump"].IsInProgress());
            }

            else
            {
                var jumpInput = _input.actions["Jump"].WasPressedThisFrame();
                if (jumpInput)
                    _jumpInputTimer = jumpContinueInput;

                var needToJump = jumpInput || _jumpInputTimer > 0;

                _physicsMover.SetJumpInput(needToJump);

                _jumpInputTimer -= Time.deltaTime;
            } 
        }
/*
        if (_flyCam != null && Input.GetKeyDown(KeyCode.I))
        {
            var noclipEnabled = _flyCam.enabled;
            if (noclipEnabled) _physicsMover.transform.position = _flyCam.transform.position;
            _physicsMover.enabled = noclipEnabled;
            GetComponentInChildren<CameraLook>().enabled = noclipEnabled;
            GetComponentInChildren<CamFollow>().enabled = noclipEnabled;;
            _flyCam.enabled = !noclipEnabled;

            if (noclipEnabled){
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
*/
    }

    public Vector2 GetGameVector2Input(string actionName){
        if (!_responseToInput) return Vector2.zero;
        return _input.actions[actionName].ReadValue<Vector2>();
    }
    
    public bool IsMenuButtonPressed(string name){
        return _input.actions[name].WasPressedThisFrame();
    }
    
    public void SetResponseToInput(bool isIt){
        _responseToInput = isIt;
    }

	private void HandleWeaponChange()
	{
		var firings = GetComponentsInChildren<IFire>();
		_firing = firings[1];
		//var gravity = GetComponentsInChildren<GravityGun>();
		//_gravity = gravity[1];
	}
}
