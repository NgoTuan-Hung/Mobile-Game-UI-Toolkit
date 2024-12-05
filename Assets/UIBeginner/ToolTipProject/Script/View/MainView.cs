using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public enum ScrollViewLockState {Locked = 0, Unlocked = 1, AutoLocked = 2}

public class MainView : ViewBase
{
	[SerializeField] private VisualTreeAsset skillHolderTemplate;
	[SerializeField] private VisualTreeAsset skillTooltipTemplate;
	[SerializeField] private VisualTreeAsset healthBarTemplate;
	// [SerializeField] private VisualTreeAsset helperLensTemplate;
	StyleSheet skillTooltipSS;
	[SerializeField] private AudioClip scrollSound;
	[SerializeField] private AudioSource audioSource;
	bool optionExpandButtonExpanded = true, scrollLockExpandButtonExpanded = true, 
	lockExpandLocked = true;

	public void Init() 
	{
		FindAllVisualElements();
		snapInterval = snapTime * snapIntervalPortion;
		audioSource.clip = scrollSound;

		HelperLensDragAndDropManipulator dragAndDropManipulator = new HelperLensDragAndDropManipulator(helperLensRoot, skillTooltipTemplate);

		HandleSkillView();
		HandleJoyStickView();
		HandleScrollLockExpandLock();
		HandleExpandButton();
		PopulateOptions();
	}

	List<ScrollView> skillScrollViews;
	VisualElement root, helperLensRoot, optionExpandButton, options, mainViewLayer, skillScrollViewHolder
	, scrollLockParent, scrollLockExpandButton, scrollLockExpandLock, joyStickHolder, joyStickOuter, joyStickInner;
	public void FindAllVisualElements()
	{
		var uiDocument = GetComponent<UIDocument>();
		root = uiDocument.rootVisualElement;
		
		mainViewLayer = GameUIManager.Instance.Layers[(int)GameUIManager.LayerUse.MainView];
		
		/* Option Views */
		optionExpandButton = mainViewLayer.Q<VisualElement>(name: "main-view__option-expand-button");
		options = mainViewLayer.Q<VisualElement>(name: "main-view__options");
		
		/* ScrollView */
		skillScrollViewHolder = mainViewLayer.Q<VisualElement>(name: "main-view__skill-scroll-view-holder");
		skillScrollViews = skillScrollViewHolder.Query<ScrollView>(classes: "main-view__skill-scroll-view").ToList();
		scrollLockParent = skillScrollViewHolder.Q<VisualElement>(name: "scroll-lock-view__lock-parent");
		scrollLockExpandButton = skillScrollViewHolder.Q<VisualElement>(name: "scroll-lock-view__expand-button");
		scrollLockExpandLock = scrollLockParent.Q<VisualElement>(name: "scroll-lock-view__lock-expand");
		
		/* JoyStick */
		joyStickHolder = mainViewLayer.Q<VisualElement>(name: "JoyStickHolder");
		joyStickOuter = joyStickHolder.ElementAt(0);
		joyStickInner = joyStickOuter.ElementAt(0);
		
		/* Create a helper lens and assign drag and drop logic to it */
		helperLensRoot = root.Q<VisualElement>("helper-lens");
		helperLensRoot.style.position = Position.Absolute;
	}

	public void PopulateOptions()
	{
		List<MainViewOptionData> mainViewOptionDatas = Resources.LoadAll<MainViewOptionData>("UI/MainViewOptionData").ToList();
		
		mainViewOptionDatas.ForEach(mainViewOptionData => 
		{
			VisualElement visualElement = new();
			visualElement.AddToClassList("main-view__option-button");
			visualElement.style.backgroundImage = mainViewOptionData.icon;

			switch(mainViewOptionData.functionName)
			{
				case "": break;
				case "OpenSetting": 
				{
					visualElement.RegisterCallback<MouseDownEvent>(evt => 
					{
						gameUIManager.ActivateLayer((int)GameUIManager.LayerUse.Config);
					});
					break;
				}
				default: break;
			}
			
			options.Add(visualElement);
		});
	}

	public void HandleExpandButton()
	{
		optionExpandButton.RegisterCallback<PointerDownEvent>((evt) => 
		{
			if (optionExpandButtonExpanded)
			{
				optionExpandButtonExpanded = false;
				optionExpandButton.AddToClassList("main-view__expand-button-collapsed");
				options.AddToClassList("main-view__options-collapsed");
			}
			else
			{
				optionExpandButtonExpanded = true;
				optionExpandButton.RemoveFromClassList("main-view__expand-button-collapsed");
				options.RemoveFromClassList("main-view__options-collapsed");
			}
		});
		
		scrollLockExpandButton.RegisterCallback<PointerDownEvent>((evt) =>
		{
			if (lockExpandLocked) return;
			if (scrollLockExpandButtonExpanded)
			{
				scrollLockExpandButtonExpanded = false;
				scrollLockExpandButton.AddToClassList("main-view__expand-button-collapsed");
				scrollLockParent.AddToClassList("scroll-lock-view__lock-parent-collapsed");
			}
			else
			{
				scrollLockExpandButtonExpanded = true;
				scrollLockExpandButton.RemoveFromClassList("main-view__expand-button-collapsed");
				scrollLockParent.RemoveFromClassList("scroll-lock-view__lock-parent-collapsed");
			}
		});
	}
	
	private void HandleScrollLockExpandLock()
	{
		scrollLockExpandLock.RegisterCallback<MouseDownEvent>(evt => 
		{
			if (lockExpandLocked) scrollLockExpandLock.AddToClassList("scroll-lock-view__lock-expand-unlocked");
			else scrollLockExpandLock.RemoveFromClassList("scroll-lock-view__lock-expand-unlocked");
			lockExpandLocked = !lockExpandLocked;
			
		});
	}

	[SerializeField] private VisualTreeAsset scrollViewLockVTA;
	/// <summary>
	/// Handle scroll view lock (mostly skill)
	/// . Lock state: Lock -> Unlocked -> AutoLocked -> Lock -> ...
	/// </summary>
	public void HandleScrollLock(SkillScrollViewUIInfo skillScrollViewUIInfo)
	{	
		switch (skillScrollViewUIInfo.ScrollViewLockState)
		{
			case ScrollViewLockState.Locked:
			{
				skillScrollViewUIInfo.ScrollViewLock.AddToClassList("scroll-lock-view__lock-unlocked");
				skillScrollViewUIInfo.ScrollViewLockState = ScrollViewLockState.Unlocked;
				break;
			}
			case ScrollViewLockState.Unlocked:
			{
				skillScrollViewUIInfo.ScrollViewLock.RemoveFromClassList("scroll-lock-view__lock-unlocked");
				skillScrollViewUIInfo.ScrollViewLock.AddToClassList("scroll-lock-view__lock-auto-lock");
				skillScrollViewUIInfo.ScrollViewLockState = ScrollViewLockState.AutoLocked;
				break;
			}
			case ScrollViewLockState.AutoLocked:
			{
				skillScrollViewUIInfo.ScrollViewLock.RemoveFromClassList("scroll-lock-view__lock-auto-lock");
				skillScrollViewUIInfo.ScrollViewLockState = ScrollViewLockState.Locked;
				break;
			}
			default: break;
		}
	}
	
	/// <summary>
	/// Populate the skill slots info
	/// </summary>
	public void HandleSkillView()
	{	
		/* Load datas from scriptable object and create skill ui.
		Also handle tooltip of each skill*/
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
			GameUIManager.AddHelper(tooltipId, skillTooltip);

			newSkillHolder.AddToClassList(tooltipId);
			newSkillHolder.AddToClassList("has-helper");
			newSkillHolder.AddToClassList("helper-type-skill-info");
			newSkillHolder.AddToClassList("helper-invisible");
		});

		/* Handle scroll logic, scrolling, snapping */
		for (int i=0;i<skillScrollViews.Count;i++)
		{
			if (skillScrollViews[i].contentContainer.childCount != 0) skillScrollViews[i].contentContainer.ElementAt(0).RemoveFromClassList("helper-invisible");

			SkillScrollViewUIInfo skillScrollViewUIInfo = new SkillScrollViewUIInfo(skillScrollViews[i], i, null);
			/* For each scroll view, we assign a lock to it */
			skillScrollViewUIInfo.ScrollViewLock = scrollViewLockVTA.Instantiate().ElementAt(0);
			skillScrollViewUIInfo.ScrollViewLockState = ScrollViewLockState.Locked;
			skillScrollViewUIInfo.ScrollViewLock.RegisterCallback<PointerDownEvent>((evt) => 
			{
				evt.StopPropagation();
				HandleScrollLock(skillScrollViewUIInfo);
			});
			
			scrollLockParent.Insert(i, skillScrollViewUIInfo.ScrollViewLock.parent);

			skillScrollViews[i].verticalScroller.valueChanged += evt => SkillScrollViewEvent(skillScrollViewUIInfo);
			
			skillScrollViews[i].RegisterCallback<PointerDownEvent>((evt) => 
			{
				/* Check When locked is set to true, lock the scroll view. Also scroll happen at 
				scrollview.contentContainer.PointerDownEvent(TrickleDown) so we can block it here */
				if (skillScrollViewUIInfo.ScrollViewLockState == ScrollViewLockState.Locked) evt.StopPropagation();
				SkillScrollViewPointerDown(skillScrollViewUIInfo);
			}, TrickleDown.TrickleDown);
			
			skillScrollViews[i].RegisterCallback<PointerDownEvent>((evt) => 
			{
				/* Used to block touch screen event */
				evt.StopPropagation();
			});
			
			/* Used to determine some final style of scroll view (height,...)*/
			skillScrollViews[i].RegisterCallback<GeometryChangedEvent>
			(
				(evt) => SkillScrollViewGeometryChanged(skillScrollViewUIInfo)
			);
		}
	}

	/// <summary>
	/// Mostly used to play sound if scroll view scroll passed a element
	/// </summary>
	/// <param name="skillScrollViewUIInfo"></param>
	public void SkillScrollViewEvent(SkillScrollViewUIInfo skillScrollViewUIInfo)
	{
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
		if (skillScrollViewUIInfo.ScrollViewLockState == ScrollViewLockState.Locked) return;
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

	public IEnumerator HandleScrollSnap(SkillScrollViewUIInfo skillScrollViewUIInfo)
	{
		/* Find any first touch that overlaps the skill scroll view */
		Touch associatedTouch = TouchExtension.GetTouchOverlapVisualElement(skillScrollViewUIInfo.ScrollView, root.panel);

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
		- Snap to the element for more accurate snapping (Use unity internal function ScrollTo)
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
		
		/* If we choose auto lock scroll view, we can handle it here, right after scrolling and snapping
		is done */
		if (skillScrollViewUIInfo.ScrollViewLockState == ScrollViewLockState.AutoLocked) HandleScrollLock(skillScrollViewUIInfo);
	}

	/// <summary>
	/// Assign a health bar to a specific transform
	/// </summary>
	/// <param name="transform"></param>
	/// <param name="camera"></param>
	public void InstantiateAndHandleHealthBar(Transform transform, Camera camera)
	{
		var healthBar = healthBarTemplate.Instantiate();
		gameUIManager.GetLayer((int)GameUIManager.LayerUse.MainView).Add(healthBar);
		StartCoroutine(HandleHealthBarFloating(transform, healthBar, camera));
	}

	[SerializeField] private Vector3 healthBarOffset = new Vector3(-0.5f, 1.5f, 0);
	[SerializeField] private float healthBarPositionLerpTime = 0.5f;
	/// <summary>
	/// Handle floating health bar position every frame. Health bar will follow the transform with a little offset
	/// and the health bar movement will be smoothed every specified duration (healthBarPositionLerpTime).
	/// </summary>
	/// <param name="transform"></param>
	/// <param name="healthBar"></param>
	/// <param name="camera"></param>
	/// <returns></returns>
	public IEnumerator HandleHealthBarFloating(Transform transform, VisualElement healthBar, Camera camera)
	{
		Vector2 newVector2Position = RuntimePanelUtils.CameraTransformWorldToPanel(root.panel, transform.position + healthBarOffset, camera)
		, prevVector2Position, expectedVector2Position;
		float currentTime;
		
		while (true)
		{
			prevVector2Position = newVector2Position;
			/* Check current health bar position */
			newVector2Position = RuntimePanelUtils.CameraTransformWorldToPanel(root.panel, transform.position + healthBarOffset, camera);
			
			/* Start lerping position for specified duration if position change detected. Note that we only lerp on screen space position.*/
			if (prevVector2Position != newVector2Position)
			{
				currentTime = 0;
				while (currentTime < healthBarPositionLerpTime + Time.fixedDeltaTime)
				{
					expectedVector2Position = Vector2.Lerp(prevVector2Position, newVector2Position, currentTime / healthBarPositionLerpTime);
					healthBar.transform.position = new Vector2(expectedVector2Position.x, expectedVector2Position.y);
					
					yield return new WaitForSeconds(currentTime += Time.fixedDeltaTime);
				}
			}
			
			healthBar.transform.position = new Vector2(newVector2Position.x, newVector2Position.y);

			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}

	float innerRadius, outerRadius, outerRadiusSqr; Vector2 joyStickCenterPosition, touchPos, centerToTouch; Vector3 joyStickInnerDefaultPosition;
	public delegate void JoyStickMoveEvent(Vector2 value);
	/// <summary>
	/// You can add your custom event here whenever joystick is moved, function will be populated with a vector2
	/// </summary>
	public JoyStickMoveEvent joyStickMoveEvent;
	public void HandleJoyStickView()
	{
		joyStickOuter.RegisterCallback<GeometryChangedEvent>((evt) => 
		{
			PrepareValue();
		});

		joyStickInner.RegisterCallback<GeometryChangedEvent>((evt) => 
		{
			PrepareValue();
		});
		

		joyStickOuter.RegisterCallback<PointerDownEvent>((evt) => 
		{
			evt.StopPropagation();
			Touch touch = TouchExtension.GetTouchOverlapVisualElement(joyStickOuter, root.panel);
			touchPos = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(touch.screenPosition.x, Screen.height - touch.screenPosition.y));
			// Check if touch inside the circle

			centerToTouch = touchPos - joyStickCenterPosition;
			if (centerToTouch.sqrMagnitude < outerRadiusSqr) StartCoroutine(HandleJoyStick(touch));            
		});
	}

	public void PrepareValue()
	{
		outerRadius = joyStickOuter.resolvedStyle.width / 2f;
		outerRadiusSqr = outerRadius * outerRadius;
		joyStickCenterPosition = new Vector2(joyStickOuter.worldBound.position.x + outerRadius, joyStickOuter.worldBound.position.y + outerRadius);
		innerRadius = joyStickInner.resolvedStyle.width / 2f;
		joyStickInnerDefaultPosition = new Vector3(outerRadius - innerRadius, outerRadius - innerRadius, joyStickInner.transform.position.z);
		joyStickInner.transform.position = joyStickInnerDefaultPosition;
	}

	/// <summary>
	/// Handle joystick inner circle movement
	/// </summary>
	/// <param name="touch"></param>
	/// <returns></returns>
	public IEnumerator HandleJoyStick(Touch touch)
	{
		while (touch.phase != UnityEngine.InputSystem.TouchPhase.Ended)
		{
			/* Ensure touch is inside the circle */
			centerToTouch *= Math.Min(1f, outerRadius / centerToTouch.magnitude);
			/* Custom event will be executed here*/
			joyStickMoveEvent?.Invoke(centerToTouch);

			/* Make inner circle follow touch position within circle bound */
			joyStickInner.transform.position = joyStickOuter.WorldToLocal
			(
				joyStickCenterPosition + centerToTouch - new Vector2(innerRadius, innerRadius)
			);
 
			yield return new WaitForSeconds(Time.deltaTime);
			touchPos = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(touch.screenPosition.x, Screen.height - touch.screenPosition.y));
			centerToTouch = touchPos - joyStickCenterPosition;
		}

		joyStickInner.transform.position = joyStickInnerDefaultPosition;
	}
}


public class SkillScrollViewUIInfo
{
	private ScrollView scrollView;
	private VisualElement scrollViewLock;
	private ScrollViewLockState scrollViewLockState;
	private int scrollViewListIndex;
	private Coroutine scrollSnapCoroutine;
	private int skillScrollViewPreviousIndex = 0;
	private int skillScrollViewNewIndex = 0;
	private float scrollViewHeight = 0f;
	private float distanceToSnap = 0f;

	public SkillScrollViewUIInfo(ScrollView scrollView, int scrollViewListIndex, Coroutine scrollSnapCoroutine)
	{
		this.scrollView = scrollView;
		this.scrollViewListIndex = scrollViewListIndex;
		this.scrollSnapCoroutine = scrollSnapCoroutine;
	}

	public ScrollView ScrollView { get => scrollView; set => scrollView = value; }
	public Coroutine ScrollSnapCoroutine { get => scrollSnapCoroutine; set => scrollSnapCoroutine = value; }
	public int SkillScrollViewPreviousIndex { get => skillScrollViewPreviousIndex; set => skillScrollViewPreviousIndex = value; }
	public int SkillScrollViewNewIndex { get => skillScrollViewNewIndex; set => skillScrollViewNewIndex = value; }
	public float ScrollViewHeight { get => scrollViewHeight; set => scrollViewHeight = value; }
	public float DistanceToSnap { get => distanceToSnap; set => distanceToSnap = value; }
	public int ScrollViewListIndex { get => scrollViewListIndex; set => scrollViewListIndex = value; }
	public VisualElement ScrollViewLock { get => scrollViewLock; set => scrollViewLock = value; }
	public ScrollViewLockState ScrollViewLockState { get => scrollViewLockState; set => scrollViewLockState = value; }
}

