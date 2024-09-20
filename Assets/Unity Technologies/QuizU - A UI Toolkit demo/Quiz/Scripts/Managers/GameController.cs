using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Quiz
{
    /// <summary>
    /// A MonoBehaviour that controls the main game mechanic and handles the quiz game flow.
    /// This functions as the game manager or game controller in addition to handling the
    /// Presenter duties for the GameScreen.
    /// </summary>
    [RequireComponent(typeof(ScoreManager))]
    public class GameController : MonoBehaviour
    {
        [Tooltip("Required ScoreManager component")]
        [SerializeField] ScoreManager m_ScoreManager;
        [Tooltip("Log debug messages at the console")]
        [SerializeField] bool m_Debug;

        LevelSelectionPresenter m_LevelSelectionPresenter;
        // The current active question
        QuestionSO m_ActiveQuestion;

        // ScriptableObject that holds the questions and quiz data (Model data)"
        QuizSO m_QuizData;

        public QuizSO QuizData => m_QuizData;  // The currently active quiz

        // Set up event listeners for game events
        private void OnEnable()
        {
            GameEvents.GameStarted += GameEvents_GameStarted;
            GameEvents.QuizContinued += GameEvents_QuizContinued;
            GameEvents.AnswerSelected += GameEvents_AnswerSelected;
            GameEvents.AnswerSubmitted += GameEvents_AnswerSubmitted;

            LevelSelectionEvents.Initialized += LevelSelectionsEvents_Initialized;
            LevelSelectionEvents.QuizDataLoaded += LevelSelectionEvents_QuizDataLoaded;
        }

        // Remove the event listeners
        private void OnDisable()
        {
            GameEvents.GameStarted -= GameEvents_GameStarted;
            GameEvents.QuizContinued -= GameEvents_QuizContinued;
            GameEvents.AnswerSelected -= GameEvents_AnswerSelected;
            GameEvents.AnswerSubmitted -= GameEvents_AnswerSubmitted;

            LevelSelectionEvents.Initialized -= LevelSelectionsEvents_Initialized;
            LevelSelectionEvents.QuizDataLoaded -= LevelSelectionEvents_QuizDataLoaded;

            m_LevelSelectionPresenter.Disable();
        }

        private void Awake()
        {
            // Log an error if any required fields in the Inspector are blank
            NullRefChecker.Validate(this);

            if (m_ScoreManager == null)
                m_ScoreManager = GetComponent<ScoreManager>();
        }

        // Event-handling methods

        // Start the game
        private void GameEvents_GameStarted()
        {
            OnGameStarted();
        }

        // Increment the index of the current question when the user selects an answer
        private void GameEvents_QuizContinued()
        {
            m_ScoreManager.Responses.CurrentIndex++;

            // If we've reached the end of the Quiz, send an event
            if (m_ScoreManager.Responses.CurrentIndex >= m_QuizData.Questions.Count)
            {
                OnGameOver();
                return;
            }

            // Set the active question to the next question in the QuizSO
            m_ActiveQuestion = m_QuizData.Questions[m_ScoreManager.Responses.CurrentIndex];

            // Trigger a game event to update the UI
            GameEvents.QuestionUpdated?.Invoke(m_ActiveQuestion);
            GameEvents.IndexUpdated?.Invoke(m_ScoreManager.Responses.CurrentIndex);
        }

        // Update the user's selected response when an answer is selected
        private void GameEvents_AnswerSelected(List<string> response)
        {
            m_ScoreManager.Responses.SelectedResponses = response;
        }

        // Process the user's response and trigger appropriate game events
        private void GameEvents_AnswerSubmitted()
        {
            if (m_ActiveQuestion == null)
            {
                Debug.LogWarning("QuizController: Missing active question", transform);
                return;
            }

            // Check if the user's response is correct or incorrect
            if (m_ActiveQuestion.AreAnswersCorrect(m_ScoreManager.Responses.SelectedResponses))
            {
                // Increment the score and trigger the correct answer event
                m_ScoreManager.OnScoreQuestion(true);

                GameEvents.CorrectlyAnswered?.Invoke(m_ActiveQuestion.FeedbackCorrect);
            }
            else
            {
                // Trigger the incorrect answer event, and update the lives
                m_ScoreManager.OnScoreQuestion(false);
                GameEvents.IncorrectlyAnswered?.Invoke(m_ActiveQuestion.FeedbackIncorrect);

                // Highlight user responses that are incorrect
                List<string> allIncorrect = m_ActiveQuestion.GetIncorrectAnswers();
                List<string> userIncorrect = allIncorrect.Intersect(m_ScoreManager.Responses.SelectedResponses).ToList();

                GameEvents.IncorrectButtonsHighlighted?.Invoke(userIncorrect);

                // If we still have lives left, notify the LifeBar
                if (m_ScoreManager.LivesLeft >= 0)
                    GameEvents.LivesUpdated?.Invoke(m_ScoreManager.LivesLeft);
            }

            GameEvents.CorrectButtonsHighlighted?.Invoke(m_ActiveQuestion.GetCorrectAnswers());

            // Update the progress bar
            OnUpdateProgress();
        }

        // Handle quiz selection
        private void LevelSelectionEvents_QuizDataLoaded(QuizSO selectedQuiz)
        {
            m_QuizData = selectedQuiz;
        }

        // Wait for the UI setup process, then create the LevelSelectionPresenter
        private void LevelSelectionsEvents_Initialized(LevelSelectionScreen levelSelectionScreen)
        {
            m_LevelSelectionPresenter = new LevelSelectionPresenter(levelSelectionScreen);
        }

        // Methods

        // Raises events when starting up
        private void OnGameStarted()
        {
            // Initialize the quiz data
            m_ScoreManager.Initialize(m_QuizData);

            // Set the active question to the first question in the QuizSO
            m_ActiveQuestion = m_QuizData.Questions[m_ScoreManager.Responses.CurrentIndex];

            // Raise the appropriate game and UI events
            GameEvents.QuestionUpdated?.Invoke(m_ActiveQuestion);
            GameEvents.LivesSetup?.Invoke(m_ScoreManager.LivesLeft);
            GameEvents.TotalQuestionsSetup?.Invoke(m_ScoreManager.TotalQuestions);

            UIEvents.GameScreenShown?.Invoke();
            UIEvents.ProgressBarUpdated?.Invoke(0f);
        }

        // Raises an event with the quiz completion percentage.
        private void OnUpdateProgress()
        {
            // Update the Progress Meter
            int maxQuestions = m_ScoreManager.TotalQuestions;
            int completedQuestions = m_ScoreManager.AnsweredQuestions;
            float progress = (float)completedQuestions / maxQuestions;

            UIEvents.ProgressBarUpdated?.Invoke(progress * 100f);

            // Check for win/lose condition
            OnGameOver();
        }

        // Raise events when the quiz is complete
        private void OnGameOver()
        {
            // Check if the quiz is over
            if (IsQuizComplete())
            {
                GameEvents.QuizCompleted?.Invoke();

                // Raise win/lose events
                if (m_ScoreManager.IsWinner())
                {
                    GameEvents.GameWon?.Invoke();
                }
                else
                {
                    GameEvents.GameLost?.Invoke();
                }

                // Send quiz name, accuracy score, and pass/fail message via UIEvents
                UIEvents.QuizTitleShown?.Invoke(m_QuizData.Title);
                UIEvents.AccuracyCalculated?.Invoke(m_ScoreManager.Accurracy * 100);
                UIEvents.ScoresTotaled?.Invoke(m_ScoreManager.CorrectAnswers, m_ScoreManager.IncorrectAnswers,
                    m_ScoreManager.TotalQuestions);

            }
        }
        
        // Check if the quiz is over (reached last question or run out of lives)
        private bool IsQuizComplete()
        {
            // Have we reached the last question?
            if (m_ScoreManager.Responses.CurrentIndex == m_QuizData.Questions.Count - 1)
            {
                return true;
            }

            // Have we used the last life?
            if (m_ScoreManager.LivesLeft == 0)
            {
                GameEvents.LivesExceeded?.Invoke();
                return true;
            }

            return false;
        }
    }
}
