using UnityEngine;

public class DragRigidbody : MonoBehaviour
{
	public float force = 600;
	public float damping = 6;

	Transform jointTrans;
	float dragDepth;

	private void Update()
	{
		if (Input.GetMouseButtonDown (0))
		{
			HandleInputBegin(Input.mousePosition);
		}

		if (jointTrans && Input.GetMouseButton(0))
		{
			HandleInput(Input.mousePosition);
		}

		if (jointTrans && Input.GetMouseButtonUp(0))
		{
			HandleInputEnd(Input.mousePosition);
		}
	}

	public void HandleInputBegin (Vector3 screenPosition)
	{
		var ray = Camera.main.ScreenPointToRay (screenPosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 1000, Layers.PhysicsObjects))
		{
			if (hit.rigidbody)
			{
				dragDepth = CameraPlane.CameraToPointDepth (Camera.main, hit.point);
				jointTrans = AttachJoint (hit.rigidbody, hit.point);
			}
		}
	}

	public void HandleInput (Vector3 screenPosition)
	{
		if (jointTrans == null)
			return;
		var worldPos = Camera.main.ScreenToWorldPoint (screenPosition);
		jointTrans.position = CameraPlane.ScreenToWorldPlanePoint (Camera.main, dragDepth, screenPosition);
	}

	public void HandleInputEnd (Vector3 screenPosition)
	{
		Destroy (jointTrans.gameObject);
	}

	Transform AttachJoint (Rigidbody rb, Vector3 attachmentPosition)
	{
		GameObject go = new GameObject("Attachment Point");
		go.transform.position = attachmentPosition;

		var newRb = go.AddComponent<Rigidbody> ();
		newRb.isKinematic = true;

		var joint = go.AddComponent<ConfigurableJoint> ();
		joint.connectedBody = rb;
		joint.configuredInWorldSpace = true;
		joint.xDrive = NewJointDrive (force, damping);
		joint.yDrive = NewJointDrive (force, damping);
		joint.zDrive = NewJointDrive (force, damping);
		joint.slerpDrive = NewJointDrive (force, damping);
		joint.rotationDriveMode = RotationDriveMode.Slerp;

		return go.transform;
	}

	private JointDrive NewJointDrive (float force, float damping)
	{
		JointDrive drive = new JointDrive ();
		drive.mode = JointDriveMode.Position;
		drive.positionSpring = force;
		drive.positionDamper = damping;
		drive.maximumForce = Mathf.Infinity;
		return drive;
	}
}
