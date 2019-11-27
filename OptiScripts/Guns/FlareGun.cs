using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FlareGun : MonoBehaviour
{
    public List<Transform> FlareGuns = new List<Transform>();
    [SerializeField] private GameObject flare = null;

    private void Awake()
    {
        Assert.IsNotNull(flare, "No flare prefab found");
    }
    public void Shoot()
    {
        foreach(Transform t in FlareGuns)
        {
            Instantiate(flare, t.position, t.rotation);
        }
    }
}
