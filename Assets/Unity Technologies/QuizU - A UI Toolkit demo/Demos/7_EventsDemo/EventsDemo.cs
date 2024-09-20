using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quiz
{
    /// <summary>
    /// Demonstrates how to handle various user interface events. Note
    /// how elements can have multiple callbacks, depending on context.
    /// </summary>
    public class EventsDemo : DemoBase
    {
        // Defines the maximum and minimum widths for the m_Button
        const float k_ButtonMaxWidth = 450;
        const float k_ButtonMinWidth = 100;

        // References to UI components
        Slider m_Slider;
        TextField m_TextField;
        Button m_Button;

        Label m_SliderLabel;
        Label m_TextFieldLabel;
        Label m_ButtonLabel;

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterCallbacks();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnregisterCallbacks();
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            // Query UI components by names
            m_Slider = m_Root.Q<Slider>("demo__slider");
            m_TextField = m_Root.Q<TextField>("demo__text-field");
            m_Button = m_Root.Q<Button>("demo__button");
            m_SliderLabel = m_Root.Q<Label>("demo__slider-label");
            m_TextFieldLabel = m_Root.Q<Label>("demo__text-field-label");
            m_ButtonLabel = m_Root.Q<Label>("demo__button-label");

            // Initial values
            m_Slider.value = 50;
            SetButtonWidth(50);
            m_SliderLabel.text = string.Empty;
            m_ButtonLabel.text = string.Empty;
            m_TextFieldLabel.text = string.Empty;
        }

        private void RegisterCallbacks()
        {
            // Slider callbacks
            m_EventRegistry.RegisterCallback<ChangeEvent<float>>(m_Slider, SliderChangeHandler);

            // TextField callbacks
            m_EventRegistry.RegisterCallback<ChangeEvent<string>>(m_TextField, TextFieldInputHandler);
            m_EventRegistry.RegisterCallback<FocusEvent>(m_TextField, TextFieldFocusHandler);

            // Button callbacks

            // Using the EventRegistry allows you to unregister the callback, even if it's a lambda 
            m_EventRegistry.RegisterCallback<ClickEvent>(m_Button, evt => m_ButtonLabel.text = "ClickEvent");

            // The GeometryChangedEvent invokes when the Button changes layout (position/size)
            m_EventRegistry.RegisterCallback<GeometryChangedEvent>(m_Button, ButtonGeometryChangedHandler);

        }

        private void UnregisterCallbacks()
        {
            m_EventRegistry.Dispose();
        }

        // Changes the m_ButtonLabel text when the Button resizes
        private void ButtonGeometryChangedHandler(GeometryChangedEvent evt)
        {
            m_ButtonLabel.text = "GeometryChangedEvent.newRect:\n" + evt.newRect;
        }

        private void TextFieldFocusHandler(FocusEvent evt)
        {
            m_TextFieldLabel.text = "TextField FocusEvent";
        }

        private void TextFieldInputHandler(ChangeEvent<string> evt)
        {
            m_TextFieldLabel.text = "TextField ChangeEvent.newData:\n  " + evt.newValue;
        }

        private void SliderChangeHandler(ChangeEvent<float> evt)
        {
            SetButtonWidth(evt.newValue);
        }

        // Set the button width in pixels
        private void SetButtonWidth(float newValue)
        {
            float buttonWidth = (k_ButtonMaxWidth - k_ButtonMinWidth) / 100f;
            m_Button.style.width = newValue * buttonWidth + k_ButtonMinWidth;

            m_SliderLabel.text = "Slider ChangeEvent.newValue: " + newValue;
        }
    }
}
