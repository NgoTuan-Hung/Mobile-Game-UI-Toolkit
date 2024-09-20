using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quiz
{
    /// <summary>
    /// The EndScreen class handles the display and functionality of the end screen (EndScreen.uxml),
    /// including the score, accuracy, and pass/fail status.
    /// </summary>
    public class EndScreen : UIScreen
    {
        Button m_MainMenuButton;
        Button m_RetryButton;
        Label m_Accuracy;
        Label m_PassFail;

        Label m_CorrectScore;
        Label m_IncorrectScore;
        Label m_IncompleteScore;
        Label m_TotalQuestions;

        Label m_QuizTitle;
        bool m_IsWinner;
        bool m_LivesExceeded;

        public EndScreen(VisualElement rootElement): base(rootElement)
        {
            SubscribeToEvents();
            SetVisualElements();
            RegisterCallbacks();
            Reset();
        }

        // Event unsubscriptions
        public override void Disable()
        {
            base.Disable();
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            UIEvents.GameScreenShown += UIEvents_GameScreenShown;
            UIEvents.ScoresTotaled += UIEvents_ScoresTotaled;
            UIEvents.AccuracyCalculated += UIEvents_AccuracyCalculated;
            UIEvents.QuizTitleShown += UIEvents_QuizTitleShown;

            GameEvents.GameWon += GameEvents_GameWon;
            GameEvents.LivesExceeded += GameEvents_LivesExceeded;
        }

        private void UnsubscribeFromEvents()
        {
            UIEvents.GameScreenShown -= UIEvents_GameScreenShown;
            UIEvents.ScoresTotaled -= UIEvents_ScoresTotaled;
            UIEvents.AccuracyCalculated -= UIEvents_AccuracyCalculated;
            UIEvents.QuizTitleShown -= UIEvents_QuizTitleShown;

            GameEvents.GameWon -= GameEvents_GameWon;
            GameEvents.LivesExceeded -= GameEvents_LivesExceeded;
        }

        #region Event-handling methods

        private void UIEvents_GameScreenShown()
        {
            Reset();
        }

        private void UIEvents_ScoresTotaled(int correctAnswers, int incorrectAnswers, int totalQuestions)
        {
            ShowScores(correctAnswers, incorrectAnswers, totalQuestions);
        }

        private void UIEvents_AccuracyCalculated(float value)
        {
            ShowPassFail(value);
        }

        private void UIEvents_QuizTitleShown(string title)
        {
            m_QuizTitle.text = title;
        }

        private void GameEvents_GameWon()
        {
            // Set the winner flag if the user has passed the quiz
            m_IsWinner = true;
        }

        private void GameEvents_LivesExceeded()
        {
            m_LivesExceeded = true;
        }
        #endregion

        #region Methods

        // Reset the end screen for a new game
        private void Reset()
        {
            m_IsWinner = false;
            m_LivesExceeded = false;
        }

        // Set up references to the UI elements
        private void SetVisualElements()
        {
            m_Accuracy = m_RootElement.Q<Label>("accuracy__text-value");
            m_PassFail = m_RootElement.Q<Label>("pass-fail__text");

            m_QuizTitle = m_RootElement.Q<Label>("win-lose__quiz-title");

            m_CorrectScore = m_RootElement.Q<Label>("correct-score__number");
            m_IncorrectScore = m_RootElement.Q<Label>("incorrect-score__number");
            m_IncompleteScore = m_RootElement.Q<Label>("incomplete-score__number");
            m_TotalQuestions = m_RootElement.Q<Label>("total__number");

            m_MainMenuButton = m_RootElement.Q<Button>("main-menu-button");
            m_RetryButton = m_RootElement.Q<Button>("retry-button");
        }

        // Note: the m_EventRegister disposes of all registered callbacks in the base class

        private void RegisterCallbacks()
        {
            // Use a lambda to pass the static delegate as a System.Action; automatically unregistered in UIScreen.Dispose
            m_EventRegistry.RegisterCallback<ClickEvent>(m_MainMenuButton, evt => UIEvents.MainMenuShown());
            m_EventRegistry.RegisterCallback<ClickEvent>(m_RetryButton, evt => GameEvents.GameStarted());
        }

        private void ShowScores(int correctAnswers, int incorrectAnswers, int totalQuestions)
        {
            m_CorrectScore.text = correctAnswers.ToString();
            m_IncorrectScore.text = incorrectAnswers.ToString();
            m_TotalQuestions.text = totalQuestions.ToString();

            int incompleteScore = totalQuestions - correctAnswers - incorrectAnswers;
            m_IncompleteScore.text = incompleteScore.ToString();
        }

        // Display the end game statistics, including pass/fail status
        private void ShowPassFail(float value)
        {
            m_Accuracy.text = (m_LivesExceeded) ? "INCOMPLETE" : value.ToString("F0") + " % ACCURACY";
            m_PassFail.text = (m_IsWinner) ? "SUCCESS" : "TRY AGAIN";
        }
        #endregion
    }
}
