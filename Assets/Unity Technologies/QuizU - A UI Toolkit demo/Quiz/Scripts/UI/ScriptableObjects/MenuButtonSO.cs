using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Quiz
{
    /// <summary>
    /// ScriptableObject to hold associated data for each Menu button.
    /// </summary>
    [CreateAssetMenu(fileName = "MenuButton_Data", menuName = "Quiz/MenuButtonData", order = 1)]
    public class MenuButtonSO : ScriptableObject
    {
        [SerializeField] string m_ElementID;    // The ID of the button element in the UI
        [TextArea(3, 10)]
        [SerializeField] string m_Description;  // The description text for each button
        [SerializeField] Sprite m_Image;        // The optional poster image of the button
        [SerializeField] string m_URL;          // Optional URL associated with the button

        Button m_MenuButton;                    // The UI Element Button object

        public string ElementID => m_ElementID;
        public string Description => m_Description;
        public Sprite Image => m_Image;

        public Button MenuButton { get => m_MenuButton; set => m_MenuButton = value; }
        public string URL => m_URL;

    }
}