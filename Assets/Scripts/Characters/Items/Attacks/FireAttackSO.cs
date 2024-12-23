using UnityEngine;

[CreateAssetMenu(fileName = "FireAttack", menuName = "Attacks/Fire")]
public class FireAttackSO : AttackTypeSO
{
    [HideInInspector] public int attackTriggerHash = Animator.StringToHash("FireAttack");

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
        SoundManager.PlaySound(SoundType.FIREATTACK);
        animator.SetTrigger(attackTriggerHash);
        timerCooldown = 0;
    }
}