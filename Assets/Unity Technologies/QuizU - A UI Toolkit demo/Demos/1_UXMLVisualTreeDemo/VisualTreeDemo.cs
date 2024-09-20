using System.Collections.Generic;
using System.Text;
using UnityEngine.UIElements;


namespace Quiz
{
    public class VisualTreeDemo : DemoBase
    {
        // References to VisualElements
        Button m_Button1;
        Button m_Button2;
        Slider m_Slider1;
        TextField m_TextField1;
        Label m_InfoLabel;
        Label m_ParentInfoLabel;
        GroupBox m_Container1;
        GroupBox m_Container2;
        GroupBox m_Container3;

        // OnDisable called in the base class
        protected override void OnEnable()
        {
            base.OnEnable();
            SetVisualElements();
            RegisterCallbacks();
        }

        // Sets references to the root VisualElement and other necessary UI components.
        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_Button1 = m_Root.Q<Button>("button-1");
            m_Button2 = m_Root.Q<Button>("button-2");
            m_Slider1 = m_Root.Q<Slider>("slider-1");
            m_TextField1 = m_Root.Q<TextField>("text-field-1");
            m_InfoLabel = m_Root.Q<Label>("info-label");
            m_ParentInfoLabel = m_Root.Q<Label>("parent-info-label");
            m_Container1 = m_Root.Q<GroupBox>("container-1");
            m_Container2 = m_Root.Q<GroupBox>("container-2");
            m_Container3 = m_Root.Q<GroupBox>("container-3");
        }

        // Registers callback functions for certain events on the UI components

        // Note: we use a custom m_EventRegistry to register each callback to make unregistration
        // and disposal easier. The base class unregisters all callbacks in OnDisable
        private void RegisterCallbacks()
        {
            m_EventRegistry.RegisterCallback<MouseEnterEvent>(m_Button1, MouseEnterEventHandler);
            m_EventRegistry.RegisterCallback<MouseLeaveEvent>(m_Button1, MouseLeaveEventHandler);

            m_EventRegistry.RegisterCallback<MouseEnterEvent>(m_Button2, MouseEnterEventHandler);
            m_EventRegistry.RegisterCallback<MouseLeaveEvent>(m_Button2, MouseLeaveEventHandler);

            m_EventRegistry.RegisterCallback<MouseEnterEvent>(m_TextField1, MouseEnterEventHandler);
            m_EventRegistry.RegisterCallback<MouseLeaveEvent>(m_TextField1, MouseLeaveEventHandler);

            m_EventRegistry.RegisterCallback<MouseEnterEvent>(m_Slider1, MouseEnterEventHandler);
            m_EventRegistry.RegisterCallback<MouseLeaveEvent>(m_Slider1, MouseLeaveEventHandler);

            m_EventRegistry.RegisterCallback<MouseEnterEvent>(m_Container1, MouseEnterEventHandler);
            m_EventRegistry.RegisterCallback<MouseLeaveEvent>(m_Container1, MouseLeaveEventHandler);

            m_EventRegistry.RegisterCallback<MouseEnterEvent>(m_Container2, MouseEnterEventHandler);
            m_EventRegistry.RegisterCallback<MouseLeaveEvent>(m_Container2, MouseLeaveEventHandler);

            m_EventRegistry.RegisterCallback<MouseEnterEvent>(m_Container3, MouseEnterEventHandler);
            m_EventRegistry.RegisterCallback<MouseLeaveEvent>(m_Container3, MouseLeaveEventHandler);
        }

        // Clears the text when the mouse exits
        private void MouseLeaveEventHandler(MouseLeaveEvent evt)
        {
            VisualElement element = (VisualElement)evt.target;
            m_InfoLabel.text = string.Empty;
            m_ParentInfoLabel.text = string.Empty;
        }

        // Updates the info label text with information about the UI hierarchy
        // when the mouse enters
        private void MouseEnterEventHandler(MouseEnterEvent evt)
        {
            VisualElement element = (VisualElement)evt.target;

            string parentInfo = GetParentHierarchy(element);

            m_InfoLabel.text = element.name + ":";
            m_ParentInfoLabel.text = parentInfo;    
        }

        // Generates a string representation of the parent hierarchy for a given
        // VisualElement
        public static string GetParentHierarchy(VisualElement element)
        {
            List<string> names = new List<string>();

            // Go up the hierarchy and collect all element names
            VisualElement current = element;
            while (current != null)
            {
                names.Add(current.name);
                current = current.parent;
            }

            // Reverse the list to start from the root
            names.Reverse();

            // Build the result string with appropriate indent
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < names.Count; i++)
            {
                // draw the tree lines
                if (i < names.Count) 
                {
                    sb.AppendLine(new string(' ', i * 2) + "├─ " + names[i]);
                }
            }

            return sb.ToString();
        }
    }
}
