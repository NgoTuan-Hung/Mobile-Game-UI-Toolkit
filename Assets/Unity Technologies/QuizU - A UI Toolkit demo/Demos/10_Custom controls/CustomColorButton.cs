using UnityEngine;
using UnityEngine.UIElements;

namespace Quiz
{
    /// <summary>
    /// This button toggles between an active and inactive color, showing a simple custom control
    /// available in the UI Builder.
    /// </summary>
    public class CustomColorButton : Button
    {
        // Properties to hold the active and inactive colors
        Color activeColor = new Color(0f, 0.5f, 0.85f, 1f);
        Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);

        bool m_IsActive = false;

        public new class UxmlFactory : UxmlFactory<CustomColorButton, UxmlTraits> { }

        // This exposes attributes in the UI Builder
        public new class UxmlTraits : Button.UxmlTraits
        {
            UxmlColorAttributeDescription m_AttrActiveColor =
                new UxmlColorAttributeDescription
                {
                    name = "active-color",
                    defaultValue = new Color(0f, 0.5f, 0.85f, 1f)
                };

            UxmlColorAttributeDescription m_AttrInactiveColor =
                new UxmlColorAttributeDescription
                {
                    name = "inactive-color",
                    defaultValue = new Color(0.5f, 0.5f, 0.5f, 1f)
                };

            /// <summary>
            /// Initializes the visual element, sets its active and inactive colors, and configures its initial style.
            /// This method is called when the visual element is created from UXML.
            /// </summary>
            /// <param name="ve">The visual element to initialize.</param>
            /// <param name="bag">The IUxmlAttributes that contains the data loaded from UXML.</param>
            /// <param name="cc">Passes additional information during creation (e.g the visual tree asset, parent element, etc.).</param>

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var button = (CustomColorButton)ve;

                button.activeColor = m_AttrActiveColor.GetValueFromBag(bag, cc);
                button.inactiveColor = m_AttrInactiveColor.GetValueFromBag(bag, cc);

                button.style.backgroundColor = button.inactiveColor;
            }
        }

        public CustomColorButton()
        {
            this.style.minWidth = 100;
            this.style.minHeight = 100;

            //// Optional: this shows the syntax for setting up a basic USS transition using the C# syntax
            //// More information: https://docs.unity3d.com/Manual/UIE-Transitions.html

            // Omitted for simplicitiy. Uncomment this block to add some "built-in" USS Transitions:

            /*
            // TransitionDuration needs a List of TimeValues

            List<TimeValue> durations = new List<TimeValue>();
            durations.Add(new TimeValue(0.5f, TimeUnit.Second));
            this.style.transitionDuration = new StyleList<TimeValue>(durations);

            // easingFunctions also needs a List

            List<EasingFunction> easingFunctions = new List<EasingFunction>();
            easingFunctions.Add(new EasingFunction(EasingMode.EaseInOut));
     
            this.style.transitionTimingFunction = new StyleList<EasingFunction>(easingFunctions);
            */

            // Use the Clickable Manipulator to toggle between active and inactive color.
            // The clicked property is a shorthand way to subscribe to click events (versus RegisterCallback).
            this.clicked += OnClick;

        }

        // Note: CustomColorButton instance is tightly bound to its OnClick handler; when the button is destroyed, the OnClick handler will be
        // be garbage collected alongside the button. 
        private void OnClick()
        {
            m_IsActive = !m_IsActive;
            this.style.backgroundColor = m_IsActive ? activeColor : inactiveColor;
        }
    }
}
