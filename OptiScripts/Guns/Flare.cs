using UnityEngine;

public class Flare : MonoBehaviour
{
    private int health = 3;


    // Attract missile within collider radius
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Missile")
        {
            MissileGuidance _misGuidance = collision.GetComponent<MissileGuidance>();
            
            if(_misGuidance != null)
            {
                if (_misGuidance.target != null && _misGuidance.target.tag != "Flare")
                {
                    _misGuidance.ChangeTarget(gameObject);
                }
                else if (_misGuidance.target == null)
                {
                    _misGuidance.ChangeTarget(gameObject);
                }
            }
        }
    }

    // If missile comes within set distance, destroy missile and lose health
    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (collision.tag == "Missile")
        {
            
            if ((collision.transform.position - transform.position).magnitude < 0.3f)
            {
                if (health > 0)
                {
                    health--;
                    Destroy(collision.gameObject);
                }
                else
                {
                    Destroy(collision.gameObject);
                    Destroy(gameObject);
                }
            }
        }
    }
}
