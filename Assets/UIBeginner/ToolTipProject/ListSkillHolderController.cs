using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
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
        snapInterval = snapTime * snapIntervalPortion;
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
            skillScrollView.RegisterCallback<PointerDownEvent>((evt) => {SkillScrollViewPointerDown(skillScrollView, evt);});
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

    public void SkillScrollViewPointerDown(ScrollView scrollView, PointerDownEvent evt)
    {
        StartCoroutine(HandleScrollSnap(scrollView, evt));
    }

    [SerializeField] private float scrollSnapCheckDelay = 0.03f;
    [SerializeField] private float snapTime = 0.3f;
    [SerializeField] private float snapIntervalPortion = 0.1f;
    private float snapInterval;
    private float scrollViewHeight;
    [SerializeField] private float distanceToSnapScale = 0.5f;
    [SerializeField] private float distanceToSnap;
    private float defaultScrollDecelerationRate = 0.135f;
    public IEnumerator HandleScrollSnap(ScrollView scrollView, PointerDownEvent evt)
    {
        foreach (var touch in Touch.activeTouches)
        {
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                Rect temp = new Rect
                (
                    new Vector2(touch.screenPosition.x, Screen.height - touch.screenPosition.y),
                    new Vector2(0.01f, 0.01f)
                );
                print(scrollView.worldBound.center + "-----scroll view pos: " + scrollView.worldBound.Overlaps(temp));
                print(temp.center);
                while (true)
                {
                    yield return new WaitForSeconds(Time.deltaTime);

                    if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
                    {
                        break;
                    }
                }

                break;
            }
        }

        float prevPosition = float.MaxValue; 
        float finalPosition, currentPosition;
        int finalIndex;
        while (true)
        {
            if (Math.Abs(scrollView.verticalScroller.value - prevPosition) < distanceToSnap)
            {
                scrollView.scrollDecelerationRate = 0f;
                break;
            }
            prevPosition = scrollView.verticalScroller.value;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        currentPosition = scrollView.verticalScroller.value;
        finalIndex = (int)Math.Floor(scrollView.verticalScroller.value/scrollViewHeight + 0.5f);
        finalPosition = finalIndex * scrollViewHeight;

        float currentTime = 0, progress;
        while (true)
        {
            progress = currentTime / snapTime;
            if (progress > 1.01f) break;
            scrollView.verticalScroller.value = Mathf.Lerp(currentPosition, finalPosition, progress);
            yield return new WaitForSeconds(snapInterval);
            currentTime += snapInterval;
        }
        scrollView.scrollDecelerationRate = defaultScrollDecelerationRate;
        scrollView.ScrollTo(scrollView.contentContainer.Children().ElementAt(finalIndex));
    }

    public bool CheckTouchIsReleasedThisFrame()
    {
        

        return false;
    }
}
