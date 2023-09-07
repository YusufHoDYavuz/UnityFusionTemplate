using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); // Sağ/Sol tuşları (A ve D)
        float verticalInput = Input.GetAxis("Vertical"); // İleri/Geri tuşları (W ve S)

        Vector3 movement = new Vector3(horizontalInput, 0.0f, verticalInput) * moveSpeed * Time.deltaTime;

        transform.Translate(movement);
    }
}