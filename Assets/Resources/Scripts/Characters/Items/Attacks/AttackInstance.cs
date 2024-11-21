using UnityEngine;

public class AttackInstance
{
    public AttackTypeSO baseAttack;
    public float currentDamage;
    public float currentCooldown;
    public int currentProjectileCount;
    public bool currentFollowEnemies;

    public AttackInstance(AttackTypeSO attack)
    {
        this.baseAttack = attack;
        currentDamage = attack.damage;
        currentCooldown = attack.cooldown;
        currentProjectileCount = attack.projectileCount;
        currentFollowEnemies = attack.followEnemies;
    }

    public void ApplyUpgrade(AttackUpgradeSO upgrade)
    {
        currentDamage += upgrade.additionalDamage;
        currentCooldown -= upgrade.cooldownReduction;
        currentProjectileCount += upgrade.additionalProjectiles;
        if (upgrade.enableTracking)
            currentFollowEnemies = true;
    }
}
