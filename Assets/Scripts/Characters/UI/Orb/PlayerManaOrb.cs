using Game;
using UnityEngine;

public class PlayerManaOrb : Orb
{
    private void Update()
    {
        if (PlayerManager.Instance.Mana < PlayerManager.Instance.MaxMana)
        {
            PlayerManager.Instance.RefillMana(false);
        }
        SetValue(PlayerManager.Instance.Mana);
    }
}
