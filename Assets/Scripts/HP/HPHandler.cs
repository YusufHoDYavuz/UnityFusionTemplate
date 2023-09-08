using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class HPHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnHPChanged))]
    private byte HP { get; set; }
    
    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool isDead { get; set; }

    private bool isInitialized = false;
    private const byte startingHP = 100;
    
    void Start()
    {
        HP = startingHP;
        isDead = false;
    }

    public void OnTakeDamage()
    {
        //Only take damage while alive
        if (isDead)
            return;

        HP -= 20;
        
        Debug.Log($"{Time.time} {transform.name} took damage got {HP} left");

        //Player die
        if (HP <= 0)
        {
            Debug.Log($"{Time.time} {transform.name} die");
            isDead = false;
        }
    }

    static void OnHPChanged(Changed<HPHandler> changed)
    {
        Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.HP} die");
    }

    static void OnStateChanged(Changed<HPHandler> changed)
    {
        Debug.Log($"{Time.time} OnStateChanged isDead {changed.Behaviour.isDead} die");
    }
    
}
