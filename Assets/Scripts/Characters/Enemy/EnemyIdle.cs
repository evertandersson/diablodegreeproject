using Game;
using UnityEngine;

namespace Game
{
    public class EnemyIdle : EnemyEvent
    {
        private float standStillTimer = 0;

        public override void OnUpdate()
        {
            base.OnUpdate();

            standStillTimer += Time.deltaTime;

            if (standStillTimer > 2.0f)
            {
                EnemyRoaming roamingEvent = gameObject.AddComponent<EnemyRoaming>();
                enemyEventHandler.PushEvent(roamingEvent);
                standStillTimer = 0;
            }
        }

        public override bool IsDone()
        {
            return false;
        }
    }
}