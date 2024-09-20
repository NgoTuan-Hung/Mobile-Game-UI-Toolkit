using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quiz
{
    [System.Serializable]
    public class DemoTextScreen
    {
        [Header("Content")]
        [Tooltip("ScriptableObject that holds the text and events")]
        [SerializeField] TextScreenSO m_DemoScreenData;

        Button m_NextButton;
        Button m_LastButton;
        VisualElement m_Root;
        Label m_CurrentPage;
        Label m_Title;
        int m_PageIndex;

        EventRegistry m_EventRegistry;

        // Set up the TextScreen
        public void Initialize(VisualElement root)
        {
            m_EventRegistry = new EventRegistry();
            m_Root = root;
            SetVisualElements();
            RegisterCallbacks();
            ShowPage(m_PageIndex);
        }

        // Locate the UI Elements using string IDs. Then register the Button callbacks.
        private void SetVisualElements()
        {
            m_NextButton = m_Root.Q<Button>("text-screen__button-next");
            m_LastButton = m_Root.Q<Button>("text-screen__button-last");
            m_CurrentPage = m_Root.Q<Label>("text-screen__page-text");
            m_Title = m_Root.Q<Label>("text-screen__title");

            m_Title.text = m_DemoScreenData.Title;
            
        }

        private void RegisterCallbacks()
        {
            m_EventRegistry.RegisterCallback<ClickEvent>(m_NextButton, ShowNextPage);
            m_EventRegistry.RegisterCallback<ClickEvent>(m_LastButton, ShowLastPage);
        }

        // Unregister the callbacks to prevent errors
        private void OnDisable()
        {
            m_EventRegistry.Dispose();
        }

        // Show a specific text block in the body UI
        private void ShowPage(int index)
        {
            int clampedIndex = Mathf.Clamp(index, 0, m_DemoScreenData.BodyText.Count - 1);
            m_CurrentPage.text = m_DemoScreenData.BodyText[clampedIndex];

            UpdateNextLastButtons();
        }

        // Increment to the next page of text
        private void ShowNextPage(ClickEvent evt)
        {
            m_PageIndex++;
            m_PageIndex = Mathf.Clamp(m_PageIndex, 0, m_DemoScreenData.BodyText.Count - 1);
            ShowPage(m_PageIndex);
            DemoEvents.TextPageChanged?.Invoke();
        }

        // Decrement to the previous page of text
        private void ShowLastPage(ClickEvent evt)
        {
            m_PageIndex--;
            m_PageIndex = Mathf.Clamp(m_PageIndex, 0, m_DemoScreenData.BodyText.Count - 1);
            ShowPage(m_PageIndex);
            DemoEvents.TextPageChanged?.Invoke();
        }

        // Toggle the m_NextButton and m_LastButton depending on index.
        private void UpdateNextLastButtons()
        {
            if (m_DemoScreenData.BodyText.Count <= 1)
            {
                FadeElement(m_NextButton, false);
                FadeElement(m_LastButton, false);
                return;
            }

            if (m_PageIndex == 0)
            {
                FadeElement(m_NextButton, true);
                FadeElement(m_LastButton, false);
                return;
            }

            if (m_PageIndex >= m_DemoScreenData.BodyText.Count - 1)
            {
                FadeElement(m_NextButton, false);
                FadeElement(m_LastButton, true);
                return;
            }

            FadeElement(m_NextButton, true);
            FadeElement(m_LastButton, true);
        }

        // Enable/disable a specific UI Element.
        private void DisplayElement(VisualElement element, bool state)
        {
            element.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;
        }

        // Fade an element on or off; use USS transitions for the in-betweens
        private void FadeElement(VisualElement element, bool state)
        {
            element.style.opacity = (state) ? 1f : 0f;
        }
    }
}