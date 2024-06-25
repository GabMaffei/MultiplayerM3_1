using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{

    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float rotationspeed = 500f;
    [SerializeField] private float positionRange = 5f;
    private Animator animator;

    void Start()
    {
        // reference to the animator
        animator = GetComponent<Animator>();
    }

    public override void OnNetworkSpawn()
    {
        UpdatePositionRequestRpc();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void UpdatePositionRequestRpc(RpcParams rpcParams = default)
    {
        transform.position = new Vector3 (Random.Range(positionRange, -positionRange), 0, Random.Range(positionRange, -positionRange));
        transform.Translate(transform.up);
        transform.rotation = new Quaternion(0,180,0,0);
    }

    void Update()
    {
        if(!IsOwner) return;
        // cache imput values in floats
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // create movement direction vector3 and store the vertical and horizontal values in it
        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
        movementDirection.Normalize();

        // Moves the transform in the movement direction
        transform.Translate(movementDirection * movementSpeed * Time.deltaTime, Space.World);

        // rotate the player to face the movement direction
        if (movementDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationspeed * Time.deltaTime);
        }

        // change the animation based on the movement value
        animator.SetFloat("run", movementDirection.magnitude);
        
    }
}
