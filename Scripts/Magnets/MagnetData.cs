using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MagnetData", menuName = "ScriptableObjects/MagnetData", order = 1)]
public class MagnetData : ScriptableObject
{
    [System.NonSerialized] public List<Magnetizable> magnetizedList = new List<Magnetizable>();
    [System.NonSerialized] public List<Magnetizable> nonMagnetizedList = new List<Magnetizable>();
    public float magnetForceScalar = 20;


    public void AddMagnetizable(Magnetizable mag)
    {
        if (!nonMagnetizedList.Remove(mag))
        {
            throw new System.Exception("Non Magnetized List did not contain this Magnetizable. Did you mean to use nonMagnetizedList.Add()?");
        }
        magnetizedList.Add(mag);
    }

    public void RemoveMagnetizable(Magnetizable mag)
    {
        magnetizedList.Remove(mag);
        nonMagnetizedList.Add(mag);
    }

    public void ReleaseLastMagnetized()
    {
        if(magnetizedList.Count > 0)
            magnetizedList.GetLast().DeMagnetize();
    }

    public void DestroyMagnetizable(Magnetizable mag)
    {
        
        if (magnetizedList.Contains(mag))
        {
            magnetizedList.Remove(mag);
        }
        if (nonMagnetizedList.Contains(mag))
        {
            nonMagnetizedList.Remove(mag);
        }
    }
}
