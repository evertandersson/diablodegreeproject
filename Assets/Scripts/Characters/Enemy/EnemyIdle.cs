using Game;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class EnemyIdle : EnemyEvent
    {
        private float standStillTimer = 0;

        public override void OnBegin(bool firstTime) 
        {
            base.OnBegin(firstTime);
            standStillTimer = 0; // Reset timer when idle begins
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (enemy.standStill)
                return;

            standStillTimer += Time.deltaTime;

            if (enemy.CanSeeTarget(PlayerManager.Instance.transform, offset)) // When sees player, transition to follow target
            {
                standStillTimer = 0;
                enemy.FollowTarget();
            }

            if (standStillTimer > 2.0f) // After 2 seconds, transition to roaming
            {
                standStillTimer = 0; 
                enemy.SetNewEvent<EnemyRoaming>();
            }
        }



        public override bool IsDone()
        {
            return false; // Idle never completes on its own
        }
    }
}
