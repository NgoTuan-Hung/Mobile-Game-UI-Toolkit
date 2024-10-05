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

public class MainView : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset skillHolderTemplate;
    [SerializeField] private VisualTreeAsset skillTooltipTemplate;
    // [SerializeField] private VisualTreeAsset helperLensTemplate;

    List<ScrollView> skillScrollViews;
    VisualElement root, safeAreaVE;
    VisualElement skillTooltipRoot, skillTooltip, helperLensRoot;
    VisualElement testObject;
    [SerializeField] private AudioClip scrollSound;
    [SerializeField] private AudioSource audioSource;

    public void Awake()
    {
        snapInterval = snapTime * snapIntervalPortion;
        audioSource.clip = scrollSound;
    }
    private void OnEnable() 
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        safeAreaVE = root.Q<VisualElement>("safe-area") ;
        Rect safeArea = Screen.safeArea;
        Vector2 leftTop = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(safeArea.xMin, Screen.height - safeArea.yMax));
        Vector2 rightBottom = RuntimePanelUtils.ScreenToPanel
        (   
            root.panel,
            new Vector2(Screen.width - safeArea.xMax, safeArea.yMin
        ));

        safeAreaVE.style.paddingLeft = leftTop.x;
        safeAreaVE.style.paddingTop = leftTop.y;
        safeAreaVE.style.paddingRight = rightBottom.x;
        safeAreaVE.style.paddingBottom = rightBottom.y;

        skillScrollViews = root.Query<ScrollView>().ToList();

        helperLensRoot = root.Q<VisualElement>("helper-lens");
        helperLensRoot.style.position = Position.Absolute;
        HelperLensDragAndDropManipulator dragAndDropManipulator = new HelperLensDragAndDropManipulator(helperLensRoot, skillTooltipTemplate);

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

                skillScrollView.Add(newSkillHolder);
            }

            SkillScrollViewUIInfo skillScrollViewUIInfo = new SkillScrollViewUIInfo(skillScrollView, null);

            skillScrollView.verticalScroller.valueChanged += evt => SkillScrollViewEvent(skillScrollViewUIInfo);
            skillScrollView.RegisterCallback<PointerDownEvent>((evt) => {SkillScrollViewPointerDown(skillScrollViewUIInfo);});
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

    public void SkillScrollViewEvent(SkillScrollViewUIInfo skillScrollViewUIInfo)
    {
        // play sound if scroll view scroll passed a element
        skillScrollViewUIInfo.SkillScrollViewNewIndex = (int)(skillScrollViewUIInfo.ScrollView.verticalScroller.value / scrollViewHeight);
        if (skillScrollViewUIInfo.SkillScrollViewNewIndex != skillScrollViewUIInfo.SkillScrollViewPreviousIndex) audioSource.Play();

        skillScrollViewUIInfo.SkillScrollViewPreviousIndex = skillScrollViewUIInfo.SkillScrollViewNewIndex;
    }

    public void SkillScrollViewPointerDown(SkillScrollViewUIInfo skillScrollViewUIInfo)
    {
        if (skillScrollViewUIInfo.ScrollSnapCoroutine != null) StopCoroutine(skillScrollViewUIInfo.ScrollSnapCoroutine);
        skillScrollViewUIInfo.ScrollView.scrollDecelerationRate = defaultScrollDecelerationRate;
        skillScrollViewUIInfo.ScrollSnapCoroutine = StartCoroutine(HandleScrollSnap(skillScrollViewUIInfo.ScrollView));
    }

    [SerializeField] private float scrollSnapCheckDelay = 0.03f;
    [SerializeField] private float snapTime = 0.3f;
    [SerializeField] private float snapIntervalPortion = 0.1f;
    private float snapInterval;
    private float scrollViewHeight;
    [SerializeField] private float distanceToSnapScale = 0.5f;
    [SerializeField] private float distanceToSnap;
    private float defaultScrollDecelerationRate = 0.135f;

    [SerializeField] private int testIndex;
    public IEnumerator HandleScrollSnap(ScrollView scrollView)
    {
        Touch associatedTouch = new Touch();
        foreach (var touch in Touch.activeTouches)
        {
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                var touchPosition = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(touch.screenPosition.x, Screen.height - touch.screenPosition.y));

                if (scrollView.worldBound.Overlaps(new Rect(touchPosition, new Vector2(20, 20))))
                {
                    associatedTouch = touch;
                    break;
                }
            }
        }

        while (true)
        {
            yield return new WaitForSeconds(Time.deltaTime);

            if (associatedTouch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
            {
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
}


public class SkillScrollViewUIInfo
{
    private ScrollView scrollView;
    private Coroutine scrollSnapCoroutine;
    private int skillScrollViewPreviousIndex = 0;
    private int skillScrollViewNewIndex = 0;

    public SkillScrollViewUIInfo(ScrollView scrollView, Coroutine scrollSnapCoroutine)
    {
        this.scrollView = scrollView;
        this.scrollSnapCoroutine = scrollSnapCoroutine;
    }

    public ScrollView ScrollView { get => scrollView; set => scrollView = value; }
    public Coroutine ScrollSnapCoroutine { get => scrollSnapCoroutine; set => scrollSnapCoroutine = value; }
    public int SkillScrollViewPreviousIndex { get => skillScrollViewPreviousIndex; set => skillScrollViewPreviousIndex = value; }
    public int SkillScrollViewNewIndex { get => skillScrollViewNewIndex; set => skillScrollViewNewIndex = value; }
}

