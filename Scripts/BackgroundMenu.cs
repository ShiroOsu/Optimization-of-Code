using UnityEngine;

public class BackgroundMenu : MonoBehaviour
{
    void Update()
    {
        transform.position = new Vector3(Mathf.PingPong(Time.time/4, 10), transform.position.y, transform.position.z);
    }
}