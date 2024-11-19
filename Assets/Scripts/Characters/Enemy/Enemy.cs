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

        public void EnableRagdoll()
        {
            Animator animator = GetComponent<Animator>();
            if (animator != null)
                animator.enabled = false;

            Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rigidbodies)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            DetachWeapons();
        }

        public void DetachWeapons()
        {
            // Locate the sword and shield GameObjects (adjust the names based on your hierarchy)
            Transform sword = transform.Find("Group/Geometry/geo/sword_low");
            Transform shield = transform.Find("Group/Geometry/geo/shield_low");

            if (sword != null)
            {
                Destroy(sword.gameObject);
            }
            if (shield != null)
            {
                Destroy(shield.gameObject);
            }
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
