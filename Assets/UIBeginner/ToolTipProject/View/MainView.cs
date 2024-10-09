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
    VisualElement root, safeAreaVE, helperLensRoot;
    StyleSheet skillTooltipSS;
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

        /* Calculate the safe area so UIs don't touch unreachable parts of the screen */
        safeAreaVE = root.Q<VisualElement>("safe-area") ;
        Rect safeArea = Screen.safeArea;
        Vector2 leftTop = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(safeArea.xMin, Screen.height - safeArea.yMax));
        Vector2 rightBottom = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(Screen.width - safeArea.xMax, safeArea.yMin));

        safeAreaVE.style.paddingLeft = leftTop.x;
        safeAreaVE.style.paddingTop = leftTop.y;
        safeAreaVE.style.paddingRight = rightBottom.x;
        safeAreaVE.style.paddingBottom = rightBottom.y;
        /*  */

        skillScrollViews = root.Query<ScrollView>().ToList();

        /* Create a helper lens and assign drag and drop logic to it */
        helperLensRoot = root.Q<VisualElement>("helper-lens");
        helperLensRoot.style.position = Position.Absolute;
        HelperLensDragAndDropManipulator dragAndDropManipulator = new HelperLensDragAndDropManipulator(helperLensRoot, skillTooltipTemplate);

        InitializeSkillHolderList();
    }

    /* Populate the skill slots info */
    public void InitializeSkillHolderList()
    {
        List<SkillData> skillDatas = Resources.LoadAll<SkillData>("SkillData").ToList();
        skillTooltipSS = Resources.Load<StyleSheet>("SkillTooltipSS");

        skillDatas.ForEach(skillData => 
        {
            var newSkillHolder = skillHolderTemplate.Instantiate();
            skillScrollViews[skillData.skillButtonIndex].contentContainer.Add(newSkillHolder);
            new skillHolderView(skillData, newSkillHolder);
            var skillTooltip = new SkillTooltipView(skillTooltipTemplate.Instantiate(), skillData.skillName, skillData.skillHelperImage, skillData.skillHelperDescription, skillTooltipSS).VisualElement();
            newSkillHolder.GetLayer().Add(skillTooltip);
            skillTooltip.style.position = new StyleEnum<Position>(Position.Absolute);
            skillTooltip.style.left = new StyleLength(99999f);
            string tooltipId = "helper__skill-info__" + skillData.name;
            UIManager.AddHelper(tooltipId, skillTooltip);

            newSkillHolder.AddToClassList(tooltipId);
            newSkillHolder.AddToClassList("has-helper");
            newSkillHolder.AddToClassList("helper-type-skill-info");
            newSkillHolder.AddToClassList("helper-invisible");
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

    [SerializeField] private float snapTime = 0.3f;
    [SerializeField] private float snapIntervalPortion = 0.1f;
    private float snapInterval;
    [SerializeField] private float distanceToSnapScale = 0.5f;
    private float defaultScrollDecelerationRate = 0.135f;

    [SerializeField] private int testIndex;
    public IEnumerator HandleScrollSnap(SkillScrollViewUIInfo skillScrollViewUIInfo)
    {
        /* Find any first touch that overlaps the skill scroll view */
        Touch associatedTouch = new Touch();
        foreach (var touch in Touch.activeTouches)
        {
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                /* Convert from screen space to panel space */
                var touchPosition = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(touch.screenPosition.x, Screen.height - touch.screenPosition.y));

                if (skillScrollViewUIInfo.ScrollView.worldBound.Overlaps(new Rect(touchPosition, new Vector2(20, 20))))
                {
                    associatedTouch = touch;
                    break;
                }
            }
        }

        /* snap logic only happens when we release the touch */
        while (associatedTouch.phase != UnityEngine.InputSystem.TouchPhase.Ended) yield return new WaitForSeconds(Time.deltaTime);

        float prevPosition = float.MaxValue; 
        float finalPosition, currentPosition;
        int finalIndex;

        /* snap logic only happens when the scroll speed is low enough */
        while (Math.Abs(skillScrollViewUIInfo.ScrollView.verticalScroller.value - prevPosition) > skillScrollViewUIInfo.DistanceToSnap)
        {
            prevPosition = skillScrollViewUIInfo.ScrollView.verticalScroller.value;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        } skillScrollViewUIInfo.ScrollView.scrollDecelerationRate = 0f;

        /* snap logic:
        - Grab the element that the center of the scroll view is inside
        - Lerp from the current scroll position to the element's position
        - Snap to the element For more accurate snapping (since unity scroll view is very closed source and this snap behavior is not perfect)
         */
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

