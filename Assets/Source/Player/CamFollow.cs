using UnityEngine;

public class CamFollow : MonoBehaviour{
	[SerializeField] private Transform target;
	[SerializeField] private Transform rotationTarget;
	[SerializeField] private float damping = 100f;
	// [SerializeField] private float desiredAdditionalPositionDamping = 2f;
	[SerializeField] private float additionalPositionDamping = 2f;
	[SerializeField, Range(0f, 1f)] private float bounce = 0.9f;

    private Vector3 _additionalPosition;
    //private Vector3 _desiredAdditionalPosition;

	// private Vector3 _oldPos;
	//
	// private void Awake(){
	// 	_oldPos = transform.position;
	// }

	private void Update(){
		//var oldPos = transform.position;
        

		transform.position = Vector3.Lerp(transform.position, target.position + _additionalPosition, damping * Time.deltaTime);
		//transform.position = (1+bounce) * transform.position - bounce * _oldPos;

		transform.rotation = rotationTarget.rotation;

        _additionalPosition = Vector3.Lerp(_additionalPosition, Vector3.zero, Time.deltaTime * additionalPositionDamping);//Vector3.Lerp(_additionalPosition, _desiredAdditionalPosition, Time.deltaTime * additionalPositionDamping);
        // if (_additionalPosition.sqrMagnitude >= _desiredAdditionalPosition.sqrMagnitude-0.01f){
        //     _desiredAdditionalPosition = Vector3.Lerp(_desiredAdditionalPosition, Vector3.zero, Time.deltaTime * desiredAdditionalPositionDamping);
        // }

		//_oldPos = oldPos;
	}

    public void AddPosition(Vector3 addPosition){
        _additionalPosition += addPosition;
        _additionalPosition.y = Mathf.Clamp(_additionalPosition.y, -1.5f, 2);
    }

    public void AddPositionSmooth(Vector3 minPos, Vector3 maxPos, float duration){
        AnimationsHelper.Instance.ChangeVector(AddPosition, minPos, maxPos, duration);
    }
}
