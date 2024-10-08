
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillTooltipView
{
    VisualElement root;
    public SkillTooltipView(VisualElement root, string skillName, Texture2D skillHelperImage, string skillDescription, StyleSheet styleSheet)
    {
        this.root = root.Children().First();
        this.root.Q<Label>("skill-tooltip__text").text = skillName;
        this.root.Q<VisualElement>("skill-tooltip__helper-image").style.backgroundImage = new StyleBackground(skillHelperImage);
        this.root.Q<Label>("skill-tooltip__description").text = skillDescription;
        this.root.styleSheets.Add(styleSheet);
    }

    public VisualElement VisualElement() => root;
}