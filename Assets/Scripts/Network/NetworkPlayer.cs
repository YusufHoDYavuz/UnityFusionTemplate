using System;
using Fusion;
using UnityEngine;
using TMPro;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    [SerializeField] private TextMeshProUGUI playerNickNameTM;

    public static NetworkPlayer localPlayer { get; set; }
    [SerializeField] private Transform playerModel;

    [Networked(OnChanged = nameof(OnNickNameChanged))]
    public NetworkString<_16> nickName { get; set; }

    private bool isPublicJoinMessageSent = false;

    public LocalCameraHandler localCameraHandler;
    [SerializeField] private GameObject localUI;

    //other components
    private NetworkInGameUIMessages networkInGameUIMessages;

    private void Awake()
    {
        networkInGameUIMessages = GetComponent<NetworkInGameUIMessages>();
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            localPlayer = this;

            //Sets the layer of the local players model
            Utils.SetRenderLayerInChildren(playerModel, LayerMask.NameToLayer("LocalPlayerModel"));

            //Disable main camera
            Camera.main.gameObject.SetActive(false);

            RPC_SetNickName(PlayerPrefs.GetString("PlayerNickname"));

            Debug.Log("Spawn Local Player");
        }
        else
        {
            //Disable the camera if we are not the local player
            Camera localCamera = GetComponentInChildren<Camera>();
            localCamera.enabled = false;

            //Only 1 audio listener is allowed in the scene so disable remote players audio listener
            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;

            localUI.SetActive(false);

            Debug.Log("Spawn remote Player");
        }

        Runner.SetPlayerObject(Object.InputAuthority, Object);

        //Make it easier to tell which player is which
        transform.name = $"P_{Object.Id}";
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (Object.HasStateAuthority)
        {
            if (Runner.TryGetPlayerObject(player, out NetworkObject playerLeftGameObjcet))
            {
                if (playerLeftGameObjcet == Object)
                    localPlayer.GetComponent<NetworkInGameUIMessages>()
                        .SendInGameRPCMessage(playerLeftGameObjcet.GetComponent<NetworkPlayer>().nickName.ToString(),
                            "left!");
            }
        }

        if (player == Object.InputAuthority)
            Runner.Despawn(Object);
    }

    static void OnNickNameChanged(Changed<NetworkPlayer> changed)
    {
        Debug.Log($"{Time.time} OnNicknameChanged value {changed.Behaviour.nickName}");

        changed.Behaviour.OnNickNameChanged();
    }

    private void OnNickNameChanged()
    {
        Debug.Log($"Nickname changed for player to {nickName} for player {gameObject.name}");

        playerNickNameTM.text = nickName.ToString();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetNickName(string nickName, RpcInfo info = default)
    {
        Debug.Log($"[RPC] SetNickName {nickName}");
        this.nickName = nickName;

        if (!isPublicJoinMessageSent)
        {
            networkInGameUIMessages.SendInGameRPCMessage(nickName, "joined!");

            isPublicJoinMessageSent = true;
        }
    }
}