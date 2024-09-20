using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quiz
{
    public class UQueryDemo : DemoBase
    {
        // Class ID for highlighted elements
        const string k_HighlightClass = "highlighted";

        DropdownField m_DropdownField;
        Label m_DescriptionText;
        List<VisualElement> m_SelectedElements;

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterCallbacks();
        }

        protected override void OnDisable()
        {
            UnregisterCallbacks();
        }

        // Set up references to the elements
        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            // Note: 'name:' is optional as the first parameter
            m_DescriptionText = m_Root.Q<Label>(name: "demo__description-text");
            m_DropdownField = m_Root.Q<DropdownField>(name: "demo__dropdown-field");

            m_SelectedElements = new List<VisualElement>();
            m_DropdownField.choices = new List<string>()
            {
                "None",
                "Q<VisualElement>(name: \"button-1\")",
                "Q<VisualElement>(name: \"button-2\")",
                "Query<VisualElement>(className: \"round-outline-button\").ToList()",
                "Q<VisualElement>(className: \"outline-slider\")",
                "Q<VisualElement>(className: \"outline-slider\").Q<VisualElement>(name: \"unity-dragger\")"
            };

            m_DropdownField.value = "Choose a selector";
        }

        private void RegisterCallbacks()
        {
            m_DropdownField.RegisterCallback<ChangeEvent<string>>(ExecuteChoice);

            // Alternatively:
            // m_DropdownField.RegisterValueChangedCallback(ExecuteChoice);
        }

        protected void UnregisterCallbacks()
        {
            m_DropdownField.UnregisterValueChangedCallback(ExecuteChoice);
        }

        // Shows text and highlights elements when the user changes the value of the dropdown field.
        private void ExecuteChoice(ChangeEvent<string> evt)
        {
            string selectedChoice = evt.newValue;
            
            UnhighlightElements();
            m_SelectedElements.Clear();

            switch (selectedChoice)
            {
                case "None":
                    m_DescriptionText.text = "Choose a selector to highlight the selected elements.";
                    break;
                case "Q<VisualElement>(name: \"button-1\")":
                    m_SelectedElements.Add(m_Root.Q<VisualElement>("button-1"));
                    m_DescriptionText.text = "Select the element named 'button-1'.";
                    break;
                case "Q<VisualElement>(name: \"button-2\")":
                    m_SelectedElements.Add(m_Root.Q<VisualElement>("button-2"));
                    m_DescriptionText.text = "Select the element named 'button-2'.";
                    break;
                case "Query<VisualElement>(className: \"round-outline-button\").ToList()":
                    m_SelectedElements = m_Root.Query<VisualElement>(className: "round-outline-button").ToList();
                    m_DescriptionText.text = "Select all elements with the class 'round-outline-button'";
                    break;
                case "Q<VisualElement>(className: \"outline-slider\")":
                    m_SelectedElements.Add(m_Root.Q<VisualElement>(className: "outline-slider"));
                    m_DescriptionText.text = "Select the first element with the class 'outline-slider'";
                    break;
                case "Q<VisualElement>(className: \"outline-slider\").Q<VisualElement>(name: \"unity-dragger\")":
                    m_SelectedElements.Add(m_Root.Q<VisualElement>(className: "outline-slider").Q<VisualElement>(name: "unity-dragger"));
                    m_DescriptionText.text = "Select the first element named 'unity-dragger' under the first element using class 'outline-slider'.";
                    break;
                default:
                    break;
            }
            HighlightElements();
        }

        // Add a k_HighlightClass style to selected elements
        private void HighlightElements()
        {
            if (m_SelectedElements.Count == 0)
            {
                return;
            }

            foreach (VisualElement visualElement in m_SelectedElements)
            {
                visualElement.AddToClassList(k_HighlightClass);
            }
        }

        // Clear the k_HighlightClass style from selected elements
        private void UnhighlightElements()
        {
            if (m_SelectedElements.Count == 0)
            {
                return;
            }

            foreach (VisualElement visualElement in m_SelectedElements)
            {
                visualElement.RemoveFromClassList(k_HighlightClass);
            }
        }
    }
}
