using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SomethingCoolListController
{
    VisualTreeAsset m_ListEntryTemplate;
    
    // UI element references
    ListView m_CharacterList;
    Label m_CharClassLabel;
    Label m_CharNameLabel;
    VisualElement m_CharPortrait;

    List<SomethingCoolDataHolder> m_AllCharacters;
    
    public void InitializeCharacterList(VisualElement root, VisualTreeAsset listElementTemplate)
    {
        EnumerateAllCharacters();
    
        // Store a reference to the template for the list entries
        m_ListEntryTemplate = listElementTemplate;
    
        // Store a reference to the character list element
        m_CharacterList = root.Q<ListView>("CharacterList");
    
        // Store references to the selected character info elements
        m_CharClassLabel = root.Q<Label>("CharacterClass");
        m_CharNameLabel = root.Q<Label>("CharacterName");
        m_CharPortrait = root.Q<VisualElement>("CharacterPortrait");
    
        FillCharacterList();
    
        // Register to get a callback when an item is selected
        m_CharacterList.selectionChanged += OnCharacterSelected;
    }

    void EnumerateAllCharacters()
    {
        m_AllCharacters = new List<SomethingCoolDataHolder>();
        m_AllCharacters.AddRange(Resources.LoadAll<SomethingCoolDataHolder>("CharacterData"));
    }

    [SerializeField] private float listElementHeight = 75f;
    void FillCharacterList()
    {
        // Set up a make item function for a list entry
        m_CharacterList.makeItem = () =>
        {
            // Instantiate the UXML template for the entry
            var newListEntry = m_ListEntryTemplate.Instantiate();
    
            // Instantiate a controller for the data
            var newListEntryLogic = new SomethingCoolListElementController();
    
            // Assign the controller script to the visual element
            newListEntry.userData = newListEntryLogic;
    
            // Initialize the controller script
            newListEntryLogic.SetVisualElement(newListEntry);
    
            // Return the root of the instantiated visual tree
            return newListEntry;
        };
    
        // Set up bind function for a specific list entry
        m_CharacterList.bindItem = (item, index) =>
        {
            (item.userData as SomethingCoolListElementController)?.SetCharacterData(m_AllCharacters[index]);
        };
    
        // Set a fixed item height matching the height of the item provided in makeItem. 
        // For dynamic height, see the virtualizationMethod property.
        m_CharacterList.fixedItemHeight = listElementHeight;
    
        // Set the actual item's source list/array
        m_CharacterList.itemsSource = m_AllCharacters;
    }

    void OnCharacterSelected(IEnumerable<object> selectedItems)
    {
        // Get the currently selected item directly from the ListView
        var selectedCharacter = m_CharacterList.selectedItem as SomethingCoolDataHolder;
    
        // Handle none-selection (Escape to deselect everything)
        if (selectedCharacter == null)
        {
            // Clear
            m_CharClassLabel.text = "";
            m_CharNameLabel.text = "";
            m_CharPortrait.style.backgroundImage = null;
    
            return;
        }
    
        // Fill in character details
        m_CharClassLabel.text = selectedCharacter.job.ToString();
        m_CharNameLabel.text = selectedCharacter.name;
        m_CharPortrait.style.backgroundImage = new StyleBackground(selectedCharacter.Portrait);
    }
}