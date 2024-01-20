using UnityEngine;

public class ManekenController : MonoBehaviour{

    public struct ManekenInfo{
        public Maneken maneken;
        public Vector3 startPosition;
        public bool died;
    }

    private ManekenInfo[] _manekensInfo;
    
    private float _physicsDelay;
    private float _checkDelay = 0.5f;
    private float _checkTimer;
    
    private int _aliveCount;
    
    private bool _setuped;
    
    private void Update(){
        if (_physicsDelay > 0) _physicsDelay -= Time.deltaTime;
        if (GameManager.Instance.LevelTime < 3 || _physicsDelay > 0) return;
        
        if (!_setuped){
            var manekensOnScene = FindObjectsOfType<Maneken>();
            _manekensInfo = new ManekenInfo[manekensOnScene.Length];
            for (int i = 0; i < manekensOnScene.Length; i++){
                _manekensInfo[i].maneken = manekensOnScene[i];
                _manekensInfo[i].startPosition = manekensOnScene[i].transform.position;
            }
            _aliveCount = _manekensInfo.Length;
            _setuped = true;
            return;
        }
        _checkTimer += Time.deltaTime;
        if (_checkTimer < _checkDelay) return;
        _checkTimer -= _checkDelay;
        
        for (int i = 0; i < _manekensInfo.Length; i++){
            if (!_manekensInfo[i].died && _manekensInfo[i].maneken.transform.position != _manekensInfo[i].startPosition){
                _aliveCount--;
                Debug.Assert(_aliveCount >= 0);
                UiLogger.Instance.LogLong(Languages.GetTextByTag("ManekenDown") + _aliveCount, 0.03f, Random.Range(0, 0.2f), 3);
                _manekensInfo[i].died = true;
            }
        }
    }
    
    public void ResetDelay(){
        _physicsDelay = 3f;
    }
}



