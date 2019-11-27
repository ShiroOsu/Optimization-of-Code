using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [System.NonSerialized] public float spread = 0f, speed = 8f;
    private float timer = 0;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(rb, "RigidBody2D component not found");
    }

    // Start with forward rotation in relation to shooter
    private void Start()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, Random.Range(-spread, spread)));
    }

    // Move forward, die if lasted for 5 seconds
    void FixedUpdate()
    {
        transform.position = transform.position + transform.up * Time.fixedDeltaTime * speed;
        timer += Time.fixedDeltaTime;
        if (timer > 5)
            Destroy(gameObject);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger || collision.tag == "Shield")
        {
            Character c = collision.gameObject.GetComponent<Character>();
            if (c != null)
                c.TakeDamage(gameObject.tag);
            Destroy(gameObject);
        }
    }

    // Destroy self if hits anything
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.isTrigger)
        {
            Character c = collision.gameObject.GetComponent<Character>();
            if (c != null)
                c.TakeDamage(gameObject.tag);
            Destroy(gameObject);
        }
    }

    // Ignore set colliders, often shooter and eventual shield of shooter
    public void UpdateColliderIgnore(List<Collider2D> colliders)
    {
        foreach (Collider2D c in colliders)
        {
            if(c != null)
                Physics2D.IgnoreCollision(c, GetComponent<Collider2D>());
        }
    }
}
