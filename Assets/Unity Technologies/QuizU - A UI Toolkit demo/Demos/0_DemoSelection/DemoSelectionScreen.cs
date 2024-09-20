using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quiz
{
    /// <summary>
    /// Class to manage the View logic for the Demo scene selection. This is similar
    /// to the LevelSelectionScreen but loads a Scene rather than ScriptableObject data.
    /// </summary>
    public class DemoSelectionScreen : UIScreen
    {
        NavigationBar m_NavigationBar;

        // UI elements
        Label m_Title;
        VisualElement m_Icon;
        Label m_Summary;
        Button m_BackButton;
        
    
        readonly DemoInfoSO[] m_DemoInfo; // Information about the demo scenes (from the Presenter)

        public Button BackButton  => m_BackButton;

        // Exposes action buttons to assign click events
        public NavigationBar NavigationBar => m_NavigationBar;

        public DemoSelectionScreen(VisualElement parentElement, DemoInfoSO[] demoInfo): base(parentElement)
        {
            m_DemoInfo = demoInfo;
            m_RootElement = parentElement;
            SetVisualElements();
            m_HideOnAwake = false;

        }


        private void DemoEvents_SetupCompleted()
        {
            foreach (VisualElement child in m_RootElement.Children())
            {
                Debug.Log("Child name: " + child.name);
            }

            Debug.Log("Root Element: " + m_RootElement.name);
            Debug.Log("Number of children: " + m_RootElement.childCount);
        }

        public override void Disable()
        {
            base.Disable();
            DemoEvents.SetupCompleted -= DemoEvents_SetupCompleted;
    
        }
        
        private void SetVisualElements()
        {
            m_Title = m_RootElement.Q<Label>("demo-info__title");
            m_Summary = m_RootElement.Q<Label>("demo-info__summary");
            m_Icon = m_RootElement.Q<VisualElement>("demo-info__icon");
            m_BackButton = m_RootElement.Q<Button>("back-button");
            SetupNavigationBar(m_DemoInfo.Length);


        }

        public void RegisterCallbacks()
        {
            // Notify the DemoSelectionSetup component to unload the DemoSelectionScene
            m_EventRegistry.RegisterCallback<ClickEvent>(m_BackButton, evt => DemoEvents.BackButtonClicked?.Invoke());

            // Assign ClickEvent handlers for each button in the navigation bar
            for (int i = 0; i < m_DemoInfo.Length; i++)
            {
                int index = i; // Closure capture

                NavigationBar.SetButtonLabelTextAtIndex(index, m_DemoInfo[index].Title);
                Button button = NavigationBar.Buttons[index];

                m_EventRegistry.RegisterCallback<ClickEvent>(button, evt => ButtonClickHandler(index, evt));
                m_EventRegistry.RegisterCallback<MouseEnterEvent>(button, MouseEnterHandler);
            }
        }

        // The EventRegistry can unregister all callbacks with one call to Dispose
        public void UnregisterCallbacks()
        {
            m_EventRegistry.Dispose();
        }


        // Initialize the container of buttons used to select levels/quizzes
        public void SetupNavigationBar(int numberOfButtons)
        {
            // Use the text button VisualTreeAsset (stretches horizontal width)
            m_NavigationBar = new NavigationBar();
            m_NavigationBar.Initialize(m_RootElement, numberOfButtons, "menu-text-button", false);

        }

        public void ShowSummary(string text)
        {
            m_Summary.text = text;
        }

        public void ShowTitle(string text)
        {
            m_Title.text = text;
        }

        public void ShowIcon(Sprite sprite)
        {
            if (sprite == null)
                return;

            m_Icon.style.backgroundImage = new StyleBackground(sprite);
        }



        // Event-handling methods

        // The DemoScenes are self-contained Unity scenes; this shows how you can
        // use the additive scene workflow. This additively loads a Unity scene by index.
        private void ButtonClickHandler(int index, ClickEvent evt)
        {
            SceneEvents.SceneIndexLoaded?.Invoke(m_DemoInfo[index].SceneIndex);
        }

        private void MouseEnterHandler(MouseEnterEvent evt)
        {
            Button hoverOverButton = evt.target as Button;
            
            if (hoverOverButton != null)
            {
                // If we are hovering the mouse over a Button in the NavigationBar, check the button's index
                int index = NavigationBar.Buttons.IndexOf(hoverOverButton);

                // If index is valid, highlight the button and pass the index to an event
                if (index != -1 && index < m_DemoInfo.Length)
                {
                    NavigationBar.HighlightButton(index);

                    DemoEvents.ButtonHighlighted?.Invoke(index);

                }
            }
        }
    }
}
