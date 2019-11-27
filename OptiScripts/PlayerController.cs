using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
	// Amount of thrust etc
	[SerializeField, Range(500f, 2000f)] private float thrustForce = 1000f;
	[SerializeField, Range(100f, 500f)] private float rotationSpeed = 200f;
	[SerializeField, Range(1.5f, 20f)] private float boostMultiplyer = 4f;
	[SerializeField, Range(0.1f, 1f)] private float turnSpeedMult = 0.5f;

    // TODO ObjectPool
    public PoolableObject type;
    private GameObject temp;
    private GameObject temp0;

	// Engine particle effect
	public GameObject burnParticle;
	public Transform leftEngine;
	public Transform rightEngine;

	public Animator anim;
	private Light[] engineLight;

	// Afterburner
	public bool isBurning = false;
	public float thrust = 0f;
	private AudioLowPassFilter audioFilter;
	private float cutoffTarget;
	// Magnet
	[SerializeField] private MagnetData magData = null;

	// Missiles
	[SerializeField] private MissileTargetFinder targetFinder = null;

	// Machinegun
	[SerializeField] private Gun machineGun = null;
	public float rateOfFire = 1f;
	private float lastFire = 0;

	// Camera
	private CameraScript cameraScript;

	// Shield
	[SerializeField] private GameObject shield = null;
    [SerializeField] private Shield shieldScript = null;
    private Shield s = null;

	private float thrustInput, rotationInput;
	public float dragCoefficient;
	private Rigidbody2D rB;
	private Player player;
	private float[] speeds;
	private void Awake()
	{
        type = PoolableObject.BurnParticle;

		speeds = new float[6];

		cameraScript = Camera.main.GetComponent<CameraScript>();
		player = GetComponent<Player>();
		audioFilter = GetComponent<AudioLowPassFilter>();
		cutoffTarget = audioFilter.cutoffFrequency;
		engineLight = GetComponentsInChildren<Light>();
		anim = GetComponent<Animator>();
		rB = GetComponent<Rigidbody2D>();
		if (targetFinder == null)
			targetFinder = GetComponent<MissileTargetFinder>();
		if (machineGun == null)
			machineGun = GetComponent<Gun>();
		if (shield == null)
			shield = transform.Find("Shield")?.gameObject;
        if (shieldScript == null)
            shieldScript = shield.GetComponent<Shield>();
        s = shield.GetComponent<Shield>();


        Assert.IsNotNull(anim, "Animator not found");
		Assert.IsNotNull(player, "Player script not found");
		Assert.IsNotNull(rB, "Rigidbody not found on player");
		Assert.IsNotNull(leftEngine, "Left engine transform not found");
		Assert.IsNotNull(rightEngine, "Right engine transform not found");
		Assert.IsNotNull(magData, "MagnetData script not found");
		Assert.IsNotNull(targetFinder, "MissileTargetFinder script not found");
		Assert.IsNotNull(shield, "Could not find shield");
        Assert.IsNotNull(shieldScript, "Shieldscript not found");
	}


	private void Update()
	{
		PlayerInput();
	}

	private void FixedUpdate()
	{
		// Todo physics and drag
		PlayerMovement();
		// Apply drag
		float dragX = dragCoefficient * rB.velocity.x * rB.velocity.x;
		float dragY = dragCoefficient * rB.velocity.y * rB.velocity.y;
		var velo = rB.velocity;
		velo.x *= 1 - dragX;
		velo.y *= 1 - dragY;

		rB.velocity = velo;

		for (int i = speeds.Length - 1; i > 0; --i)
		{
			speeds[i] = speeds[i - 1];
		}
		speeds[0] = rB.velocity.magnitude;

		if (cutoffTarget - audioFilter.cutoffFrequency < 0.5f || cutoffTarget - audioFilter.cutoffFrequency > 0.5f)
			if (audioFilter.cutoffFrequency != cutoffTarget)
			{
				if (audioFilter.cutoffFrequency > cutoffTarget)
				{
					audioFilter.cutoffFrequency -= 5f;
				}
				else
				{
					audioFilter.cutoffFrequency += 5f;
				}
			}

	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// Camera shake and impact sound
		float avg = 0;
		foreach (var item in speeds)
		{
			avg += item;
		}
		avg /= speeds.Length;
		if (avg > 2.5f)
		{
			if (collision.gameObject.tag != "LaserShot" &&
                collision.gameObject.tag != "Bullet" &&
                collision.gameObject.tag != "Cargo" &&
                collision.gameObject.tag != "Bounds" &&
                collision.gameObject.tag != "Debris")
			{
				cameraScript.ShakeScreen(Mathf.Log10(avg + 1) / 5f, Mathf.Log10(avg + 1) / 3f);
                if (shieldScript.toggled)
                    shieldScript.TakeDamage(2f * avg);
                else
                    player.TakeDamage(2f * avg);
			}
			AudioManager.instance.PlayPitched("Impact1", 1.2f - avg / 15);
		}
	}

	private void PlayerInput()
	{
		thrustInput = Input.GetAxisRaw("Vertical");
		rotationInput = Input.GetAxisRaw("Horizontal");
		if (thrustInput <= 0 && !isBurning)
		{
			anim.SetBool("isIdle", true);
			cutoffTarget = 50f;
		}
		else
		{
			cutoffTarget = 200f;
			anim.SetBool("isIdle", false);
		}

		// Engage afterburner
		if (Input.GetButton("Boost"))
		{
			cutoffTarget = 600;
			isBurning = true;
			anim.SetBool("isBurning", true);

			// Check if need to fix later
			engineLight[0].intensity = 50f;
			engineLight[1].intensity = 50f;
		}
		else
		{
			isBurning = false;
			anim.SetBool("isBurning", false);
			engineLight[0].intensity = 5f;
			engineLight[1].intensity = 5f;
		}

		if (Input.GetButtonDown("Drop"))
		{
			ReleaseLastMagnetizedObject();
		}

		if (Input.GetButtonDown("Missile"))
		{
			FireMissiles();
		}

		if (Input.GetAxisRaw("Gun") > 0)
		{
			if (lastFire + 1f/rateOfFire < Time.time)
			{
				Shoot();
				lastFire = Time.time;
			}
		}

		if (Input.GetButtonDown("Shield"))
		{
			ToggleShield();
		}
	}

	private void PlayerMovement()
	{
		if (!isBurning)
		{
			thrust = thrustForce;
		}
		else
		{
			thrust = thrustForce * boostMultiplyer;
		}

		if (thrustInput > 0 || isBurning)
		{
			rB.AddRelativeForce(Vector2.up * thrust * Time.deltaTime);

            // TODO ObjectPool
            temp = ObjectPool.instance.GetObjectForType(type);
            temp.transform.position = leftEngine.position;
            temp.transform.rotation = leftEngine.rotation;

            temp0 = ObjectPool.instance.GetObjectForType(type);
            temp0.transform.position = rightEngine.position;
            temp0.transform.rotation = rightEngine.rotation;

        } else { PoolMe(); } 

        if (rotationInput != 0)
		{
			if (thrustInput > 0 || isBurning)
			{
				if (isBurning)
				{
					transform.Rotate(Vector3.forward * -rotationInput * rotationSpeed * turnSpeedMult * turnSpeedMult * Time.deltaTime);
				}
				else
				{
					transform.Rotate(Vector3.forward * -rotationInput * rotationSpeed * turnSpeedMult * Time.deltaTime);
				}
			}
			else
			{
				transform.Rotate(Vector3.forward * -rotationInput * rotationSpeed * Time.deltaTime);
			}
		}
	}

	private void ReleaseLastMagnetizedObject()
	{
		magData.ReleaseLastMagnetized();
	}

	private void FireMissiles()
	{
        AudioManager.instance.Play("Missile");
        if (targetFinder.GetNearestTrackable(out GameObject _go))
		{
			targetFinder.FireMissiles(_go);			
		}
        else
        {
            targetFinder.FireMissiles();
        }
	}

	private void Shoot()
	{
		if (player.Ammo >= 1)
		{
			machineGun.Shoot();
			player.Ammo--;
			AudioManager.instance.Play("Laser");
		}
	}

	private void ToggleShield()
	{
        // TODO moved getComponent to awake
        s.ToggleShield();
	}

    private void PoolMe()
    {
        ObjectPool.instance.PoolObject(type, gameObject);
    }
}
