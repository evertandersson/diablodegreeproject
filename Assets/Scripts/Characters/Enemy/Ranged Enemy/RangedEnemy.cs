using UnityEngine;

namespace Game
{
    public class RangedEnemy : Enemy
    {
        protected override void Awake()
        {
            base.Awake();
            attackAnims = new int[] { Animator.StringToHash("Attack1") };
        }

        public override void Attack(int attackIndex = 0)
        {
            SetNewEvent<RangedEnemyAttack>();
        }
    }

}
