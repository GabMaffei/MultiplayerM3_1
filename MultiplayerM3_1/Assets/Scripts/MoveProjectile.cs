using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MoveProjectile : NetworkBehaviour
{
    public ShootFireBall parent;
    [SerializeField] private GameObject hitParticles;
    [SerializeField] private float shootForce = 50f;
    private Rigidbody rb;

    void Start()
    {
        // reference for the rigidbody
        rb = GetComponent<Rigidbody>();
        // rb.velocity = rb.transform.forward * shootForce;
    }

    void Update()
    {
        // Move the fireball foward based on the player facing direction
        rb.velocity = rb.transform.forward * shootForce;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!IsOwner) return;
        InstantiateHitParticlesRequestRpc();
        //Add hit
        // Debug.Log("Hit by: " + (int) parent.OwnerClientId);
        parent.HitDetectedRpc(parent.OwnerClientId);
        parent.DestroyRequestRpc();
    }

    [Rpc(SendTo.Server)]
    private void InstantiateHitParticlesRequestRpc()
    {
        // instantiate the hit particles when we collide with something then destroy the fireball
        GameObject hitImpact = Instantiate(hitParticles, transform.position, Quaternion.identity);
        hitImpact.GetComponent<NetworkObject>().Spawn();
        hitImpact.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
    }
}
