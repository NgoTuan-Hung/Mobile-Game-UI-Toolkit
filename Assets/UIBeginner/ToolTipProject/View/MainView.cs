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
        List<SkillData> skillDatas = Resources.LoadAll<SkillData>("SkillData").ToList();

        skillDatas.ForEach(skillData => 
        {
            var newSkillHolder = skillHolderTemplate.Instantiate();
            new skillHolderView(skillData, newSkillHolder);
            var skillTooltip = new SkillTooltipView(skillTooltipTemplate.Instantiate(), skillData.skillName, skillData.skillHelperImage, skillData.skillHelperDescription).VisualElement();
            string tooltipId = "helper__skill-info__" + skillData.name;
            UIManager.AddHelper(tooltipId, skillTooltip);

            newSkillHolder.AddToClassList(tooltipId);
            newSkillHolder.AddToClassList("has-helper");
            newSkillHolder.AddToClassList("helper-type-skill-info");
            newSkillHolder.AddToClassList("helper-invisible");

            skillScrollViews[skillData.skillButtonIndex].contentContainer.Add(newSkillHolder);
        });

        skillScrollViews.ForEach(skillScrollView => 
        {
            skillScrollView.contentContainer.Children().First().RemoveFromClassList("helper-invisible");

            SkillScrollViewUIInfo skillScrollViewUIInfo = new SkillScrollViewUIInfo(skillScrollView, null);

            skillScrollView.verticalScroller.valueChanged += evt => SkillScrollViewEvent(skillScrollViewUIInfo);
            skillScrollView.RegisterCallback<PointerDownEvent>((evt) => {SkillScrollViewPointerDown(skillScrollViewUIInfo);});
            skillScrollView.RegisterCallback<GeometryChangedEvent>
            (
                (evt) => SkillScrollViewGeometryChanged(skillScrollViewUIInfo)
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
        skillScrollViewUIInfo.SkillScrollViewNewIndex = (int)Math.Floor(skillScrollViewUIInfo.ScrollView.verticalScroller.value / skillScrollViewUIInfo.ScrollViewHeight + 0.5f);
        if (skillScrollViewUIInfo.SkillScrollViewNewIndex != skillScrollViewUIInfo.SkillScrollViewPreviousIndex)
        {
            audioSource.Play();
            skillScrollViewUIInfo.ScrollView.contentContainer.ElementAt(skillScrollViewUIInfo.SkillScrollViewNewIndex).RemoveFromClassList("helper-invisible");
            skillScrollViewUIInfo.ScrollView.contentContainer.ElementAt(skillScrollViewUIInfo.SkillScrollViewPreviousIndex).AddToClassList("helper-invisible");
        }

        skillScrollViewUIInfo.SkillScrollViewPreviousIndex = skillScrollViewUIInfo.SkillScrollViewNewIndex;
    }

    public void SkillScrollViewPointerDown(SkillScrollViewUIInfo skillScrollViewUIInfo)
    {
        if (skillScrollViewUIInfo.ScrollSnapCoroutine != null) StopCoroutine(skillScrollViewUIInfo.ScrollSnapCoroutine);
        skillScrollViewUIInfo.ScrollView.scrollDecelerationRate = defaultScrollDecelerationRate;
        skillScrollViewUIInfo.ScrollSnapCoroutine = StartCoroutine(HandleScrollSnap(skillScrollViewUIInfo));
    }
    
    public void SkillScrollViewGeometryChanged(SkillScrollViewUIInfo skillScrollViewUIInfo)
    {
        skillScrollViewUIInfo.ScrollViewHeight = skillScrollViewUIInfo.ScrollView.resolvedStyle.height;
        skillScrollViewUIInfo.DistanceToSnap = skillScrollViewUIInfo.ScrollViewHeight * distanceToSnapScale;
    }

    [SerializeField] private float scrollSnapCheckDelay = 0.03f;
    [SerializeField] private float snapTime = 0.3f;
    [SerializeField] private float snapIntervalPortion = 0.1f;
    private float snapInterval;
    [SerializeField] private float distanceToSnapScale = 0.5f;
    private float defaultScrollDecelerationRate = 0.135f;

    [SerializeField] private int testIndex;
    public IEnumerator HandleScrollSnap(SkillScrollViewUIInfo skillScrollViewUIInfo)
    {
        Touch associatedTouch = new Touch();
        foreach (var touch in Touch.activeTouches)
        {
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                var touchPosition = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(touch.screenPosition.x, Screen.height - touch.screenPosition.y));

                if (skillScrollViewUIInfo.ScrollView.worldBound.Overlaps(new Rect(touchPosition, new Vector2(20, 20))))
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
            if (Math.Abs(skillScrollViewUIInfo.ScrollView.verticalScroller.value - prevPosition) < skillScrollViewUIInfo.DistanceToSnap)
            {
                skillScrollViewUIInfo.ScrollView.scrollDecelerationRate = 0f;
                break;
            }
            prevPosition = skillScrollViewUIInfo.ScrollView.verticalScroller.value;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        currentPosition = skillScrollViewUIInfo.ScrollView.verticalScroller.value;
        finalIndex = (int)Math.Floor(skillScrollViewUIInfo.ScrollView.verticalScroller.value/skillScrollViewUIInfo.ScrollViewHeight + 0.5f);
        finalPosition = finalIndex * skillScrollViewUIInfo.ScrollViewHeight;

        float currentTime = 0, progress;
        while (true)
        {
            progress = currentTime / snapTime;
            if (progress > 1.01f) break;
            skillScrollViewUIInfo.ScrollView.verticalScroller.value = Mathf.Lerp(currentPosition, finalPosition, progress);
            yield return new WaitForSeconds(snapInterval);
            currentTime += snapInterval;
        }
        skillScrollViewUIInfo.ScrollView.scrollDecelerationRate = defaultScrollDecelerationRate;
        skillScrollViewUIInfo.ScrollView.ScrollTo(skillScrollViewUIInfo.ScrollView.contentContainer.Children().ElementAt(finalIndex));
    }
}


public class SkillScrollViewUIInfo
{
    private ScrollView scrollView;
    private Coroutine scrollSnapCoroutine;
    private int skillScrollViewPreviousIndex = 0;
    private int skillScrollViewNewIndex = 0;
    private float scrollViewHeight = 0f;
    private float distanceToSnap = 0f;

    public SkillScrollViewUIInfo(ScrollView scrollView, Coroutine scrollSnapCoroutine)
    {
        this.scrollView = scrollView;
        this.scrollSnapCoroutine = scrollSnapCoroutine;
    }

    public ScrollView ScrollView { get => scrollView; set => scrollView = value; }
    public Coroutine ScrollSnapCoroutine { get => scrollSnapCoroutine; set => scrollSnapCoroutine = value; }
    public int SkillScrollViewPreviousIndex { get => skillScrollViewPreviousIndex; set => skillScrollViewPreviousIndex = value; }
    public int SkillScrollViewNewIndex { get => skillScrollViewNewIndex; set => skillScrollViewNewIndex = value; }
    public float ScrollViewHeight { get => scrollViewHeight; set => scrollViewHeight = value; }
    public float DistanceToSnap { get => distanceToSnap; set => distanceToSnap = value; }
}

