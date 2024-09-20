
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace Quiz
{
    /// <summary>
    /// EventDispatchDemo showcases how events propagate through a UI hierarchy. This example specifically demonstrates Buttons with
    /// ClickEvents. The top UI features an array of Buttons, where the parent container handles the events and the actual event propagates
    /// to child elements.
    /// 
    /// The bottom UI shows how nested Buttons (Parent, Child, GrandChild) handle the ClickEvent. When clicking a button note the event's
    /// propagation phase.
    ///
    /// Use the Clear button to clear the log lines as click the UI Buttons.
    /// </summary>
    public class EventDispatchDemo : DemoBase
    {
        VisualElement m_ButtonArrayContainer;
        Button m_ParentButton;
        Button m_ChildButton;
        Button m_GrandChildButton;
        Label m_LogLabel;
        Button m_ClearLogButton;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_EventRegistry.RegisterCallback<ClickEvent>(m_ParentButton, ClickParentButton);
            m_EventRegistry.RegisterCallback<ClickEvent>(m_ChildButton, ClickChildButton);
            m_EventRegistry.RegisterCallback<ClickEvent>(m_GrandChildButton, ClickGrandChildButton);

            m_EventRegistry.RegisterCallback<ClickEvent>(m_ClearLogButton, ClearLogLabel);

            // This shows event propagation to register ClickEvents on the child Buttons
            m_EventRegistry.RegisterCallback<ClickEvent>(m_ButtonArrayContainer, ClickArrayButton);
            
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            m_EventRegistry.Dispose();
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            m_ParentButton = m_Root.Q<Button>("parent-button");
            m_ChildButton = m_Root.Q<Button>("child-button");
            m_GrandChildButton = m_Root.Q<Button>("grand-child-button");
            m_LogLabel = m_Root.Q<Label>("log-label");
            m_ButtonArrayContainer = m_Root.Q<VisualElement>("button-array-container");
            m_ClearLogButton = m_Root.Q<Button>("clear-log-button");
       }

        private void ClearLogLabel(ClickEvent evt)
        {
            // Clear the Label
            m_LogLabel.text = string.Empty;
        }

        private void LogMessage(string message)
        {
            // Limit the number of lines in the log label
            var lines = m_LogLabel.text.Split('\n');
            if (lines.Length > 5)
            {
                m_LogLabel.text = string.Join("\n", lines.Skip(lines.Length - 5)) + "\n" + message;
            }
            else
            {
                m_LogLabel.text += "\n" + message;
            }
        }

        // Note the difference between the ClickEvent's target and currentTarget:

        // - target refers to the original element where the event was first dispatched. If a user clicks on a button, the target would be that specific button.
        // - currentTarget refers to the element currently handling the event during the event propagation process. In an event chain, this could be the target
        //   itself or any of its ancestors, depending on the propagation phase of the event (capturing/trickling, at target, or bubbling).


        private void LogClickEvent(ClickEvent evt, Button button)
        {
            string message = "Clicked: " + button.name + "  Propagation phase: " + evt.propagationPhase.ToString();
            Debug.Log(message);
            LogMessage(message);
        }

        private void ClickParentButton(ClickEvent evt)
        {
            LogClickEvent(evt, m_ParentButton);
        }

        private void ClickChildButton(ClickEvent evt)
        {
            LogClickEvent(evt, m_ChildButton);
        }

        private void ClickGrandChildButton(ClickEvent evt)
        {
            LogClickEvent(evt, m_GrandChildButton);
        }


        // Note that we don't need to reference the individual buttons; we set up the event on the
        // parent container and then propagate to the child buttons automatically
        private void ClickArrayButton(ClickEvent evt)
        {
            // Logic only runs if we click one of the child buttons
            if (evt.target is Button targetButton)
            {
                LogClickEvent(evt, targetButton);

                // Mark the event as "handled" and stop propagation
                evt.StopPropagation();
            }
        }

    }
}
