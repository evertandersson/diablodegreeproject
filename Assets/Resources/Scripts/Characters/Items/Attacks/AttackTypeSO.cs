using UnityEngine;

public abstract class AttackTypeSO : ActionItemSO
{
    [HideInInspector]
    public AttackInstance attackInstance;

    public float attackDelay;
    public GameObject projectile;
    public string attackTrigger;
    public float damage = 5;
    public int projectileCount = 1;
    public bool followEnemies = false;
}
