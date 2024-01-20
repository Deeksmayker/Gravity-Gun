using UnityEngine;

public class CameraHelper : MonoBehaviour{
    public static CameraHelper Instance;

    private CameraShake _cameraShake;

    private void Awake(){
        if (Instance){
            Destroy(Instance);
            Instance = null;
        }

        _cameraShake = GetComponentInChildren<CameraShake>();
        Instance = this;
    }

    public void ShakeCamera(float stress){
        _cameraShake.InduceStress(stress);
    }
}
