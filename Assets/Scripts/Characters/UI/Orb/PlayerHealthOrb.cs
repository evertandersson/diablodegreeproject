using Game;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthOrb : Orb
{
    private void Update()
    {
        if (PlayerManager.Instance.Health < PlayerManager.Instance.MaxHealth)
        {
            PlayerManager.Instance.RefillHealth(false);
        }
        SetValue(PlayerManager.Instance.Health);
    }
}
