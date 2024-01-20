using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform yCameraTransform;
    [SerializeField] private float m_XSensitivity = 2f;
    [SerializeField] private float m_YSensitivity = 2f;
    [SerializeField] private bool m_ClampVerticalRotation = true;
    [SerializeField] private float m_MinimumX = -90F;
    [SerializeField] private float m_MaximumX = 90F;
    [SerializeField] private bool m_Smooth = false;
    [SerializeField] private float m_SmoothTime = 5f;
    [SerializeField] private bool m_LockCursor = true;
    [SerializeField] private float maxAddedFov = 30;
    [SerializeField] private float speedForMaxFov = 50;
    [SerializeField] private float minSenseMultiplier = 0.2f;
    [SerializeField] private Rigidbody playerRb;

    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    private bool m_cursorIsLocked = true;

    private bool _responseToInput = true;
    
    private float _startFov;
    private float _senseMultiplier = 1;
    private float _rotationSpeed;

    private PlayerInputHandler _playerInput;

    private void Awake()
    { 
        m_CharacterTargetRot = transform.localRotation;
        m_CameraTargetRot = cameraTransform.localRotation;

        _playerInput = GetComponentInParent<PlayerInputHandler>();

        if (m_cursorIsLocked)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        _startFov = Camera.main.fieldOfView;
    }

    private void Update()
    {
        SetFovAndSense();
        LookRotation();
    }

    public void LookRotation()
    {
        if (/*TimeController.Instance.IsPaused || */!_responseToInput)
            return;

        // if (Time.timeScale < 1)
        // {
        //     m_Smooth = true;
        //     m_SmoothTime = Mathf.Lerp(10, 20, Time.timeScale);
        // }
        // else
        // {
        //     m_Smooth = false;
        // }
        //var timeScaleSensMultiplier = Time.timeScale < 0.5f ? 0.5f : Time.timeScale;
        var mouseDelta = _playerInput.GetGameVector2Input("Look");
        float yRot = mouseDelta.x * m_XSensitivity * _senseMultiplier;
        float xRot = mouseDelta.y * m_YSensitivity * _senseMultiplier;
        _rotationSpeed = Mathf.Abs(yRot) + Mathf.Abs(xRot);

        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        if (m_ClampVerticalRotation)
        {
            m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);
        }

        if (m_Smooth)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, m_CharacterTargetRot,
                    m_SmoothTime * Time.deltaTime);
            cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, m_CameraTargetRot,
                    m_SmoothTime * Time.deltaTime);
        }
        else
        {
            yCameraTransform.localRotation = m_CharacterTargetRot;
            cameraTransform.localRotation = m_CameraTargetRot;
        }

        //UpdateCursorLock();
    }
    
    private void SetFovAndSense(){
        var currentSpeed = playerRb.velocity.magnitude;
        
        var speedProgress01 = Mathf.InverseLerp(20, speedForMaxFov, currentSpeed) * Mathf.Max(0.5f, Vector3.Dot(playerRb.velocity.normalized, Camera.main.transform.forward.normalized));
        var wishFov = Mathf.Lerp(_startFov, _startFov + maxAddedFov, speedProgress01);
        _senseMultiplier = Mathf.Lerp(1, minSenseMultiplier, speedProgress01);
        
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, wishFov, Time.deltaTime * 10);
    }

    public void SetCursorLock(bool value)
    {
        m_LockCursor = value;
        if (!m_LockCursor)
        {//we force unlock the cursor if the user disable the cursor locking helper
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void SetSense(float value)
    {
        m_XSensitivity = value;
        m_YSensitivity = value;
    }

    /*public void UpdateCursorLock()
      {
    //if the user set "lockCursor" we check & properly lock the cursos
    if (m_LockCursor)
    {
    InternalLockUpdate();
    }
    }*/

    /*private void InternalLockUpdate()
      {
      if (Input.GetKeyUp(KeyCode.Escape))
      {
      m_cursorIsLocked = false;
      }
      else if (Input.GetMouseButtonUp(0))
      {
      m_cursorIsLocked = true;
      }

      if (m_cursorIsLocked)
      {
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
      }
      else if (!m_cursorIsLocked)
      {
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
      }
      }*/
      
    public float GetRotationSpeed(){
        return _rotationSpeed;
    }

    private Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, m_MinimumX, m_MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }   

    public void DisableInputResponse()
    {
        _responseToInput = false;
    }

    public void EnableInputResponse()
    {
        _responseToInput = true;
    }
    
    public void SetSensitivity(float value){
        m_XSensitivity = value;
        m_YSensitivity = value;
    }

}
