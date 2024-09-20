using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quiz
{
    /// <summary>
    /// The ManipulatorDemo class uses SimpleDragManipulator to make elements draggable.
    /// This script adds the manipulator to every element assigned with the corresponding
    /// class (k_DraggableClass).
    /// </summary>
    public class ManipulatorDemo : DemoBase
    {
        // Defines the class name of draggable elements
        const string k_DraggableClass = "draggable";

        // List of VisualElement objects that are draggable.
        List<VisualElement> m_DraggableElements;

        protected override void OnEnable()
        {
            base.OnEnable();

            SetManipulators();
        }

        // Sets up the VisualElements and stores all draggable elements in the
        // m_DraggableElements list.
        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            // Store all elements containing the k_DraggableClass name in a list
            m_DraggableElements = m_Document.rootVisualElement.Query(className: k_DraggableClass).ToList();

        }

        // SetManipulators adds a new instance of SimpleDragManipulator to each draggable
        // element, making it respond to drag-and-drop interactions.

        private void SetManipulators()
        {
            foreach (VisualElement visualElement in m_DraggableElements)
            {
                visualElement.AddManipulator(new SimpleDragManipulator());
            }
        }
    }
}
