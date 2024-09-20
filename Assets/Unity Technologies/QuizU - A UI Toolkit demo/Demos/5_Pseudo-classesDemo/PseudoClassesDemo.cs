using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quiz
{
    /// <summary>
    /// This demo scene shows basic usage of pseudo-classes for simple animations.
    /// Note that no extra code is required in most cases. Here we disable button-3
    /// and button-4 to activate the :disabled pseudo-class.
    /// </summary>
    public class PseudoClassesDemo : DemoBase
    {
        // Element IDs
        const string k_Button3 = "demo__button--3";
        const string k_Button4 = "demo__button--4";

        // References to VisualElements
        Button m_Button3;
        Button m_Button4;


        protected override void OnEnable()
        {
            base.OnEnable();
            SetVisualElements();
        }

        // Sets references to the root VisualElement and other necessary UI components.
        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            m_Button3 = m_Root.Q<Button>(k_Button3);
            m_Button4 = m_Root.Q<Button>(k_Button4);

            m_Button3.SetEnabled(false);
            m_Button4.SetEnabled(false);

            m_Button4.pickingMode = PickingMode.Ignore;

        }
    }
}
