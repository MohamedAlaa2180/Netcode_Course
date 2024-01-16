using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private GameObject _serverProjectilePrefab;
    [SerializeField] private GameObject _clientProjectilePrefab;
    [SerializeField] private Collider2D _playerCollider;

    [Header("Settings")]
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _fireRate;

    private float _lastFireTime;
    private bool _shouldFire;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        _inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }
    private void HandlePrimaryFire(bool shouldFire)
    {
        _shouldFire = shouldFire;
    }

    #region Firing
    private void Update()
    {       
        if (!IsOwner || !_shouldFire) return;

        if (Time.time < (1 / _fireRate) + _lastFireTime) return;

        SpawnClientProjectile(_projectileSpawnPoint.position, _projectileSpawnPoint.up);
        SpawnServerProjectileServerRpc();

        _lastFireTime = Time.time;
    }

    [ServerRpc]
    private void SpawnServerProjectileServerRpc()
    {
        GameObject serverProjectileInstance = SpawnServerProjectile(_projectileSpawnPoint.position, _projectileSpawnPoint.up);

        SpawnClientProjectileClientRpc(serverProjectileInstance.transform.position, serverProjectileInstance.transform.up);
    }

    [ClientRpc]
    private void SpawnClientProjectileClientRpc(Vector3 spawnPos, Vector3 direction)
    {
        if (!IsOwner)
        {
            SpawnClientProjectile(spawnPos, direction);
        }
    }

    private GameObject SpawnClientProjectile(Vector3 spawnPos, Vector3 direction)
    {
        GameObject clientProjectileInstance = Instantiate(_clientProjectilePrefab, spawnPos, Quaternion.identity);
        clientProjectileInstance.transform.up = direction;
        Physics2D.IgnoreCollision(_playerCollider, clientProjectileInstance.GetComponent<Collider2D>());
        Fire(clientProjectileInstance);
        return clientProjectileInstance;
    }
    private GameObject SpawnServerProjectile(Vector3 spawnPos, Vector3 direction)
    {
        GameObject serverProjectileInstance = Instantiate(_serverProjectilePrefab, spawnPos, Quaternion.identity);
        serverProjectileInstance.transform.up = direction;
        Physics2D.IgnoreCollision(_playerCollider, serverProjectileInstance.GetComponent<Collider2D>());
        Fire(serverProjectileInstance);
        return serverProjectileInstance;
    }

    private void Fire(GameObject projectileInstance)
    {
        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * _projectileSpeed;
        }
    }
    #endregion

}
