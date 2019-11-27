using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindPuffs : MonoBehaviour
{
    private Animator anim;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.enabled = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.enabled = true;
        }        
    }
}