using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Quiz
{
    /// <summary>
    /// A struct pairing an answer text string with a bool (correct or incorrect).
    /// </summary>
    [System.Serializable]
    public struct Answer
    {
        public bool IsCorrect;
        [TextArea(3, 5)]
     
        public string Text;
    }

    /// <summary>
    /// A struct pairing a string with a font size enum.
    /// </summary>
    [System.Serializable]
    public struct QuestionText
    {
        public QuestionFontSize FontSize;
        [TextArea(5, 10)]
        public string Text;
    }
    /// <summary>
    /// This class defines the data used for one question of the Quiz.
    /// </summary>
    [CreateAssetMenu(fileName = "Question", menuName = "Quiz/Question")]
    public class QuestionSO : ScriptableObject
    {
        [Tooltip("List of question text, supporting different font sizes.")]
        [SerializeField] List<QuestionText> m_QuestionText;
        [Tooltip("Optional image associated with the question.")]
        [SerializeField] [Optional] Sprite m_QuestionImage;
        [TextArea(3, 5)]
        [Tooltip("Skills tested or used in the question.")]
        [SerializeField] string m_Skills;
        [Tooltip("Should the answers should be shuffled?")]
        [SerializeField] bool m_ShuffleAnswers;
        [Tooltip("Additional information about the correct/incorrect responses")]
        [SerializeField] List<Answer> m_Answers;
        [TextArea(3, 5)]
        [Tooltip("Additional information provided when the correct answer is chosen.")]
        [SerializeField] string m_FeedbackCorrect;
        [TextArea(3, 5)]
        [Tooltip("Additional information provided when an incorrect answer is chosen.")]
        [SerializeField] string m_FeedbackIncorrect;
        [Tooltip("Optional link to the documentation for more information.")]
        [SerializeField][Optional] string m_FeedbackLink;

        public Sprite QuestionImage => m_QuestionImage;
        public string Skills { get => m_Skills; set => m_Skills = value; }
        public List<QuestionText> QuestionText { get => m_QuestionText; set => m_QuestionText = value; }
        public string FeedbackCorrect { get => m_FeedbackCorrect; set => m_FeedbackCorrect = value; }
        public string FeedbackIncorrect { get => m_FeedbackIncorrect; set => m_FeedbackIncorrect = value; }
        public List<Answer> Answers { get => m_Answers; set => m_Answers = value; }
        public bool ShuffleAnswers { get => m_ShuffleAnswers; set => m_ShuffleAnswers = value; }

        // Use a HashSet to compare the user provided answers to the correct answers
        public bool AreAnswersCorrect(List<string> providedAnswers)
        {
            // Create HashSets for correct answers and provided answers
            HashSet<string> correctAnswers = new HashSet<string>();
            HashSet<string> userAnswers = new HashSet<string>(providedAnswers);

            // Populate correctAnswers HashSet
            foreach (var answer in m_Answers)
            {
                if (answer.IsCorrect)
                {
                    correctAnswers.Add(answer.Text);
                }
            }

            // Compare HashSets
            return correctAnswers.SetEquals(userAnswers);
        }

        // Returns true if there are multiple correct answers
        public bool HasMultipleCorrectAnswers()
        {
            int correctAnswerCount = 0;

            foreach (Answer answer in m_Answers)
            {
                if (answer.IsCorrect)
                {
                    correctAnswerCount++;
                }

                if (correctAnswerCount > 1)
                {
                    return true;
                }
            }

            return false;
        }

        // Returns a list of correct answers
        public List<string> GetCorrectAnswers()
        {
            List<string> correctAnswers = new List<string>();

            foreach (Answer answer in m_Answers)
            {
                if (answer.IsCorrect)
                {
                    correctAnswers.Add(answer.Text);
                }
            }

            return correctAnswers;
        }

        // Returns a list of incorrect answers
        public List<string> GetIncorrectAnswers()
        {
            List<string> incorrectAnswers = new List<string>();

            foreach (Answer answer in m_Answers)
            {
                if (!answer.IsCorrect)
                {
                    incorrectAnswers.Add(answer.Text);
                }
            }

            return incorrectAnswers;
        }

    }
}
