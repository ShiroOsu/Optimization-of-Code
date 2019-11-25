using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject cameraParent;
    public GameObject target = null;
    public Collider2D bounds;
    private Camera cam;
    [Range(0f,2f)]
    [SerializeField]
    private float shakeDuration = 0f;
    [Range(0f, 10f)]
    [SerializeField]
    private float shakeIntensity = 1f;
    private Vector3 fallBackPosition;
    private float verticalOffset;
    private float horizontalOffset;
    private bool shake = false;
    private Vector3 cameraNewPos;
    private float shakeFallback = 0;
    
    void Start()
    {
        fallBackPosition = Vector3.zero;
        cam = GetComponent<Camera>();
        verticalOffset = cam.orthographicSize;
        horizontalOffset = verticalOffset * Screen.width / Screen.height;
    }

    
    void Update()
    {
        if (target == null)
            return;

        // Lerp camera towards player x and y position
        cameraParent.transform.position = Vector3.Lerp(cameraParent.transform.position, target.transform.position, Time.deltaTime * 5);
        cameraParent.transform.position = new Vector3(cameraParent.transform.position.x, cameraParent.transform.position.y, -12);

        // Keep camera within bounds
        cameraNewPos = cameraParent.transform.position;
        if (cameraParent.transform.position.x + horizontalOffset > bounds.bounds.max.x)
            cameraNewPos.x = bounds.bounds.max.x - horizontalOffset;
        else if(cameraParent.transform.position.x - horizontalOffset < bounds.bounds.min.x)
                cameraNewPos.x = bounds.bounds.min.x + horizontalOffset;
        if (cameraParent.transform.position.y + verticalOffset > bounds.bounds.max.y)
            cameraNewPos.y = bounds.bounds.max.y - verticalOffset;
        else if (cameraParent.transform.position.y - verticalOffset < bounds.bounds.min.y)
            cameraNewPos.y = bounds.bounds.min.y + verticalOffset;
        cameraParent.transform.position = cameraNewPos;
    }

    private void FixedUpdate()
    {
        // Camera shake
        if (shake)
        {
            if (shakeDuration > 0)
            {
                transform.localPosition = fallBackPosition + Random.insideUnitCircle.ToVector3() * shakeIntensity;
                shakeDuration -= Time.fixedDeltaTime;
            }
            else
            {
                shakeDuration = shakeFallback;
                shake = false;
                transform.localPosition = fallBackPosition;
            }
        }
    }

    // Camera shake
    public void ShakeScreen(float duration = 0.4f, float intensity = 0.15f)
    {
        shakeDuration = shakeFallback = duration;
        shakeIntensity = intensity;
        shake = true;
    }
}
