using System.Collections;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField]
    protected int health;

    [SerializeField]
    protected int maxHealth = 20;

    //Components for flash effect:
    [SerializeField] private SkinnedMeshRenderer[] renderers;
    private Color originalColor;
    private Color flashColor = Color.red;
    private float flashDuration = 0.1f;

    #region Properties

    public int Health => health;

    #endregion

    protected virtual void Start()
    {
        health = maxHealth;
        renderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);
        originalColor = renderers[0].material.color;
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        StartCoroutine(FlashRoutine());
        if (health <= 0)
        {
            Die();
        }
    }

    protected IEnumerator FlashRoutine()
    {
        foreach (Renderer r in renderers)
        {
            if (r.enabled)
                r.material.color = flashColor;
        }
        yield return new WaitForSeconds(flashDuration);
        foreach (Renderer r in renderers)
        {
            if (r.enabled)
                r.material.color = originalColor;
        }
    }

    protected abstract void Die();

    public abstract void Attack(int attackIndex);
}
