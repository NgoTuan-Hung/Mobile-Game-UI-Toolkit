using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ConfigView : ViewBase
{
    UIDocument uIDocument;
    VisualElement root, configMenu, configExitButton;
    ConfigData configData;
    VisualTreeAsset configMenuContentAsset, configCheckboxAsset, configDropdownAsset, configSliderAsset;
    VisualElement currentSelectedMenuItem, currentDisplayedConfigContent;

    public void Init()
    {
        uIDocument = GetComponent<UIDocument>();
        root = uIDocument.rootVisualElement.Q<VisualElement>(name: "config__menu-root");
        configMenu = root.Q<VisualElement>(name: "config__menu");
        configExitButton = root.Q<VisualElement>(name : "config__exit-button");
        configExitButton.RegisterCallback<PointerDownEvent>((evt) => 
        {
            gameUIManager.DeactivateLayer((int)GameUIManager.LayerUse.Config);
        });
        
        configData = Resources.Load<ConfigData>("UI/ConfigData/Config");
        GetVisualTreeAsset();
        PopulateView();
    }

    public void GetVisualTreeAsset()
    {
        configMenuContentAsset = Resources.Load<VisualTreeAsset>("UI/Config/ConfigMenuContent");
        configCheckboxAsset = Resources.Load<VisualTreeAsset>("UI/Config/ConfigCheckbox");
        configDropdownAsset = Resources.Load<VisualTreeAsset>("UI/Config/ConfigDropdown");
        configSliderAsset = Resources.Load<VisualTreeAsset>("UI/Config/ConfigSlider");
    }

    List<VisualElement> configMenuContents = new List<VisualElement>();
    public void PopulateView()
    {
        VisualElement configContent, configCheckbox, configDropdown, configSlider;
        DropdownField configDropdownField;
        configData.configMenuItems.ForEach(configMenuItem => 
        {
            Label menuItem = new Label();
            menuItem.AddToClassList("config__menu-item");
            switch (configMenuItem.configType)
            {
                case ConfigMenuItem.ConfigType.Video: menuItem.text = "Video"; break;
                case ConfigMenuItem.ConfigType.Sound: menuItem.text = "Audio"; break;
                case ConfigMenuItem.ConfigType.Other: menuItem.text = "Other"; break;
                default: break;
            }

            menuItem.RegisterCallback<ClickEvent>(evt => MenuItemClickEvent(menuItem));
            configMenu.Add(menuItem);

            configContent = configMenuContentAsset.Instantiate().ElementAt(0);
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
                    default: break;
                }
            });

            configContent.style.position = Position.Absolute;
            configContent.style.display = DisplayStyle.None;
            configMenuContents.Add(configContent);
            
            root.Add(configContent);
        });

        currentSelectedMenuItem = configMenu.contentContainer.Children().First();
        currentSelectedMenuItem.AddToClassList("config__menu-item-selected");

        currentDisplayedConfigContent = configMenuContents[0];
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

        currentDisplayedConfigContent = configMenuContents[configMenu.contentContainer.IndexOf(menuItem)];
        currentDisplayedConfigContent.style.position = Position.Relative;
        currentDisplayedConfigContent.style.display = DisplayStyle.Flex;
    }
}
