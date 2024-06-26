using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShootFireBall : NetworkBehaviour
{

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform[] projectileSpawnPoint;
    // List to hold all the instantiated fireballs
    [SerializeField] private List<GameObject> spawnedBullets = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetMouseButtonDown(0))
        {
            foreach(Transform SpawnPoints in projectileSpawnPoint)
            {
                InstantiateBulletRequestRpc(SpawnPoints.position);
            }
        }
    }

    [Rpc(SendTo.Server)]
    void InstantiateBulletRequestRpc(Vector3 shootingPosition, RpcParams rpcParams = default)
    {
        GameObject bullet = Instantiate(projectilePrefab, shootingPosition, transform.rotation);
        Physics.IgnoreCollision(bullet.GetComponent<Collider>(), GetComponentInChildren<CapsuleCollider>());
        spawnedBullets.Add(bullet);
        bullet.GetComponent<MoveProjectile>().parent = this;
        bullet.GetComponent<NetworkObject>().Spawn();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void DestroyRequestRpc(RpcParams rpcParams= default)
    {
        GameObject toDestroy = spawnedBullets[0];
        toDestroy.GetComponent<NetworkObject>().Despawn();
        spawnedBullets.Remove(toDestroy);
        Destroy(toDestroy);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void HitDetectedRpc(ulong playerId, RpcParams rpcParams= default)
    {
        gameObject.GetComponent<PlayerSettings>().CountHitPointsRequestRpc(playerId);
    }
}
