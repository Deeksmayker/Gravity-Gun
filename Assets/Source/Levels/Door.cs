using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour{
    [SerializeField] private float distance = 10f;
    [SerializeField] private float timeToOpen = 1f;
    [SerializeField] private string levelToCheck;

    private bool _opened;

    private Vector3 _startPos;

    private Coroutine _doorCoroutine;

    private ActivateTarget _activate;

    private void Awake(){
        _activate = GetComponent<ActivateTarget>();

        _startPos = transform.position;
    }
    
    private void Start(){
        if (levelToCheck != ""){
            if (GameManager.Instance.GetLevelById(levelToCheck).Completed) Open();
        }
    }

    private void OnEnable(){
        _activate.OnChangeState += HandleActivate;
    }

    private void OnDisable(){
        _activate.OnChangeState -= HandleActivate;
    }

    private void HandleActivate(bool open){
        if (open) Open();
        else Close();
    }

    [ContextMenu("Open")]
    public void Open(){
        Debug.Assert(!_opened);
        if (_doorCoroutine != null) StopCoroutine(_doorCoroutine);
        _doorCoroutine = StartCoroutine(MoveDoor(_startPos + transform.up * distance, timeToOpen));
        _opened = true;
        GetComponent<AudioSource>().Play();
        
        if (TryGetComponent<Activator>(out var activator)){
            activator.SetState(true);
        }
    }

    [ContextMenu("Close")]
    public void Close(){
        Debug.Assert(_opened);
        if (_doorCoroutine != null) StopCoroutine(_doorCoroutine);
        _doorCoroutine = StartCoroutine(MoveDoor(_startPos, timeToOpen/2f));
        _opened = false;
        
        if (TryGetComponent<Activator>(out var activator)){
            activator.SetState(false);
        }

    }

    private IEnumerator MoveDoor(Vector3 pos, float time){
        var t = 0f;
        var startPos = transform.position;

        while (t < 1){
            t += Time.deltaTime / time;
            transform.position = Vector3.Lerp(startPos, pos, t*t);
            yield return null;
        }
    }
}
