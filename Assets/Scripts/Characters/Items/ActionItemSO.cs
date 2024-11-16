using UnityEngine;

[CreateAssetMenu(fileName = "ActionItem", menuName = "Scriptable Objects/ActionItem")]
public abstract class ActionItemSO : ItemSO
{
    public float cooldown;
    public float timerCooldown;

    public abstract void PerformAction(Animator animator);
}
