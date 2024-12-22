using UnityEngine;

public abstract class AttackTypeSO : ActionItemSO
{
    public float nextAttackDelay;
    public float bufferedInputDelay;
    public GameObject projectile;
    public string attackTrigger;
}
