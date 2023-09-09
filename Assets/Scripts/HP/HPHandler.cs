using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class HPHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnHPChanged))]
    private byte HP { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool isDead { get; set; }

    private bool isInitialized = false;
    private const byte startingHP = 100;

    [SerializeField] private Color uiOnHitColor;
    [SerializeField] private Image uiOnHitImage;

    [SerializeField] private MeshRenderer bodyMeshRenderer;
    private Color defaultMeshBodyColor;

    [SerializeField] private GameObject playerModel;
    [SerializeField] private GameObject deathGameObjectPrefab;

    //Other Components
    private HitboxRoot hitboxRoot;
    private CharacterMovementHandler characterMovementHandler;
    private NetworkInGameUIMessages networkInGameUIMessages;
    private NetworkPlayer networkPlayer;

    private void Awake()
    {
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
        hitboxRoot = GetComponentInChildren<HitboxRoot>();
        networkInGameUIMessages = GetComponentInChildren<NetworkInGameUIMessages>();
        networkPlayer = GetComponentInChildren<NetworkPlayer>();
    }

    void Start()
    {
        HP = startingHP;
        isDead = false;
        defaultMeshBodyColor = bodyMeshRenderer.material.color;

        isInitialized = true;
    }

    private IEnumerator OnHitCO()
    {
        bodyMeshRenderer.material.color = Color.white;

        if (Object.HasInputAuthority)
            uiOnHitImage.color = uiOnHitColor;

        yield return new WaitForSeconds(0.2f);

        bodyMeshRenderer.material.color = defaultMeshBodyColor;

        if (Object.HasInputAuthority && !isDead)
            uiOnHitImage.color = new Color(0, 0, 0, 0);
    }

    private IEnumerator ServerReviveCO()
    {
        yield return new WaitForSeconds(2.0f);
        
        characterMovementHandler.RequestSpawn();
    }

    public void OnTakeDamage(string damageCausedByPlayerNickname)
    {
        //Only take damage while alive
        if (isDead)
            return;

        HP -= 20;

        Debug.Log($"{Time.time} {transform.name} took damage got {HP} left");

        //Player die
        if (HP <= 0)
        {
            networkInGameUIMessages.SendInGameRPCMessage(damageCausedByPlayerNickname, $"Killed <b>{networkPlayer.nickName.ToString()}</b>");
            
            Debug.Log($"{Time.time} {transform.name} die");

            StartCoroutine(ServerReviveCO());
            
            isDead = false;
        }
    }

    static void OnHPChanged(Changed<HPHandler> changed)
    {
        Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.HP} die");

        byte newHP = changed.Behaviour.HP;

        changed.LoadOld();

        byte oldHP = changed.Behaviour.HP;

        //Check if the HP has been decreased
        if (newHP < oldHP)
            changed.Behaviour.OnHPReduced();
    }

    private void OnHPReduced()
    {
        if (!isInitialized)
            return;

        StartCoroutine(OnHitCO());
    }

    static void OnStateChanged(Changed<HPHandler> changed)
    {
        Debug.Log($"{Time.time} OnStateChanged isDead {changed.Behaviour.isDead} die");

        bool isDeadCurrent = changed.Behaviour.isDead;

        changed.LoadOld();

        bool isDeadOld = changed.Behaviour.isDead;

        //Handle on death for the player. Also check if the player was dead but is now alive in that case revieve the player.
        if (isDeadCurrent)
            changed.Behaviour.OnDeath();
        else if (!isDeadCurrent && isDeadOld)
            changed.Behaviour.OnRevive();
    }

    private void OnDeath()
    {
        Debug.Log($"{Time.time} death");
        
        playerModel.SetActive(false);
        hitboxRoot.HitboxRootActive = false;
        characterMovementHandler.SetCharacterControllerEnabled(false);

        Instantiate(deathGameObjectPrefab, transform.position, Quaternion.identity);
    }

    private void OnRevive()
    {
        Debug.Log($"{Time.time} revive");

        if (Object.HasInputAuthority)
            uiOnHitImage.color = new Color(0, 0, 0, 0);            
        
        playerModel.SetActive(true);
        hitboxRoot.HitboxRootActive = true;
        characterMovementHandler.SetCharacterControllerEnabled(true);
    }

    public void OnRespawned()
    {
        HP = startingHP;
        isDead = false;
    }
}