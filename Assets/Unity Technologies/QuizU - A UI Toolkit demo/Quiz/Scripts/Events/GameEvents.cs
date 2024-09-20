using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Quiz
{
    /// <summary>
    /// Public static delegates to manage gameplay (note these are "events" in the conceptual sense
    /// and not the strict C# sense).
    /// </summary>
    public static class GameEvents
    {
        #region Gameplay events

        public static Action<int> IndexUpdated;

        // Refreshed the current Question with new ScriptableObject data
        public static Action<QuestionSO> QuestionUpdated;

        // Selected one or more user selected responses
        public static Action<List<string>> AnswerSelected;

        // Submitted the selected answer
        public static Action AnswerSubmitted;

        // Passes feedback for correct/incorrect responses
        public static Action<string> CorrectlyAnswered;
        public static Action<string> IncorrectlyAnswered;

        // Highlighted the response buttons with correct and incorrect answers
        public static Action<List<string>> CorrectButtonsHighlighted;
        public static Action<List<string>> IncorrectButtonsHighlighted;

        // Continue button has been pressed (delay allows the user to read feedback)
        public static Action QuizContinued;

        // All questions have been answered
        public static Action QuizCompleted;

        #endregion

        #region Game statistics events:

        // Initialize listeners with total number of questions
        public static Action<int> TotalQuestionsSetup;

        // Start a new streak
        public static Action<int> StreakActivated;

        // Break the current streak and reset the message
        public static Action StreakSnapped;

        // Initialize the number of lives
        public static Action<int> LivesSetup;

        // Update the number of guesses left (before failing)
        public static Action<int> LivesUpdated;

        public static Action LivesExceeded;

        #endregion

        #region Game state change events:

        // Start the game
        public static Action GameStarted;

        // Pause the game during gameplay
        public static Action GamePaused;

        // Return to gameplay from the pause screen
        public static Action GameUnpaused;

        // Quit the game while on the pause screen
        public static Action GameAborted;

        // Quiz is complete with enough correct answers
        public static Action GameWon;

        // Quiz is complete but not enough correct answers
        public static Action GameLost;
        
        #endregion

    }
}
