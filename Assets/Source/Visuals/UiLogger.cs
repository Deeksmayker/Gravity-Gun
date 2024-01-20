using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UiLogger : MonoBehaviour{
    [SerializeField] private float longLifeTime, shortLifeTime;
    [SerializeField] private Transform bigLogTransform;

    public static UiLogger Instance;

    [SerializeField] private TextMeshProUGUI baseTextBlock;
    [SerializeField] private TextMeshProUGUI bigTextBlock;

    public void Start(){
        if (Instance){
            Destroy(Instance);
            Instance = null;
        }

        Instance = this;
    }

    // private void Update(){
    //     for (int i = 0; i < _logs.Count; i++){
    //         if (Time.time > _logs[i].lifeTime + _logs[i].appearTime){
    //             Destroy(_logs[i].textObject);
    //             _logs.RemoveAt(i);
    //         }
    //     }
    // }

    public void LogLong(string text, float charDelay = 0, float delay = 0, float lifeTime = 5){
        var textBlock = Instantiate(baseTextBlock, transform);
        textBlock.text = "";
        AnimationsHelper.Instance.TypeText(textBlock, text, charDelay, delay);

        AnimationsHelper.Instance.RemoveText(textBlock, 0.05f, lifeTime);
    }
    
    public void BigLog(string text, float charDelay = 0, float delay = 0, float lifeTime = 5){
        var textBlock = Instantiate(bigTextBlock, bigLogTransform);
        textBlock.text = "";
        AnimationsHelper.Instance.TypeText(textBlock, text, charDelay, delay);

        AnimationsHelper.Instance.RemoveText(textBlock, 0.05f, lifeTime);
    }
}

