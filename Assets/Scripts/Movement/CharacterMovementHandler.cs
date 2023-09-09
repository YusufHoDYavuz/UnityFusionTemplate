using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterMovementHandler : NetworkBehaviour
{
    private bool isSpawnRequested;
    
    private NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;
    private HPHandler hpHandler;

    private NetworkInGameUIMessages networkInGameUIMessages;
    private NetworkPlayer networkPlayer;

    private void Awake()
    {
        networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        hpHandler = GetComponent<HPHandler>();
        networkInGameUIMessages = GetComponent<NetworkInGameUIMessages>();
        networkPlayer = GetComponent<NetworkPlayer>();
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            if (isSpawnRequested)
            {
                Respawn();
                return;
            }
            
            if (hpHandler.isDead)
                return;
        }
        
        
        if (GetInput(out NetworkInputData networkInputData))
        {
            //Rotate the transform according to the client aim vector
            transform.forward = networkInputData.aimForwardVector;

            //Cancel out rotation on X axis as we don't want our character to tilt
            Quaternion rotation = transform.rotation;
            rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
            transform.rotation = rotation;

            //Move
            Vector3 moveDirection = transform.forward * networkInputData.movementInput.y +
                                    transform.right * networkInputData.movementInput.x;

            moveDirection.Normalize();
            networkCharacterControllerPrototypeCustom.Move(moveDirection);

            //Jump
            if (networkInputData.isJumpPressed)
                networkCharacterControllerPrototypeCustom.Jump();

            //Ceheck if we've fallen off the world
            if (transform.position.y < -12)
                CheckFallRespawn();
        }
    }

    private void CheckFallRespawn()
    {
        if (Object.HasStateAuthority)
        {
            Debug.Log($"{Time.time} Respawn due to fall outside of map at position {transform.position}");

            networkInGameUIMessages.SendInGameRPCMessage(networkPlayer.nickName.ToString(), "fell of the world");
            
            Respawn();
        }
    }

    public void RequestSpawn()
    {
        isSpawnRequested = true;
    }

    private void Respawn()
    {
        networkCharacterControllerPrototypeCustom.TeleportToPosition(Utils.GetRandomSpawnPoint());

        hpHandler.OnRespawned();
        
        isSpawnRequested = false;
    }

    public void SetCharacterControllerEnabled(bool isEnabled)
    {
        networkCharacterControllerPrototypeCustom.Controller.enabled = isEnabled;
    }
}