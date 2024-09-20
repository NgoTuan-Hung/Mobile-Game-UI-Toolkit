using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quiz
{
    /// <summary>
    /// This is a custom section of the GameScreen that handles submitting answers and
    /// advancing to the next question. Located the bottom of the screen, it contains
    /// the Submit and Continue buttons. When the user submits the answer, it shows feedback
    /// for each question.
    /// </summary>
    [RequireComponent(typeof(GameScreen))]
    public class MessageDisplay : IDisposable
    {

        // USS Class Styles
        const string k_ResultActiveClassName = "result-text-active";
        const string k_ResultInactiveClassName = "result-text-inactive";
        const string k_FeedbackCorrectClassName = "feedback-correct";
        const string k_FeedbackIncorrectClassName = "feedback-incorrect";
        const string k_FeedbackInactiveClassName = "feedback-inactive";

        // Visual elements
        VisualElement m_Root;
        Button m_SubmitButton;
        Button m_ContinueButton;
        Label m_ResultText;
        VisualElement m_FeedbackPanel;
        Label m_FeedbackText;

        bool m_ContinueToEndScreen; // flag to show correct UI

        public MessageDisplay(VisualElement parentElement)
        {
            Initialize(parentElement);
            SubscribeToEvents();
        }

        // Event subscriptions
        private void SubscribeToEvents()
        {
            UIEvents.GameScreenShown += UIEvents_GameScreenShown;

            GameEvents.AnswerSelected += GameEvents_AnswerSelected;
            GameEvents.QuestionUpdated += GameEvents_QuestionUpdated;
            GameEvents.CorrectlyAnswered += GameEvents_CorrectlyAnswered;
            GameEvents.IncorrectlyAnswered += GameEvents_IncorrectlyAnswered;
            GameEvents.QuizCompleted += GameEvents_QuizCompleted;

            UIEvents.ResponsesDeselected += UIEvents_ResponsesDeselected;
        }

        // Unsubscribes all the event handlers to prevent memory leaks and errors if
        // an event gets invoked while the class is already disposed. 
        private void UnsubscribeFromEvents()
        {
            UIEvents.GameScreenShown -= UIEvents_GameScreenShown;

            GameEvents.AnswerSelected -= GameEvents_AnswerSelected;
            GameEvents.QuestionUpdated -= GameEvents_QuestionUpdated;
            GameEvents.CorrectlyAnswered -= GameEvents_CorrectlyAnswered;
            GameEvents.IncorrectlyAnswered -= GameEvents_IncorrectlyAnswered;
            GameEvents.QuizCompleted -= GameEvents_QuizCompleted;

            UIEvents.ResponsesDeselected -= UIEvents_ResponsesDeselected;
        }

        // Event-handlers

        // Show the game screen / first question
        private void UIEvents_GameScreenShown()
        {
            Reset();
        }

        // Enables the Submit button when an answer is selected
        private void GameEvents_AnswerSelected(List<string> selection)
        {
            EnableSubmitButton(true);
        }

        // If the user deselects all responses, disable the Submit button
        private void UIEvents_ResponsesDeselected()
        {
            EnableSubmitButton(false);
        }
        private void GameEvents_QuizCompleted()
        {
            m_ContinueToEndScreen = true;
        }

        // Methods

        // Sets up any necessary dependencies from the MessageBar
        public void Initialize(VisualElement parentElement)
        {

            m_Root = parentElement;
            SetVisualElements();
            RegisterCallbacks();
        }

        // Query for UI elements
        private void SetVisualElements()
        {
            m_SubmitButton = m_Root.Q<Button>("question-screen__check-button");
            m_ResultText = m_Root.Q<Label>("question-screen__result-text");
            m_ContinueButton = m_Root.Q<Button>("question-screen__continue-button");

            m_FeedbackPanel = m_Root.Q<VisualElement>("question-screen__feedback-panel");
            m_FeedbackText = m_Root.Q<Label>("question-screen__feedback-text");
        }

        // Register button click callbacks
        private void RegisterCallbacks()
        {
            m_SubmitButton.RegisterCallback<ClickEvent>(SubmitButtonClickHandler);
            m_ContinueButton.RegisterCallback<ClickEvent>(ContinueButtonClickHandler);

            //  Alternatively, you can register a callback through its Clickable Manipulator

            //m_SubmitButton.clicked += SubmitButtonClickHandler;
            //m_ContinueButton.clicked += ContinueButtonClickHandler;
        }

        public void Dispose()
        {

            UnsubscribeFromEvents();

            // Unregisters the click events from the Submit and Continue buttons.
            // In this instance, the MessageDisplay will dispose along with the GameScreen, so 
            // it's unnecessary here. However, in general, it's important to unregister any callbacks
            // when the object gets disposed to prevent possible issues.

            m_SubmitButton.UnregisterCallback<ClickEvent>(SubmitButtonClickHandler);
            m_ContinueButton.UnregisterCallback<ClickEvent>(ContinueButtonClickHandler);
        }

        private void SubmitButtonClickHandler(ClickEvent evt)
        {
            GameEvents.AnswerSubmitted?.Invoke();
            EnableSubmitButton(false);
        }

        private void ContinueButtonClickHandler(ClickEvent evt)
        {
            // If still playing the game
            if (!m_ContinueToEndScreen)
            {
                GameEvents.QuizContinued?.Invoke();
                return;
            }

            // Game over
            UIEvents.EndScreenShown?.Invoke();
        }

        // Reset message bar to show the next question
        private void GameEvents_QuestionUpdated(QuestionSO questionData)
        {
            // No more questions, only enable the Continue button
            if (m_ContinueToEndScreen)
            {
                EnableContinueButton(true);
            }
            else
            {
                Reset();
            }
        }

        // Show the results in the text panel in footer
        private void GameEvents_IncorrectlyAnswered(string message)
        {
            ShowResultText("Incorrect");
            ShowFeedback(false, message);
        }

        // Show the results in the text panel in footer
        private void GameEvents_CorrectlyAnswered(string message)
        {
            ShowResultText("Correct");
            ShowFeedback(true, message);
        }

        // Methods

        // Enables or disables the Submit button
        private void EnableSubmitButton(bool state)
        {
            m_SubmitButton.SetEnabled(state);
            m_SubmitButton.focusable = state;
            m_SubmitButton.MarkDirtyRepaint();
        }

        // Enables or disables the continue button
        private void EnableContinueButton(bool state)
        {
            m_ContinueButton.SetEnabled(state);
            m_ContinueButton.focusable = state;

            m_ContinueButton.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;
        }

        // Show Incorrect or Correct text
        private void ShowResultText(string text)
        {
            m_ResultText.AddToClassList(k_ResultActiveClassName);
            m_ResultText.RemoveFromClassList(k_ResultInactiveClassName);
            m_ResultText.text = text;
        }

        // Shows feedback text message with correct/incorrect styling
        private void ShowFeedback(bool isCorrect, string message)
        {
            m_FeedbackPanel.ClearClassList();
            m_FeedbackText.ClearClassList();
            EnableContinueButton(true);

            if (isCorrect)
            {
                m_FeedbackPanel.AddToClassList(k_FeedbackCorrectClassName);
                m_FeedbackText.AddToClassList(k_FeedbackCorrectClassName);
            }
            else
            {
                m_FeedbackPanel.AddToClassList(k_FeedbackIncorrectClassName);
                m_FeedbackText.AddToClassList(k_FeedbackIncorrectClassName);
            }

            m_FeedbackText.text = message;
        }

        // Hide Incorrect or Correct text
        private void HideResultText()
        {
            m_ResultText.AddToClassList(k_ResultInactiveClassName);
            m_ResultText.RemoveFromClassList(k_ResultActiveClassName);
        }

        // Clears the feedback styling from previous question.
        private void HideFeedback()
        {

            m_FeedbackPanel.AddToClassList(k_FeedbackInactiveClassName);
            m_FeedbackText.AddToClassList(k_FeedbackInactiveClassName);
        }

        // Sets up UI elements for a new question
        private void Reset()
        {
            m_ContinueToEndScreen = false;

            EnableSubmitButton(false);
            EnableContinueButton(false);
            HideFeedback();
            HideResultText();
        }
    }
}
