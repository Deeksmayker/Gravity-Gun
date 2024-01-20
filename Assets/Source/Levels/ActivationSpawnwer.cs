using UnityEngine;

public class ActivationSpawnwer : MonoBehaviour{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float distance;
    [SerializeField] private float velocity;
    [SerializeField] private bool activated;
    [SerializeField] private float delay = 4f;
    
    private float _spawnTimer;

    private GameObject _spawnedPrefab;

    private void OnEnable(){
        GetComponent<ActivateTarget>().OnChangeState += HandleActivation;
        SetColor();
    }

    private void OnDisable(){
        GetComponent<ActivateTarget>().OnChangeState -= HandleActivation;
    }
    
    private void Update(){
        if (activated){
            _spawnTimer += Time.deltaTime;
            if (_spawnTimer >= delay){
                SpawnObject();
            }
        }
        
        else _spawnTimer = 0;
    }

    private void HandleActivation(bool isIt){
        if (isIt && !activated) SpawnObject();

        activated = isIt;
        SetColor();
        /*
        if (isIt){
            if (_spawnedPrefab) Destroy(_spawnedPrefab);
            _spawnedPrefab = Instantiate(prefab, transform.position + transform.forward*distance, Quaternion.identity);
            _spawnedPrefab.GetComponent<Rigidbody>().velocity = transform.forward * velocity;
        }
        */
    }
    
    private void SpawnObject(){
        if (_spawnedPrefab) Destroy(_spawnedPrefab);
        _spawnedPrefab = Instantiate(prefab, transform.position + transform.forward*distance, Quaternion.identity);
        _spawnedPrefab.GetComponent<Rigidbody>().velocity = transform.forward * velocity;
        _spawnTimer = 0;

    }
    
    private void SetColor(){
        var propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetColor("_EmissionColor", activated ? Colors.Orange : Colors.Blue);
        GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
    }
}
