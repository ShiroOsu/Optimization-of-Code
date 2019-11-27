using UnityEngine;

public class ExplodeParts : MonoBehaviour
{    
    public float partLifeTime = 2f;
    public GameObject[] parts;

    private void Awake()
    {
        // Reasonable
        foreach (var part in parts)
        {
            part.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-4f, 4f), Random.Range(2f, 6));
        }
    }

    private void FixedUpdate()
    {
        partLifeTime -= Time.deltaTime;

        if (partLifeTime < 0)
        {
            Destroy(gameObject);
        }
    }
}