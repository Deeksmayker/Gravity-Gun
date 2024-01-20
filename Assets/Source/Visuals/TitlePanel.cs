using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : MonoBehaviour{
    private Image _image;
    
    private float _timer = 1f;
    private float _delay = 12f;
    
    private void Awake(){
        _image = GetComponent<Image>();
    }
    
    private void Update(){
        _delay -= Time.deltaTime;
        if (_delay > 0) return;
        _timer -= Time.deltaTime * 0.3f;
        var color = _image.color;
        color.a = _timer;
        _image.color = color;
        
        if (_timer <= 0){
            gameObject.SetActive(false);
            return;
        }
    }
    
    public void SetDelay(float value){
        _delay = value;
    }
}
