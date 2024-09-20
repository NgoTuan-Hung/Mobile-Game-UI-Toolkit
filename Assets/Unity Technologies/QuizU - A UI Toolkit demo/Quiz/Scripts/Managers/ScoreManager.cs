using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quiz
{
    /// <summary>
    /// This class tracks correct and incorrect answers for the current quiz. 
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {

        [Tooltip("This is the minimum correct answers in a row to create a winning streak")]
        [Range(2, 5)]
        [SerializeField] int m_MinimumStreak = 3;

        // The user's responses and progress (Model data)
        UserResponses m_Responses;

        // Use these properties to retrieve data from the UserResponses
        public int CurrentStreak => Responses.CurrentStreak;
        public bool IsComplete => Responses.TotalQuestions <= Responses.AnsweredQuestions;
        public int IncorrectAnswers => Responses.IncorrectAnswers;
        public int TotalQuestions => Responses.TotalQuestions;
        public int CorrectAnswers => Responses.CorrectAnswers;
        public int MaxIncorrect => Responses.MaxIncorrect;
        public int AnsweredQuestions => Responses.AnsweredQuestions;
        public int LivesLeft => Responses.MaxIncorrect - Responses.IncorrectAnswers;

        public UserResponses Responses => m_Responses;

        // Percentage correct
        public float Accurracy => (float)CorrectAnswers / TotalQuestions;

        // New way - store any data that pertains to game logic in the User Responses
        public void Initialize(QuizSO quizData)
        {
            m_Responses = new UserResponses(quizData);
        }

        // Keep score if we answer the question right or wrong.
        public void OnScoreQuestion(bool isCorrect)
        {
            Responses.AnsweredQuestions++;

            // Answer correctly
            if (isCorrect)
            {
                Responses.CorrectAnswers++;
                Responses.CurrentStreak++;

                // Notify the UI to show the streak label/effect
                if (Responses.CurrentStreak >= m_MinimumStreak)
                {
                    GameEvents.StreakActivated?.Invoke(Responses.CurrentStreak);
                }
            }
            // Answer incorrectly
            else
            {
                // Notify the UI that any previous streak is over
                if (Responses.CurrentStreak >= m_MinimumStreak)
                {
                    GameEvents.StreakSnapped?.Invoke();
                }

                Responses.IncorrectAnswers++;
                ResetStreak();
            }
        }

        private void ResetStreak()
        {
            Responses.CurrentStreak = 0;
        }

        // If the quiz is complete, and player has correctly answered the minimum number
        public bool IsWinner()
        {
            return IsComplete && LivesLeft >= 0;
        }

    }
}
