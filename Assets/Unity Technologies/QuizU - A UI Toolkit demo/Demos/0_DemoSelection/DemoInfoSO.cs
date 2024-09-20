using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Quiz
{
	/// <summary>
	/// This stores some supporting data about each Demo scene for use by the
	/// DemoSelector and DemoSelectionScreen.
	/// </summary>
	[CreateAssetMenu(fileName = "DemoInfoSO", menuName = "Demos/DemoInfo_Data")]
	public class DemoInfoSO : DescriptionSO
	{
        [Space(20)]
        [Tooltip("Corresponding index for the Demo Scene in the Build Settings")]
        [SerializeField] int m_BuildSettingsIndex;
        [Tooltip("Title name shown in DemoSelectionScreen button")]
        [SerializeField] string m_Title;
        [Tooltip("Description shown on DemoSelectionScreen")]
        [TextArea(5,8)]
        [SerializeField] string m_Summary;
        [Tooltip("Sprite used on DemoSelectionScreen")]
        [SerializeField] [Optional] Sprite m_Icon;

        public string Title { get => m_Title; set => m_Title = value; }
        public string Summary { get => m_Summary; set => m_Summary = value; }
        public Sprite Icon => m_Icon;
        public int SceneIndex => m_BuildSettingsIndex;

        private void OnValidate()
        {
            // Replace asterisk characters with ASCII bullet point characters
            m_Summary = m_Summary.Replace('*', '•');
        }
    }
}