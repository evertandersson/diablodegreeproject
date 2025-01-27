using Game;
using System.Collections;
using UnityEngine;

public class EnemyExplosion : Explosion
{
    private Enemy parentEnemy;
    private Vector3 offset = new Vector3(0, 1.2f, 0);

    public void SetEnemy(Enemy enemy)
    {
        parentEnemy = enemy;
    }

    public override void OnObjectSpawn()
    {
        SoundManager.PlaySound(SoundType.EXPLOSION);

        StartCoroutine(PlayerTakeDamage());
    }

    private IEnumerator PlayerTakeDamage()
    {
        yield return new WaitForEndOfFrame();

        if (Vector3.Distance(transform.position, PlayerManager.Instance.transform.position + offset) < damageRange)
        {
            StatsCalculator.CalculateDamage(parentEnemy.Damage, PlayerManager.Instance);
            parentEnemy = null;
        }
    }
}
