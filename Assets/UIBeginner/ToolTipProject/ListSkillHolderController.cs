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
    VisualElement skillTooltipRoot, skillTooltip;
    private void OnEnable() 
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        skillScrollView = root.Q<ScrollView>();

        skillTooltipRoot = skillTooltipTemplate.Instantiate();
        skillTooltip = skillTooltipRoot.Q<VisualElement>("skill-tooltip");

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
            DragAndDropManipulator dragAndDropManipulator = new DragAndDropManipulator(skillHolderIn);

            skillScrollView.Add(newSkillHolder);
        }

        // skillTooltipRoot.style.left = Random.Range(0, 1000);
        // skillTooltipRoot.style.top = Random.Range(0, 1000);
        // skillTooltipRoot.style.position = Position.Absolute;
        // root.Add(skillTooltipRoot);
    }

    // Show tooltip on hover at mouse position
    public void ShowTooltip(MouseEnterEvent evt)
    {
        var target = evt.target as VisualElement;
        if (target == null) return;

        // skillTooltipRoot.style.left = Input.mousePosition.x;
        // skillTooltipRoot.style.top = Screen.height - Input.mousePosition.y;

        // skillTooltipRoot.style.left = evt.mousePosition.x;
        // skillTooltipRoot.style.top = evt.mousePosition.y;
    }
}
