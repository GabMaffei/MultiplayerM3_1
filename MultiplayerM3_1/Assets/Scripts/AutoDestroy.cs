using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AutoDestroy : NetworkBehaviour
{
    [SerializeField] float delayBeforeDestroy = 5f;

    [Rpc(SendTo.Server)]
    private void DestroyParticlesrequestRpc(RpcParams rpcParams = default)
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject, delayBeforeDestroy);
    }

}
