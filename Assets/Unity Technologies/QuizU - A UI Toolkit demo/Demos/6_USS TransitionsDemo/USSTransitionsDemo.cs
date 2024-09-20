using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quiz
{
    public class USSTransitionsDemo : DemoBase
    {
        const string k_Toggle = "demo__toggle";
        const string k_AnimatedContainer = "demo__animated-container";

        const string k_EnabledStyle = "animated-container--enabled";
        const string k_DisabledStyle = "animated-container--disabled";

        Toggle m_Toggle;
        VisualElement m_AnimatedContainer;

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

        // Handles toggle value changes by adding/removing styles
        private void OnToggleValueChanged(ChangeEvent<bool> evt)
        {
            if (m_AnimatedContainer == null)
            {
                return;
            }

            if (evt.newValue)
            {
                m_AnimatedContainer.RemoveFromClassList(k_DisabledStyle);
            }
            else
            {
                m_AnimatedContainer.AddToClassList(k_DisabledStyle);
            }
        }

        // Sets up references to the visual elements
        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            m_Toggle = m_Root.Q<Toggle>(k_Toggle);
            m_AnimatedContainer = m_Root.Q<VisualElement>(k_AnimatedContainer);

            if (m_Toggle == null || m_AnimatedContainer == null)
            {
                return;
            }

            // Set default styles
            m_AnimatedContainer.AddToClassList(k_EnabledStyle);

            if (m_Toggle.value)
            {
                m_AnimatedContainer.RemoveFromClassList(k_DisabledStyle);
            }
            else
            {
                m_AnimatedContainer.AddToClassList(k_DisabledStyle);
            }

        }

        // Registers the value changed callback for the toggle
        private void RegisterCallbacks()
        {
            m_Toggle.RegisterValueChangedCallback(OnToggleValueChanged);
        }

        private void UnregisterCallbacks()
        {
            m_Toggle.UnregisterValueChangedCallback(OnToggleValueChanged);
        }
    }
}
