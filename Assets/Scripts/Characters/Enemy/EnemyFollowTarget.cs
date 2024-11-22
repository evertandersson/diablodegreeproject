using System.Threading;
using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class EnemyFollowTarget : EnemyEvent
    {
        float updateTargetDelay = 0.4f;
        float timer = 0;

        public override void OnBegin(bool firstTime)
        {
            base.OnBegin(firstTime);
            enemy.Agent.isStopped = false;
            SetNewDestination(enemy.Player.position);
            Debug.Log("Following player");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (IsAnyAttackAnimationPlaying())
                return;

            timer += Time.deltaTime;

            if (IsCloseToPlayer())
            {
                enemy.SetNewEvent<EnemyAttack>();
            }

            if (timer > updateTargetDelay)
            {
                // Update target pos
                SetNewDestination(enemy.Player.position);
                timer = 0;
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();
        }

        public override bool IsDone()
        {
            return isDone;
        }
    }
}