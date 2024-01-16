using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody2D rb;
    /* [SerializeField] private Transform weaponHandler; */

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 4f;

    private Vector2 previousMovementInput;

    private void Update()
    {
        if (!IsOwner) return;

        if (previousMovementInput.x > 0)
        {
            bodyTransform.localScale = new Vector3(-Mathf.Abs(bodyTransform.localScale.x), bodyTransform.localScale.y, bodyTransform.localScale.z);
        }
        else if (previousMovementInput.x < 0)
        {
            bodyTransform.localScale = new Vector3(Mathf.Abs(bodyTransform.localScale.x), bodyTransform.localScale.y, bodyTransform.localScale.z);
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        rb.velocity = previousMovementInput * movementSpeed;
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        inputReader.MovementEvent += HandleMovement;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputReader.MovementEvent -= HandleMovement;
    }

    private void HandleMovement(Vector2 movementInput)
    {
        previousMovementInput = movementInput;
    }
}
