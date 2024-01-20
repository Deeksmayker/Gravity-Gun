using UnityEngine;

public class PlayerLocator : MonoBehaviour
{
    [SerializeField] private float locatePlayerRange;

    private PlayerUnit _targetPoint;

    private void Awake()
    {
        _targetPoint = FindObjectOfType<PlayerUnit>();
    }

    public bool IsPlayerVisible()
    {
        if (Physics.Raycast(transform.position, GetDirectionToPlayerNorm(), out var hit, locatePlayerRange, Layers.PlayerBase | Layers.Environment))
        {
            return hit.transform.GetComponent<PlayerUnit>();
        }
        return false;
    }

    public Vector3 GetDirectionToPlayerNorm()
    {
        var direction = (_targetPoint.transform.position - transform.position).normalized;
        return direction;
    }
} 
