using Game;
using UnityEngine;

public class JumpAttack : EnemyEvent
{
    private float initialYPosition;

    public override void OnBegin(bool firstTime)
    {
        if (!IsCloseToPlayer(enemy.golem.distanceToJumpAttack + 0.5f))
        {
            isDone = true;
            return;
        }

        enemy.Agent.isStopped = true;
        enemy.Agent.enabled = false;
        enemy.CharacterAnimator.SetTrigger(enemy.golem.jumpAttackTrigger);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        //enemy.HandleRotation(PlayerManager.Instance.transform.position);

        // If current attack animation is playing
        if (IsAnimationPlaying(enemy.golem.jumpAttackAnim))
        {
            if (enemy.CharacterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
            {
                isDone = true;
            }
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();

        // Re-enable NavMeshAgent and disable root motion after the roll
        enemy.Agent.enabled = true;
        enemy.Agent.isStopped = false;

        // Synchronize NavMeshAgent with current Transform position
        enemy.Agent.Warp(transform.position);

        enemy.CharacterAnimator.ResetTrigger(enemy.golem.jumpAttackTrigger);
        enemy.CharacterAnimator.ResetTrigger(enemy.attackTrigger);


        if (IsCloseToPlayer(enemy.distanceToAttack))
        {
            enemy.Attack();
        }
    }

    public override bool IsDone()
    {
        return isDone;
    }

    private void OnAnimatorMove()
    {
        // Apply root motion position, but lock the Y position
        Vector3 newPosition = transform.position + enemy.CharacterAnimator.deltaPosition;
        newPosition.y = initialYPosition; // Maintain the original Y position
        transform.position = newPosition;
    
        // Force rotation to stay aligned with rollDirection
        //enemy.transform.rotation = Quaternion.LookRotation(rollDirection);
    }
}
