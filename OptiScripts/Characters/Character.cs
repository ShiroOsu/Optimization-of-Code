using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    [System.NonSerialized] protected float health = 100;
    [SerializeField] protected GameObject deathExplosion;
    [SerializeField] protected List<GameObject> loot = new List<GameObject>();

    public static float missileDamage = 6f;
    public static float bulletDamage = 8f;
    public static float laserDamage = 30f;
    public static float laserShotDamage = 5f;

    public virtual float Health
    {
        get { return health; }
        set { health = value; }
    }


    protected virtual void Update()
    {
        CheckAliveState();
    }

    private void CheckAliveState()
    {
        if (health < 1)
        {
            if (deathExplosion != null)
                Instantiate(deathExplosion, transform.position, transform.rotation);
            if (loot.Count > 0)
            {
                int _dropAmount = Random.Range(1, loot.Count);
                for (int i = 0; i < _dropAmount; i++)
                {
                    Instantiate(loot[Random.Range(0, loot.Count)], transform.position, transform.rotation);
                }
            }
            Destroy(gameObject);
            AudioManager.instance?.Play("DeathSound");
        }
    }

    public virtual void TakeDamage(float damage)
    {
		Health -= damage;
    }
    public virtual void TakeDamage<T>(float damage, T sender)
    {
        Health -= damage;
    }

    public virtual void TakeDamage(string tag)
    {
        switch (tag)
        {
            case "Bullet":
                TakeDamage(bulletDamage);
                break;
            case "LaserShot":
                TakeDamage(laserShotDamage);
                break;
        }
    }


    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Missile")
        {
            TakeDamage(missileDamage);
            Destroy(collision.gameObject);
        }
        else if (collision.collider.tag == "Bullet")
        {
            TakeDamage(bulletDamage);
            Destroy(collision.gameObject);
        }
        else if (collision.collider.tag == "LaserShot")
        {
            TakeDamage(laserShotDamage);
            Destroy(collision.gameObject);
        }
    }
}
