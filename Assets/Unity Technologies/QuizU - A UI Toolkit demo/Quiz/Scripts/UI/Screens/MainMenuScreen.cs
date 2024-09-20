using UnityEngine.UIElements;
using UnityEngine;
using System;

namespace Quiz
{

    /// <summary>
    /// This class adds some custom text to the MainMenu screen and functionality to buttons.
    /// 
    /// </summary>
    public class MainMenuScreen : UIScreen
    {
        MenuButtonSO[] m_MenuButtonData;

        // The Demo Selector scene path
        [SerializeField] string m_DemosScenePath = "Unity Technologies/QuizU - A UI Toolkit demo/Assets/Demos/0_DemoSelection/DemoSelection.unity";
        [SerializeField] int m_DemosSceneIndex = 11;

        Label m_Description;        // Label element to display descriptions
        Button m_PlayButton;        // Button to switch to the Level Selection screen
        Button m_SettingsButton;    // Button to switch to the Settings screen
        Button m_DemosButton;       // Button load the DemoSelection scene
        Button m_MoreButton;        // Button to open a URL to the sample project
        Button m_BackButton;

        VisualElement m_ButtonContainer1;
        VisualElement m_ButtonContainer2;

        public MainMenuScreen(VisualElement parentElement): base(parentElement)
        {
            SetVisualElements();
            RegisterCallbacks();
            ShowButtonContainer(1);
        }

        private void SetVisualElements()
        {
            m_Description = m_RootElement.Q<Label>("menu__description");
            m_Description.text = string.Empty;

            m_SettingsButton = m_RootElement.Q<Button>("menu__button-settings");
            m_PlayButton = m_RootElement.Q<Button>("menu__button-play");
            m_DemosButton = m_RootElement.Q<Button>("menu__button-demos");

            m_MoreButton = m_RootElement.Q<Button>("menu__button-more");
            m_BackButton = m_RootElement.Q<Button>("menu__button-back");

            m_ButtonContainer1 = m_RootElement.Q<VisualElement>("menu__button-container--1");
            m_ButtonContainer2 = m_RootElement.Q<VisualElement>("menu__button-container--2");

            m_MenuButtonData = Resources.LoadAll<MenuButtonSO>("MenuButtonData");
            // The ScriptableObjects store the MenuButton and ElementID fields store references to the
            // Button and button id, so we don't have to store it here (we use this for the buttons that
            // link to the Dragon Crashers, UI Documentation, How To articles, and UI Artist resources
            for (int i = 0; i < m_MenuButtonData.Length; i++)
            {
                m_MenuButtonData[i].MenuButton = m_RootElement.Q<Button>(m_MenuButtonData[i].ElementID);

                // Use the userData property to store custom description for use later
                m_MenuButtonData[i].MenuButton.userData = m_MenuButtonData[i].Description;
            }
        }

        private void RegisterCallbacks()
        {
            // Register System.Action delegates to each Button's ClickEvent. 
            m_EventRegistry.RegisterCallback<ClickEvent>(m_SettingsButton, evt => UIEvents.SettingsShown?.Invoke());
            m_EventRegistry.RegisterCallback<ClickEvent>(m_PlayButton, evt => UIEvents.LevelSelectionShown?.Invoke());

            m_EventRegistry.RegisterCallback<ClickEvent>(m_DemosButton, evt => LoadSceneByIndex(m_DemosSceneIndex));

            m_EventRegistry.RegisterCallback<ClickEvent>(m_MoreButton, evt => ShowButtonContainer(2));
            m_EventRegistry.RegisterCallback<ClickEvent>(m_BackButton, evt => ShowButtonContainer(1));

            // Loop through all MenuButtonData ScriptableObjects, get the corresponding Button objects,
            // and register callbacks for MouseEnterEvent and MouseLeaveEvent.
            for (int i = 0; i < m_MenuButtonData.Length; i++)
            {
                m_EventRegistry.RegisterCallback<MouseEnterEvent>(m_MenuButtonData[i].MenuButton, EnterMenuHandler);
                m_EventRegistry.RegisterCallback<MouseLeaveEvent>(m_MenuButtonData[i].MenuButton, ExitMenuHandler);

                // If a MenuButton has a corresponding URL, verify the link and then set up the corresponding Button ClickEvent
                if (Uri.IsWellFormedUriString(m_MenuButtonData[i].URL, UriKind.Absolute))
                {
                    m_EventRegistry.RegisterCallback<ClickEvent>(m_MenuButtonData[i].MenuButton, evt => OpenURL(evt.target as Button));
                }
            }
        }

        private void ShowButtonContainer(int index)
        {
            if (index == 1)
            {
                m_ButtonContainer1.style.display = DisplayStyle.Flex;
                m_ButtonContainer2.style.display = DisplayStyle.None;
            }
            else if (index == 2)
            {
                m_ButtonContainer1.style.display = DisplayStyle.None;
                m_ButtonContainer2.style.display = DisplayStyle.Flex;
            }
            else
            {
                Debug.LogWarning("[MenuMenuScreen]: Invalid button container...");
            }
        }

        // Handle MouseEnterEvent by updating the description text based on the button being hovered.
        private void EnterMenuHandler(MouseEnterEvent evt)
        {
            // Get the button that raised the event
            Button eventButton = evt.target as Button;

            // Update the description text using the previously stored custom data
            m_Description.text = (string)eventButton.userData;
        }

        // Handle MouseLeaveEvent by clearing the description text.
        private void ExitMenuHandler(MouseLeaveEvent evt)
        {
            m_Description.text = string.Empty;
        }

        private void OpenURL(Button button)
        {
            for (int i = 0; i < m_MenuButtonData.Length; i++)
            {
                if (m_MenuButtonData[i].MenuButton == button)
                {
                    UIEvents.UrlOpened?.Invoke(m_MenuButtonData[i].URL);
                    break;
                }
            }
        }

        private void LoadScenePath(string scenePath)
        {
            SceneEvents.LoadSceneByPath?.Invoke(scenePath);
        }

        private void LoadSceneByIndex(int sceneIndex)
        {
            SceneEvents.SceneIndexLoaded?.Invoke(sceneIndex);
        }
    }
}
