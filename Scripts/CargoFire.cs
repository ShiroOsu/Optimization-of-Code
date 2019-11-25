using UnityEngine;

public class CargoFire : MonoBehaviour
{
    //For instantiating from exploding parts.
    public GameObject particleFly = null;
    private ExplodeParts explodeParts;

    void Update()
    {
        Instantiate(particleFly, transform.position,transform.rotation);
    }
}