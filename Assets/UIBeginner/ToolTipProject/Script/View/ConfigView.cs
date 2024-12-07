using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class ConfigView : ViewBase
{
	UIDocument uIDocument;
	ConfigData configData;
	VisualTreeAsset configMenuContentAsset, configCheckboxAsset, configDropdownAsset, configSliderAsset,
	configButtonAsset;

	public void Init()
	{
		GetAllRequiredVisualElements();
		configExitButton.RegisterCallback<PointerDownEvent>((evt) => 
		{
			gameUIManager.DeactivateLayer((int)GameUIManager.LayerUse.Config);
		});
		
		configData = Resources.Load<ConfigData>("UI/ConfigData/Config");
		GetVisualTreeAsset();
		PopulateView();
		HandleDynamicUI();
	}
	
	VisualElement root, configMenu, configExitButton, currentSelectedMenuItem, currentDisplayedConfigContent,
	configContent, configCheckbox, configDropdown, configSlider, configButton, globalRoot, dynamicUIButton, dynamicLayer;
	void GetAllRequiredVisualElements()
	{
		uIDocument = GetComponent<UIDocument>();
		root = uIDocument.rootVisualElement.Q<VisualElement>(name: "config__menu-root");
		configMenu = root.Q<VisualElement>(name: "config__menu");
		configExitButton = root.Q<VisualElement>(name : "config__exit-button");
		globalRoot = root.panel.visualTree;
		dynamicUIButton = globalRoot.Q<VisualElement>(name: "dynamic-ui-button");
		moveableElements = globalRoot.Query<VisualElement>(classes: "dynamic-ui__movable").ToList();
	}

	public void GetVisualTreeAsset()
	{
		configMenuContentAsset = Resources.Load<VisualTreeAsset>("UI/Config/ConfigMenuContent");
		configCheckboxAsset = Resources.Load<VisualTreeAsset>("UI/Config/ConfigCheckbox");
		configDropdownAsset = Resources.Load<VisualTreeAsset>("UI/Config/ConfigDropdown");
		configSliderAsset = Resources.Load<VisualTreeAsset>("UI/Config/ConfigSlider");
		configButtonAsset = Resources.Load<VisualTreeAsset>("UI/Config/ConfigButton");
	}


	Dictionary<int, VisualElement> configMenuContentsDict = new();
	DropdownField configDropdownField;
	/// <summary>
	/// Populate the config menu and config content tabs with predefined data.
	/// It might look like this:<br/>
	/// <b>config data</b> <br/>
	/// ├── config menu item 1 ~ config menu content 1 <br/>
	/// │   ├── config content item 1 <br/>
	/// │   ├── config content item 2 <br/>
	/// │   └── config content item 3 <br/>
	/// └── config menu item 2 ~ config menu content 2 <br/>
	/// ...
	/// </summary>
	public void PopulateView()
	{
		configData.configMenuItems.ForEach(configMenuItem => 
		{
			Label menuItem = new();
			menuItem.AddToClassList("config__menu-item");
			switch (configMenuItem.configType)
			{
				case ConfigMenuItem.ConfigType.Video: menuItem.text = "Video"; break;
				case ConfigMenuItem.ConfigType.Sound: menuItem.text = "Audio"; break;
				case ConfigMenuItem.ConfigType.Other: menuItem.text = "Other"; break;
				default: break;
			}

			/* Switch tab event */
			menuItem.RegisterCallback<ClickEvent>(evt => MenuItemClickEvent(menuItem));
			configMenu.Add(menuItem);

			/* Each menu item should have its own content */
			configContent = configMenuContentAsset.Instantiate().ElementAt(0);
			configMenuContentsDict.Add(menuItem.GetHashCode(), configContent);
			
			configMenuItem.configContentItems.ForEach(configContentItem => 
			{
				switch (configContentItem.itemType)
				{
					case ConfigContentItem.ItemType.ConfigSlider:
					{
						configSlider = configSliderAsset.Instantiate().ElementAt(0); 
						((SliderInt)configSlider).label = configContentItem.sliderName;
						configContent.ElementAt(0).Add(configSlider);
						break;
					}
					case ConfigContentItem.ItemType.ConfigDropdown:
					{
						configDropdown = configDropdownAsset.Instantiate().ElementAt(0);
						configDropdown.Q<Label>().text = configContentItem.dropdownName;
						configDropdownField = configDropdown.Q<DropdownField>();
						configContentItem.dropdownOptions.ForEach(dropdownOption => configDropdownField.choices.Add(dropdownOption));
						configContent.ElementAt(0).Add(configDropdown);
						break;
					}
					case ConfigContentItem.ItemType.ConfigCheckbox:
					{
						configCheckbox = configCheckboxAsset.Instantiate().ElementAt(0);
						configCheckbox.Q<Label>().text = configContentItem.checkboxName;
						configContent.ElementAt(0).Add(configCheckbox);
						break;
					}
					case ConfigContentItem.ItemType.ConfigButton:
					{
						configButton = configButtonAsset.Instantiate().ElementAt(0);
						configButton.Q<Label>(classes: "menu-content-button__label").text = configContentItem.buttonLabelName;
						configButton.Q<Label>(classes: "menu-content-button__button").text = configContentItem.buttonName;
						
						/* Custom call back for each config button. Custom method is placed in this class */
						Type thisType = GetType();
						MethodInfo theMethod = thisType.GetMethod(configContentItem.buttonCallback);
						
						configButton.RegisterCallback<PointerDownEvent>((evt) => 
						{
							theMethod.Invoke(this, null);
						});
						
						configContent.ElementAt(0).Add(configButton);
						break;
					}
					default: break;
				}
			});

			configContent.style.position = Position.Absolute;
			configContent.style.display = DisplayStyle.None;
			
			root.Add(configContent);
		});

		currentSelectedMenuItem = configMenu.contentContainer.ElementAt(0);
		currentSelectedMenuItem.AddToClassList("config__menu-item-selected");

		currentDisplayedConfigContent = configMenuContentsDict[currentSelectedMenuItem.GetHashCode()];
		currentDisplayedConfigContent.style.position = Position.Relative;
		currentDisplayedConfigContent.style.display = DisplayStyle.Flex;
	}

	public void MenuItemClickEvent(VisualElement menuItem)
	{
		currentSelectedMenuItem.RemoveFromClassList("config__menu-item-selected");
		currentSelectedMenuItem = menuItem;
		currentSelectedMenuItem.AddToClassList("config__menu-item-selected");

		currentDisplayedConfigContent.style.position = Position.Absolute;
		currentDisplayedConfigContent.style.display = DisplayStyle.None;

		currentDisplayedConfigContent = configMenuContentsDict[menuItem.GetHashCode()];
		currentDisplayedConfigContent.style.position = Position.Relative;
		currentDisplayedConfigContent.style.display = DisplayStyle.Flex;
	}
	
	List<GameUIManager.LayerUse> dynamicLayerUses = new List<GameUIManager.LayerUse>() {GameUIManager.LayerUse.MainView, GameUIManager.LayerUse.DynamicUI};
	public void ChangeDynamicVisualElement()
	{
		GameUIManager.ActivateOnlyLayers(dynamicLayerUses);
	}
	
	/* Dynamic UI parts */
	Dictionary<int, VisualElement> dynamicUIDictionary = new Dictionary<int, VisualElement>();
	List<VisualElement> moveableElements;
	void HandleDynamicUI()
	{
		dynamicLayer = GameUIManager.Instance.Layers[(int)GameUIManager.LayerUse.DynamicUI];
		
		dynamicUIButton.RegisterCallback<PointerDownEvent>((evt) => 
		{
			evt.StopPropagation();
			GameUIManager.Instance.ActivateOnlyLayer(GameUIManager.LayerUse.MainView);
		});
		
		for (int i=0;i<moveableElements.Count;i++)
		{
			VisualElement visualElement = new VisualElement();
			visualElement.style.position = Position.Absolute;
			visualElement.style.backgroundColor = new Color(1, 1, 0, 0.5f);
			dynamicLayer.Add(visualElement);
			visualElement.PlaceBehind(dynamicUIButton);
			
			moveableElements[i].RegisterCallback<GeometryChangedEvent>((evt) => 
			{
				var element = evt.currentTarget as VisualElement;
				visualElement.style.width = element.worldBound.width;
				visualElement.style.height = element.worldBound.height;
				visualElement.transform.position = element.worldBound.position;
			});
			
			dynamicUIDictionary.Add(visualElement.GetHashCode(), moveableElements[i]);
			
			visualElement.RegisterCallback<PointerDownEvent>((evt) => 
			{
				Touch touch = TouchExtension.GetTouchOverlapVisualElement(visualElement, dynamicLayer.panel);
				StartCoroutine(MoveDynamicUI(touch, visualElement, dynamicUIDictionary[visualElement.GetHashCode()]));
			});
		}
	}
	
	IEnumerator MoveDynamicUI(Touch touch, VisualElement controlElement, VisualElement manipulatedElement)
	{
		manipulatedElement.style.right = manipulatedElement.style.left = manipulatedElement.style.top = manipulatedElement.style.bottom = 0;
		
		while (touch.phase != UnityEngine.InputSystem.TouchPhase.Ended)
		{
			Vector2 touchPosition = RuntimePanelUtils.ScreenToPanel(controlElement.panel, new Vector2(touch.screenPosition.x, Screen.height - touch.screenPosition.y));
			controlElement.transform.position = controlElement.parent.WorldToLocal(touchPosition);
			manipulatedElement.transform.position = manipulatedElement.parent.WorldToLocal(touchPosition);
		
			/* Because we are using translate when moving the dynamic UI, GeometryChangedEvent will not be called. If we want
			some custom call back when moving the dynamic UI, we can use manually call GeometryChangedEvent. Then we can register
			GeometryChangedEvent on manipulatedElement. */
			using GeometryChangedEvent evt = GeometryChangedEvent.GetPooled();
			evt.target = manipulatedElement;
			manipulatedElement.SendEvent(evt);
		
			yield return new WaitForSeconds(Time.fixedDeltaTime);	
		}
	}
}
