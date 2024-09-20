
using UnityEngine;
using System;
using System.Linq;

namespace Quiz
{
    /// <summary>
    /// This is an example of Model-View-Presenter design pattern.
    ///
    /// The LevelSelector is the Presenter, a layer of management that facilitates communication
    /// between the QuizSO ScriptableObject data (Model) and the LevelSelectionScreen user-interface (View).
    /// This separates the UI from the data to improve testing and maintenance.
    /// </summary>
    /// 
    [RequireComponent(typeof(LevelSelectionScreen))]
    public class LevelSelectionPresenter
    {
        [Tooltip("Available quizzes in project; the data (Model)")]
        [SerializeField] QuizSO[] m_Quizzes;

        LevelSelectionScreen m_LevelSelectionScreen;

        public LevelSelectionPresenter(LevelSelectionScreen levelSelectionScreen)
        {
            m_LevelSelectionScreen = levelSelectionScreen ?? throw new ArgumentNullException(nameof(levelSelectionScreen));

            LevelSelectionEvents.ButtonSelected += LevelSelectionEvents_ButtonSelected;
            Initialize();
        }

        // Event unsubscriptions
        public void Disable()
        {
            LevelSelectionEvents.ButtonSelected -= LevelSelectionEvents_ButtonSelected;
        }

        // Event-handling methods

        // Load the Quiz ScriptableObject data to update the LevelSelectionScreen and then highlight
        // the selected button in the NavigationBar
        private void LevelSelectionEvents_ButtonSelected(int selectedIndex)
        {
            if (selectedIndex >= 0 && selectedIndex < m_Quizzes.Length)
            {
                LevelSelectionEvents.QuizDataLoaded?.Invoke(m_Quizzes[selectedIndex]);
                m_LevelSelectionScreen.NavigationBar.HighlightButton(selectedIndex);
            }
        }

        // Methods

        // Sets up the LevelSelectionScreen and NavigationBar. Assigns actions to each
        // button n the NavigationBar.
        private void Initialize()
        {
            // Load the ScriptableObjects from the project
            m_Quizzes = Resources.LoadAll<QuizSO>("Quizzes");

            m_LevelSelectionScreen.SetupNavigationBar(m_Quizzes.Length);

            // Assign tooltips from quiz titles
            string[] quizTitles = m_Quizzes.Select(quiz => quiz.Title).ToArray();
            m_LevelSelectionScreen.SetupTooltips(quizTitles);

            RegisterCallbacks();

            OnLoadFirstQuiz();
        }

        private void RegisterCallbacks()
        {
            // An alternate way to register callbacks, uses the Clickable Manipulator's clicked property for convenience.
            // You can also use the m_LevelSelectionScreen's EventRegistry.

            m_LevelSelectionScreen.BackButton.clicked += () => UIEvents.ScreenClosed?.Invoke();
            m_LevelSelectionScreen.StartButton.clicked += () => GameEvents.GameStarted?.Invoke();

            // Assign ClickEvent handlers for each button in the navigation bar
            for (int i = 0; i < m_Quizzes.Length; i++)
            {
                int index = i;

                // Raises the static LevelSelectionEvents with the button index

                m_LevelSelectionScreen.NavigationBar.Buttons[index].clicked += () => LevelSelectionEvents.ButtonSelected(index);
            }
        }

        // Load the first quiz and highlight first button
        private void OnLoadFirstQuiz()
        {
            if (m_Quizzes[0] != null)
            {
                LevelSelectionEvents.QuizDataLoaded?.Invoke(m_Quizzes[0]);
                m_LevelSelectionScreen.NavigationBar.HighlightButton(0);
            }
        }
    }
}
