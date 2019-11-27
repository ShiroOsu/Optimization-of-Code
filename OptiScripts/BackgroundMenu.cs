using UnityEngine;

public class BackgroundMenu : MonoBehaviour
{
    void Update()
    {
        // TODO Changed (Time.time/4) because multiplication is faster than division.
        transform.position = new Vector3(Mathf.PingPong(Time.time * 0.25f, 10), transform.position.y, transform.position.z);
    }
}