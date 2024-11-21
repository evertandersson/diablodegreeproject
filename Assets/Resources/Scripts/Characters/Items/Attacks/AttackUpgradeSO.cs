using UnityEngine;

[CreateAssetMenu(fileName = "AbilityUpgrade", menuName = "Abilities/Upgrade")]
public class AttackUpgradeSO : ScriptableObject
{
    public float additionalDamage = 0f;
    public float cooldownReduction = 0f;
    public int additionalProjectiles = 0;
    public bool enableTracking = false;
    public float additionalTrackingSpeed = 0f;
}