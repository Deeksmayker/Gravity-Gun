using UnityEngine;
using System;

public class ActivateTarget : MonoBehaviour{
    [SerializeField] private string tag = "UNDEFINED";
    [SerializeField] private int activationsToActivate = 1;
    [SerializeField] private bool canActivateWithGravity;
    [SerializeField] private bool onlyActivate;
    [SerializeField] private Transform customStartPos;

    [SerializeField] private GameObject[] objectsToEnable;

    private int _activationCount;

    private Vector3 _startPos;

    public event Action<bool> OnChangeState;
    
    private AudioClip _activationSound;

    private void Awake(){
        _startPos = customStartPos ? customStartPos.position : transform.position;
    }
    
    private void Start(){
        _activationSound = PlayerSound.Instance.GetClip("Activation");
    }

    public void SetState(bool value){
        _activationCount += value ? 1 : -1;
        //Debug.Assert(_activationCount >= 0 && _activationCount <= activationsToActivate);
        if (_activationCount == activationsToActivate && value){
            OnChangeState?.Invoke(true);
            EnableObjects();
            PlayerSound.Instance.PlaySoundAtPosition(_activationSound, transform.position, 0.5f);
        }
        if (_activationCount == activationsToActivate-1 && !value && !onlyActivate)
            OnChangeState?.Invoke(false);
        //OnChangeState?.Invoke(value);
    }

    public string GetTag() => tag;

    public bool CanWithGravity() => canActivateWithGravity;

    private void EnableObjects(){
        for (var i = 0; i < objectsToEnable.Length; i++){
            objectsToEnable[i].SetActive(true);
        }
    }

    public Vector3 GetStartPos() => _startPos;
}

