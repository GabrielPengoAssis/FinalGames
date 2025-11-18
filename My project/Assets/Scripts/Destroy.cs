using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Destroy : MonoBehaviour // destroy o objeto
{

    void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {

            Destroy(gameObject,1f); // Destroi o objeto coletado
        }
        
    }
}