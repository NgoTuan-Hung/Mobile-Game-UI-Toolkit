using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quiz
{
    /// <summary>
    /// Base class for setting up UI Document and root element for the Demo Scene UIs
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class DemoBase : MonoBehaviour
    {
        [Tooltip("Required UI Document")]
        [SerializeField] protected UIDocument m_Document;       

        [Header("Instructions")]
        [Tooltip("Pages of text paired with next/last buttons")]
        [SerializeField] protected DemoTextScreen m_TextScreen;

        protected VisualElement m_Root;
        protected Button m_BackButton;

        // Use a helper class to simplify registering and unregistering callbacks
        protected EventRegistry m_EventRegistry;

        protected virtual void OnEnable()
        {
            m_EventRegistry = new EventRegistry();
            SetVisualElements();

            m_EventRegistry.RegisterCallback<ClickEvent>(m_BackButton, evt => SceneEvents.LastSceneUnloaded());
        }

        protected virtual void OnDisable()
        {
            // One call to Dispose unregisters all the EventRegistry's managed callbacks. Otherwise,
            // register and unregister each callback individually.
            m_EventRegistry.Dispose();
        }

        // Set references to the visual elements
        protected virtual void SetVisualElements()
        {
            if (m_Document == null)
                m_Document = GetComponent<UIDocument>();

            m_Root = m_Document.rootVisualElement;

            // Verify required fields in the Inspector
            NullRefChecker.Validate(this);

            m_TextScreen.Initialize(m_Root);

            m_BackButton = m_Root.Q<Button>("back-button");
        }
    }
}
