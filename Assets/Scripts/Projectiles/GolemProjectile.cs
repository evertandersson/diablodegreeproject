using UnityEngine;

namespace Game
{
    public class GolemProjectile : EnemyProjectile
{
        protected override void FixedUpdate()
        {
            transform.position += -transform.up * moveSpeed * Time.fixedDeltaTime;

            timer += Time.fixedDeltaTime;
            if (timer >= lifeTime)
            {
                gameObject.SetActive(false);
            }
        }
    }

}
