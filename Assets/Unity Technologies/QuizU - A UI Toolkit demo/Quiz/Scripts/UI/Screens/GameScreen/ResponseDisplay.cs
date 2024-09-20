using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using System;
using Random = UnityEngine.Random;


namespace Quiz
{
    /// <summary>
    /// This is a custom part of the GameScreen that manages how the user can respond to
    /// the question through a set of buttons. Some questions allow the user to select
    /// more than one correct answer and some questions shuffle the response buttons.
    /// </summary>
    public class ResponseDisplay : IDisposable
    {
        const int k_MaxResponses = 6; // Max number of choices 

        // USS class names
        const string k_SelectedButtonClassName = "response-button-selected"; // Style Class for selected button


        // String IDs for Visual Elements to remove (used for UI Builder layout)
        List<string> m_Placeholders = new List<string> {
            "question-screen__placeholder-text-large",
            "question-screen__placeholder-text-medium",
            "question-screen__placeholder-text-small",
            "question-screen__response-placeholder"
        };

        VisualElement m_Root;  // Root VisualElement for querying
        List<Button> m_ResponseButtons;  // Two to six buttons used to answer the questions
        Label m_ChoiceText;  // A, B, C, D, etc.
        Label m_InstructionText;    // toggles between "Select one:" or "Selected"

        List<string> m_SelectedAnswers;  // The player's chosen text responses
        List<int> m_SelectedIndices;  //  The player's chosen response (as index numbers)
        bool m_HasMultipleCorrectAnswers;  // Does the current question have more than one correct response?
        List<Answer> m_Answers;  // List of text for each button (with correct/incorrect state)
        ResponseHighlighter m_Highlighter;  // Highlights incorrect and correct responses


        public ResponseDisplay(VisualElement parentElement)
        {
            Initialize(parentElement);
            SubscribeEvents();
        }

        // Event subscriptions
        private void SubscribeEvents()
        {
            GameEvents.QuestionUpdated += GameEvents_QuestionUpdated;

            GameEvents.CorrectlyAnswered += GameEvents_CorrectlyAnswered;
            GameEvents.IncorrectlyAnswered += GameEvents_IncorrectlyAnswered;
            GameEvents.CorrectButtonsHighlighted += GameEvents_CorrectButtonHighlighted;
            GameEvents.IncorrectButtonsHighlighted += GameEvents_IncorrectButtonHighlighted;
        }

        // Event unsubscriptions
        private void UnsubscribeEvents()
        {
            GameEvents.QuestionUpdated -= GameEvents_QuestionUpdated;

            GameEvents.CorrectlyAnswered -= GameEvents_CorrectlyAnswered;
            GameEvents.IncorrectlyAnswered -= GameEvents_IncorrectlyAnswered;
            GameEvents.CorrectButtonsHighlighted -= GameEvents_CorrectButtonHighlighted;
            GameEvents.IncorrectButtonsHighlighted -= GameEvents_IncorrectButtonHighlighted;
        }

        public void Dispose()
        {
            UnsubscribeEvents();
            UnregisterCallbacks();
        }

        // Event handlers

        private void GameEvents_QuestionUpdated(QuestionSO questionData)
        {
            // Copy relevant data that affects the UI
            m_HasMultipleCorrectAnswers = questionData.HasMultipleCorrectAnswers();
            m_Answers = ShuffleAnswers(questionData);
            ResetPanel();
            ResetInstructions();
        }

        // Disable response buttons after Submit but before Continue
        private void GameEvents_CorrectlyAnswered(string s)
        {
            DisableResponseButtons();
        }

        // Disable response buttons after Submit but before Continue
        private void GameEvents_IncorrectlyAnswered(string s)
        {
            DisableResponseButtons();
        }

        // Highlight the correct response buttons
        private void GameEvents_CorrectButtonHighlighted(List<string> correctAnswers)
        {
            m_Highlighter.HighlightAllCorrectAnswers(correctAnswers);
        }

        // Highlight user responses that are incorrect
        private void GameEvents_IncorrectButtonHighlighted(List<string> incorrectAnswers)
        {
            m_Highlighter.HighlightAllIncorrectAnswers(incorrectAnswers);
        }

        // Methods

        // Set up UI elements and buttons
        public void Initialize(VisualElement parentElement)
        {
            // The controller UIScreen passes in the topmost element of the Hierarchy
            m_Root = parentElement;

            HidePlaceholders();
            SetVisualElements();
            RegisterCallbacks();
            SubscribeEvents();

            m_Highlighter = new ResponseHighlighter(m_ResponseButtons);
        }

        // Hide placeholders UI elements used only for layout
        private void HidePlaceholders()
        {
            foreach (string placeholder in m_Placeholders)
            {
                m_Root.Query<VisualElement>(placeholder).
                    ForEach((VisualElement x) => x.style.display = DisplayStyle.None);
            }
        }

        // Cache references to Visual Elements
        private void SetVisualElements()
        {
            // Use UQueryBuilder to get all Buttons that start with the k_ResponseBaseName,
            // then convert to a list and store it as m_ResponseButtons
            m_ResponseButtons = m_Root.Query<Button>().
                Where(x => x.name.StartsWith("question-screen__response-")).
                ToList();

            m_ChoiceText = m_Root.Query<Label>("question-screen__choice-text");
            m_InstructionText = m_Root.Query<Label>("question-screen__instruction-text");
        }

        // Set up callback action for Buttons
        private void RegisterCallbacks()
        {
            // Use the RegisterCallback here so we can pass the ClickEvent with extra information to callback
            foreach (Button button in m_ResponseButtons)
            {
                if (button != null)
                {
                    button.RegisterCallback<ClickEvent>(ResponseButtonClicked);
                }
            }
        }

        private void UnregisterCallbacks()
        {
            foreach (Button button in m_ResponseButtons)
            {
                if (button != null)
                {
                    button.UnregisterCallback<ClickEvent>(ResponseButtonClicked);
                }
            }
        }

        // Handle click event on a response button
        private void ResponseButtonClicked(ClickEvent evt)
        {
            Button clickedButton = evt.target as Button;

            // If the question has more than one correct answer, toggle the selection
            if (m_HasMultipleCorrectAnswers)
            {
                ToggleButton(clickedButton);
            }
            // Otherwise, clear previous selections and only select one button
            else
            {
                SelectSingleButton(clickedButton);
            }

            // Convert m_SelectedIndices to A, B, C, etc.
            ShowChoices();

            // Convert m_SelectedAnswers from m_SelectedIndices
            m_SelectedAnswers = ConvertSelectionsToAnswers(m_SelectedIndices);

            // Send selected answers to QuizController
            if (m_SelectedIndices.Count > 0)
                GameEvents.AnswerSelected?.Invoke(new List<string>(m_SelectedAnswers));

        }

        // Update the UI with the new question data
        private void ResetPanel()
        {
            m_SelectedAnswers = new List<string>();
            m_SelectedIndices = new List<int>();

            ShowAnswers(m_Answers);
        }

        private void ResetInstructions()
        {
            m_InstructionText.text = (m_HasMultipleCorrectAnswers) ? "SELECT ALL:" : "SELECT ONE:";
            m_ChoiceText.text = string.Empty;
        }

        // Show the answers for the current question and hides unused buttons.
        private void ShowAnswers(List<Answer> shuffledAnswers)
        {
            ClearSelection();
            m_Highlighter.ClearHighlight();

            EnableResponseButtons(true);

            for (int i = 0; i < k_MaxResponses; i++)
            {
                Button response = m_ResponseButtons[i];

                if (i >= shuffledAnswers.Count)
                {
                    response.style.display = DisplayStyle.None;
                }
                else
                {
                    response.style.display = DisplayStyle.Flex;
                    response.text = shuffledAnswers[i].Text;
                }
            }
        }

        // Shuffles temporary copy of the answers for the current question, enabled for certain questions
        private List<Answer> ShuffleAnswers(QuestionSO questionData)
        {
            List<Answer> shuffledAnswers = new List<Answer>(questionData.Answers);

            // Shuffle only if flagged
            if (questionData.ShuffleAnswers)
            {

                for (int i = shuffledAnswers.Count - 1; i > 0; i--)
                {
                    int j = Random.Range(0, i + 1);

                    Answer temp = shuffledAnswers[i];
                    shuffledAnswers[i] = shuffledAnswers[j];
                    shuffledAnswers[j] = temp;
                }
            }

            return shuffledAnswers;
        }

        // Enable or disable response buttons
        private void EnableResponseButtons(bool state)
        {
            foreach (Button button in m_ResponseButtons)
            {
                button.SetEnabled(state);
            }
        }

        // Set response buttons so they cannot be clicked (after the user presses Submit
        // but before clicking Continue)
        private void DisableResponseButtons()
        {
            EnableResponseButtons(false);
        }

        // Clears previous selection and selects one response 
        private void SelectSingleButton(Button clickedButton)
        {
            ClearSelection();

            clickedButton.AddToClassList(k_SelectedButtonClassName);

            // Set the m_SelectedIndices to a single element with the selected index
            int clickedButtonIndex = m_ResponseButtons.IndexOf(clickedButton);
            m_SelectedIndices.Clear();
            m_SelectedIndices.Add(clickedButtonIndex);
        }

        // For questions with multiple correct answers, allows the user to toggle button selection
        private void ToggleButton(Button clickedButton)
        {
            // Record the index of the clicked button
            int clickedButtonIndex = m_ResponseButtons.IndexOf(clickedButton);


            // If the button is already selected, deselect it
            if (clickedButton.ClassListContains(k_SelectedButtonClassName))
            {
                clickedButton.RemoveFromClassList(k_SelectedButtonClassName);
                m_SelectedAnswers.Remove(clickedButton.text);
                m_SelectedIndices.Remove(clickedButtonIndex);
            }
            else
            {
                clickedButton.AddToClassList(k_SelectedButtonClassName);
                m_SelectedAnswers.Add(clickedButton.text);
                m_SelectedIndices.Add(clickedButtonIndex);
            }

            // If we have toggled off all responses, notify the MessageBar to disable the Submit button
            if (m_SelectedIndices.Count == 0)
            {
                UIEvents.ResponsesDeselected?.Invoke();
            }
        }

        // Deselect all buttons
        private void ClearSelection()
        {
            m_SelectedAnswers.Clear();

            List<Button> selectedButtons = m_Root.Query<Button>().
                Where(x => x.ClassListContains(k_SelectedButtonClassName)).ToList();

            foreach (Button button in selectedButtons)
            {
                button.RemoveFromClassList(k_SelectedButtonClassName);
            }
        }

        // Convert the button index numbers to a List of corresponding answers
        private List<string> ConvertSelectionsToAnswers(List<int> indices)
        {
            // Initialize a list to hold the converted answers
            List<string> answers = new List<string>();

            // Loop through the selected indices
            foreach (int index in indices)
            {
                // Convert the current index to an answer and add it to the list
                answers.Add(m_ResponseButtons[index].text);
            }

            // Return the list of converted answers
            return answers;
        }


        // Handles the Selected label at the top of the panel ("Selected: A, B, C, ...")
        private void ShowChoices()
        {

            m_ChoiceText.text = ConvertSelectionToChoiceText(m_SelectedIndices);

            // If we toggle off all selections, reset the panel
            if (m_SelectedIndices.Count == 0)
            {
                ResetInstructions();
            }
            else
            {
                m_InstructionText.text = "SELECTED:";
            }
        }

        // Converts selected button numbers to a string of letters
        private string ConvertSelectionToChoiceText(List<int> buttonIndices)
        {
            // Remove duplicates and sort the list
            var sortedIndices = buttonIndices.Distinct().OrderBy(i => i).ToList();

            // Initialize a list to hold the converted letters
            List<string> letters = new List<string>();

            // Loop through the sorted indices
            for (int i = 0; i < sortedIndices.Count; i++)
            {
                // Convert the current index to a letter and add it to the list
                letters.Add(sortedIndices[i].ConvertToLetter().ToString());
            }

            // Join the letters with a comma and a space, and return the result
            return string.Join(", ", letters);
        }


    }
}
