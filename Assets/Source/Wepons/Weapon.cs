using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform gunTip;

    public Vector3 GetGunTipPos() => gunTip.position;
    public Transform GetDirectionTarget() => transform.parent;
}
