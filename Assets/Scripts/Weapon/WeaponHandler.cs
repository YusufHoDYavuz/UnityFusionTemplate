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
    [SerializeField] private Transform aimPoint;
    [SerializeField] private LayerMask collisionLayers;

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

        //WEAPON HIT INTERACT
        Runner.LagCompensation.Raycast(aimPoint.position, aimForwardVector, 100f, Object.InputAuthority,
            out var hitInfo, collisionLayers, HitOptions.IncludePhysX);

        float hitDistance = 100f;
        bool isHitOtherPlayer = false;

        if (hitInfo.Distance > 0)
            hitDistance = hitInfo.Distance;

        if (hitInfo.Hitbox != null)
        {
            Debug.Log($"{Time.time} {transform.name} hit hitbox {hitInfo.Hitbox.transform.root.name}");
            isHitOtherPlayer = true;
        }
        else if (hitInfo.Collider != null)
        {
            Debug.Log($"{Time.time} {transform.name} hit PhysX collider {hitInfo.Collider.transform.root.name}");
        }
        
        //Debug
        if (isHitOtherPlayer)
            Debug.DrawRay(aimPoint.position, aimForwardVector * hitDistance, Color.green, 1);
        else
            Debug.DrawRay(aimPoint.position, aimForwardVector * hitDistance, Color.red, 1);

        
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
