using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetData", menuName = "ScriptableObjects/TargetData", order = 3)]
public class TargetData : ScriptableObject
{
    [System.NonSerialized] public List<GameObject> friendlies = new List<GameObject>();
}
