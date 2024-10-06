
using UnityEngine;


[CreateAssetMenu(fileName = "SkillData", menuName = "ScriptableObjects/SkillData", order = 1)]
public class SkillData : ScriptableObject
{
    public string skillName;
    public Texture2D skillImage;
    public Texture2D skillHelperImage;
    public string skillHelperDescription;
    public int skillButtonIndex;
}