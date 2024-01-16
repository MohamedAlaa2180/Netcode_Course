using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] InputReader inputReader;
    [SerializeField] Transform weaponHandle;

    private Vector2 aimingPosition;
    private void LateUpdate()
    {
        if (!IsOwner) return;

        weaponHandle.up = new Vector2 (
            aimingPosition.x - weaponHandle.position.x,
            aimingPosition.y - weaponHandle.position.y);
    }

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) return;
        inputReader.AimingEvent += HandleAiming; 
    }    

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputReader.AimingEvent -= HandleAiming;
    }

    private void HandleAiming(Vector2 aimingScreenPosition)
    {
        aimingPosition = Camera.main.ScreenToWorldPoint(aimingScreenPosition);
    }
}
