using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Quiz
{
    /// <summary>
    /// Data container for pages of text.
    /// </summary>
	[CreateAssetMenu(fileName = "DemoScreen_Data", menuName = "Demos/TextScreen Data", order = 5)]
	public class TextScreenSO : DescriptionSO
	{
        [SerializeField] private string m_Title;
        [TextArea(5, 10)]
        [SerializeField] private List<string> m_BodyText;


        public string Title => m_Title;
        public List<string> BodyText => m_BodyText;

    }
}