using UnityEngine;

public abstract class AttackTypeSO : ActionItemSO
{
    public float damageMultiplier;
    public float nextAttackDelay;
    public float bufferedInputDelay;
    public GameObject projectile;
    public string attackTrigger;
    public int manaCost;
}
