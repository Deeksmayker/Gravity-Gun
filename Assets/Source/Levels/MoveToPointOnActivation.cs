using UnityEngine;

public class MoveToPointOnActivation : MonoBehaviour{
    [SerializeField] private Transform point;

    private Vector3 _startPos;

    private ActivateTarget _activateTarget;

    private void Awake(){
        _startPos = transform.position;
        _activateTarget = GetComponent<ActivateTarget>();
    }

    private void OnEnable(){
        _activateTarget.OnChangeState += HandleActivate;
    }

    private void OnDisable(){
        _activateTarget.OnChangeState -= HandleActivate;
    }

    private void HandleActivate(bool isIt){
        if (isIt){
            AnimationsHelper.Instance.AddMoveTask(transform, _startPos, point.position, 1f);
        }
    }
}
