
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System;

namespace Quiz
{
    /// <summary>
    /// The ProgressMeter class controls the progress bar at the top of the UI, showing
    /// the user how many questions have been answered and how many incorrect guesses
    /// remain. This is a custom section of the GameScreen.
    /// </summary>


    public class ProgressDisplay : IDisposable
    {
        const string k_StreakMessage = " in a row!";
        const string k_StreakOffClassName = "streak-disabled";
        const string k_StreakOnClassName = "streak-enabled";
        const float k_LerpTime = 0.2f;

        // UI elements
        Label m_StreakLabel;
        Label m_LivesLeftLabel;
        ProgressBar m_ProgressBar;
        VisualElement m_Root;
        Button m_PauseButton;
        Button m_SettingsButton;
        TooltipController m_TooltipController;

        Coroutine m_ProgressBarAnimation;

        public ProgressDisplay(VisualElement parentElement)
        {
            Initialize(parentElement);
        }

        // Event subscriptions
        private void SubscribeEvents()
        {
            UIEvents.GameScreenShown += UIEvents_GameScreenShown;
            UIEvents.ProgressBarUpdated += UIEvents_ProgressBarUpdated;

            GameEvents.StreakActivated += GameEvents_StreakActivated;
            GameEvents.StreakSnapped += GameEvents_StreakSnapped;

            GameEvents.LivesSetup += GameEvents_LivesSetup;
            GameEvents.LivesUpdated += GameEvents_LivesUpdated;
        }

        private void UnsubscribeEvents()
        {
            UIEvents.GameScreenShown -= UIEvents_GameScreenShown;
            UIEvents.ProgressBarUpdated -= UIEvents_ProgressBarUpdated;

            GameEvents.StreakActivated -= GameEvents_StreakActivated;
            GameEvents.StreakSnapped -= GameEvents_StreakSnapped;

            GameEvents.LivesSetup -= GameEvents_LivesSetup;
            GameEvents.LivesUpdated -= GameEvents_LivesUpdated;
        }

        // Initialize the progress meter UI elements
        public void Initialize(VisualElement parentElement)
        {
            m_Root = parentElement;
            SetVisualElements();
            RegisterCallbacks();
            ResetStreak();
            SubscribeEvents();
        }

        public void Dispose()
        {
            UnsubscribeEvents();

            // Stop all coroutines
            if (m_ProgressBarAnimation != null)
            {
                Coroutines.StopCoroutine(m_ProgressBarAnimation);
            }
        }

        // Set up references to the UI elements
        private void SetVisualElements()
        {
            m_StreakLabel = m_Root.Q<Label>("progress-bar__streak");
            m_ProgressBar = m_Root.Q<ProgressBar>("progress-bar__bar");
            m_LivesLeftLabel = m_Root.Q<Label>("progress-bar__lives-label");
            m_PauseButton = m_Root.Q<Button>("pause__button");
            m_SettingsButton = m_Root.Q<Button>("settings__button");

            SetTooltips();
        }

        private void SetTooltips()
        {
            m_TooltipController = new TooltipController(m_Root);

            m_TooltipController.AssignTooltipToElement(m_StreakLabel, "Current streak");
            m_TooltipController.AssignTooltipToElement(m_ProgressBar, "Remaining questions");
            m_TooltipController.AssignTooltipToElement(m_LivesLeftLabel, "Guesses remaining");
            m_TooltipController.AssignTooltipToElement(m_PauseButton, "Pause/quit game");
            m_TooltipController.AssignTooltipToElement(m_SettingsButton, "Sound volume settings");
        }

        // Alternate way to subscribe to events if they are simple; because this is not a MonoBehaviour,
        // this object disposes with the GameScreen
        private void RegisterCallbacks()
        {
            m_PauseButton.clicked += () => UIEvents.PauseScreenShown?.Invoke();
            m_SettingsButton.clicked += () => UIEvents.SettingsShown?.Invoke();
        }

        // Event-handling methods

        private void UIEvents_GameScreenShown()
        {
            ResetStreak();
        }

        private void GameEvents_StreakSnapped()
        {
            ResetStreak();
        }

        private void GameEvents_StreakActivated(int count)
        {
            ActivateStreak(count);
        }

        private void UIEvents_ProgressBarUpdated(float newValue)
        {
            // Use the coroutine helper to stop any coroutines already in progress
            if (m_ProgressBarAnimation != null)
            {
                Coroutines.StopCoroutine(m_ProgressBarAnimation);
            }

            m_ProgressBarAnimation = Coroutines.StartCoroutine(AnimateProgress(newValue));
        }

        // Show 
        private void GameEvents_LivesSetup(int livesMax)
        {
            m_LivesLeftLabel.text = livesMax.ToString();
        }

        private void GameEvents_LivesUpdated(int livesLeft)
        {
            m_LivesLeftLabel.text = livesLeft.ToString();
        }

        // Methods

        // Animate the progress bar using a lerping function like Mathf.SmoothStep
        private IEnumerator AnimateProgress(float newValue)
        {
            float originalValue = m_ProgressBar.value;

            float startTime = Time.time;

            while (Time.time < startTime + k_LerpTime)
            {
                m_ProgressBar.value = Mathf.SmoothStep(originalValue, newValue, (Time.time - startTime) / k_LerpTime);
                yield return null;
            }

            m_ProgressBar.value = newValue;
        }

        // Reset the streak display to its default state
        private void ResetStreak()
        {
            m_StreakLabel.RemoveFromClassList(k_StreakOnClassName);
            m_StreakLabel.AddToClassList(k_StreakOffClassName);
        }

        // Activate the streak display and update its text with the current streak count
        private void ActivateStreak(int count)
        {
            m_StreakLabel.text = count + k_StreakMessage;
            m_StreakLabel.RemoveFromClassList(k_StreakOffClassName);
            m_StreakLabel.AddToClassList(k_StreakOnClassName);
        }
    }
}
