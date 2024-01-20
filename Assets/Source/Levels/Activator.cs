using UnityEngine;
using System.Linq;

public class Activator : MonoBehaviour{
    [SerializeField] private string tag = "UNDEFINED";

    private bool _activated;

    private ActivateTarget[] _targets;
    private LineRenderer[] _lineRenderers;

    private LineRenderer _lineRendererPrefab;
    
    private AudioClip _activationAudio;

    private void Awake(){
        _targets = FindObjectsOfType<ActivateTarget>().Where(t => t.GetTag() == tag).ToArray();
        _lineRenderers = new LineRenderer[_targets.Length];

        _lineRendererPrefab = (Resources.Load(ResPath.Levels + "ActivatorLineRenderer") as GameObject).GetComponent<LineRenderer>();
        _activationAudio = Resources.Load(ResPath.Audio + "Activator") as AudioClip;
    }

    public void SetState(bool value){
        if (_activated == value) return;
        _activated = value;
        for (var i = 0; i < _targets.Length; i++){
            _targets[i].SetState(value);

            if (value){
                _lineRenderers[i] = Instantiate(_lineRendererPrefab);
                _lineRenderers[i].positionCount = 2;
                _lineRenderers[i].SetPosition(0, transform.position);
                _lineRenderers[i].SetPosition(1, _targets[i].GetStartPos());
                
                if (TryGetComponent<AudioSource>(out var source)){
                    source.Play();
                }
            } else{
                Destroy(_lineRenderers[i].gameObject);
            }

        }
    }
}
