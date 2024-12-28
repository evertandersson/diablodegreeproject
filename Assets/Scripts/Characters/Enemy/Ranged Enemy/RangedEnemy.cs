using UnityEngine;

namespace Game
{
    public class RangedEnemy : Enemy
    {
        public override void Attack(int attackIndex = 0)
        {
            SetNewEvent<RangedEnemyAttack>();
        }
    }

}
