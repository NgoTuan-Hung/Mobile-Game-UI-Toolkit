using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Quiz
{
	/// <summary>
	/// Data container for use with the USS Styles Demo scene.
	/// </summary>
	[CreateAssetMenu(fileName = "StyleInfoSO", menuName = "Demos/StyleInfo_Data")]
	public class StyleInfoSO : DescriptionSO
	{
		[Space(20)]
		[TextArea(3,5)]
		[SerializeField] string m_ClassSelectorText;
        [TextArea(15, 25)]
        [SerializeField] string m_UssStyleText;
        [TextArea(5, 8)]
        [SerializeField] string m_DescriptionText;

		public string ClassSelectorText => m_ClassSelectorText;
		public string UssStyleText => m_UssStyleText;
		public string DescriptionText => m_DescriptionText;


	}
}