using UnityEngine;

[CreateAssetMenu(fileName = "FireAttack", menuName = "Attacks/Fire")]
public class FireAttackSO : AttackTypeSO
{
    FireAttackSO()
    {
        cooldown = 2.0f;
        attackDelay = 1.0f;
        timerCooldown = cooldown;
    }

    public override void PerformAction(Animator animator)
    {
        Debug.Log("Fire Attack");
        animator.SetTrigger(attackTrigger);
        timerCooldown = 0;
    }
}