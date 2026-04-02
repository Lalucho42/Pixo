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
        if (player.WeaponManager == null || !player.WeaponManager.HasWeapon) return;
        if (Time.time < nextAttackTime) return;

        if (OnAttackRequested != null) OnAttackRequested.Invoke();
        nextAttackTime = Time.time + player.WeaponManager.CurrentTool.attackRate;
    }

    public void ExecuteHit()
    {
        ToolItem tool = player.WeaponManager.CurrentTool;
        if (tool == null) return;

        if (tool.attackSound != null && AudioManager.instance != null)
            AudioManager.instance.PlaySFX(tool.attackSound);

        Collider[] hitEnemies = Physics.OverlapSphere(player.attackPoint.position, tool.attackRange);

        foreach (Collider hit in hitEnemies)
        {
            HealthSystem health = hit.GetComponentInParent<HealthSystem>();
            if (health != null) health.TakeDamage(tool.attackDamage);

            ResourceNode node = hit.GetComponentInParent<ResourceNode>();
            if (node != null) node.TakeDamage(tool.resourceDamage);
        }

        // SE ELIMINÓ EL GENERATE IMPULSE AQUÍ TAMBIÉN
    }
}