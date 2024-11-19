using UnityEngine;
using UnityEngine.AI;

namespace Game 
{ 
    public class EnemyEvent : EventHandler.GameEventBehaviour
    {
        protected NavMeshAgent agent;
        protected Animator animator;
        protected Enemy enemy;
        protected EventHandler enemyEventHandler;

        protected bool isDone = false;


        public override void OnBegin(bool firstTime)
        {
            enemy = GetComponent<Enemy>();  // Get reference to Enemy script
            agent = enemy.GetComponent<NavMeshAgent>();
            enemyEventHandler = enemy.enemyEventHandler;
            animator = enemy.GetComponent<Animator>();
            Debug.Log("Enemy Event Started: " + enemyEventHandler.CurrentEvent);
        }

        public override void OnUpdate()
        {
            float speed = agent.velocity.magnitude / agent.speed;
            animator.SetFloat("RunSpeed", speed);
        }

        public override void OnEnd()
        {
            Debug.Log("Enemy Event Ended");
        }

        public override bool IsDone()
        {
            return true;
        }
    }
}