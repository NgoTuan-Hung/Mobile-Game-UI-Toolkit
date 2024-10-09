
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillTooltipView
{
    VisualElement root;
    public SkillTooltipView(VisualElement root, string skillName, Texture2D skillHelperImage, string skillDescription, StyleSheet styleSheet)
    {
        this.root = root.Children().First();
        this.root.Q<Label>("tooltip__text").text = skillName;
        this.root.Q<VisualElement>("tooltip__helper-image").style.backgroundImage = new StyleBackground(skillHelperImage);
        this.root.Q<Label>("tooltip__description").text = skillDescription;
        this.root.Q<Button>("tooltip__exit-button").RegisterCallback<ClickEvent>
        (
            evt => 
            {
                this.root.RemoveFromClassList("tooltip-showup");
                this.root.style.left = 99999f;
            }
        );
        this.root.styleSheets.Add(styleSheet);
    }

    public VisualElement VisualElement() => root;
}