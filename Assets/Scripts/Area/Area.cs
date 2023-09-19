using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Area : NetworkBehaviour
{
    
    private float coolDownTime = 3f;
    private bool isStartTime = false;

    private GameObject otherObject;
    private MeshRenderer bodyMeshRenderer;

    private void Start()
    {
        bodyMeshRenderer = GetComponent<MeshRenderer>();
    }

    public void FixedUpdate()
    {
        if (isStartTime)
        {
            coolDownTime -= Time.deltaTime;
            if (coolDownTime <= 0f)
            {
                FinishedTime();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerColor>() != null)
        {
            otherObject = other.gameObject;
            isStartTime = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerColor>() != null)
        {
            isStartTime = false;
            coolDownTime = 3f;
        }
    }
    
    private void FinishedTime()
    {
        bodyMeshRenderer.material.color = otherObject.GetComponent<PlayerColor>().networkedColor;
        coolDownTime = 3f;
        isStartTime = false;
    }
}
