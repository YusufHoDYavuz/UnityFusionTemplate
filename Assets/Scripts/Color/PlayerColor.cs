using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerColor : NetworkBehaviour
{
    [SerializeField] private MeshRenderer bodyMesh;
    
    [Networked(OnChanged = nameof(NetworkColorChanged))]

    public Color networkedColor { get; set; }
    
    void Start()
    {
        float randomValue = Random.Range(0f, 1f);
        networkedColor = new Color(randomValue, randomValue, randomValue, 1f);
    }

    public static void NetworkColorChanged(Changed<PlayerColor> changed)
    {
        changed.Behaviour.bodyMesh.material.color = changed.Behaviour.networkedColor;
    }
}
