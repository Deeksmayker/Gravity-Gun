using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConsoleController : MonoBehaviour{
	private bool _showConsole;

	private string _input;

	private List<ConsoleCommand> _commands;

	private WeaponGiver _weaponGiver;

    private GameObject _boxPrefab;

	private void Awake(){
		_weaponGiver = FindObjectOfType<WeaponGiver>();

        _boxPrefab = Resources.Load(ResPath.PhysicsObjects + "BaseCubic") as GameObject;

		SetupCommands();
	}

	public void SetConsoleToggle(bool show){
		_showConsole = show;
	}

	public void SetEnterInput(){
		HandleInput();
	}

	private void OnGUI(){
		if (!_showConsole) return;

		var y = 0;

		GUI.Box(new Rect(0, y, Screen.width, 30), "");
		GUI.backgroundColor = new Color(0, 0, 0, 0);
		
		GUI.SetNextControlName("Console");
		_input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), _input);
		GUI.FocusControl("Console");
	}

	private void SetupCommands(){
		_commands = new()
        {
            new ConsoleCommand("give_weapon", "Gives weapon LOL", (weaponName) => _weaponGiver.SetCurrentWeapon(weaponName)),
            new ConsoleCommand("level", "Load level by name", (levelName) => GameManager.Instance.LoadLevelById(levelName)),
            new ConsoleCommand("reset_physics", "Reset all physics objects (mostly boxes)", (a) => GameManager.Instance.ResetPhysics()),
            new ConsoleCommand("spawn", "Spawn obj", (objectName) => ObjectsSpawner.Instance.SpawnObject(objectName)),
            new ConsoleCommand("reload", "Reload level", (a) => SceneManager.LoadScene(SceneManager.GetActiveScene().name)),
            new ConsoleCommand("next", "Next level", (a) => GameManager.Instance.LoadNextLevel())
        };
	}

	private void HandleInput(){
		string[] properties = _input.Split(' ');

		for (var i = 0; i < _commands.Count; i++){
			if (!_input.Contains(_commands[i].Name))
				continue;
			var haveAdditionalProperty = properties.Length > 1;
			_commands[i].Perform(properties[haveAdditionalProperty ? 1 : 0]);
		}
	}

    private void SpawnObject(string name){
        if (name == "cubic"){
            Instantiate(_boxPrefab, transform.position + transform.forward * 2, Quaternion.identity);
        }
    }
}

public struct ConsoleCommand{
	public string Name;
	public string Description;

	public Action<string> action;

	public ConsoleCommand(string name, string description, Action<string> action){
		Name = name;
		Description = description;
		this.action = action;
	}

	public void Perform(string what){
		action.Invoke(what);
	}
}
