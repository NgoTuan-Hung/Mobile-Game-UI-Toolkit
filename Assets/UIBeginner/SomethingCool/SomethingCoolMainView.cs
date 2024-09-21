using UnityEngine;
using UnityEngine.UIElements;

public class SomethingCoolMainView : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset m_ListEntryTemplate;

    private void OnEnable() 
    {
        var uiDocument = GetComponent<UIDocument>();

        // Initialize the character list controller
        var characterListController = new SomethingCoolListController();
        characterListController.InitializeCharacterList(uiDocument.rootVisualElement, m_ListEntryTemplate);    
    }
}