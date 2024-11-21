using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class Enemy : Character
    {
        public NavMeshAgent Agent { get; private set; }
        public Animator Animator { get; private set; }
        public EventHandler EnemyEventHandler { get; private set; }

        public string damageAnimName = "damage";

        public List<EnemyEvent> Events { get; private set; } 

        private void Awake()
        {
            // Cache components on Awake
            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
            EnemyEventHandler = EventHandler.CreateEventHandler();

            if (EnemyEventHandler == null)
            {
                Debug.LogError("EventHandler not found on Enemy.");
            }

            Events = new List<EnemyEvent>(GetComponents<EnemyEvent>());
        }

        protected override void Start()
        {
            maxHealth = 20;
            base.Start();

            EnemyEvent idleEvent = Events.FirstOrDefault(e => e is EnemyIdle);
            EnemyEventHandler.PushEvent(idleEvent);

        }

        public void PerformAction()
        {
            // Push an event to perform an action
            EnemyEvent enemyEvent = new EnemyEvent();
            EnemyEventHandler.PushEvent(enemyEvent);
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            EnemyEvent takeDamageEvent = Events.FirstOrDefault(e => e is EnemyTakeDamage);
            EnemyEventHandler.PushEvent(takeDamageEvent);

        }

        public void EnableRagdoll()
        {
            if (Animator != null)
                Animator.enabled = false;

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

        private void OnGUI()
        {

#if UNITY_EDITOR
            const float LINE_HEIGHT = 32.0f;
            GUI.color = new Color(0.0f, 0.0f, 0.0f, 0.7f);
            Rect r = new Rect(0, 0, 250.0f, LINE_HEIGHT * EnemyEventHandler.EventStack.Count);
            GUI.DrawTexture(r, Texture2D.whiteTexture);

            Rect line = new Rect(10, 0, r.width - 20, LINE_HEIGHT);
            for (int i = 0; i < EnemyEventHandler.EventStack.Count; i++)
            {
                GUI.color = EnemyEventHandler.EventStack[i] == EnemyEventHandler.CurrentEvent ? Color.green : Color.white;
                GUI.Label(line, "#" + i + ": " + EnemyEventHandler.EventStack[i].ToString(), i == 0 ? UnityEditor.EditorStyles.boldLabel : UnityEditor.EditorStyles.label);
                line.y += line.height;
            }
#endif
        }
    }
}
