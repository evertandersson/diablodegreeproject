using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class Enemy : Character
    {
        [SerializeField]
        public EventHandler enemyEventHandler;

        public string damageAnimName = "damage"; 

        private void Awake()
        {
            // Initialize event handler for enemy events
            enemyEventHandler = EventHandler.CreateEventHandler();
        }

        protected override void Start()
        {
            maxHealth = 20;
            base.Start();

            // Add the EnemyRoaming component dynamically
            EnemyIdle idleEvent = gameObject.AddComponent<EnemyIdle>();
            enemyEventHandler.PushEvent(idleEvent);

        }

        public void PerformAction()
        {
            // Push an event to perform an action
            EnemyEvent enemyEvent = new EnemyEvent();
            enemyEventHandler.PushEvent(enemyEvent);
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            EnemyTakeDamage takeDamageEvent = gameObject.AddComponent<EnemyTakeDamage>();
            enemyEventHandler.PushEvent(takeDamageEvent);

        }

        public override void Attack(int attackIndex)
        {
            throw new System.NotImplementedException();
        }

        protected override void Die()
        {
            throw new System.NotImplementedException();
        }


    }
}
