using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quiz
{
    /// <summary>
    /// This enum defines font sizes.
    /// </summary>
    [System.Serializable]
    public enum QuestionFontSize
    {
        Small,
        Medium,
        Large
    }

    /// <summary>
    /// Utility class for holding text styles. Use this to pair a USS class to a corresponding font enum
    /// </summary> 
    public static class TextStyles
    {
        // Static array of strings for font sizes
        public static string[] FontSizes =
        {
            "question-text-small",
            "question-text-medium",
            "question-text-large"
        };

        // Use this to pair a USS Class Selector with a FontSize enum
        public static string GetFontClass(QuestionFontSize fontSize)
        {
            switch (fontSize)
            {
                case QuestionFontSize.Small:
                    return FontSizes[0];
                case QuestionFontSize.Medium:
                    return FontSizes[1];
                case QuestionFontSize.Large:
                    return FontSizes[2];
                default:
                    return string.Empty;
            }
        }

        // Get array of other font class names (useful for removing the other USS classes)
        public static string[] GetOtherFontClasses(QuestionFontSize currentFontSize)
        {
            var currentIndex = (int)currentFontSize;

            List<string> otherFontSizes = new List<string>();

            for (int i = 0; i < FontSizes.Length; i++)
            {
                if (i != currentIndex)
                    otherFontSizes.Add(FontSizes[i]);
            }

            return otherFontSizes.ToArray();
        }

    }
}
