using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterMovementHandler : NetworkBehaviour
{
    private NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;

    private Camera localCamera;

    private void Awake()
    {
        networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        localCamera = GetComponentInChildren<Camera>();
    }

    public override void FixedUpdateNetwork()
    {
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
        transform.position = Utils.GetRandomSpawnPoint();
    }
}