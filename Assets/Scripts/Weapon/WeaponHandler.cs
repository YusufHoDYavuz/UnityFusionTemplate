using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Object = System.Object;

public class WeaponHandler : NetworkBehaviour
{
    public bool isFiring { get; set; }

    [SerializeField] private ParticleSystem fireParticle;

    private float lastTimeFired;
    
    public override void FixedUpdateNetwork()
    {
        //Get the Input from network
        if (GetInput(out NetworkInputData networkInputData))
        {
            if (networkInputData.isFirePressed)
                Fire(networkInputData.aimForwardVector);
        }
    }

    private void Fire(Vector3 aimForwardVector)
    {
        //Limit fire rate
        if (Time.time - lastTimeFired < 0.15f)
            return;

        StartCoroutine(FireEffectCO());

        lastTimeFired = Time.time;
    }

    private IEnumerator FireEffectCO()
    {
        isFiring = true;
        
        fireParticle.Play();

        yield return new WaitForSeconds(0.09f);
        
        isFiring = false;
    }

    static void OnFireChanged(Changed<WeaponHandler> changed)
    {
        Debug.Log($"{Time.time} On Fire Changed value {changed.Behaviour.isFiring}");

        bool isFiringCurrent = changed.Behaviour.isFiring;
        
        //Load the old value
        changed.LoadOld();;

        bool isFiringOld = changed.Behaviour.isFiring;

        if (isFiringCurrent && !isFiringOld)
            changed.Behaviour.OnFireRemote();
    }

    private void OnFireRemote()
    {
        if (!Object.HasInputAuthority)
            fireParticle.Play();
    }
}
