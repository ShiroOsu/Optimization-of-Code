using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject projectile = null;
    [SerializeField] private float speed = 16f, spread = 0f;
    private Rigidbody2D rb;

    [SerializeField] List<GameObject> nozzleList = new List<GameObject>();
    [SerializeField] List<Collider2D> ignoreCollisionList = new List<Collider2D>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(projectile, "Couldn't find bullet");
        Assert.IsNotNull(rb, "Couldn't find RigidBody component");
    }

    public void Shoot()
    {
        foreach(GameObject nozzle in nozzleList)
        {
            GameObject go = Instantiate(projectile, nozzle.transform.position, transform.rotation);

            Projectile b = go.GetComponent<Projectile>();
            if (b == null)
                b = go.AddComponent<Projectile>();
                
            b.speed = speed;
            b.spread = spread;
            b.UpdateColliderIgnore(ignoreCollisionList);
        }
    }


}
