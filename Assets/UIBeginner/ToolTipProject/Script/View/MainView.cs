using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MainView : ViewBase
{
	[SerializeField] private VisualTreeAsset skillHolderTemplate;
	[SerializeField] private VisualTreeAsset skillTooltipTemplate;
	[SerializeField] private VisualTreeAsset healthBarTemplate;
	// [SerializeField] private VisualTreeAsset helperLensTemplate;

	List<ScrollView> skillScrollViews;
	VisualElement root, helperLensRoot, optionExpandButton, options;
	StyleSheet skillTooltipSS;
	[SerializeField] private AudioClip scrollSound;
	[SerializeField] private AudioSource audioSource;
	bool optionExpandButtonExpanded = true;

	public void Init() 
	{
		snapInterval = snapTime * snapIntervalPortion;
		audioSource.clip = scrollSound;

		var uiDocument = GetComponent<UIDocument>();
		root = uiDocument.rootVisualElement;

		skillScrollViews = root.Query<ScrollView>(classes: "main-view__skill-scroll-view").ToList();

		/* Create a helper lens and assign drag and drop logic to it */
		helperLensRoot = root.Q<VisualElement>("helper-lens");
		helperLensRoot.style.position = Position.Absolute;
		HelperLensDragAndDropManipulator dragAndDropManipulator = new HelperLensDragAndDropManipulator(helperLensRoot, skillTooltipTemplate);

		HandleSkillView();
		HandleJoyStickView();
		HandleOptionFunctionality();
	}

	public void HandleOptionFunctionality()
	{
		HandleOptionExpandButton();
		PopulateOptions();
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

	public void HandleOptionExpandButton()
	{
		optionExpandButton = root.Q<VisualElement>(name: "main-view__option-expand-button");
		options = root.Q<VisualElement>(name: "main-view__options");
		optionExpandButton.RegisterCallback<PointerDownEvent>((evt) => 
		{
			if (optionExpandButtonExpanded)
			{
				optionExpandButtonExpanded = false;
				optionExpandButton.AddToClassList("main-view__option-expand-button-collapsed");
				options.AddToClassList("main-view__options-collapsed");
			}
			else
			{
				optionExpandButtonExpanded = true;
				optionExpandButton.RemoveFromClassList("main-view__option-expand-button-collapsed");
				options.RemoveFromClassList("main-view__options-collapsed");
			}
		});
	}

	/* Populate the skill slots info */
	public void HandleSkillView()
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
			GameUIManager.AddHelper(tooltipId, skillTooltip);

			newSkillHolder.AddToClassList(tooltipId);
			newSkillHolder.AddToClassList("has-helper");
			newSkillHolder.AddToClassList("helper-type-skill-info");
			newSkillHolder.AddToClassList("helper-invisible");
		});

		skillScrollViews.ForEach(skillScrollView => 
		{
			skillScrollView.contentContainer.ElementAt(0).RemoveFromClassList("helper-invisible");

			SkillScrollViewUIInfo skillScrollViewUIInfo = new SkillScrollViewUIInfo(skillScrollView, null);

			skillScrollView.verticalScroller.valueChanged += evt => SkillScrollViewEvent(skillScrollViewUIInfo);
			skillScrollView.RegisterCallback<PointerDownEvent>((evt) => 
			{
				evt.StopPropagation();
				SkillScrollViewPointerDown(skillScrollViewUIInfo);
			});
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

	public void InstantiateAndHandleHealthBar(Transform transform, Camera camera)
	{
		var healthBar = healthBarTemplate.Instantiate();
		gameUIManager.GetLayer((int)GameUIManager.LayerUse.MainView).Add(healthBar);
		StartCoroutine(HandleHealthBarFloating(transform, healthBar, camera));
	}

	[SerializeField] private Vector3 healthBarOffset = new Vector3(-0.5f, 1.5f, 0);
	[SerializeField] private float healthBarPositionLerpTime = 0.5f;
	public IEnumerator HandleHealthBarFloating(Transform transform, VisualElement healthBar, Camera camera)
	{
		Vector2 newVector2Position = RuntimePanelUtils.CameraTransformWorldToPanel(root.panel, transform.position + healthBarOffset, camera), prevVector2Position, expectedVector2Position;
		
		float currentTime = 0;
		while (true)
		{
			prevVector2Position = newVector2Position;
			newVector2Position = RuntimePanelUtils.CameraTransformWorldToPanel(root.panel, transform.position + healthBarOffset, camera);
			
			if (prevVector2Position != newVector2Position)
			{
				currentTime = 0;
				while (currentTime < healthBarPositionLerpTime + Time.fixedDeltaTime)
				{
					expectedVector2Position = Vector2.Lerp(prevVector2Position, newVector2Position, currentTime / healthBarPositionLerpTime);
					healthBar.transform.position = new Vector2(expectedVector2Position.x, expectedVector2Position.y);
					
					currentTime += Time.fixedDeltaTime;
					yield return new WaitForSeconds(Time.fixedDeltaTime);
				}
			}
			
			healthBar.transform.position = new Vector2(newVector2Position.x, newVector2Position.y);

			yield return new WaitForSeconds(Time.fixedDeltaTime);
		}
	}

	VisualElement joyStickHolder, joyStickOuter, joyStickInner;
	float innerRadius, outerRadius, outerRadiusSqr; Vector2 joyStickCenterPosition, touchPos, centerToTouch; Vector3 joyStickInnerDefaultPosition;
	public delegate void JoyStickMoveEvent(Vector2 value);
	public JoyStickMoveEvent joyStickMoveEvent;
	public void HandleJoyStickView()
	{
		joyStickHolder = root.Q<VisualElement>(name: "JoyStickHolder");
		joyStickOuter = joyStickHolder.ElementAt(0);
		joyStickInner = joyStickOuter.ElementAt(0);

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

	public IEnumerator HandleJoyStick(Touch touch)
	{
		while (touch.phase != UnityEngine.InputSystem.TouchPhase.Ended)
		{
			centerToTouch *= Math.Min(1f, outerRadius / centerToTouch.magnitude);
			joyStickMoveEvent?.Invoke(centerToTouch);

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

