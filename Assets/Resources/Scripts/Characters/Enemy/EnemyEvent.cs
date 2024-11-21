using UnityEngine;
using UnityEngine.AI;

namespace Game 
{ 
    public class EnemyEvent : EventHandler.GameEventBehaviour
    {
        protected Enemy enemy;

        protected bool isDone = false;

        private void Start()
        {
            enemy = GetComponent<Enemy>();
        }

        public override void OnBegin(bool firstTime)
        {
            isDone = false;
            Debug.Log("Enemy Event Started: " + enemy.EnemyEventHandler.CurrentEvent);
        }

        public override void OnUpdate()
        {
            float speed = enemy.Agent.velocity.magnitude / enemy.Agent.speed;
            enemy.Animator.SetFloat("RunSpeed", speed);
        }

        public override void OnEnd()
        {
            isDone = false;
            Debug.Log("Enemy Event Ended");
        }

        public override bool IsDone()
        {
            return true;
        }
    }
}