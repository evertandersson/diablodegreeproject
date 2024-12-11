using Game;
using UnityEngine;

[CreateAssetMenu(fileName = "Potion", menuName = "Scriptable Objects/Potion")]
public class PotionSO : ActionItemSO
{
    public int healAmount;

    public override void PerformAction(Animator animator)
    {
        Debug.Log("Drank potion");
        timerCooldown = 0;
        PlayerManager.Instance.Heal(healAmount);
    }
}
