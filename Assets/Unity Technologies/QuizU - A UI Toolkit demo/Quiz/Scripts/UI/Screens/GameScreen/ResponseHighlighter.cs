using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quiz
{
    /// <summary>
    /// Highlights the response buttons when the user makes selections.
    /// </summary>
    public class ResponseHighlighter
    {
        const string k_ResponseHighlightedClassName = "response-highlighted"; // class for outlined button
        const string k_ResponseIncorrectClassName = "response-incorrect"; // class for mistakes

        List<Button> m_ResponseButtons;

        public ResponseHighlighter(List<Button> responseButtons)
        {
            m_ResponseButtons = responseButtons;
        }

        // Remove the previous highlighted response button.
        public void ClearHighlight()
        {
            foreach (Button button in m_ResponseButtons)
            {
                button.RemoveFromClassList(k_ResponseHighlightedClassName);
                button.RemoveFromClassList(k_ResponseIncorrectClassName);
            }
        }

        // Highlight all correct response buttons; this checks the string in the
        // event the response buttons are shuffled
        public void HighlightAllCorrectAnswers(List<string> correctAnswers)
        {
            for (int i = 0; i < m_ResponseButtons.Count; i++)
            {
                if (correctAnswers.Contains(m_ResponseButtons[i].text))
                {
                    m_ResponseButtons[i].AddToClassList(k_ResponseHighlightedClassName);
                }
            }
        }

        // Highlight all correct response buttons.
        public void HighlightAllIncorrectAnswers(List<string> incorrectAnswers)
        {
            for (int i = 0; i < m_ResponseButtons.Count; i++)
            {
                if (incorrectAnswers.Contains(m_ResponseButtons[i].text))
                {
                    m_ResponseButtons[i].AddToClassList(k_ResponseIncorrectClassName);
                }
            }
        }
    }
}
