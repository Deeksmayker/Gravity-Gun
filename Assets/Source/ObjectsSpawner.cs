using UnityEngine;
using System.Collections.Generic;

public class ObjectsSpawner : MonoBehaviour{
    public static ObjectsSpawner Instance;
    
    private GameObject _cubic;
    private GameObject _cubicNoGravity;
    private GameObject _sphere;
    private GameObject _maneken;
    private GameObject _manekenNoGravity;
    private GameObject _plank;
    private GameObject _plankBig;
    private GameObject _stickman;
    private GameObject _stickmanNoAi;
    private GameObject _grenade;
    
    private Dictionary<string, GameObject> _objectsByName;
    private List<GameObject> _objectsBuffer = new();
    
    private AudioClip _undoClip, _failUndoClip;
    
    private PlayerInputHandler _input;
    
    private void Awake(){
        if (Instance && Instance != this){
            Destroy(Instance);
            Instance = null;
            return;
        }
        
        Instance = this;
        
        _input = GetComponentInParent<PlayerInputHandler>();
        
        _cubic            = Resources.Load(ResPath.PhysicsObjects + "BaseCubic") as GameObject;
        _cubicNoGravity   = Resources.Load(ResPath.PhysicsObjects + "BaseCubicNoGravity") as GameObject;
        _sphere           = Resources.Load(ResPath.PhysicsObjects + "BaseSphere") as GameObject;
        _maneken          = Resources.Load(ResPath.PhysicsObjects + "Maneken") as GameObject;
        _manekenNoGravity = Resources.Load(ResPath.PhysicsObjects + "ManekenNoGravity") as GameObject;
        _stickman         = Resources.Load(ResPath.PhysicsObjects + "Stickman") as GameObject;
        _stickmanNoAi     = Resources.Load(ResPath.PhysicsObjects + "StickmanNoAi") as GameObject;
        _grenade          = Resources.Load(ResPath.Weapons + "Grenade") as GameObject;
        _plank            = Resources.Load(ResPath.Levels + "Plank") as GameObject;
        _plankBig         = Resources.Load(ResPath.Levels + "PlankBig") as GameObject;
        
        _objectsByName = new Dictionary<string, GameObject>{
            {"cubic",        _cubic},
            {"cubicng",      _cubicNoGravity},
            {"sphere",       _sphere},
            {"plank",        _plank},
            {"plankbig",     _plankBig},
            {"maneken",      _maneken},
            {"manekenng",    _manekenNoGravity},
            {"stickman",     _stickman},
            {"stickmannoai", _stickmanNoAi},
            {"grenade",      _grenade}
        };
        
        _undoClip = Resources.Load(ResPath.Audio + "Activation") as AudioClip;
        _failUndoClip = Resources.Load(ResPath.Audio + "FailUndo") as AudioClip;
    }
    
    private void Update(){
        if (GameManager.Instance.GetCurrentLevelIndex() != 0 && !GameManager.Instance.IsGameCompleted())
            return;
            
    
        if (_input.IsMenuButtonPressed("Undo")){
            for (int i = _objectsBuffer.Count - 1; i >= 0; i--){
                if (!_objectsBuffer[i]) _objectsBuffer.RemoveAt(i);
            }
            
            if (_objectsBuffer.Count <= 0){
                UiLogger.Instance.LogLong(Languages.GetTextByTag("FUndo"), 0.01f, 0f, 1f);
                
                PlayerSound.Instance.PlaySound(_failUndoClip, 0.05f, Random.Range(0.7f, 1.3f));
            } else{
                UiLogger.Instance.LogLong(Languages.GetTextByTag("Undo"), 0.02f, 0f, 1.5f);
                Destroy(_objectsBuffer[_objectsBuffer.Count - 1]);
                _objectsBuffer.RemoveAt(_objectsBuffer.Count - 1);
                
                PlayerSound.Instance.PlaySound(_undoClip, 0.03f, Random.Range(1.5f, 2f));
            }
        }
    }
    
    public void SpawnObject(string name){
        if (!_objectsByName.ContainsKey(name)) return;
        
        var dir = Camera.main.transform.forward * 5;
        dir.y = Mathf.Clamp(dir.y, 0, 10);
        _objectsBuffer.Add(Instantiate(_objectsByName[name], Camera.main.transform.position + dir, Quaternion.identity));
    }
}
