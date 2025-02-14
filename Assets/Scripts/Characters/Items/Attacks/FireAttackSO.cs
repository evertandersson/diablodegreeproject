using UnityEngine;

[CreateAssetMenu(fileName = "FireAttack", menuName = "Attacks/Fire")]
public class FireAttackSO : AttackTypeSO
{
    FireAttackSO()
    {
        cooldown = 2.0f;
        nextAttackDelay = 1.0f;
        bufferedInputDelay = 0.5f;
        timerCooldown = cooldown;
    }

    public override void PerformAction(Animator animator)
    {
        Debug.Log("Fire Attack");
        attackHashString = Animator.StringToHash("FireAttack");
        SoundManager.PlaySound(SoundType.FIREATTACK);
        animator.SetTrigger(attackHashString);
        timerCooldown = 0;
    }
}