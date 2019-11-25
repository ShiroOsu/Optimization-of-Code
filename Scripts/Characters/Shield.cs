using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(SpriteRenderer))]
public class Shield : Character
{
    public List<Sprite> damageStates = new List<Sprite>();
    public SpriteRenderer rend = null;
    public Collider2D col = null;
    float healthStep;
    private int currentDamageState;
    [SerializeField]
    public float hp = 100;
    [System.NonSerialized]
    public bool toggled = false;
    public float activationTimer = 0;
    private float activationTime = 1;
    public bool activating = true;

	public IntEvent shieldChanged;

	private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        Assert.IsNotNull(rend, "Could not find renderer");
        Assert.IsNotNull(col, "Could not find collider");
    }

	public override float Health
	{
		get => base.Health;
		set
		{
			base.Health = value;
			var temp = GetComponentInParent<Player>();
            if(temp)
            {
			    shieldChanged.Invoke((int)Health);
            }
		}
	}
	private void Start()
    {
        Health = hp;
        rend.sprite = damageStates[damageStates.Count - 1];
        healthStep = health / damageStates.Count;
        currentDamageState = damageStates.Count - 1;
    }

    protected override void Update()
    {
        if (!toggled) // Regen shield
        {
            Health += Time.deltaTime * 5;
            if (health > hp)
                health = hp;
            rend.enabled = false;
            col.enabled = false;
        }
        else if(health < 1) // Force-remove shield
        {
            toggled = false;
            rend.enabled = false;
            col.enabled = false;
        }
        else if (activating) // Initiate activation
        {
            Activate();
        }
        else if (health < currentDamageState * healthStep) // Render stage according to amount of health
        {
            currentDamageState--;
            rend.sprite = damageStates[currentDamageState];
        }
    }

    // Iterate through damagestates when "charging" up shields
    public void Activate()
    {
        if (activationTimer < activationTime * (health / hp))
        {
            rend.sprite = damageStates[Mathf.RoundToInt((((activationTimer / activationTime) * hp) - ((activationTimer / activationTime) * hp) % healthStep) / healthStep)];
            activationTimer += Time.deltaTime / activationTime;
        }
        else
        {
            activationTimer = 0;
            activating = false;
        }
    }

    public void ToggleShield()
    {
        rend.enabled = !rend.enabled;
        activating = rend.enabled;
        activationTimer = 0;
        rend.sprite = damageStates[0];
        toggled = !toggled;
        col.enabled = !col.enabled;
    }

    public void ActivateShield()
    {
        rend.enabled = true;
        activating = true;
        activationTimer = 0;
        rend.sprite = damageStates[0];
        toggled = true;
    }

    public void SetShieldHP(float _hp)
    {
        hp = _hp;
        Start();
    }
}
