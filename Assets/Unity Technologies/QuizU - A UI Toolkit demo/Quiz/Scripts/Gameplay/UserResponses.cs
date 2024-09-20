using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quiz
{
    /// <summary>
    /// Class for holding the current user responses and progress. The separate class makes it easier
    /// to store past runthroughs of a quiz.
    /// 
    /// You would also include a reference to the quiz taken, time/date completed, etc. if you wanted a complete
    /// record of the previous attempts. This can help with player statistics, tracking performance, etc.
    ///
    /// </summary>
    public class UserResponses
    {

        string m_QuizTitle;  // Using the Quiz name as a unique ID 
        int m_TotalQuestions;
        int m_MaxIncorrect;     // The most we can get wrong and still pass the Quiz
        int m_AnsweredQuestions;    // How many questions (right or wrong) we have completed
        int m_IncorrectAnswers;    // Current number of incorrectly answered questions
        int m_CorrectAnswers;    // Current number of correctly answered questions
        int m_CurrentStreak;    // Correct answers in succession
        
        int m_CurrentIndex; // The current index of the question being answered

        List<string> m_SelectedResponses; // Text of the selected buttons

        // Properties
        public string QuizID => m_QuizTitle;
        public int TotalQuestions => m_TotalQuestions;
        public int MaxIncorrect => m_MaxIncorrect;

        public int AnsweredQuestions { get => m_AnsweredQuestions; set => m_AnsweredQuestions = value; }
        public int IncorrectAnswers { get => m_IncorrectAnswers; set => m_IncorrectAnswers = value; }
        public int CorrectAnswers { get => m_CorrectAnswers; set => m_CorrectAnswers = value; }
        public int CurrentStreak { get => m_CurrentStreak; set => m_CurrentStreak = value; }
        public int CurrentIndex { get => m_CurrentIndex; set => m_CurrentIndex = value; }

        public List<string> SelectedResponses { get => m_SelectedResponses; set => m_SelectedResponses = value; }

        // Constructor
        public UserResponses(QuizSO quizData)
        { 
            // Copy any necessary data from the ScriptableObject
            m_QuizTitle = quizData.Title;
            m_TotalQuestions = quizData.Questions.Count;
            m_MaxIncorrect = quizData.MaxIncorrect;

            m_AnsweredQuestions = 0;
            m_IncorrectAnswers = 0;
            m_CorrectAnswers = 0;

            m_CurrentStreak = 0;
            CurrentIndex = 0;
            SelectedResponses = new List<string>();
        }
    }
}
