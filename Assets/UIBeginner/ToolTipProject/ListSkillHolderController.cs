using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class ListSkillHolderController : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset skillHolderTemplate;
    [SerializeField] private VisualTreeAsset skillTooltipTemplate;

    ScrollView skillScrollView;
    VisualElement root;
    VisualElement skillTooltip;
    private void OnEnable() 
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        skillScrollView = root.Q<ScrollView>();

        skillTooltip = skillTooltipTemplate.Instantiate();

        InitializeSkillHolderList();
    }

    [SerializeField] private int skillCount = 5;
    public void InitializeSkillHolderList()
    {
        for (int i = 0; i < skillCount; i++)
        {
            var newSkillHolder = skillHolderTemplate.Instantiate();
            VisualElement skillHolderIn = newSkillHolder.Q<VisualElement>("SkillHolderIn");
            // add event to skillHolderIn
            skillHolderIn.RegisterCallback<PointerEnterEvent>(ShowTooltip);
            skillHolderIn.RegisterCallback<PointerLeaveEvent>(HideTooltip);

            skillScrollView.Add(newSkillHolder);
        }
    }

    // Show tooltip on hover at mouse position
    public void ShowTooltip(PointerEnterEvent evt)
    {
        var target = evt.target as VisualElement;
        if (target == null) return;

        // skillTooltip.style.left = Input.mousePosition.x;
        // skillTooltip.style.top = Screen.height - Input.mousePosition.y;
        skillTooltip.style.left = evt.position.x;
        skillTooltip.style.top = evt.position.y;
        skillTooltip.style.position = Position.Absolute;
        root.Add(skillTooltip);
    }

    // Hide tooltip on mouse exit
    public void HideTooltip(PointerLeaveEvent evt)
    {
        root.Remove(skillTooltip);
    }
}
