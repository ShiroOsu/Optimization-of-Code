using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Magnet : MonoBehaviour
{
    [SerializeField] MagnetData magnetData = null;
    [SerializeField] float snapOnRange = 4;
    [SerializeField] bool elasticMagnets = false, chainMagnets = true;

    Magnetizable headMagnetizable;


    void Start()
    {
        Assert.IsNotNull(magnetData, "ScriptableObject MagnetData not found. Was this removed from the Resources/ScriptableObjects/ folder?");
        headMagnetizable = gameObject.AddComponent<Magnetizable>();
        headMagnetizable.magnetActive = true;
        magnetData.nonMagnetizedList.Remove(headMagnetizable);
    }


    private void FixedUpdate()
    {
        HandleMagnets();
    }

    void HandleMagnets()
    {
        // If no magnet is attached to the ship, simply look for nearby magnets and magnetize those in range
        if (headMagnetizable.nextMagnet == null)
        {
            List<Magnetizable> _nonMagnetizedList = new List<Magnetizable>(magnetData.nonMagnetizedList);
            foreach (Magnetizable magnet in _nonMagnetizedList)
            {
                if (magnet == null)
                {
                    // Remove unnecessary magnet from lists
                    magnetData.DestroyMagnetizable(magnet);
                    continue;
                }

                // Snap on nearest magnet to head if in range
                float _distance = (headMagnetizable.transform.position - (magnet.transform.position)).magnitude;
                if (_distance < snapOnRange)
                {
                    if (magnet.magnetActive)
                    {
                        magnet.Magnetize(headMagnetizable);
                        break;
                    }
                        
                }
                else if(_distance < snapOnRange + 3) // Detach magnet if not in range
                {
                    if (!magnet.magnetActive)
                    {
                        magnet.magnetActive = true;
                    }

                }
            }
        }

        // Look for nearest magnet and attach to last magnet in the chain
        if (magnetData.magnetizedList.Count > 0)
        {
            List<Magnetizable> _nonMagnetizedList = new List<Magnetizable>(magnetData.nonMagnetizedList);
            
            foreach (Magnetizable m in _nonMagnetizedList)
            {
                // Demagnetize magnets that are no longer chained to the ship
                Magnetizable _mag = magnetData.magnetizedList.GetLast();
                while (_mag.magnetizedTo == null && magnetData.magnetizedList.Count > 0)
                {
                    magnetData.RemoveMagnetizable(_mag);
                    _mag = magnetData.magnetizedList.GetLast();
                }
                if (m == null)
                {
                    magnetData.DestroyMagnetizable(m);
                    continue;
                }
                else if(m.magnetizedTo != null || m.nextMagnet != null)
                {
                    m.magnetizedTo = null;
                    m.nextMagnet = null;
                }

                // Snap first nearby magnet to the end of the chain
                float _distance = (_mag.transform.position - m.transform.position).magnitude;
                if (_distance < snapOnRange)
                {
                    if (m.magnetActive)
                    {
                        m.Magnetize(_mag);
                    }
                }
                else if(_distance > snapOnRange + 3)
                {
                    if(!m.magnetActive)
                    {
                        m.magnetActive = true;
                    }
                }
            }

            // Move every magnetized magnet towards target magnet in chain
            List<Magnetizable> _magnetizedList = new List<Magnetizable>(magnetData.magnetizedList);
            foreach (Magnetizable m in _magnetizedList)
            {
                if (m.magnetizedTo == null)
                {
                    m.DeMagnetize();
                    continue;
                }
                float _distance = (m.magnetizedTo.transform.position - m.transform.position).magnitude;
                if (_distance < snapOnRange + 1)
                {
                    if (m.magnetActive)
                    {
                        // Allows for elastic movement between magnets. Increasing speed increases distance between magnets
                        #region ElasticRopeMethod
                        if (elasticMagnets)
                        {
                            Vector3 moveVector = (m.magnetizedTo.posMag.position - m.negMag.position) * magnetData.magnetForceScalar;
                            m.rigidBody.AddForce(moveVector);
                        }
                        #endregion

                        // Allows for chain-like movement between magnets (Default), such as whip-like movement and dangling down.
                        #region AbsoluteMovementMethod
                        if (chainMagnets)
                        {
                            m.transform.parent.position = m.transform.parent.position + (m.magnetizedTo.posMag.position - m.negMag.position) / 3;
                            m.rigidBody.AddForce(Vector2.down * m.rigidBody.drag * m.rigidBody.mass - m.rigidBody.gravityScale * Vector2.down);
                        }
                        #endregion

                        // Rotate towards target magnet
                        Vector3 rotateVector = (m.magnetizedTo.transform.position - m.negMag.position).normalized;
                        if (rotateVector.y >= 0)
                            m.transform.parent.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Acos(rotateVector.x) * (180 / Mathf.PI) - 90));
                        else
                            m.transform.parent.rotation = Quaternion.Euler(new Vector3(0, 0, -Mathf.Acos(rotateVector.x) * (180 / Mathf.PI) - 90));
                    }
                }
                else if(_distance > snapOnRange + 3) // Detach if too far away from target magnet
                {
                    if (!m.magnetActive)
                    {
                        m.magnetActive = true;
                    }
                }
            }
        }
    }
}

