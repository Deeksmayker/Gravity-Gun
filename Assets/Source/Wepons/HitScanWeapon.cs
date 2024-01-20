using UnityEngine;
using System;

public class HitScanWeapon : MonoBehaviour, IFire
{
	public event Action OnFire;

	[SerializeField] private float fireRate;
	[SerializeField] private float spread;
	[SerializeField] private float damage;
	[SerializeField] private float power;
	[SerializeField] private float shootRadius = 0.2f;

	[SerializeField] private ParticleSystem shootParticles;
	private ParticleSystem _hitParticles;

	private float _reloadTimer;

	private bool _input;
	private bool _needToFire;

	private Weapon _weapon;

	private void Awake()
	{
		_weapon = GetComponent<Weapon>();
		_hitParticles = (Resources.Load(ResPath.PhysicsObjects + "DustParticlesBig") as GameObject).GetComponent<ParticleSystem>();
	}

	private void Update()
	{
		if (_reloadTimer > 0)
			_reloadTimer -= Time.deltaTime;

		if (_input && _reloadTimer <= 0)
		{
			_needToFire = true;
			_reloadTimer = 1f/fireRate;
		}

		else if (_needToFire)
		{
			Fire();
		}
	}

	private void Fire()
	{
		_needToFire = false;	
		var direction = (_weapon.GetDirectionTarget().forward + UnityEngine.Random.insideUnitSphere * spread).normalized;
		OnFire?.Invoke();

		shootParticles.transform.rotation = Quaternion.LookRotation(direction);
		shootParticles.Play();
		
		//var ray = Camera.main.ScreenPointToRay (new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
		RaycastHit hit;
		if (Physics.SphereCast (_weapon.GetDirectionTarget().position, shootRadius, direction, out hit, 1000, Layers.PhysicsObjects | Layers.Environment | Layers.Interactable | Layers.PhysicsInteractable))
		{
			if (hit.rigidbody)
			{
				hit.rigidbody.AddForceAtPosition(power * direction, hit.point);
			}
			var physicsObject = hit.collider.GetComponentInParent<PhysicsObject>();
			Instantiate(_hitParticles, hit.point, Quaternion.identity);

			if (physicsObject){
				physicsObject.EnableSmoothOnTime(1);
			}
			
			if (hit.collider.TryGetComponent<Grenade>(out var grenade)){
			    grenade.Explode();   
			}
		}
	}

	public void SetInput(bool input)
	{
		_input = input;
	}
}
