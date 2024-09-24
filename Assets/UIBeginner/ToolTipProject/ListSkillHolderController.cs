using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class ListSkillHolderController : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset skillHolderTemplate;
    [SerializeField] private VisualTreeAsset skillTooltipTemplate;
    // [SerializeField] private VisualTreeAsset helperLensTemplate;

    List<ScrollView> skillScrollViews;
    VisualElement root;
    VisualElement skillTooltipRoot, skillTooltip, helperLensRoot;
    private void OnEnable() 
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        skillScrollViews = root.Query<ScrollView>().ToList();

        helperLensRoot = root.Q<VisualElement>("helper-lens");
        helperLensRoot.style.position = Position.Absolute;
        DragAndDropManipulator dragAndDropManipulator = new DragAndDropManipulator(helperLensRoot);

        skillTooltipRoot = skillTooltipTemplate.Instantiate();
        skillTooltip = skillTooltipRoot.Q<VisualElement>("skill-tooltip");


        InitializeSkillHolderList();
    }

    [SerializeField] private int skillCount = 5;
    public void InitializeSkillHolderList()
    {
        skillScrollViews.ForEach(skillScrollView => 
        {
            for (int i = 0; i < skillCount; i++)
            {
                var newSkillHolder = skillHolderTemplate.Instantiate();
                VisualElement skillHolderIn = newSkillHolder.Q<VisualElement>("SkillHolderIn");
                // add event to skillHolderIn
                skillHolderIn.RegisterCallback<MouseDownEvent>(MouseDownEvent);
                skillHolderIn.RegisterCallback<MouseOutEvent>(MouseOutEvent);

                skillScrollView.Add(newSkillHolder);
            }
        });
    }

    // Show tooltip on hover at mouse position
    public void ShowTooltip(MouseEnterEvent evt)
    {
        var target = evt.target as VisualElement;
        if (target == null) return;
    }

    public void MouseDownEvent(MouseDownEvent evt)
    {
        Debug.Log("OnMouseDown");
    }

    public void MouseOutEvent(MouseOutEvent evt)
    {
        Debug.Log("OnMouseOut");
    }
}
