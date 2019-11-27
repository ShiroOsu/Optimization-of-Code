using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class Cargo : MonoBehaviour
{

    // TODO Instead of instantiate a new explodePrefab we can use the objectPool
    public int value;
    [SerializeField] private float health = 100;
    [SerializeField] private GameObject explodePrefab = null;
    private PoolableObject type;
    private GameObject temp;
    private Rigidbody2D rb;
    private Magnetizable mag;
    private MagnetData magData;

    private bool invincible = true;

    float lastSpeed = 0;
    private void Awake()
    {
        type = PoolableObject.Cargo;

        rb = GetComponent<Rigidbody2D>();
        magData = Resources.Load<MagnetData>("ScriptableObjects/MagnetData");
        Assert.IsNotNull(transform.Find("Magnet"), "Magnet not attached");

        mag = transform.Find("Magnet").GetComponent<Magnetizable>();
        StartCoroutine(BecomeInvincibleForSeconds(2f));
    }


    private void FixedUpdate()
    {
        lastSpeed = rb.velocity.magnitude;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!invincible)
        {
            // TODO readability
            if (lastSpeed > 5f &&
                collision.collider.tag != "Cargo" &&
                collision.collider.tag != "Asteroid" &&
                collision.collider.tag != "Bounds" &&
                collision.collider.tag != "Debris")
            {
                health -= lastSpeed * 10;
                if (health < 0)
                {
                    if(magData.magnetizedList.Contains(mag))
                        mag.DeMagnetize();
                    magData.DestroyMagnetizable(mag);

                    // TODO ObjectPool
                    temp = ObjectPool.instance.GetObjectForType(type);
                    temp.transform.position = transform.position;
                    temp.transform.rotation = Quaternion.identity;
                    PoolMe();
                }
            }
        }
    }
	private IEnumerator BecomeInvincibleForSeconds(float s)
    {
        invincible = true;

        yield return new WaitForSeconds(s);
        invincible = false;
    }

    private void PoolMe()
    {
        ObjectPool.instance.PoolObject(type, gameObject);
    }
}
