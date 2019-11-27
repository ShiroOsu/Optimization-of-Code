using UnityEngine;

public class SpiffParallax : MonoBehaviour
{
    float length, startposX, startposY;    
    [SerializeField] private float speedX = 0;
    [SerializeField] private float speedY = 0;    
    [SerializeField] private GameObject cam = null;

    void Start()
    {        
        startposX = transform.position.x;
        startposY = transform.position.y;

        length = GetComponent<SpriteRenderer>().bounds.size.x; //pop back to loop horizontal
    }

    void LateUpdate()
    {
        //X axis
        float distX = (cam.transform.position.x * speedX);
        transform.position = new Vector2(startposX + distX, transform.position.y);

        //Y axis
        float distY = (cam.transform.position.y * speedY);
        transform.position = new Vector2(transform.position.x, startposY + distY);

        //xboundspop
        float temp = (cam.transform.position.x * (1 - speedX));

        if (temp > startposX + length/2)
        {
            startposX += length;
        }
        else if (temp < startposX - length/2)
        {
            startposX -= length;
        }
    }

}