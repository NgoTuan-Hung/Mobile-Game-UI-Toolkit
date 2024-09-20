using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quiz
{
    /// <summary>
    /// Class to control the dropdown and sliders in the PositionElements Demo.
    /// </summary>
    public class FlexboxDemo : DemoBase
    {
        DropdownField m_DirectionDropdown;
        DropdownField m_WrapDropdown;
        DropdownField m_AlignDropdown;
        DropdownField m_JustifyDropdown;
        SliderInt m_WidthSlider;
        SliderInt m_HeightSlider;

        // The on-screen container holding all of the other elements in the demo
        VisualElement m_ButtonContainer; 

        protected override void OnEnable()
        {
            base.OnEnable();

            SetVisualElements();
            RegisterCallbacks();
        }

        private void Start()
        {
            InitializeValues();
        }

        // Setup the visual elements and controls
        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            m_ButtonContainer = m_Root.Q<VisualElement>("demo__container");
            m_DirectionDropdown = m_Root.Q<DropdownField>("controls__direction");
            m_WrapDropdown = m_Root.Q<DropdownField>("controls__wrap");
            m_AlignDropdown = m_Root.Q<DropdownField>("controls__align");
            m_JustifyDropdown = m_Root.Q<DropdownField>("controls__justify");

            m_WidthSlider = m_Root.Q<SliderInt>("controls__width");
            m_HeightSlider = m_Root.Q<SliderInt>("controls__height");
        }

        private void RegisterCallbacks()
        {
            // A DropdownField responds to Change events. The base class includes an EventRegistry to help with callbacks.
            // This can simplify unregistering the callbacks with one call to Dispose in OnDisable. See the DemoBase class
            // for more information.

            m_EventRegistry.RegisterCallback<ChangeEvent<string>>(m_DirectionDropdown, DirectionChangedHandler);
            m_EventRegistry.RegisterCallback<ChangeEvent<string>>(m_WrapDropdown, WrapChangedHandler);
            m_EventRegistry.RegisterCallback<ChangeEvent<string>>(m_AlignDropdown, AlignChangedHandler);
            m_EventRegistry.RegisterCallback<ChangeEvent<string>>(m_JustifyDropdown, JustifyChangedHandler);

            m_EventRegistry.RegisterCallback<ChangeEvent<int>>(m_WidthSlider, WidthChangedHandler);
            m_EventRegistry.RegisterCallback<ChangeEvent<int>>(m_HeightSlider, HeightChangedHandler);
        }

        // Set initial values of controls
        private void InitializeValues()
        {
            m_DirectionDropdown.SetValueWithoutNotify("Row");
            m_WrapDropdown.SetValueWithoutNotify("Wrap");
            m_AlignDropdown.SetValueWithoutNotify("FlexStart");
            m_JustifyDropdown.SetValueWithoutNotify("FlexStart");

            m_WidthSlider.SetValueWithoutNotify(50);
            m_HeightSlider.SetValueWithoutNotify(50);
        }

        // Event-handling methods

        // DropdownMenus convert string values to the appropriate enum values before
        // setting the style properties.

        private void DirectionChangedHandler(ChangeEvent<string> evt)
        {
            if (Enum.TryParse(evt.newValue, out FlexDirection newDirection))
            {
                m_ButtonContainer.style.flexDirection = newDirection;
                m_ButtonContainer.MarkDirtyRepaint();
            }
            else
            {
                Debug.LogWarning($"Invalid direction value: {evt.newValue}");
            }
        }

        private void AlignChangedHandler(ChangeEvent<string> evt)
        {
            if (Enum.TryParse(evt.newValue, out Align newAlign))
            {
                m_ButtonContainer.style.alignItems = newAlign;
                m_ButtonContainer.MarkDirtyRepaint();
            }
            else
            {
                Debug.LogWarning($"Invalid align value: {evt.newValue}");
            }
        }

        private void WrapChangedHandler(ChangeEvent<string> evt)
        {
            if (Enum.TryParse(evt.newValue, out Wrap newWrap))
            {
                m_ButtonContainer.style.flexWrap = newWrap;
                m_ButtonContainer.MarkDirtyRepaint();
            }
            else
            {
                Debug.LogWarning($"Invalid wrap value: {evt.newValue}");
            }
        }

        private void JustifyChangedHandler(ChangeEvent<string> evt)
        {
            if (Enum.TryParse(evt.newValue, out Justify newJustify))
            {
                m_ButtonContainer.style.justifyContent = newJustify;
                m_ButtonContainer.MarkDirtyRepaint();
            }
            else
            {
                Debug.LogWarning($"Invalid justify value: {evt.newValue}");
            }
        }

        // Approximate the width percentage based on the parent's width (works only if
        // the parent element does not change)
        private void WidthChangedHandler(ChangeEvent<int> evt)
        {
            // Convert the value from the slider (0 to 100) to a fraction
            float newWidthFraction = evt.newValue / 100f;

            // Access the parent's style width and get new width
            float parentWidth = m_ButtonContainer.parent.resolvedStyle.width;
            float newWidth = parentWidth * newWidthFraction;

            // Apply the new width
            m_ButtonContainer.style.width = newWidth;
            m_ButtonContainer.MarkDirtyRepaint();
        }

        private void HeightChangedHandler(ChangeEvent<int> evt)
        {
            // Convert the value from the slider (0 to 100) to a Length

            Length newHeight = new Length(evt.newValue, LengthUnit.Percent);
            m_ButtonContainer.style.flexBasis = newHeight;
            m_ButtonContainer.MarkDirtyRepaint();
        }
    }
}

