using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EarthShatterAttack", menuName = "Attacks/EarthShatter")]
public class EarthShatterAttackSO : AttackTypeSO
{
    EarthShatterAttackSO()
    {
        cooldown = 4.0f;
        nextAttackDelay = 1.0f;
        bufferedInputDelay = 0.5f;
        timerCooldown = cooldown;
    }

    public override void PerformAction(Animator animator)
    {
        Debug.Log("Earth Shatter Attack");
        if (attackHashString == 0) attackHashString = Animator.StringToHash("EarthShatterAttack");
        
        SoundManager.PlaySound(SoundType.FIREATTACK);
        animator.SetTrigger(attackHashString);
        timerCooldown = 0;
    }
}
