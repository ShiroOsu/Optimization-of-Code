using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Turret : Enemy
{
    private TargetData tData;

    // States
    private const int idle = 1;
    private const int idleMove = 1;
    private const int idleStay = 2;
    private const int tracking = 2;
    private int state;
    private float idleRotation;
    private float idleTick = 0;

    // Speed values
    private readonly float rotationSpeed = 60f;
    [SerializeField] private float rotationMagnitude = 0.3f;

    // Laser
    private GameObject laser;
    private Character targetCharacterScript;
    private GameObject currentTarget = null;
    private float laserCharge = 0;
    private float laserChargeTime = 1;
    private float laserChargeAngle = 20;
    private float maxIntensity = 20;
    private float range = 10f;
    private Light laserLight;
    [SerializeField] private Vector2 minMaxAngles = Vector2.zero; 
    public LayerMask ignoreLayers = new LayerMask();

    private void Awake()
    {
        laser = transform.Find("LaserPoint")?.Find("Laser")?.gameObject;
        laserLight = transform.Find("LaserPoint")?.Find("Point Light")?.GetComponent<Light>();
        tData = Resources.Load<TargetData>("ScriptableObjects/TargetData");
        Assert.IsNotNull(tData, "Could not find TargetData");
        Assert.IsNotNull(laser, "Could not find LaserObject");
        Assert.IsNotNull(laserLight, "Could not find Light");
    }
    void Start()
    {
        idleRotation = rotationMagnitude;
        missileHitDamage = 3;
        state = idle;
        while(minMaxAngles.x < 0)
            minMaxAngles.x += 360;
    }

    protected override void Update()
    {
        CheckBounds();
        base.Update();
    }

    // Keeps laser within bounded angles
    private void CheckBounds()
    {
        if (minMaxAngles.x > minMaxAngles.y)
        {
            if (transform.localRotation.eulerAngles.z > minMaxAngles.y && transform.localRotation.eulerAngles.z < minMaxAngles.x)
            {
                idleRotation *= -1;
            }
        }
        else
        {
            if (transform.localRotation.eulerAngles.z > minMaxAngles.y || transform.localRotation.eulerAngles.z < minMaxAngles.x)
            {
                idleRotation *= -1;
            }
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case idle:
                Idling();
                break;
            case tracking:
                Track();
                break;
        }   
    }

    private void Idling()
    {
        // Swaps between standing still and rotating randomly
        if (idleTick < 2)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, idleRotation * Time.fixedDeltaTime));
            idleTick += Time.fixedDeltaTime;
        }
        else
        {
            idleRotation = rotationMagnitude * Random.Range(-1, 2);
            idleTick = 0;
        }

        // Lose charge on laser if there is any
        if (laserCharge > 0)
        {
            laserCharge -= Time.deltaTime;
        }
        if (laserLight.intensity > 0)
            laserLight.intensity = laserCharge * (20 / laserChargeTime);

        // Look for target
        List<GameObject> _friendlies = tData.friendlies;
        foreach(GameObject friendly in _friendlies)
        {
            if (friendly == null)
                continue;
            if ((friendly.transform.position - transform.position).magnitude < range)
                state = tracking;
        }
    }

    // Find and rotate towards nearest target
    private void Track()
    {
        GameObject _target;
        if(FindClosestFriendly(out _target))
        {
            if(currentTarget != _target)
            {
                targetCharacterScript = _target.GetComponent<Character>();
                currentTarget = _target;
            }
            RotateToTarget(_target);
            ChargeLaser(_target);
        }
        else
        {
            state = idle;
        }
    }

    private bool FindClosestFriendly(out GameObject c)
    {
        GameObject _friendly = null;
        List<GameObject> _friendlies = new List<GameObject>(tData.friendlies);
        foreach (GameObject _c in _friendlies)
        {
            if (_c == null)
            {
                tData.friendlies.Remove(_c);
                c = null;
                laser.SetActive(false);
                laserLight.intensity = 0;
                return false;
            }
            float _distance = float.MaxValue;
            if((_c.transform.position - transform.position).magnitude < _distance)
            {
                _friendly = _c;
            }
        }
        if (_friendly == null)
        {
            c = null;
            return false;
        }
        else
        {
            c = _friendly;
            return true;
        }
    }
    
    private void RotateToTarget(GameObject target)
    {
        Vector2 _direction = target.transform.position - transform.position;
        float _angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg - 90;
        Quaternion q = Quaternion.AngleAxis(_angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, Time.deltaTime * rotationSpeed);
    }

    private void ChargeLaser(GameObject target)
    {
        // Stop charging if player moves out of range
        Vector2 _direction = target.transform.position - laser.transform.parent.position;
        if (_direction.magnitude > range)
        {
            state = idle;
            laser.SetActive(false);
            if (laserCharge > 0)
            {
                laserCharge -= Time.deltaTime;
            }
            laserLight.intensity = laserCharge * (20 / laserChargeTime);
            return;
        }

        // Shoot if within angle, player or shield (if shield is active), otherwise charge laser
        float _angle = Vector3.Angle(transform.up, _direction);
        if (_angle < laserChargeAngle)
        {
            if(laserCharge < laserChargeTime)
            {
                laserCharge += Time.deltaTime;
                laser.SetActive(false);
            }
            else if(_angle < 4)
            {
                var hits = Physics2D.RaycastAll(laser.transform.position, _direction, range, ~ignoreLayers);
				foreach (var hit in hits)
				{
					if (hit.collider.tag == "Player")
					{
                        if (!hit.collider.transform.Find("Shield").GetComponent<Shield>().toggled)
                        {
                            laser.SetActive(true);
                            laser.transform.localPosition = new Vector3(0, hit.distance / 2f, 0);
                            laser.transform.localScale = new Vector3(laser.transform.localScale.x, hit.distance * 13);
                            targetCharacterScript = hit.collider.GetComponent<Character>();
                            targetCharacterScript.TakeDamage(laserDamage * Time.deltaTime, this);
                        }
                    }
                    else if (hit.collider.tag == "Shield")
                    {
                        laser.SetActive(true);
                        laser.transform.localPosition = new Vector3(0, hit.distance / 2f, 0);
                        laser.transform.localScale = new Vector3(laser.transform.localScale.x, hit.distance * 13);
                        targetCharacterScript = hit.collider.GetComponent<Character>();
                        targetCharacterScript.TakeDamage(laserDamage * Time.deltaTime, this);
                    }

                }
			}
        }
        else if(laserCharge > 0)
        {
            laser.SetActive(false);
            laserCharge -= Time.deltaTime;
        }

        laserLight.intensity = laserCharge * (maxIntensity / laserChargeTime);

        if (laserCharge < laserChargeTime)
            laser.SetActive(false);
    }

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }
}
