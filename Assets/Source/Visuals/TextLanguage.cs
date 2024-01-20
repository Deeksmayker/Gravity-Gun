using UnityEngine;
using TMPro;

public class TextLanguage : MonoBehaviour{
    [SerializeField] private string tag;
    
    private TextMeshProUGUI _textMesh;
    
    private void OnEnable(){
        _textMesh = GetComponentInChildren<TextMeshProUGUI>();
        GameManager.Instance.OnLanguageChanged += SetText;
        SetText();
    }
    
    private void OnDisable(){
        GameManager.Instance.OnLanguageChanged -= SetText;
    }
    
    private void SetText(){
        _textMesh.text = Languages.GetTextByTag(tag);
    }
}