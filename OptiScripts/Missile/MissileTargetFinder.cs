using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MissileTargetFinder : MonoBehaviour
{

    [SerializeField] private MissileData data;
    [SerializeField] List<Collider2D> ignoreColliderList = new List<Collider2D>();

    private Rigidbody2D rb = null;
    public float trackRange = 5;
    public int missileBurst = 3;
    int trackableLayers = 1 << 12 | 1 << 14;

    public GameObject crosshair = null;
    private GameObject spawnedCrosshair = null;
    private GameObject nearestMarked = null;

    private void Awake()
    {
        data = Resources.Load<MissileData>("ScriptableObjects/MissileData");
        rb = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(data, "Could not find MissileData");
        Assert.IsNotNull(rb, "Could not find Rigidbody2D component");
    }

    private void Start()
    {
        Assert.IsNotNull(crosshair, "Could not find crosshair prefab");
    }

    private void FixedUpdate()
    {
        // Unless the gameObject is a missile, constantly try to find the nearest trackable targets and apply a crosshair on it
        if (gameObject.tag != "Missile")
        {
            if (GetNearestTrackable(out GameObject nearest))
            {
                if (nearestMarked != null)
                {
                    if (nearestMarked != nearest)
                    {
                        Destroy(spawnedCrosshair);
                        spawnedCrosshair = Instantiate(crosshair, nearest.transform);

                        if (nearest.tag == "Turret")
                        {
                            spawnedCrosshair.transform.localPosition = Vector3.up;
                        }
                        nearestMarked = nearest;
                    }
                }
                else
                {
                    spawnedCrosshair = Instantiate(crosshair, nearest.transform);
                    if (nearest.tag == "Turret")
                    {
                        spawnedCrosshair.transform.localPosition = Vector3.up;
                    }
                    nearestMarked = nearest;
                }
            }
            else if (nearestMarked != null)
            {
                Destroy(spawnedCrosshair);
                nearestMarked = null;
            }
        }
    }

    // Find nearest trackable target, if in range of missile guidance, add a crosshair on top of it.
    public bool GetNearestTrackable(out GameObject nearest)
    {
        RaycastHit2D hit;
        List<GameObject> _reachableObjects = new List<GameObject>();
        List<GameObject> _trackableObjects = new List<GameObject>(data.trackableObjects);
        foreach (GameObject go in _trackableObjects)
        {
            if (go == null)
            {
                data.trackableObjects.Remove(go);
                continue;
            }
                
            Vector2 _direction = (go.transform.position - transform.position).normalized * trackRange;
            Debug.DrawRay(transform.position, _direction, Color.yellow);

            hit = Physics2D.Raycast(transform.position, _direction, trackRange, trackableLayers);

            if(hit.collider != null)
            {
                if (hit.transform.gameObject == go)
                    _reachableObjects.Add(hit.transform.gameObject);
            }
        }

        nearest = null;
        foreach (GameObject go in _reachableObjects)
        {
            if ((go.transform.position - transform.position).magnitude < trackRange)
            {
                nearest = go;
            }
        }


        if (nearest == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // Fire missile from each missile source on self
    public void FireMissiles(GameObject go = null)
    {
        for (int i = 0; i < missileBurst; i++)
        {
            GameObject _missile = Instantiate(data.missile, transform.position + transform.up, transform.rotation);
            MissileGuidance _misGuidance = _missile.GetComponent<MissileGuidance>();
            _misGuidance.crosshair = crosshair;
            _misGuidance.target = go;
            _misGuidance.UpdateColliderIgnore(ignoreColliderList);
            if (i % 2 == 0)
            {
                _misGuidance.swirlSpeed *= -1;
                _misGuidance.thetaPos = false;
            }
        }
    }
}
