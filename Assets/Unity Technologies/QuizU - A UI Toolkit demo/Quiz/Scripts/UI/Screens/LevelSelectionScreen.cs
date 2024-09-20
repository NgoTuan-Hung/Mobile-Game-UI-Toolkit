using UnityEngine;
using UnityEngine.UIElements;

namespace Quiz
{
    /// <summary>
    /// Class to manage View logic for the selection screen. This displays relevant
    /// information about one level/quiz in our game. The UI updates when the user
    /// selects different levels/quizzes using the NavigationBar.
    /// </summary>

    public class LevelSelectionScreen : UIScreen
    {
        // Utility for showing a group of buttons
        NavigationBar m_NavigationBar;

        // UI elements
        Label m_Title;
        VisualElement m_Icon;
        Label m_Summary;
        Label m_Objectives;
        Label m_Difficulty;
        Label m_EstimatedTime;
        Button m_BackButton;
        Button m_StartButton;

        public NavigationBar NavigationBar => m_NavigationBar;
        public Button BackButton => m_BackButton;
        public Button StartButton => m_StartButton;

        public LevelSelectionScreen(VisualElement rootElement) : base(rootElement)
        {
            SetVisualElements();
            ResetQuizData();

            LevelSelectionEvents.QuizDataLoaded += LevelSelectionEvents_QuizDataLoaded;
        }

        // Event unsubscriptions
        public override void Disable()
        {
            base.Disable();

            LevelSelectionEvents.QuizDataLoaded -= LevelSelectionEvents_QuizDataLoaded;
        }

        // Event-handling methods

        // Updates the UI data when selecting a new Quiz
        private void LevelSelectionEvents_QuizDataLoaded(QuizSO quizData)
        {
            LoadQuizData(quizData);
        }

        // Methods

        // Get references to UI elements
        private void SetVisualElements()
        {
            m_Title = m_RootElement.Q<Label>("level-data__title");
            m_Icon = m_RootElement.Q<VisualElement>("level-data__icon");
            m_Summary = m_RootElement.Q<Label>("level-data__summary");
            m_Objectives = m_RootElement.Q<Label>("level-data__objectives");
            m_Difficulty = m_RootElement.Q<Label>("level-data__difficulty");
            m_EstimatedTime = m_RootElement.Q<Label>("level-data__estimated-time");

            m_BackButton = m_RootElement.Q<Button>("back-button");
            m_StartButton = m_RootElement.Q<Button>("game__start-button");
        }

        // Clears any placeholder text or images
        private void ResetQuizData()
        {
            m_Title.text = string.Empty;
            m_Summary.text = string.Empty;
            m_Objectives.text = string.Empty;
            m_EstimatedTime.text = string.Empty;
            m_Difficulty.text = string.Empty;
            m_Icon.style.backgroundImage = new StyleBackground((Texture2D)null);
        }

        // Syncs corresponding text and images from a QuizSO ScriptableObject to UI
        private void LoadQuizData(QuizSO quizData)
        {
            m_Title.text = quizData.Title;
            m_Summary.text = quizData.Summary;
            m_Objectives.text = quizData.Objectives;
            m_EstimatedTime.text = "Estimated time: " + quizData.EstimatedTime;
            m_Difficulty.text = "Difficulty: " + quizData.DifficultyLevel;

            // Load the sprite as a StyleBackground Image
            m_Icon.style.backgroundImage = new StyleBackground(quizData.Icon);

        }

        // Initialize the container of buttons used to select levels/quizzes
        public void SetupNavigationBar(int numberOfButtons)
        {
            m_NavigationBar = new NavigationBar();

            // We want to assign the TooltipController for the NavigationBar at the root level
            // of the Hierarchy. Because the m_RootElement of the LevelSelectionScreen is "select__container,"
            // and not actually at the root level, we use the grandparent in this case.
            m_NavigationBar.Initialize(m_RootElement.parent.parent, numberOfButtons);
        }

        // Optional setup for button tooltips
        public void SetupTooltips(string[] tooltips)
        {
            m_NavigationBar.AssignTooltipTextToButtons(tooltips);
        }

    }
}
