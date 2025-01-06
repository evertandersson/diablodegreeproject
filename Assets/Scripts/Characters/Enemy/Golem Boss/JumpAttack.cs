using Game;
using UnityEngine;

public class JumpAttack : EnemyEvent
{
    private float initialYPosition;
    private Vector3 impactOffset = new Vector3 (0, 0, 1.3f);

    public override void OnBegin(bool firstTime)
    {
        if (!IsCloseToPlayer(enemy.golem.distanceToJumpAttack + 0.5f))
        {
            isDone = true;
            return;
        }

        initialYPosition = transform.position.y;

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

    public void GroundSlam()
    {
        Vector3 slamPosition = transform.position + transform.forward * impactOffset.z;

        Quaternion slamRotation = Quaternion.Euler(-90f, transform.eulerAngles.y, 0f);

        // Spawn the effect at the calculated position with the adjusted rotation
        EnemyExplosion impact = ObjectPooling.Instance.SpawnFromPool("BossImpact", slamPosition, slamRotation).GetComponent<EnemyExplosion>();
        impact.SetEnemy(enemy);
    }

    public override bool IsDone()
    {
        return isDone;
    }

    private void OnAnimatorMove()
    {
        if (enemy.EnemyEventHandler.CurrentEvent is not JumpAttack)
            return;

        // Apply root motion position, but lock the Y position
        Vector3 newPosition = transform.position + enemy.CharacterAnimator.deltaPosition;
        newPosition.y = initialYPosition; // Maintain the original Y position
        transform.position = newPosition;

        // Force rotation to stay aligned with rollDirection
        //enemy.transform.rotation = Quaternion.LookRotation(rollDirection); 
    }
}
