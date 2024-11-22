using UnityEngine;

namespace Game
{
    public class EnemyAttack : EnemyEvent
    {
        public override void OnBegin(bool firstTime)
        {
            enemy.Animator.SetTrigger("Attack");
        }

        public override void OnUpdate()
        {
            
        }

        public override void OnEnd()
        {
            
        }

        public override bool IsDone()
        {
            return isDone;
        }
    }
}