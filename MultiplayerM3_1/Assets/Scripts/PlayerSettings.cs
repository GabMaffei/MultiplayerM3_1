using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
// using Unity.Tutorials.Core.Editor;
using UnityEngine;

public class PlayerSettings : NetworkBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI displayHitCount;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshProUGUI loserText;
    private NetworkVariable<FixedString128Bytes> networkPlayerName = new NetworkVariable<FixedString128Bytes>(
        "Player: 0", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public List<Color> colors = new List<Color>();
    public NetworkList<int> playerHitCount = new NetworkList<int>(
        default, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);

    private void Awake() {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        networkPlayerName.Value = "Player: " + (OwnerClientId + 1);
        playerName.text = networkPlayerName.Value.ToString();
        meshRenderer.material.color = colors[(int)OwnerClientId];
        InstantiateHitPointListRequestRpc(OwnerClientId);
    }

    private void Update()
    {
        displayHitCount.text = "Hits: " + playerHitCount[(int) OwnerClientId].ToString();
        try 
        {

            if (playerHitCount[(int) OwnerClientId] >=  10)
            {
                winnerText.text = "Winner";
                // LossAnRequestRpc();
            }
        } catch (Exception)
        {
            Debug.Log("playerHitCount not instantiated in client!");
        }
        
    }

    [Rpc(SendTo.Everyone, RequireOwnership = false)]
    public void LossAnRequestRpc(RpcParams rpcParams = default)
    {
        if (winnerText.text != "Winner")
        {
            loserText.text = "Loser";
        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void InstantiateHitPointListRequestRpc(ulong playerId, RpcParams rpcParams = default)
    {
        playerHitCount.Insert((int) playerId, 0);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void CountHitPointsRequestRpc(ulong playerId, RpcParams rpcParams = default)
    {
        playerHitCount[(int) playerId] += 1;

        // string result = "List contents: ";
        // foreach (var item in playerHitCount)
        // {
        //     result += item.ToString() + ", ";
        // }
        // Debug.Log(result);
    }

}
