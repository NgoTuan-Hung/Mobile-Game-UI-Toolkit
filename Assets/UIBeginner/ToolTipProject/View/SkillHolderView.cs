
using UnityEngine.UIElements;

public class skillHolderView
{
    VisualElement holderIn;
    VisualElement holderOut;
    public skillHolderView(SkillData skillData, VisualElement root)
    {
        holderOut = root.Q<VisualElement>("skill-holder-out");
        holderIn = root.Q<VisualElement>("skill-holder-in");

        holderIn.style.backgroundImage = new StyleBackground(skillData.skillImage);
    }
}