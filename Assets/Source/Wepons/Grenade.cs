using UnityEngine;

public class Grenade : MonoBehaviour{
    [SerializeField] private float activationTime = 4f;
    [SerializeField] private float explosionForce;
    [SerializeField] private float explosionRadius;
    
    private float _timer;
    private float _baseTickTime;
    
    private bool _exploded;
    private bool _flag;
    
    private ParticleSystem _particles;
    private AudioClip _explosionSound;
    private AudioSource _source;
    private AudioClip _switchSound;
    
    private MeshRenderer _mesh;
    private MaterialPropertyBlock _propertyBlock;
    
    private void OnEnable(){
        GetComponent<ActivateTarget>().OnChangeState += HandleActivation;
        _mesh = GetComponent<MeshRenderer>();
        _propertyBlock = new MaterialPropertyBlock();
        
        _source = GetComponent<AudioSource>();
        
        _particles = (Resources.Load(ResPath.Weapons + "ExplosionParticles") as GameObject).GetComponent<ParticleSystem>();
        _explosionSound = Resources.Load(ResPath.Audio + "GrenadeExplosion") as AudioClip;
        _switchSound = PlayerSound.Instance.GetClip("Activation");        
    }

    private void OnDisable(){
        GetComponent<ActivateTarget>().OnChangeState -= HandleActivation;
    }
    
    private void Update(){
        if (_timer <= 0) return;
        
        var sin = Mathf.Sin((10f / _timer) * Mathf.PI);
        
        if (!_flag && sin > 0){ 
            _propertyBlock.SetColor("_BaseColor", Colors.Orange);
            _mesh.SetPropertyBlock(_propertyBlock);
            _flag = true;
            //PlayerSound.Instance.PlaySoundAtPosition(_switchSound, transform.position, 0.2f, Random.Range(0.7f, 1.5f));
            _source.volume = 0.2f;
            _source.pitch = Random.Range(0.7f, 1.5f);
            _source.PlayOneShot(_switchSound);
        } else if (_flag && sin <= 0){
            _propertyBlock.SetColor("_BaseColor", Colors.Blue);
            _mesh.SetPropertyBlock(_propertyBlock);
            _flag = false;
            //PlayerSound.Instance.PlaySoundAtPosition(_switchSound, transform.position, 0.2f, Random.Range(0.7f, 1.5f));
            _source.volume = 0.2f;
            _source.pitch = Random.Range(0.7f, 1.5f);
            _source.PlayOneShot(_switchSound);

        }
        
        _timer -= Time.deltaTime;
        
        if (_timer <= 0){
            Explode();
        }
    }
    
    public void Explode(){
        if (_exploded) return;
    
        _exploded = true;
    
        var targets = Physics.OverlapSphere(transform.position, explosionRadius, Layers.PhysicsObjects | Layers.PlayerBase);
        
        for (int i = 0; i < targets.Length; i++){
            if (targets[i].TryGetComponent<Grenade>(out var grenade)){
                grenade.Explode();
            } else if (targets[i].TryGetComponent<Rigidbody>(out var rb)){
                if (rb.constraints == (RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation)){
                    rb.constraints = RigidbodyConstraints.None;
                }
                
                var forceMultiplier = 1;
                if (targets[i].transform.parent && targets[i].transform.parent.gameObject.name.Contains("JumpRamp")){
                    forceMultiplier = 50;
                }
                rb.AddExplosionForce(explosionForce * forceMultiplier, transform.position, explosionRadius);
            }
            
            if (targets[i].GetComponent<PlayerUnit>()){
                CameraHelper.Instance.ShakeCamera(Mathf.Lerp(0, 1f, Mathf.InverseLerp(50, 0, Vector3.Distance(transform.position, targets[i].transform.position))));
            }
        }
        
        PlayerSound.Instance.PlaySoundAtPosition(_explosionSound, transform.position, 0.4f, Random.Range(0.7f, 1.3f), 3);
        
        Instantiate(_particles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void HandleActivation(bool isIt){
        _timer = activationTime;
    }
}
