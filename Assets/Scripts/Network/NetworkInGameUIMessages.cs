using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkInGameUIMessages : NetworkBehaviour
{
    private InGameMessagesUIHandler inGameMessagesUIHandler;
    
    void Start()
    {
        
    }

    public void SendInGameRPCMessage(string userNickName, string message)
    {
        RPC_InGameMessage($"<b>{userNickName}</b> {message}");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_InGameMessage(string message, RpcInfo info = default)
    {
        Debug.Log($"[RPC] InGameMessage {message}");

        if (inGameMessagesUIHandler == null)
        {
            inGameMessagesUIHandler = NetworkPlayer.localPlayer.localCameraHandler
                .GetComponentInChildren<InGameMessagesUIHandler>();
        }
        
        if (inGameMessagesUIHandler != null)
            inGameMessagesUIHandler.OnGameMessageReceived(message);
    }
}
