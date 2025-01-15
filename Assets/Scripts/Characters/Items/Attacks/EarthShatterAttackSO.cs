using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EarthShatterAttack", menuName = "Attacks/EarthShatter")]
public class EarthShatterAttackSO : AttackTypeSO
{
    [HideInInspector] public int attackTriggerHash = Animator.StringToHash("EarthShatterAttack");

    EarthShatterAttackSO()
    {
        cooldown = 4.0f;
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
