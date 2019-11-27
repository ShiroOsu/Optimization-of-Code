using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PoolableObject
{
    // Cargo Prefabs, example names
    Cargo = 0,
    Cargo1 = 1,
    Cargo2 = 2,
    Cargo3 = 3,
    Cargo4 = 4,

    Bullet,
    BurnParticle,
    Enemy,
    explodePrefab,
    explodeParts,


}

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    public List<GameObject> objectTemplates = new List<GameObject>();
    public List<int> poolableObjectsAmounts = new List<int>();

    private List<List<GameObject>> pooledObjects = new List<List<GameObject>>();
    private GameObject tempObj;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < objectTemplates.Count; i++)
        {
            List<GameObject> temp = new List<GameObject>();

            for (int p = 0; p < poolableObjectsAmounts[i]; p++)
            {
                temp.Add(InstantiateObject(objectTemplates[i]));
            }

            pooledObjects.Add(temp);
        }
    }

    public GameObject GetObjectForType(PoolableObject objectType)
    {
        if (pooledObjects[(int)objectType].Count > 0)
        {
            int index = pooledObjects[(int)objectType].Count - 1;

            tempObj = pooledObjects[(int)objectType][index];
            pooledObjects[(int)objectType].RemoveAt(index);
        }
        else
        {
            tempObj = InstantiateObject(objectTemplates[(int)objectType]);
        }

        if (!tempObj)
        {
            tempObj.SetActive(true);
            tempObj.transform.parent = null;
        }

        return tempObj;
    }

    public void PoolObject(PoolableObject objectType, GameObject gameObject)
    {
        gameObject.SetActive(false);
        gameObject.transform.parent = transform;
        pooledObjects[(int)objectType].Add(gameObject);
    }

    private GameObject InstantiateObject(GameObject template)
    {
        tempObj = Instantiate(template);
        tempObj.SetActive(false);
        tempObj.transform.parent = transform;
        return tempObj;
    }
}