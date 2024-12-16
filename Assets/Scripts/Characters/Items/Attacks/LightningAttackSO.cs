using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "LightningAttack", menuName = "Attacks/Lightning")]
public class LightningAttackSO : AttackTypeSO
{
    LightningAttackSO()
    {
        cooldown = 2.0f;
        attackDelay = 1.3f;
        timerCooldown = cooldown;
    }

    public override void PerformAction(Animator animator)
    {
        Debug.Log("Lightning Attack");
        SoundManager.PlaySound(SoundType.LIGHTNINGATTACK);
        animator.SetTrigger(attackTrigger);
        timerCooldown = 0;
    }
}
