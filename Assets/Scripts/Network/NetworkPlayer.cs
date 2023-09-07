using Fusion;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer localPlayer { get; set; }

    [SerializeField] private Transform playerModel;
    
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            localPlayer = this;
            
            //Sets the layer of the local players model
            Utils.SetRenderLayerInChildren(playerModel, LayerMask.NameToLayer("LocalPlayerModel"));
            
            //Disable main camera
            Camera.main.gameObject.SetActive(false);
            
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
            
            Debug.Log("Spawn remote Player");
        }
        
        //Make it easier to tell which player is which
        transform.name = $"P_{Object.Id}";
    }
    
    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
            Runner.Despawn(Object);
    }
}
