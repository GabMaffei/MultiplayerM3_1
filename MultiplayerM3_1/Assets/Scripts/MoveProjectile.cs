using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveProjectile : MonoBehaviour
{

    [SerializeField] private GameObject hitParticles;
    [SerializeField] private float shootForce = 50f;
    [SerializeField] private float timeParticles = 2f;
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
        // instantiate the hit particles when we collide with something then destroy the fireball
        GameObject hitImpact = Instantiate(hitParticles, transform.position, Quaternion.identity);
        hitImpact.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
        Destroy(hitImpact, timeParticles);
        Destroy(gameObject);
    }
}
