using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    private Vector2 moveInputVector = Vector2.zero;
    private Vector2 viewInputVector = Vector2.zero;
    private bool isJumpButtonPressed;
    private bool isFireButtonPressed;

    //Other Components
    private LocalCameraHandler localCameraHandler;
    private CharacterMovementHandler characterMovementHandler;

    private void Awake()
    {
        localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
        characterMovementHandler = GetComponentInChildren<CharacterMovementHandler>();
    }
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!characterMovementHandler.Object.HasInputAuthority)
            return;
        
        //View Input
        viewInputVector.x = Input.GetAxis("Mouse X");
        viewInputVector.y = Input.GetAxis("Mouse Y") * -1; //for invert;

        //Move Input
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        //Jump
        if (Input.GetButtonDown("Jump"))
            isJumpButtonPressed = true; 
        
        //Fire
        if (Input.GetButtonDown("Fire1"))
            isFireButtonPressed = true;
        
        //Set view
        localCameraHandler.SetViewInputVector(viewInputVector);
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        //Aim Data
        networkInputData.aimForwardVector = localCameraHandler.transform.forward;

        //Move Data
        networkInputData.movementInput = moveInputVector;

        //Jump Data
        networkInputData.isJumpPressed = isJumpButtonPressed;
        
        //Fire Data
        networkInputData.isFirePressed = isFireButtonPressed;
        
        // Reset variables now that we have read their states
        isJumpButtonPressed = false;
        isFireButtonPressed = false;
        
        return networkInputData;
    }
}