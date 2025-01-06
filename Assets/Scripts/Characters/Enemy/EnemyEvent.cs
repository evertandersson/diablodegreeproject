using TMPro;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.AI;

namespace Game 
{ 
    public class EnemyEvent : EventHandler.GameEventBehaviour
    {
        protected Enemy enemy;

        protected bool isDone = false;

        protected Vector3 targetPosition;
        protected float elapsedTime = 0f;

        protected Vector3 offset = new Vector3(0, 1.2f, 0);

        float animationCheckDelay = 0.2f; // Delay for checking animations
        float animationTimer = 0;
        protected bool isAttackAnimationPlaying = false; // Cached result for animation check

        private void Start()
        {
            enemy = GetComponent<Enemy>();
        }

        public override void OnBegin(bool firstTime)
        {
            animationTimer = 0f; // Reset animation timer
            isDone = false;
        }

        public override void OnUpdate()
        {
            SetFloatRunSpeed();

            if (enemy.isAggro && enemy.EnemyEventHandler.CurrentEvent is not EnemyTakeDamage)
            {
                enemy.SetNewEvent<EnemyFollowTarget>();
                enemy.isAggro = false;
            }
        }

        public override void OnEnd()
        {
            isDone = false;
        }

        public override bool IsDone()
        {
            return true;
        }

        public void SetFloatRunSpeed()
        {
            float speed = enemy.Agent.velocity.magnitude / enemy.Agent.speed;
            float currentSpeed = enemy.CharacterAnimator.GetFloat("RunSpeed");
            float targetSpeed = Mathf.Clamp01(speed);
            float smoothSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 5f);
            enemy.CharacterAnimator.SetFloat("RunSpeed", smoothSpeed);


            //float speed = enemy.Agent.velocity.magnitude / enemy.Agent.speed;
            //enemy.CharacterAnimator.SetFloat("RunSpeed", speed);
        }

        protected void SetNewDestination(Vector3 pos)
        {
            for (int i = 0; i < 3; i++) // Try up to 3 random positions
            {
                if (IsValidDestination(pos))
                {
                    targetPosition = pos;
                    enemy.Agent.SetDestination(targetPosition);
                    elapsedTime = 0f; // Reset timeout
                    return;
                }
            }

            isDone = true; // Mark as done if no valid destination is found
        }

        protected bool IsValidDestination(Vector3 position)
        {
            NavMeshPath path = new NavMeshPath();
            enemy.Agent.CalculatePath(position, path);
            return path.status == NavMeshPathStatus.PathComplete;
        }

        protected void CheckAnimationInterval()
        {
            animationTimer += Time.deltaTime;
            if (animationTimer >= animationCheckDelay)
            {
                isAttackAnimationPlaying = IsAnyAttackAnimationPlaying();
                animationTimer = 0;
            }
        }

        public bool IsAnimationPlaying(int animationHash)
        {
            AnimatorStateInfo stateInfo = enemy.CharacterAnimator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.shortNameHash == animationHash && stateInfo.normalizedTime < 1.0f;
        }

        protected bool IsAnyAttackAnimationPlaying()
        {
            foreach (int attackAnim in enemy.attackAnims)
            {
                if (IsAnimationPlaying(attackAnim))
                {
                    return true;
                }
            }
            return false; // No attack animations are playing
        }

        protected bool IsCloseToPlayer(float distance)
        {
            return Vector3.Distance(enemy.transform.position, enemy.Player.position) < distance;
        }

        protected bool IsTargetedAtPlayer()
        {
            RaycastHit hit;

            if (Physics.Raycast(enemy.transform.position + offset, transform.forward, out hit, enemy.detectionMask))
            {
                // Check if the raycast hit the player
                return hit.transform == enemy.Player;
            }

            return false; // Raycast did not hit the player
        }
    }
}