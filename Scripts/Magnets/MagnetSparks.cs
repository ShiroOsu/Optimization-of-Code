using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(LineRenderer))]
public class MagnetSparks : MonoBehaviour
{
    Magnet magnet;
    LineRenderer lineRenderer;

    void Start()
    {
        magnet = GetComponent<Magnet>();
        lineRenderer = GetComponent<LineRenderer>();

        Assert.IsNotNull(magnet, "Could not find Magnet component");
        Assert.IsNotNull(lineRenderer, "Could not find LineRenderer component");
    }

}
