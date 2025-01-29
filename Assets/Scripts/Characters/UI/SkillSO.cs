using Game;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Skill", menuName = "Scriptable Objects/Skill")]
public class SkillSO : ScriptableObject, IHasInfo
{
    public string skillName;
    public string skillDescription;
    public Texture2D skillIcon;
    public int skillCost;

    public Stat[] statsImprovements;

    public Stat[] Stats { get => statsImprovements; set => Stats = statsImprovements; }

    public void GetStatIncrease()
    {
        foreach (Stat stat in statsImprovements)
        {
            stat.UpdateStatText();
        }
    }
}
