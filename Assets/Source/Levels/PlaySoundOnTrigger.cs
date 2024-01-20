using UnityEngine;

public class PlaySoundOnTrigger : MonoBehaviour{
    [SerializeField] private string path;
    [SerializeField] private float volume;
    [SerializeField] private float duration;
    [SerializeField] private bool onlyOnce;
    
    private bool _played;
    
    private void OnTriggerEnter(Collider col){
        if (onlyOnce && _played) return;
        PlayerSound.Instance.PlaySound(PlayerSound.Instance.GetClip(path), volume, 1, duration);
        _played = true;
    }
}
