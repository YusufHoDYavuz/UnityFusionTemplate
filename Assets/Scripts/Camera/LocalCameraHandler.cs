using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class LocalCameraHandler : MonoBehaviour
{
    [SerializeField] private Transform cameraAnchorPoint;
    
    //Input
    private Vector2 viewInput;
    
    //Rotation
    private float cameraRotationX;
    private float cameraRotationY;
    
    //Other components
    private Camera localCamera;
    private NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;
    
    void Awake()
    {
        localCamera = GetComponent<Camera>();
        networkCharacterControllerPrototypeCustom = GetComponentInParent<NetworkCharacterControllerPrototypeCustom>();
    }

    private void Start()
    {
        if (localCamera.enabled)
            localCamera.transform.parent = null;
    }

    void LateUpdate()
    {
        if (cameraAnchorPoint == null) return;

        if (!localCamera.enabled) return;
        
        //Move the camera to the position of the player
        localCamera.transform.position = cameraAnchorPoint.position;
        
        //Calculate rotation
        cameraRotationX += viewInput.y * Time.deltaTime *
                           networkCharacterControllerPrototypeCustom.viewUpDownRotationSpeed;

        cameraRotationX = Mathf.Clamp(cameraRotationX, -90, 90);

        cameraRotationY += viewInput.x * Time.deltaTime *
                           networkCharacterControllerPrototypeCustom.rotationSpeed;
        
        //Apply rotation
        localCamera.transform.rotation = Quaternion.Euler(cameraRotationX, cameraRotationY, 0);
    }

    public void SetViewInputVector(Vector2 viewInput)
    {
        this.viewInput = viewInput;
    }
}
