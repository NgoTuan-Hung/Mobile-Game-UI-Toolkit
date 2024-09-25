using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
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

                skillScrollView.Add(newSkillHolder);
            }

            skillScrollView.verticalScroller.valueChanged += evt => SkillScrollViewEvent(skillScrollView);
            skillScrollView.RegisterCallback<PointerDownEvent>(SkillScrollViewPointerDown);
        });

        root.pickingMode = PickingMode.Position;
    }

    // Show tooltip on hover at mouse position
    public void ShowTooltip(MouseEnterEvent evt)
    {
        var target = evt.target as VisualElement;
        if (target == null) return;
    }

    public void SkillScrollViewEvent(ScrollView scrollView)
    {
        //Debug.Log(scrollView.verticalScroller.value);

        // immediately scroll to 
        scrollView.CaptureMouse();
    }

    public void SkillScrollViewPointerDown(PointerDownEvent evt)
    {
        Debug.Log("OnMouseDown");
        root.CapturePointer(evt.pointerId);
    }

    public void skillScrollViewPointerUp(PointerUpEvent evt)
    {
        Debug.Log("OnMouseUp");
        root.ReleasePointer(evt.pointerId);
    }
}
