using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class ListSkillHolderController : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset skillHolderTemplate;
    [SerializeField] private VisualTreeAsset skillTooltipTemplate;
    // [SerializeField] private VisualTreeAsset helperLensTemplate;

    List<ScrollView> skillScrollViews;
    VisualElement root;
    VisualElement skillTooltipRoot, skillTooltip, helperLensRoot;

    public void Awake()
    {

    }
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
            skillScrollView.RegisterCallback<PointerDownEvent>((evt) => {SkillScrollViewPointerDown(skillScrollView);});
        });
    }

    // Show tooltip on hover at mouse position
    public void ShowTooltip(MouseEnterEvent evt)
    {
        var target = evt.target as VisualElement;
        if (target == null) return;
    }

    public void SkillScrollViewEvent(ScrollView scrollView)
    {
        // Debug.Log(scrollView.verticalScroller.value);

        // immediately scroll to 
    }

    public void SkillScrollViewPointerDown(ScrollView scrollView)
    {
        StartCoroutine(HandleScrollSnap(scrollView));
    }

    [SerializeField] private float scrollSnapCheckDelay = 0.03f;
    [SerializeField] private float snapTime = 0.3f;
    public IEnumerator HandleScrollSnap(ScrollView scrollView)
    {
        yield return new WaitForSeconds(scrollSnapCheckDelay);
        float prevPosition = float.MaxValue; 
        float finalPosition, currentPosition;
        while (true)
        {
            if (scrollView.verticalScroller.value == prevPosition) break;
            prevPosition = scrollView.verticalScroller.value;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        currentPosition = scrollView.verticalScroller.value;
        finalPosition = (float)Math.Floor(scrollView.verticalScroller.value/scrollView.resolvedStyle.height + 0.5f) * scrollView.resolvedStyle.height;

        float currentTime = 0;
        while (true)
        {
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            currentTime += Time.fixedDeltaTime;
            if (currentTime > snapTime) break;
            scrollView.verticalScroller.value = Mathf.Lerp(currentPosition, finalPosition, currentTime / snapTime);
        }
    }

    public IEnumerator CheckTouchIsReleasedThisFrame()
    {
        bool released = false;
        while (true)
        {
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            foreach (var touch in Touch.activeTouches)
            {
                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
                {
                    released = true;
                    break;
                }
            }

            if (released) break;
        }

        Debug.Log("Release");
    }
}
