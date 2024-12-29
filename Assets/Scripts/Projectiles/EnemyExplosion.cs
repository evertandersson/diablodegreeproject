using Game;
using System.Collections;
using UnityEngine;

public class EnemyExplosion : Explosion
{
    private Enemy parentEnemy;

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

        if (Vector3.Distance(transform.position, PlayerManager.Instance.transform.position) < 2)
        {
            PlayerManager.Instance.TakeDamage(parentEnemy.Damage);
            parentEnemy = null;
        }
    }
}
