using UnityEngine;
using UnityEngine.UIElements;

namespace Quiz
{
    public class CustomControlsDemo : DemoBase
    {
        [Header("Buttons")]
        [Range(1,50)]
        [SerializeField] int m_NumberOfButtons = 16;
        [Tooltip("UXML asset for the CustomColorButton")]
        [SerializeField] VisualTreeAsset m_CustomButtonUxml;

        // Alternatively load from Resources or even better use Addressables
        //VisualTreeAsset m_CustomButtonUxml = Resources.Load<VisualTreeAsset>("CustomColorButton");

        VisualElement m_ButtonContainer;  // Container to organize custom buttons for the demo

        protected override void OnEnable()
        {
            // Sets up the Visual Elements
            base.OnEnable();

            CreateDemoButtons();
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            m_ButtonContainer = m_Root.Q<VisualElement>("button-container");
        }

        // Fill the button container with m_NumberOfButtons
        private void CreateDemoButtons()
        {
            // Use the UIDocument's VisualTreeAsset to clone your custom buttons
            
            for (int i = 0; i < m_NumberOfButtons; i++)
            {
                // Instantiate a new button using the template
                TemplateContainer customButton = m_CustomButtonUxml.Instantiate();

                // Add the button to the container
                if (m_ButtonContainer == null)
                {
                    Debug.LogError("[CustomControlsDemo]: Invalid button container.");
                    return;
                }

                m_ButtonContainer.Add(customButton);
            }
        }
    }
}
