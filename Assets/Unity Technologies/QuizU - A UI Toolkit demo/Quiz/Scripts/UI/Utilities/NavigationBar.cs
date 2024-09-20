using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

namespace Quiz
{
    /// <summary>
    /// The NavigationBar is a reusable component for rendering sets of buttons. It supports names, icons, tooltips for buttons.
    /// Clients can access its buttons using the public Buttons properties. Use it for tabbed menus, carousel UIs, etc.
    /// </summary>
    [System.Serializable]
    public class NavigationBar
    {
        const string k_DefaultButtonResource = "VisualTreeAssets/level-button"; // Name of the UXML Template for one button
        const string k_SelectedButtonClassName = "level-button--selected"; // Class for selected button
        const int k_MinButtonSize = 100;
        const int k_TextSize = 50;

        bool m_UseSquareButtons = true;
        bool m_CapitalizeButtonTitles = true;

        // UI elements
        VisualElement m_Root;  // rootVisualElement for the screen
        VisualElement m_ButtonContainer;    // Flex container for all buttons

        VisualTreeAsset m_ButtonAsset; // Template used to create buttons in the NavigationBar

        TooltipController m_TooltipController; // Assigns Tooltip Manipulator to Buttons in NavigationBar
        List<Button> m_Buttons = new List<Button>();

        public List<Button> Buttons => m_Buttons;

        // Sets up any necessary dependencies from the MessageBar
        // TO-DO: Make this the constructor
        public void Initialize(VisualElement rootElement, int numberOfButtons, string resourceToLoad = k_DefaultButtonResource, bool useSquareButtons = true)
        {
            m_Root = rootElement;
            m_ButtonContainer = m_Root.Q<VisualElement>("button-container");

            if (m_ButtonContainer != null)
            {
                RemovePlaceHolders();
            }
            else
            {
                Debug.Log("[NavigationBar]: Missing button container");
                return;
            }

            // Initialize TooltipController after setting up m_Root; note: this works best in this implementation if assigned 
            // to the root level of the UI Hierarchy (e.g. rootVisualElement). Don't use the parent element of a UIScreen
            // if it's part of a nested UXML (like UIScreens.uxml).
            m_TooltipController = new TooltipController(m_Root);

            m_UseSquareButtons = useSquareButtons;
            SetupButtons(numberOfButtons, resourceToLoad, null);
        }

        // Set up the buttons
        public void SetupButtons(int numberOfButtons, string resourceToLoad, Sprite icon = null)
        {
            m_ButtonAsset = Resources.Load<VisualTreeAsset>(resourceToLoad);

            for (int i = 1; i <= numberOfButtons; i++)
            {
                InstantiateButton(i.ToString(), icon);
            }
        }

        // Optional setup for button tooltips; tooltip is a Label at the root container level
        public void AssignTooltipTextToButtons(string[] tooltips)
        {
            int length = Mathf.Min(tooltips.Length, m_Buttons.Count);

            for (int i = 0; i < length; i++)
            {
                if (!string.IsNullOrEmpty(tooltips[i]))
                {
                    m_TooltipController.AssignTooltipToElement(m_Buttons[i], tooltips[i]);
                }
            }
        }

        // Create a single button from a VisualTreeAsset as a child of the m_ButtonContainer
        public void InstantiateButton(string name, Sprite icon)
        {
            // Instantiate from the VisualTreeAsset
            TemplateContainer buttonInstance = m_ButtonAsset.Instantiate();
            
            //Debug.Log("NavigationBar ButtonSetup: " + name);
            m_ButtonContainer.Add(buttonInstance);

            // Get the UI Button component, then set up name and callback
            Button button = buttonInstance.Q<Button>();

            button.userData = "nav-bar-button--" + name;

            // Set the initial size to be square based on the minimum dimension of the container
            if (m_UseSquareButtons)
                MakeSquare(button, m_ButtonContainer);

            // Set the button name and icon
            SetButtonLabelText(button, name);
            button.name = button.userData as string;
            SetButtonIcon(button, icon);

            // Track the UI button in a List
            m_Buttons.Add(button);
        }

        // Set the button name
        public void SetButtonLabelText(Button button, string labelText)
        {
            Label label = button.Q<Label>("button__label");

            if (m_CapitalizeButtonTitles)
            {
                labelText = labelText.ToUpper();
            }

            label.text = labelText;
            label.style.fontSize = k_TextSize;
        }

        public void SetButtonLabelTextAtIndex(int index, string name)
        {
            if (index >= m_Buttons.Count)
            {
                Debug.Log("[NavigationBar]: Button index out of range");
                return;
            }

            SetButtonLabelText(m_Buttons[index], name);
        }

        // Set the button icon
        public void SetButtonIcon(Button button, Sprite icon)
        {
            VisualElement buttonIcon = button.Q<VisualElement>("button__icon");
            if (icon == null)
            {
                buttonIcon.style.display = DisplayStyle.None;
            }
            else
            {
                buttonIcon.style.backgroundImage = new StyleBackground(icon);
            }
        }

        // Set the icon based on index within the m_Buttons
        public void SetButtonIconAtIndex(int index, Sprite icon)
        {
            if (index >= m_Buttons.Count)
            {
                Debug.Log("[NavigationBar]: Button index out of range");
                return;
            }

            SetButtonIcon(m_Buttons[index], icon);
        }

        // Set the button tooltip
        public void SetButtonTooltip(Button button, string tooltip)
        {
            button.tooltip = tooltip;
        }

        // Removes any temporary buttons used for layout in UI Builder. This allows you to
        // keep placeholders in the UXML for visualization and then clear them at runtime.
        private void RemovePlaceHolders()
        {
            List<TemplateContainer> placeholders = m_ButtonContainer.Query<TemplateContainer>().ToList();

            foreach (TemplateContainer placeholder in placeholders)
            {
                m_ButtonContainer.Remove(placeholder);
            }
        }

        // Highlight the clicked button
        public void HighlightButton(int buttonIndex)
        {

            // Clear all currently selected buttons
            UnhighlightButtons();

            if (buttonIndex >= 0 && buttonIndex < m_Buttons.Count)
            {
                // Get the clicked button using its index
                Button clickedButton = m_Buttons[buttonIndex];

                // Add the selected button class to the clicked button
                clickedButton.AddToClassList(k_SelectedButtonClassName);
            }
            else
            {
                Debug.LogWarning("[NavigationBar]: Button index out of range");
            }
        }

        // Removes highlight styles from all Buttons
        private void UnhighlightButtons()
        {
            List<Button> selectedButtons = m_ButtonContainer.Query<Button>()
                .Where(x => x.ClassListContains(k_SelectedButtonClassName)).ToList();

            foreach (Button button in selectedButtons)
            {
                button.RemoveFromClassList(k_SelectedButtonClassName);
            }
        }

        // Forces a button to be a square; uses minimum dimension from
        // another reference VisualElement (e.g., the parent container)
        private void MakeSquare(Button button, VisualElement referenceElement)
        {
            // Get a minimum size from another Visual Element
            float size = Mathf.Min(referenceElement.resolvedStyle.width, referenceElement.resolvedStyle.height);

            // Set a minimum size constraint to avoid buttons becoming too small
            size = Mathf.Max(size, k_MinButtonSize);

            // Constrain both width and height
            button.style.width = size;
            button.style.height = size;

            // Center the button within the container using FlexBox rules
            button.style.alignSelf = Align.Center;
            button.style.justifyContent = Justify.Center;
        }
    }
}
