
using UnityEngine;
using UnityEngine.UIElements;

public class SkillTooltipView
{
    VisualElement root;
    public SkillTooltipView(VisualElement root, string skillName, Texture2D skillHelperImage, string skillDescription)
    {
        this.root = root;
        root.Q<Label>("skill-tooltip__text").text = skillName;
        root.Q<VisualElement>("skill-tooltip__helper-image").style.backgroundImage = new StyleBackground(skillHelperImage);
        root.Q<Label>("skill-tooltip__description").text = skillDescription;
    }

    public VisualElement VisualElement() => root;
}