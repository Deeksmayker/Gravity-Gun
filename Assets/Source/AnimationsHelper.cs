using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using TMPro;

public class AnimationsHelper : MonoBehaviour{
    public static AnimationsHelper Instance;

    private List<MoveTask> _tasks = new();
    private List<TextTask> _textTasks = new();
    //private List<SetFloatTask> _setFloatTasks = new();
    
    private AudioClip _typeSound, _eraseSound;

    private void Awake(){
        if (Instance && Instance != this){
            Destroy(this);
            return;
        }

        Instance = this;
    }
    
    private void Start(){
        _typeSound = PlayerSound.Instance.GetClip("Type");
        _eraseSound = PlayerSound.Instance.GetClip("Erase");
    }

    private void FixedUpdate(){
        for (var i = 0; i < _tasks.Count; i++){
            var task = _tasks[i];

            if (task.delay > 0){
                task.delay -= Time.fixedDeltaTime;
                continue;
            }

            task.t += Time.fixedDeltaTime / task.timeToChange;
            task.action.Invoke(task.t);

            if (task.t >= 1) _tasks.RemoveAt(i);
        }

        for (var i = 0; i < _textTasks.Count; i++){
            var task = _textTasks[i];

            if (task.delay > 0){
                task.delay -= Time.fixedDeltaTime;
                continue;
            }

            task.charDelayTimer += Time.fixedDeltaTime;
            if (task.charDelayTimer < task.charDelay) continue;

            if (task.fullText == null){
                task.textObject.text = task.textObject.text.Remove(task.textObject.text.Length-1, 1);
                PlayerSound.Instance.PlaySound(_eraseSound, 0.01f, UnityEngine.Random.Range(0.9f, 1.5f));
                task.charDelayTimer -= task.charDelay;
                if (task.textObject.text.Length == 0){
                    Destroy(task.textObject.gameObject);
                    _textTasks.RemoveAt(i);
                    continue;
                }
            } else{
                task.textObject.text += task.fullText[task.textObject.text.Length];
                PlayerSound.Instance.PlaySound(_typeSound, 0.01f, UnityEngine.Random.Range(0.9f, 1.5f));
                task.charDelayTimer -= task.charDelay;

                if (task.textObject.text.Length == task.fullText.Length){
                    _textTasks.RemoveAt(i);
                    continue;
                }
            }
        }
    }

    public void AddMoveTask(Transform targetTransform1, Vector3 startPos1, Vector3 endPos1, float time1, float delay1 = 0){
        // if (delay1 <= 0 && _tasks.Select(task => task.targetTransform).ToArray().Contains(targetTransform1))
        //     return;
        _tasks.Add(new MoveTask(){
                action = (t) => targetTransform1.position = Vector3.Lerp(startPos1, endPos1, t),
                timeToChange = time1, t = 0, delay = delay1}); 
    }

    public void ChangeFloat(Action<float> changeAction, float startValue1, float endValue1, float timeToChange1, float delay1 = 0){
        _tasks.Add(new MoveTask(){
                action = (t) => changeAction.Invoke(Mathf.Lerp(startValue1, endValue1, t)),
                timeToChange = timeToChange1, t = 0, delay = delay1}); 
        //_setFloatTasks.Add(new SetFloatTask(){setAction = changeAction, startValue = startValue1, endValue = endValue1, timeToChange = timeToChange1, delay = delay});
    }

    public void ChangeVector(Action<Vector3> changeAction, Vector3 startValue1, Vector3 endValue1, float timeToChange1, float delay1 = 0){
        _tasks.Add(new MoveTask(){
                action = (t) => changeAction.Invoke(Vector3.Lerp(startValue1, endValue1, t)),
                timeToChange = timeToChange1, t = 0, delay = delay1}); 
        //_setFloatTasks.Add(new SetFloatTask(){setAction = changeAction, startValue = startValue1, endValue = endValue1, timeToChange = timeToChange1, delay = delay});
    }

    public void RotateObject(Transform targetTransform, Quaternion startRotation, Quaternion endRotation, float timeToChange1, float delay1){
        _tasks.Add(new MoveTask(){
                action = (t) => targetTransform.rotation = Quaternion.Lerp(startRotation, endRotation, t),
                timeToChange = timeToChange1, t = 0, delay = delay1}); 
    }

    public void TypeText(TextMeshProUGUI textObject1, string fullText1, float charDelay1, float delay1){
        if (fullText1.Length == 0) return;
        _textTasks.Add(new TextTask(){textObject = textObject1, fullText = fullText1, charDelay = charDelay1, delay = delay1}); 
    }

    public void RemoveText(TextMeshProUGUI textObject1, float charDelay1, float delay1){
        _textTasks.Add(new TextTask(){textObject = textObject1, charDelay = charDelay1, delay = delay1}); 
    }
}

public class MoveTask{
    public Action<float> action;
    public float timeToChange;
    public float t;
    public float delay;
}

public class TextTask{
    public TextMeshProUGUI textObject;
    public string fullText;
    public float charDelay, charDelayTimer, delay;
}
