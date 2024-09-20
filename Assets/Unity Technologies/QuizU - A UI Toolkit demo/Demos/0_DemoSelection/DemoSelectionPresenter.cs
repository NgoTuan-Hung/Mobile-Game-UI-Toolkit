
using System;
using UnityEngine;
using UnityEngine.UIElements;
namespace Quiz
{
    /// <summary>
    ///
    /// Similar to the LevelSelectionPresenter, the DemoSelector is a Presenter component. This facilitates communication
    /// between a DemoSO ScriptableObject data (Model) and the DemoSelectionScreen user-interface (View).
    /// This separates the UI from the data to improve testing and maintenance.
    /// </summary>
    /// 
    public class DemoSelectionPresenter : MonoBehaviour
    {

        [Tooltip("Supporting ScriptableObject data for each Demo scene")]
        [SerializeField] DemoInfoSO[] m_DemoInfo;

        DemoSelectionScreen m_DemoSelectionScreen;

        public DemoInfoSO[] DemoInfo => m_DemoInfo;
        public DemoSelectionScreen DemoSelectionScreen { get => m_DemoSelectionScreen; set => m_DemoSelectionScreen = value; }

        private void OnEnable()
        {
            DemoEvents.SetupCompleted += DemoEvents_SetupCompleted;
            DemoEvents.ButtonHighlighted += DemoEvents_ButtonHighlighted;
        }

        private void OnDisable()
        {
            DemoEvents.SetupCompleted -= DemoEvents_SetupCompleted;
            DemoEvents.ButtonHighlighted -= DemoEvents_ButtonHighlighted;
        }

        // Event-handling methods

        // Wait for the UI to setup, then initialize the Presenter
        private void DemoEvents_SetupCompleted()
        {
            Initialize();
        }

        private void DemoEvents_ButtonHighlighted(int index)
        {
            ShowDemoInfoByIndex(index);
        }

        private void Initialize()
        {
            // Verify required Inspector fields
            NullRefChecker.Validate(this);

            // Show the info from the first/default Demo scene
            ShowDemoInfoByIndex(0);
        }

        // Show the information on the side panel 
        private void ShowDemoInfoByIndex(int index)
        {
            m_DemoSelectionScreen.ShowIcon(m_DemoInfo[index].Icon);
            m_DemoSelectionScreen.ShowSummary(m_DemoInfo[index].Summary);
            m_DemoSelectionScreen.ShowTitle(m_DemoInfo[index].Title);
        }
    }
}
