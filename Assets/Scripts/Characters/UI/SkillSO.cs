using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Objects/Skill")]
public class SkillSO : ScriptableObject
{
    public string skillName;
    public string skillDescription;
    public Texture2D skillIcon;
    public int skillCost;

    public int healthIncrease;
    public int damageIncrease;
    public int defenceIncrease;
    public float movementIncrease;
    public int manaIncrease;
    public float attackSpeedIncrease;
}
