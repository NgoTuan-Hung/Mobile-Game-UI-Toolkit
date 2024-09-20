using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quiz
{
    public class USSStylesDemo : DemoBase
    {
        // Unique IDs for demo buttons
        private readonly string[] m_ButtonNames =
        {
            "default-button",
            "blue-button",
            "red-button",
            "outline-button",
            "square-button",
            "circular-button",
            "text-button"
        };

        [Tooltip("Style information that corresponds to each button")]
        [SerializeField] List<StyleInfoSO> m_StyleInfo;

        Label m_SelectorLabel;
        Label m_UssStyleLabel;
        Label m_DescriptionLabel;

        List<Button> m_Buttons = new List<Button>();

        // OnDisabled called in base class
        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterCallbacks();
        }

        // Set up references to the elements
        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            for (int i = 0; i < m_ButtonNames.Length; i++)
            {
                Button button = m_Root.Q<Button>(m_ButtonNames[i]);

                if (button != null && !m_Buttons.Contains(button))
                {
                    // You can use the userData property to store additional information.
                    // Here, we store the button's index for later use.
                    button.userData = i;

                    m_Buttons.Add(button);
                }
            }

            // Fetch the label references
            m_SelectorLabel = m_Root.Q<Label>("style-info__selector-text");
            m_UssStyleLabel = m_Root.Q<Label>("style-info__style-text");
            m_DescriptionLabel = m_Root.Q<Label>("style-info__description-text");

            if (m_StyleInfo.Count < m_Buttons.Count)
            {
                Debug.LogWarning($"[USSStylesDemo]: Not enough StyleInfo elements ({m_StyleInfo.Count}) for buttons ({m_Buttons.Count}).");
            }
        }

        // Register the OnHover and OnClear callbacks for each button
        protected void RegisterCallbacks()
        {
            for (int i = 0; i < m_Buttons.Count; i++)
            {
                m_Buttons[i].RegisterCallback<MouseEnterEvent>(MouseHoverHandler);
                m_Buttons[i].RegisterCallback<MouseLeaveEvent>(MouseExitHandler);
            }
        }

        // Unregister callbacks if this is 
        protected void UnregisterCallbacks()
        {
            //base.UnregisterCallbacks();

            for (int i = 0; i < m_Buttons.Count; i++)
            {
                m_Buttons[i].UnregisterCallback<MouseEnterEvent>(MouseHoverHandler);
                m_Buttons[i].UnregisterCallback<MouseLeaveEvent>(MouseExitHandler);
            }
        }

        // When the mouse leaves a button, clear its style information
        private void MouseExitHandler(MouseLeaveEvent evt)
        {
            m_SelectorLabel.text = string.Empty;
            m_UssStyleLabel.text = string.Empty;
            m_DescriptionLabel.text = string.Empty;
        }

        // When the mouse enters a button, show its style information
        private void MouseHoverHandler(MouseEnterEvent evt)
        {
            VisualElement target = evt.target as VisualElement;

            // Use userData to access the button's index
            int index = (int)target.userData;
            if (index < m_StyleInfo.Count && m_StyleInfo[index] != null)
            {
                m_SelectorLabel.text = m_StyleInfo[index].ClassSelectorText;
                m_UssStyleLabel.text = m_StyleInfo[index].UssStyleText;
                m_DescriptionLabel.text = m_StyleInfo[index].DescriptionText;
            }
            else
            {
                Debug.LogWarning($"[USSStylesDemo]: No StyleInfo provided for button at index {index}");
            }
        }

    }
}
