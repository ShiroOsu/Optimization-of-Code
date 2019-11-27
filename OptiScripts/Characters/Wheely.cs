using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheely : MonoBehaviour
{
    private float fireRate = .02f;
    public float spinSpeed = 300f;
    private bool isActive = false;
    public GameObject bullet;
    private PointEffector2D gravityEffector;
    private Rigidbody2D rb;
    private Gun gun = null;

    void Start()
    {
        gun = GetComponent<Gun>();
        rb = GetComponent<Rigidbody2D>();
        gravityEffector = GetComponent<PointEffector2D>();
        gravityEffector.enabled = false;
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * spinSpeed * Time.deltaTime);
        fireRate -= Time.deltaTime;

        if (isActive)
        {
            if (fireRate < 0)
            {
                gun.Shoot();
                //Instantiate(bullet, transform.position, transform.rotation);
                AudioManager.instance.Play("Machinegun");
                fireRate = .02f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isActive = true;
            gravityEffector.enabled = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isActive = false;
            gravityEffector.enabled = false;
        }
    }

}
