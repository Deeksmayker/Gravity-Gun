using UnityEngine;

public class ExtandablePlatform : MonoBehaviour{
    [SerializeField] private float maxDistance = 10;
    [SerializeField] private float damping = 0.2f;
    [SerializeField] private float activationPower = 1f;
    [SerializeField] private bool canHoldWithGravity = true;

    private int _crossedEndFrames;

    private Vector3 _startPos;
    private Vector3 _endPos;

    private Rigidbody _rb;
    private ActivateTarget _activate;
    private AudioSource _audioSource;

    private void Awake(){
        _startPos = transform.position;
        _endPos = transform.position + transform.forward * maxDistance;

        _rb = GetComponent<Rigidbody>();
        _activate = GetComponent<ActivateTarget>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable(){
        _activate.OnChangeState += HandleActivate;
    }

    private void OnDisable(){
        _activate.OnChangeState -= HandleActivate;
    }

    private void FixedUpdate(){
        _rb.velocity *= (1f - damping * Time.fixedDeltaTime);

        var dirToEnd = (_endPos - transform.position).normalized;
        var dirToStart = (_startPos - transform.position).normalized;

        bool weCrossedEnd = Vector3.Dot(transform.forward, dirToEnd) < 0; // && Vector3.Dot(_rb.velocity.normalized, transform.forward) > 0;
        bool weCrossedStart = Vector3.Dot(transform.forward, dirToStart) > 0;//&& Vector3.Dot(_rb.velocity.normalized, transform.forward) < 0;

        if (weCrossedEnd || weCrossedStart){
            // _crossedEndFrames++;
            // if (_crossedEndFrames >= 70){
            //     var directionMultiplier = weCrossedEnd ? -1 : 1;
            //     _rb.velocity = directionMultiplier * transform.forward * 120;
            // }
            // else _rb.velocity *= -0.1f;
            var directionMultiplier = weCrossedEnd ? -1 : 1;
            var distance = weCrossedEnd ? Vector3.Distance(transform.position, _endPos) : Vector3.Distance(transform.position, _startPos);
            _rb.velocity = directionMultiplier * transform.forward * Mathf.Max(distance*5, 1);
        }

        var velocityProgress = Mathf.Lerp(0, 1, Mathf.InverseLerp(2, 40, _rb.velocity.magnitude));
        _audioSource.volume = Mathf.Lerp(_audioSource.volume, 0.1f * velocityProgress, Time.deltaTime * 3);
        _audioSource.pitch = Mathf.Lerp(_audioSource.pitch, Mathf.Lerp(0.1f, 1f, velocityProgress), Time.deltaTime * 3);
        //else _crossedEndFrames = 0;
    }

    private void HandleActivate(bool isIt){
        if (isIt){
            var directionMultiplier = Vector3.Distance(transform.position, _endPos) < maxDistance/2 ? -1 : 1;
            _rb.velocity = directionMultiplier * transform.forward * maxDistance * activationPower;
        }
    }

    private void OnCollisionEnter(Collision col){
        if (col.gameObject.TryGetComponent<Rigidbody>(out var rb) && rb.mass > 50){
            rb.mass = 50;
        }
    }
    
    public bool CanHoldWithGravity() => canHoldWithGravity;
}
