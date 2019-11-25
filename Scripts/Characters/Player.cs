using UnityEngine;

public class Player : Character
{
	public IntEvent healthChanged;
	public IntEvent ammoChanged;
	private int ammo;

	private readonly int maxAmmo = 25;
	private readonly float ammoRegenRate = 0.5f;
	private float ammoRegen;

	private void Awake()
	{
		ammo = maxAmmo;
		healthChanged.Invoke((int)health);
		ammoChanged.Invoke(ammo);
	}


	protected override void Update()
	{
		ammoRegen += ammoRegenRate * Time.deltaTime * 2;
		if (ammoRegen > 1)
		{
			if (Ammo < maxAmmo)
			{
				Ammo++;
			}
			ammoRegen--;
		}
		base.Update();
	}

	public int Ammo
	{
		get
		{
			return ammo;
		}
		set
		{
			ammo = value;
			ammoChanged.Invoke(ammo);
		}
	}
	public override float Health
	{
		get { return health; }
		set
		{
			health = value;
			healthChanged.Invoke((int)health);
		}
	}


	public override void TakeDamage(float damage)
	{
		Health -= damage;
		AudioManager.instance.PlayPitched("Impact2", 1.5f, true);
	}

	public override void TakeDamage<T>(float damage, T sender)
	{
		Health -= damage;
		if(sender is Turret)
		{
		    AudioManager.instance.PlayRepeatedly("Impact3", 0.7f);
		}
	}
}
