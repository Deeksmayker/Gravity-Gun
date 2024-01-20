using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerSound : MonoBehaviour{
    public static PlayerSound Instance;
    
    private AudioSource _baseAudioSource, _base3DAudioSource;
    
    private List<Sound> _sounds = new();
    
    private void Awake(){
        if (Instance && Instance != this){
            Destroy(Instance);
            Instance = null;
        }
        
        _baseAudioSource = (Resources.Load(ResPath.Audio + "BaseAudioSource") as GameObject).GetComponent<AudioSource>();
        _base3DAudioSource = (Resources.Load(ResPath.Audio + "Base3DAudioSource") as GameObject).GetComponent<AudioSource>();
        Instance = this;
    }
    
    private void Update(){
        for (int i = 0; i < _sounds.Count; i++){
            _sounds[i].lifeTime -= Time.deltaTime;
            if (_sounds[i].lifeTime <= 0){
                Destroy(_sounds[i].source.gameObject);
                _sounds.RemoveAt(i);
            }
        }
    }
    
    public void PlaySound(AudioClip clip, float volume = 1, float pitch = 1, float lifeTime1 = 1){
        var source1 = Instantiate(_baseAudioSource, transform);
        source1.volume = volume;
        source1.pitch = pitch;
        source1.clip = clip;
        source1.Play();
        _sounds.Add(new Sound(){source = source1, lifeTime = lifeTime1});
    }
    
    public void PlaySoundAtPosition(AudioClip clip, Vector3 position, float volume = 1, float pitch = 1, float lifeTime1 = 1){
        var source1 = Instantiate(_base3DAudioSource, position, Quaternion.identity);
        source1.volume = volume;
        source1.pitch = pitch;
        source1.clip = clip;
        source1.Play();
        _sounds.Add(new Sound(){source = source1, lifeTime = lifeTime1});
    }

    public AudioClip GetClip(string name){
        var clip = Resources.Load(ResPath.Audio + name) as AudioClip;
        if (clip == null) Debug.LogError("wrong clip name");
        return clip;
    }
    
    public AudioClip[] GetAllClips(string name){
        var clips = Resources.LoadAll(ResPath.Audio)
        .Where(o => o.name.Contains(name))
        .Select(o => o as AudioClip)
        .ToArray();
        if (clips.Length == 0) Debug.LogError("no such clips alo");
        return clips;
    }
}

public class Sound{
    public AudioSource source;
    public float lifeTime;
}
