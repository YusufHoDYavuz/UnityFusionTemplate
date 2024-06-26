using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPlayer playerPrefab;
    
    public List<Vector3> spawnPoints = new();
    private List<Vector3> firstSpawnPoint = new();

    //Other Components
    private CharacterInputHandler characterInputHandler;

    #region Network Runner Call Backs

    private void Start()
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            firstSpawnPoint.Add(spawnPoints[i]);
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Debug.Log("On Player Joined we are server. Spawning Player");
            
            int randomValue = Random.Range(0, firstSpawnPoint.Count);
            runner.Spawn(playerPrefab, firstSpawnPoint[randomValue], Quaternion.identity, player);
            firstSpawnPoint.RemoveAt(randomValue);
        }
        else Debug.Log("On Player Joined");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (characterInputHandler == null && NetworkPlayer.localPlayer != null)
            characterInputHandler = NetworkPlayer.localPlayer.GetComponent<CharacterInputHandler>();

        if (characterInputHandler != null)
            input.Set(characterInputHandler.GetNetworkInput());
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("On Shut Down");
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("On Connected To Server");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("On Disconnected From Server");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("On Connect Request");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("On Connect Failed");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    #endregion
}