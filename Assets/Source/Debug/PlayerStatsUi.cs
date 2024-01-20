using TMPro;
using UnityEngine;

public class PlayerStatsUi : MonoBehaviour{
	[SerializeField] private TextMeshProUGUI velocityValue;
	[SerializeField] private TextMeshProUGUI horizontalSpeedValue;
	[SerializeField] private TextMeshProUGUI groundedValue;

	private PlayerPhysicsController _mover;

	private void Awake(){
		_mover = FindObjectOfType<PlayerPhysicsController>();
	}

	private void Update(){
		velocityValue.text = _mover.GetVelocity().ToString();
		horizontalSpeedValue.text = _mover.GetHorizontalSpeed().ToString();
		groundedValue.text = _mover.IsGrounded().ToString();
	}
}
