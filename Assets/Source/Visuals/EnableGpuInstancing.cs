using UnityEngine;

public class EnableGpuInstancing : MonoBehaviour{
    private void Awake(){
        var mesh = GetComponent<MeshRenderer>();
        var propertyBlock = new MaterialPropertyBlock();
        mesh.SetPropertyBlock(propertyBlock);
    }
}

