using UnityEngine;

namespace Game
{
    public class GolemRangedAttack : EnemyEvent
    {
        private int projectileCount = 10;
        private float projectileSpawnHeight = 15;
        private float range = 7;

        public override void OnBegin(bool firstTime)
        {
            base.OnBegin(firstTime);

            enemy.Agent.isStopped = true;
            enemy.Agent.enabled = false;
            enemy.CharacterAnimator.SetTrigger(enemy.golem.rangedAttackTrigger);

            for (int i = 0; i < projectileCount; i++)
            {
                Vector3 spawnPos;
                if (i == 0)
                {
                    spawnPos = new Vector3(PlayerManager.Instance.transform.position.x,
                                transform.position.y + projectileSpawnHeight,
                                PlayerManager.Instance.transform.position.z);
                }
                else
                {
                    spawnPos = new Vector3(Random.Range(transform.position.x - range, transform.position.x + range),
                                transform.position.y + projectileSpawnHeight,
                                Random.Range(transform.position.z - range, transform.position.z + range));
                }

                GameObject projectile = ObjectPooling.Instance.SpawnFromPool("BossProjectile", spawnPos, Quaternion.identity);
                GolemProjectile enemyProjectile = projectile.GetComponent<GolemProjectile>();
                enemyProjectile.SetEnemy(enemy);
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            // If current attack animation is playing
            if (IsAnimationPlaying(enemy.golem.rangedAttackAnim))
            {
                if (enemy.CharacterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                {
                    isDone = true;
                }
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();

            // Re-enable NavMeshAgent and disable root motion after the roll
            enemy.Agent.enabled = true;
            enemy.Agent.isStopped = false;
        }

        public override bool IsDone()
        {
            return isDone;
        }
    }
}
