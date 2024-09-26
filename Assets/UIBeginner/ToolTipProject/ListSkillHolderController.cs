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
            skillScrollView.RegisterCallback<GeometryChangedEvent>
            (
                (evt) => 
                {
                    scrollViewHeight = skillScrollView.resolvedStyle.height;
                    distanceToSnap = scrollViewHeight * distanceToSnapScale;
                }
            );
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
    private float scrollViewHeight;
    [SerializeField] private float distanceToSnapScale = 0.5f;
    [SerializeField] private float distanceToSnap;
    public IEnumerator HandleScrollSnap(ScrollView scrollView)
    {
        while (true)
        {
            if (CheckTouchIsReleasedThisFrame()) break;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        float prevPosition = float.MaxValue; 
        float finalPosition, currentPosition;
        while (true)
        {
            if (Math.Abs(scrollView.verticalScroller.value - prevPosition) < distanceToSnap)
            {
                scrollView.scrollDecelerationRate = 0f;
                Debug.Log("Snap!");
                break;
            }
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

    public bool CheckTouchIsReleasedThisFrame()
    {
        foreach (var touch in Touch.activeTouches)
        {
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
            {
                return true;
            }
        }

        return false;
    }
}
