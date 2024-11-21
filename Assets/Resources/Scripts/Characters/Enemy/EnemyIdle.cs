using Game;
using System.Linq;
using UnityEditor.PackageManager;
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

            standStillTimer += Time.deltaTime;

            if (standStillTimer > 2.0f) // After 2 seconds, transition to roaming
            {
                EnemyEvent roamingEvent = enemy.Events.FirstOrDefault(e => e is EnemyRoaming);
                if (roamingEvent != null)
                {
                    enemy.EnemyEventHandler.PushEvent(roamingEvent);
                    standStillTimer = 0; // Reset the timer after pushing roaming
                }
            }
        }

        public override bool IsDone()
        {
            return false; // Idle never completes on its own
        }
    }
}
