using UnityEngine;

namespace Quiz
{
    /// <summary>
    /// This is a base ScriptableObject that adds a description field.
    /// </summary>
    public class DescriptionSO : ScriptableObject
    {
        [TextArea(5, 20)]
        [SerializeField] [Optional] protected string m_Description;
    }
}