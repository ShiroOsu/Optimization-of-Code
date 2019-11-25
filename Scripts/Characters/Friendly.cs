using UnityEngine;
using UnityEngine.Assertions;

public class Friendly : MonoBehaviour
{
    private void Awake()
    {
        TargetData tData = Resources.Load<TargetData>("ScriptableObjects/TargetData");
        Assert.IsNotNull(tData, "ScriptableObject (TargetData) not found, have you moved the script away from \"Resources/ScriptableObjects/TargetData\"?");
        tData.friendlies.Add(gameObject);
    }
}
