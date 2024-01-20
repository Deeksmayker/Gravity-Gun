using UnityEngine;
using System.Collections;

public class PhysicsObject : MonoBehaviour
{
    [SerializeField] private bool playEffects = true;

	private Rigidbody[] _rbs;

    private bool _enabled;

    private Vector3 _startPos;
    private Quaternion _startRotation;
    
    private ParticleSystem _dustParticles;

    private LineRenderer _lineRendererPrefab;

	private void Awake()
	{
		_rbs = GetComponentsInChildren<Rigidbody>();
        _startPos = transform.position;
        _startRotation = transform.rotation;

        _lineRendererPrefab = (Resources.Load(ResPath.Levels + "ResetPositionLineRenderer") as GameObject).GetComponent<LineRenderer>();
        if (playEffects){
            _dustParticles = (Resources.Load(ResPath.PhysicsObjects + "DustParticles") as GameObject).GetComponent<ParticleSystem>();
        }
	}

	public void EnableSmooth()
	{
		for (var i = 0; i < _rbs.Length; i++)
		{
			_rbs[i].interpolation = RigidbodyInterpolation.Interpolate;
			_rbs[i].collisionDetectionMode = CollisionDetectionMode.Continuous;
		}

        _enabled = true;
	}

	public void DisableSmooth()
	{
		for (var i = 0; i < _rbs.Length; i++)
		{
			_rbs[i].interpolation = RigidbodyInterpolation.None;
			_rbs[i].collisionDetectionMode = CollisionDetectionMode.Discrete;
		}

        _enabled = false;
	}
	
	private void OnCollisionEnter(Collision col){
	   if (playEffects){
    	   Instantiate(_dustParticles, col.GetContact(0).point, Quaternion.identity);
    	   if (TryGetComponent<AudioSource>(out var source)){
    	       source.pitch = Random.Range(0.7f, 1.3f);
    	       source.Play();
    	   }
	   }
	}

	private Coroutine _coroutine;

	public void EnableSmoothOnTime(float time)
	{
		_coroutine = StartCoroutine(MakeSmoothOnTime(time));	
	}

	private IEnumerator MakeSmoothOnTime(float time)
	{
		EnableSmooth();
        _enabled = false;
		yield return new WaitForSeconds(time);
        if (_enabled) yield break;
		DisableSmooth();
	}

    public void ResetPosition(){
        var line = Instantiate(_lineRendererPrefab);
        line.positionCount = 2;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, _startPos);
        Destroy(line, 5);

        _rbs[0].velocity = Vector3.zero;
        transform.position = _startPos;
        _rbs[0].MovePosition(_startPos);
        transform.rotation = _startRotation;
        _rbs[0].angularVelocity = Vector3.zero;
    }

    public void SetBasePosition(Vector3 newPosition){
        _startPos = newPosition;
    }
}
