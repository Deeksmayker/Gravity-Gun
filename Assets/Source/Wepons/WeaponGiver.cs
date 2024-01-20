using UnityEngine;
using System;

public class WeaponGiver : MonoBehaviour
{
	public event Action OnWeaponChange;

	[SerializeField] private string holdingWeapon = "gravity";

	private string[] _weaponNames = new[]{"pistol1", "gravity", "automata1"};
	private string _currentWeaponName;

	private Weapon _currentHoldingWeapon;

	private void Start()
	{
		_currentHoldingWeapon = GetComponentInChildren<Weapon>();

		SetCurrentWeapon(holdingWeapon);
	}

	public void SetCurrentWeapon(string name)
	{
		for (var i = 0; i < _weaponNames.Length; i++)
		{
			if (name.Equals(_weaponNames[i]))
			{
				SpawnWeapon(name);
				return;
			}
		}
		//Debug.LogWarning("No such name maaan");
	}

	public void SpawnWeapon(string name)
	{
		if (_currentHoldingWeapon){
			//_currentHoldingWeapon.GetComponent<IFire>().Dispose();
			Destroy(_currentHoldingWeapon.gameObject);
		}
		_currentHoldingWeapon = Instantiate((Resources.Load(ResPath.Weapons + "WeaponG" + name) as GameObject).GetComponent<Weapon>(), transform);
		OnWeaponChange?.Invoke();
	}
}

public static class WeaponNames
{
	public static string Pistol1 {get;} = "pistol1";
	public static string GravityGun {get;} = "gravity";
	public static string Automata1 {get;} = "automata1";
}
