using UnityEditor;
using UnityEngine.UIElements;
using Quiz;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Quiz
{
    /// <summary>
    /// Temporary struct to hold JSON data for conversion. This allows the JSON file
    /// to have fields that more human readable (without prefixes/capitalization)
    /// </summary>
    [System.Serializable]
    public struct QuestionJsonData
    {
        // The text of the question.
        public string text;

        // The image
        public Sprite image;

        // The skills associated with the question.
        public string skills;

        // If true, shuffle the options when displaying the question.
        public bool shuffleOptions;

        // A list of possible answer options.
        public List<AnswerJsonData> options;

        // Feedback text shown to the user when they answer correctly.
        public string feedbackCorrect;

        // Feedback text shown to the user when they answer incorrectly.
        public string feedbackIncorrect;

        /// <summary>
        /// Temporary storage for answer data loaded from JSON.
        /// </summary>
        [System.Serializable]
        public struct AnswerJsonData
        {
            // The text of the answer option.
            public string text;

            // Indicates whether the answer is correct or not.
            public bool isCorrect;
        }
    }

    // Custom editor for the QuestionSO scriptable object.
    [CustomEditor(typeof(QuestionSO))]
    public class QuestionSOEditor : Editor
    {
        Button importJsonButton;

        // Creates the custom inspector UI.
        public override VisualElement CreateInspectorGUI()
        {
            // Create the root visual element.
            VisualElement root = new VisualElement();

            // Add the default inspector UI.
            IMGUIContainer defaultInspector = new IMGUIContainer(() => DrawDefaultInspector());
            root.Add(defaultInspector);

            // Create and add the Import JSON button.
            importJsonButton = new Button(ImportJson)
            {
                text = "Import JSON"
            };
            root.Add(importJsonButton);

            return root;
        }

        // Imports question data from a JSON file.
        private void ImportJson()
        {
            string path = EditorUtility.OpenFilePanel("Import JSON", "", "json");

            if (!string.IsNullOrEmpty(path))
            {
                string jsonData = File.ReadAllText(path);
                PopulateQuestionDataFromJson(jsonData);
            }
        }

        // Populates the QuestionSO with data from the JSON string.
        private void PopulateQuestionDataFromJson(string jsonData)
        {
            QuestionSO questionSO = (QuestionSO)target;

            // Converts to temporary format (with JSON names)
            QuestionJsonData questionJsonData = JsonUtility.FromJson<QuestionJsonData>(jsonData);

            // Convert from temporary format to ScriptableObject
            PopulateQuestionSOFromJsonData(questionSO, questionJsonData);

            // Force refresh
            EditorUtility.SetDirty(questionSO);
        }

        // Populates the QuestionSO ScriptableObject with the temporary data.
        public static void PopulateQuestionSOFromJsonData(QuestionSO questionSO, QuestionJsonData questionJsonData)
        {
            questionSO.QuestionText = new List<QuestionText> { new QuestionText { FontSize = QuestionFontSize.Medium, Text = questionJsonData.text } };
            questionSO.Skills = questionJsonData.skills;
            questionSO.ShuffleAnswers = questionJsonData.shuffleOptions;
            questionSO.Answers = questionJsonData.options.ConvertAll(option => new Answer { Text = option.text, IsCorrect = option.isCorrect });
            questionSO.FeedbackCorrect = questionJsonData.feedbackCorrect;
            questionSO.FeedbackIncorrect = questionJsonData.feedbackIncorrect;
        }
    }

}
