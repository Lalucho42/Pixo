using UnityEngine;
using System;

public class PlayerCombat
{
    private Player player;
    private float nextAttackTime = 0f;
    public event Action OnAttackRequested;

    public PlayerCombat(Player playerBrain)
    {
        player = playerBrain;
        player.InputHandler.OnAttackEvent += HandleAttackIntent;
    }

    public void Tick(float deltaTime) { }

    private void HandleAttackIntent()
    {
       
        if (player.IsMovementLocked) return;

        if (player.WeaponManager == null || !player.WeaponManager.HasWeapon) return;

        if (player.WeaponManager.CurrentTool.usosActuales <= 0) return;

        if (Time.time < nextAttackTime) return;

        if (OnAttackRequested != null) OnAttackRequested.Invoke();

        nextAttackTime = Time.time + player.WeaponManager.CurrentTool.attackRate;
    }
}