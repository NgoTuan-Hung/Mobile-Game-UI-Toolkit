using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quiz
{
    /// <summary>
    /// This is a custom part of the GameScreen that shows the question.
    /// </summary>

    public class QuestionDisplay : IDisposable
    {
        const float k_spaceBetweenParagraphs = 15;  // pixel gap for text elements in the ScrollView

        VisualTreeAsset m_QuestionAsset;

        VisualElement m_Root;
        VisualElement m_QuestionImage;
        VisualElement m_QuestionContainer;

        Label m_QuestionNumber;
        int m_MaxQuestions;

        public QuestionDisplay(VisualElement parentElement)
        {
            Initialize(parentElement);
        }

        public void Initialize(VisualElement parentElement)
        {
            m_Root = parentElement;

            // Get m_QuestionAsset from Resources
            m_QuestionAsset = Resources.Load<VisualTreeAsset>("Uxml/question-text");

            m_QuestionImage = m_Root.Q<VisualElement>("question-panel__image");
            m_QuestionNumber = m_Root.Q<Label>("question-screen__question-number");
            m_QuestionContainer = m_Root.Q<VisualElement>("question-screen__question-container");

            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            UIEvents.GameScreenShown += UIEvents_GameScreenShown;
            GameEvents.TotalQuestionsSetup += GameEvents_TotalQuestionsSetup;
            GameEvents.QuestionUpdated += GameEvents_QuestionUpdated;
            GameEvents.IndexUpdated += GameEvents_IndexUpdated;
        }

        public void Dispose()
        {
            UIEvents.GameScreenShown -= UIEvents_GameScreenShown;

            GameEvents.TotalQuestionsSetup += GameEvents_TotalQuestionsSetup;
            GameEvents.QuestionUpdated -= GameEvents_QuestionUpdated;
            GameEvents.IndexUpdated -= GameEvents_IndexUpdated;
        }

        // Event-handling methods

        // Store the total number of questions
        private void GameEvents_TotalQuestionsSetup(int maxQuestions)
        {
            m_MaxQuestions = maxQuestions;
        }

        // Update the Question text data
        private void GameEvents_QuestionUpdated(QuestionSO questionData)
        {
            ShowQuestionData(questionData);
        }

        // Initialize Question Number
        private void UIEvents_GameScreenShown()
        {
            m_QuestionNumber.text = "1  of " + m_MaxQuestions.ToString();
        }

        // Update the Question number
        private void GameEvents_IndexUpdated(int currentQuestionIndex)
        {
            m_QuestionNumber.text = (currentQuestionIndex + 1).ToString() + " of " + m_MaxQuestions.ToString();
        }


        // Builds the question text
        private void ShowQuestionData(QuestionSO questionData)
        {
            ShowImage(questionData);

            ShowText(questionData);
        }

        private void ShowText(QuestionSO questionData)
        {

            m_QuestionContainer.Clear();

            for (int i = 0; i < questionData.QuestionText.Count; i++)
            {
                QuestionText qText = questionData.QuestionText[i];
                TemplateContainer questionInstance = m_QuestionAsset.Instantiate();

                if (m_QuestionContainer != null)
                    m_QuestionContainer.Add(questionInstance);

                // Add a small gap between different style "paragraphs" (small/medium/large text)
                questionInstance.style.marginBottom = k_spaceBetweenParagraphs;
                questionInstance.style.marginTop = (i > 0) ? k_spaceBetweenParagraphs : 0; // omit top margin for first item

                Label textLabel = questionInstance.Q<Label>("question-screen__question-text");

                if (textLabel != null)
                {
                    // Get other font classes
                    string[] otherFontClasses = TextStyles.GetOtherFontClasses(qText.FontSize);

                    // Remove other font classes from the label
                    foreach (string fontClass in otherFontClasses)
                    {
                        textLabel.RemoveFromClassList(fontClass);
                    }

                    textLabel.text = qText.Text;

                    // Add the desired font class
                    string styleClassName = TextStyles.GetFontClass(qText.FontSize);
                    textLabel.AddToClassList(styleClassName);
                }
            }
        }

        // Shows the optional Image if it's available
        private void ShowImage(QuestionSO questionData)
        {

            // Note: style.display with DisplayStyle.None removes the element from the layout entirely,
            // while setting the visible property to false hides the element without affecting the layout.
            // (e.g. m_QuestionImage.visible = false)

            if (questionData.QuestionImage == null)
            {
                m_QuestionImage.style.display = DisplayStyle.None;
            }
            else
            {
                m_QuestionImage.style.display = DisplayStyle.Flex;
                m_QuestionImage.style.backgroundImage = new StyleBackground(questionData.QuestionImage);
            }
        }

    }
}
