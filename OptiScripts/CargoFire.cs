using UnityEngine;

public class CargoFire : MonoBehaviour
{
    // TODO use ObjectPool instead of instantiate
    private GameObject temp;
    public PoolableObject type;

    //For instantiating from exploding parts.
    public GameObject particleFly = null;
    private ExplodeParts explodeParts;

    void Update()
    {
        temp = ObjectPool.instance.GetObjectForType(type);
        temp.transform.position = transform.position;
        temp.transform.rotation = transform.rotation;
    }
}