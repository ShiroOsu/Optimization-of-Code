using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class MissileTrackable : MonoBehaviour
{
    private MissileData data = null;
    private int enemyLayer, trackableLayer;
    public List<MissileGuidance> trackedList = new List<MissileGuidance>();
    

    private void Awake()
    {
        enemyLayer = LayerMask.NameToLayer("Enemy");
        trackableLayer = LayerMask.NameToLayer("Trackable");

        if(gameObject.layer != trackableLayer && gameObject.layer != enemyLayer)
            throw new System.Exception("LayerMask Busy");

        data = Resources.Load<MissileData>("ScriptableObjects/MissileData");
        Assert.IsNotNull(data, "Could not find the MissileData ScriptableObject, has it been removed from the Resources/ScriptableObjects folder?");
        data.trackableObjects.Add(gameObject);
    }
}
