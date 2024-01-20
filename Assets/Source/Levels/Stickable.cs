using UnityEngine;

public class Stickable : MonoBehaviour{
    [SerializeField] private int tag;
    
    public int GetTag() => tag;
}
