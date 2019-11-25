using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody2D))]
public class MissileGuidance : MonoBehaviour
{
    // Missile target
    [System.NonSerialized] public GameObject target = null;

    // Prefabs
    public GameObject missileExplosion;
    public GameObject crosshair = null;

    // Component references
    private Rigidbody2D rb = null;
    private MissileTargetFinder mtf = null;

    // Missile values
    private float speed = 30;
    private float rotationSpeed = 600f;
    private float theta = 0;
    private float lifeTime = 0.7f;
    private float timeExisted = 0f;
    [System.NonSerialized] public bool thetaPos = true;
    [System.NonSerialized] public float swirlSpeed = 10;
    [SerializeField] private bool swirl = false; // Whether the missile should swirl around on its way or move in a straight line

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb, "RigidBody2D was not found");
        Assert.IsNotNull(missileExplosion, "Missile Explision animation not found");
    }

    private void Start()
    {
        mtf = gameObject.AddComponent<MissileTargetFinder>();
        mtf.crosshair = crosshair;
        if(target != null)
            target.GetComponent<MissileTrackable>().trackedList.Add(this);
        Assert.IsNotNull(mtf, "Missile Target Finder component not found");
        Assert.IsNotNull(crosshair, "Crosshair prefab not found");
    }
   
    void FixedUpdate()
    {
        if (timeExisted > lifeTime)
            Destroy(gameObject);
        timeExisted += Time.fixedDeltaTime;
        if (target == null)
        {
            if (!mtf.GetNearestTrackable(out target))
            {
                transform.position = transform.position + (transform.up / 200) * speed;
                return;
            }
        }

        // Remove any unnecessary velocity and rely on absolute movement
        rb.velocity = Vector2.zero;
        transform.position = transform.position + (transform.up / 200) * speed * 1.75f;

        // Slowly rotate towards target
        Vector2 _direction = target.transform.position - transform.position;
        float _angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg - 90 + theta;
        Quaternion q = Quaternion.AngleAxis(_angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, Time.fixedDeltaTime * rotationSpeed);

        // Increase speed and rotation as time goes
        rotationSpeed += 120 * Time.fixedDeltaTime;
        speed += 6 * Time.fixedDeltaTime;

        // Causes missile to swirl back and forth during the flight
        if (swirl)
        {
            if (thetaPos)
            {
                if (theta > 80)
                {
                    swirlSpeed *= -1;
                    thetaPos = false;
                }
            }
            else
            {
                if (theta < -80)
                {
                    swirlSpeed *= -1;
                    thetaPos = true;
                }
            }
            theta += swirlSpeed;
        }
    }
    private void OnDestroy()
    {
        if (target != null)
        {
            target.GetComponent<MissileTrackable>()?.trackedList.Remove(this);
        }
        Instantiate(missileExplosion, transform.position, transform.rotation);
    }

    public void ChangeTarget(GameObject target)
    {
        if(this.target != null)
            this.target.GetComponent<MissileTrackable>()?.trackedList.Remove(this);
        this.target = target;
        target.GetComponent<MissileTrackable>()?.trackedList.Add(this);
    }

    // Ignore set colliders, often shooter and shooters shield
    public void UpdateColliderIgnore(List<Collider2D> colliders)
    {
        foreach (Collider2D c in colliders)
            Physics2D.IgnoreCollision(c, GetComponent<Collider2D>());
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.isTrigger && collision.collider.tag != "Missile")
        {
            Destroy(gameObject);
        }
    }
}
