using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField]
    protected int health;

    [SerializeField]
    protected int maxHealth = 20;

    #region Properties

    public int Health => health;

    #endregion

    protected virtual void Start()
    {
        health = maxHealth;
    }

    public virtual void TakeDamage(int damage)
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
