using UnityEngine;
using UnityEngine.UIElements;

namespace Quiz
{
    /// <summary>
    /// UI Screen for displaying a question on-screen (GameScreen.uxml). This is the main gameplay screen.
    ///
    /// This class set ups several other components that actually contain the logic: the
    /// QuestionPanel (left), ResponsePanel (right), MessageBar (bottom), and ProgressMeter (top).
    ///
    /// This class initializes those smaller UI components with a UI Document.
    /// </summary>


    public class GameScreen : UIScreen
    {
        ResponseDisplay m_ResponseDisplay;
        QuestionDisplay m_QuestionDisplay;
        MessageDisplay m_MessageDisplay;
        ProgressDisplay m_ProgressDisplay;
        LifeBarDisplay m_LifeBarDisplay;

        public GameScreen(VisualElement rootElement): base(rootElement)
        {
            // The components of the GameScreen are System objects that implement IDisposable. We use each one's
            // Constructor to pass the root element and do any setup. 
            m_ResponseDisplay = new ResponseDisplay(rootElement);
            m_QuestionDisplay = new QuestionDisplay(rootElement);
            m_MessageDisplay = new MessageDisplay(rootElement);
            m_ProgressDisplay = new ProgressDisplay(rootElement);
            m_LifeBarDisplay = new LifeBarDisplay(rootElement);
            m_LifeBarDisplay.AssignTooltip("Guesses remaining");
        }

        public override void Disable()
        {
            base.Disable();

            // Each component implements Dispose for cleanup.
            m_ResponseDisplay.Dispose();
            m_QuestionDisplay.Dispose();
            m_MessageDisplay.Dispose();
            m_ProgressDisplay.Dispose();
            m_LifeBarDisplay.Dispose();

        }
    }
}