using System.Collections.Generic;
using UnityEngine;

namespace Quiz
{
    public enum Category
    {
        UnityEssentials,
        CreativeCore,
        Programming
    }

    [CreateAssetMenu(fileName = "Quiz", menuName = "Quiz/Quiz")]
    public class QuizSO : ScriptableObject
    {
        [Tooltip("Title name shown in LevelSelectionScreen button")]
        [SerializeField] private string m_Title;
        [Tooltip("Description shown on LevelSelectionScreen")]
        [SerializeField] private string m_Summary;
        [Tooltip("What the user will learn")]
        [SerializeField] private List<string> m_Objectives;
        [Tooltip("Sprite used on LevelSelectionScreen")]
        [SerializeField] private Sprite m_Icon;
        [Tooltip("Beginning, intermediate, or advanced")]
        [SerializeField] private string m_DifficultyLevel;
        [Tooltip("Average time for completion")]
        [SerializeField] private string m_EstimatedTime;
        [Tooltip("Minimum correct responses to win")]
        [SerializeField] private int m_CorrectAnswersToPass;
        [Tooltip("Questions that make up the quiz")]
        [SerializeField] private List<QuestionSO> m_Questions;

        public string Title { get => m_Title; set => m_Title = value; }
        public string Summary => m_Summary;
        public string Objectives => string.Join("\n", m_Objectives); // Concatenates individual objectives
        public Sprite Icon => m_Icon;
        public string DifficultyLevel => m_DifficultyLevel;
        public string EstimatedTime => m_EstimatedTime;

        public List<QuestionSO> Questions => m_Questions;
        public int TotalQuestions => Questions.Count;
        public int MaxIncorrect => Questions.Count - m_CorrectAnswersToPass;

        private void OnValidate()
        {
            // Corrected OnValidate to replace asterisk characters with ASCII bullet point characters in each string
            for (int i = 0; i < m_Objectives.Count; i++)
            {
                m_Objectives[i] = m_Objectives[i].Replace('*', '•');
            }
        }
    }
}