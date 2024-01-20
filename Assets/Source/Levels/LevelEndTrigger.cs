using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndTrigger : MonoBehaviour{
    [SerializeField] private bool byName;
	[SerializeField] private string nextLevelName = "LIMBO";

	private void OnTriggerEnter(Collider col){
	    GameManager.Instance.SetCurrentLevelCompleted();
        if (byName){
            GameManager.Instance.LoadLevelById(nextLevelName);
        } else{
            GameManager.Instance.LoadNextLevel();
        }
	}
}
