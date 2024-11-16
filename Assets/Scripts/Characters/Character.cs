using UnityEngine;

public abstract class Character : MonoBehaviour
{
    protected int health;
    protected int maxHealth = 20;

    #region Properties

    public int Health => health;

    #endregion

    private void Start()
    {
        health = maxHealth;
    }

    protected virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    protected abstract void Die();

    public abstract void Attack(int attackIndex);
}
