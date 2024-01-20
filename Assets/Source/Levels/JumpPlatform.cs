using UnityEngine;

public class JumpPlatform : MonoBehaviour{
    [SerializeField] private float distance = 5f;
    [SerializeField] private float timeToFullyActivate = 0.2f;
    
    private Vector3 _startPos, _endPos;
    private bool _moving;
    private bool _movingUp;

    private void Awake(){
        _startPos = transform.position;
        _endPos = transform.position + transform.up * distance;
    }

    private void OnEnable(){
        GetComponent<ActivateTarget>().OnChangeState += HandleActivation;
    }

    private void OnDisable(){
        GetComponent<ActivateTarget>().OnChangeState -= HandleActivation;
    }

    private void HandleActivation(bool isIt){
        if (!isIt || _moving) return;

        // _moving = true;
        // _movingUp = true;
        //Debug.Log("im activated");
        
        AnimationsHelper.Instance.AddMoveTask(transform, _startPos, _endPos, timeToFullyActivate);

        //Return back
        AnimationsHelper.Instance.AddMoveTask(transform, _endPos, _startPos, timeToFullyActivate * 3, timeToFullyActivate * 2);
    }
}
