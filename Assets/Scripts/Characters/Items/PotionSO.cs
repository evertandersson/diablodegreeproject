using Game;
using UnityEngine;

[CreateAssetMenu(fileName = "Potion", menuName = "Scriptable Objects/Potion")]
public class PotionSO : ActionItemSO
{
    public PotionType potionType;
    
    public int recoverAmount;

    public override void PerformAction(Animator animator)
    {
        Debug.Log("Drank potion");
        timerCooldown = 0;
        switch (potionType)
        {
            case PotionType.Health:
                PlayerManager.Instance.Heal(recoverAmount);
                break;
            case PotionType.Mana:
                PlayerManager.Instance.RefillMana(true, recoverAmount);
                break;

        }
    }

    public override string GetStatIncrease()
    {
        return recoverAmount.ToString();
    }
}

public enum PotionType
{
    Health,
    Mana
}
