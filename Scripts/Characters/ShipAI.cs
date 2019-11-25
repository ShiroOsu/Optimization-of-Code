using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

[RequireComponent(typeof(MissileTrackable))]
public class ShipAI : Enemy
{
    // AI States
    private const int idleState = 0, activatingState = 1, chasingState = 2, trackedState = 3, orbWalkState = 4;
    private const int owKiteState = 0, owShootState = 1;
    private int state;
    private int owState;
    private GameObject target = null;
    private MissileTrackable missileTrackable = null;
    private Rigidbody2D rb = null;
    private SpriteRenderer rend = null;
    [SerializeField] private TargetData tData = null;
    [SerializeField] private float rotationSpeed = 2000f, speed = 20f;
    private float kiteTimer = 0;
    private float shootTimer = 0;
    private int kiteDirection = 1;
    private float activationTimer = 0;
    private int spriteIndex = 1;
    private float flareTimer = 0f;
    private float flareDelay = 0.4f;
    [SerializeField] private Gun gun = null;
    [SerializeField] private FlareGun flareGun = null;

    private const float detectDistance = 8f;
    private const float chaseDistance = 12f;
    private const float leaveDistance = 15f;
    
    // Activation states
    [SerializeField] List<Sprite> spriteList = new List<Sprite>();

    private void Awake()
    {
        missileTrackable = GetComponent<MissileTrackable>();
        rb = GetComponent<Rigidbody2D>();
        tData = Resources.Load<TargetData>("ScriptableObjects/TargetData");
        rend = GetComponent<SpriteRenderer>();
        if(gun == null)
            gun = GetComponent<Gun>();
        if (flareGun == null)
            flareGun = GetComponent<FlareGun>();
        Assert.IsNotNull(missileTrackable, "Could not find MissileTrackable script");
        Assert.IsNotNull(rb, "Could not find RigidBody2D component");
        Assert.IsNotNull(tData, "Could not find TargetData object");
        Assert.IsNotNull(rend, "Could not find Sprite Renderer");
        Assert.IsNotNull(gun, "Could not find Gun component");
        Assert.IsNotNull(flareGun, "Could not find FlareGun component");
    }
    
    void Start()
    {
        state = idleState;
        flareTimer = flareDelay;
    }

    private void FixedUpdate()
    {
        CheckState();
    }

    private void CheckState()
    {
        switch (state)
        {
            case idleState:
                Idling();
                break;
            case activatingState:
                Activate();
                break;
            case chasingState:
                Chase();
                break;
            case trackedState:
                Run();
                break;
            case orbWalkState:
                OrbWalk();
                break;
        }
    }

    // Looking for targets
    private void Idling()
    {
        List<GameObject> _friendlies = tData.friendlies;
        foreach (GameObject friendly in _friendlies)
        {
            if (friendly == null)
                continue;
            if ((friendly.transform.position - transform.position).magnitude < detectDistance)
            {
                target = friendly;
                state = activatingState;
                activationTimer = 0;
                break;
            }
        }
    }

    // Iterate through activationsprites
    private void Activate()
    {
        if(activationTimer > 0.5f * spriteIndex && spriteIndex < spriteList.Count)
        {
            rend.sprite = spriteList[spriteIndex];
            spriteIndex++;
        }
        else if(activationTimer > 0.5f * spriteList.Count + 0.2f)
        {
            state = chasingState;
            activationTimer = 0;
            spriteIndex = 0;
            return;
        }
        activationTimer += Time.fixedDeltaTime;
    }

    // Follow target
    private void Chase()
    {
        // Turn around if tracked by missiles
        if (missileTrackable.trackedList.Count > 0)
        {
            state = trackedState;
            return;
        }

        Vector2 _direction;
        RotateTo(target, out _direction);
        MoveForward();

        if(_direction.magnitude < detectDistance)
        {
            state = orbWalkState;
        }
        else if (_direction.magnitude > leaveDistance)
        {
            state = idleState;
            rend.sprite = spriteList[0];
            rb.velocity = Vector2.zero;
            owState = owKiteState;
            return;
        }

    }

    // Turn around
    private void Run()
    {
        if (target == null)
            return;

        // Turn back if not tracked
        if (!(missileTrackable.trackedList.Count > 0))
        {
            flareTimer = flareDelay;
            state = chasingState;
            return;
        }

        // Send flares
        if(flareTimer > flareDelay)
        {
            flareGun.Shoot();
            flareTimer = 0;
        }
        flareTimer += Time.fixedDeltaTime;

        Vector2 _direction = target.transform.position - transform.position;
        RotateTo(-_direction);
        MoveForward();
    }

    // Swap between moving in a perpendicular direction and shooting
    private void OrbWalk()
    {
        if (target == null)
        {
            state = idleState;
            return;
        }

        if (missileTrackable.trackedList.Count > 0)
        {
            state = trackedState;
            return;
        }
        Vector2 _direction = target.transform.position - transform.position;
        
        if(_direction.magnitude > chaseDistance)
        {
            state = chasingState;
        }

        switch (owState)
        {
            case owKiteState:
                OwKite(_direction);
                break;
            case owShootState:
                OwShoot(_direction);
                break;
        }
    }

    // Move perpendicular to the target in a random direction
    private void OwKite(Vector2 _direction)
    {
        RotateTo(new Vector2(-_direction.y * kiteDirection, _direction.x));
        MoveForward();

        kiteTimer += Time.fixedDeltaTime;
        if(kiteTimer > 2)
        {
            owState = owShootState;
            rb.velocity = transform.up;
            kiteTimer = 0;
        }
    }

    // Turn to the target and shoot
    private void OwShoot(Vector2 _direction)
    {
        if (target == null)
            return;
        RotateTo(target);
        float _angle = Vector3.Angle(transform.up, _direction);

        if (_direction.magnitude > chaseDistance)
            if (rb.velocity.magnitude > 0)
                rb.velocity -= Vector2.Scale(rb.velocity, new Vector2(0.2f, 0.2f));
            else
                MoveForward();

        if (_angle < 15)
        {
            if (shootTimer > 0.2f)
            {
                gun.Shoot();
                shootTimer = 0;
            }
            shootTimer += Time.fixedDeltaTime;
        }


        kiteTimer += Time.fixedDeltaTime;
        if(kiteTimer > 2)
        {
            switch (Random.Range(0, 2))
            {
                case 0:
                    kiteDirection *= -1;
                    break;
            }
            owState = owKiteState;
            kiteTimer = 0;
        }
    }

    private void MoveForward()
    {
        transform.position = transform.position + transform.up * Time.fixedDeltaTime * speed;
    }

    private void RotateTo(GameObject _target)
    {
        Vector2 _direction = _target.transform.position - transform.position;
        float _angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(_angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, Time.fixedDeltaTime * rotationSpeed);
    }

    private void RotateTo(GameObject _target, out Vector2 _direction)
    {
        _direction = _target.transform.position - transform.position;
        float _angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(_angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, Time.fixedDeltaTime * rotationSpeed);
    }

    private void RotateTo(Vector2 _direction)
    {
        float _angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(_angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, Time.fixedDeltaTime * rotationSpeed);
    }

}
