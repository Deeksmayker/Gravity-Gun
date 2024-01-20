using UnityEngine;

public class ObjectsCatcher : MonoBehaviour{
    [SerializeField] private Color baseColor = new Color(88f/255f, 200f/255f, 241f/255f, 183f/255f);
    [SerializeField] private Color activeColor = new Color(214f/255f, 78f/255f, 110f/255f, 183f/255f);
    [SerializeField] private float velocityRange = 10f;

    private float _timerInside;

    private ParticleSystem.MainModule _pModule;
    
    private float _delayTimer;

    private Rigidbody[] _rbsInTrigger;

    private Activator _activator;

    private void Awake(){
        _rbsInTrigger = new Rigidbody[5];
        _activator = GetComponent<Activator>();
        
        _pModule = GetComponent<ParticleSystem>().main;
        _pModule.startColor = baseColor;
    }

    private void FixedUpdate(){
        if (_delayTimer > 0) _delayTimer -= Time.fixedDeltaTime;
    
        _timerInside += Time.fixedDeltaTime;
        for (var i = 0; i < _rbsInTrigger.Length; i++){
            if (!_rbsInTrigger[i]) continue;
            var velocity = _rbsInTrigger[i].velocity;
            velocity    *= (1 - Time.fixedDeltaTime * 5);
            var sinValue = Mathf.Sin(_timerInside * 2) * velocityRange;
            
            if (_rbsInTrigger[i].GetComponent<ActivatableObject>()){
                _rbsInTrigger[i].useGravity = false;
                velocity = sinValue * transform.up;
            } else{
                velocity.y = _rbsInTrigger[i].transform.position.y < transform.position.y && sinValue < 0
                 ? -sinValue : sinValue;
            }
            
            _rbsInTrigger[i].velocity = velocity;
        }
    }

    private void OnTriggerEnter(Collider col){
        if (col.TryGetComponent<Rigidbody>(out var rb)){
            for (var i = 0; i < _rbsInTrigger.Length; i++){
                if (_rbsInTrigger[i]) continue; 
                _rbsInTrigger[i] = rb;
                _pModule.startColor = activeColor;
                _activator.SetState(true);

                _timerInside = 0;

                if (_delayTimer <= 0 && (rb.GetComponent<ActivatableObject>() || rb.GetComponent<BeegBox>())){
                    Catch(rb);
                }
                _delayTimer = 0.5f;
                break;
            }
        }
    }

    private void OnTriggerExit(Collider col){
        if (col.TryGetComponent<Rigidbody>(out var rb)){
            var aliveCount = 0;
            for (var i = 0; i < _rbsInTrigger.Length; i++){
                if (_rbsInTrigger[i])       aliveCount++;
                if (_rbsInTrigger[i] != rb) continue;
                
                if (_rbsInTrigger[i].GetComponent<ActivatableObject>()){
                    _rbsInTrigger[i].useGravity = true;
                }
                _rbsInTrigger[i] = null;
            }
            if (aliveCount-1 == 0){
                _pModule.startColor = baseColor;
                _activator.SetState(false);
            }
        }
    }
    
    public void Catch(Rigidbody rb){
        rb.velocity *= 0.1f;
        AnimationsHelper.Instance.AddMoveTask(rb.transform, rb.transform.position, transform.position + transform.up * 5, 0.3f);
    }
    
    public bool IsActivated(){
        return _rbsInTrigger[0] != null;
    }
}
