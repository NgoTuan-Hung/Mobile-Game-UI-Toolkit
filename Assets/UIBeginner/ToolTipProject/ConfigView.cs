using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConfigView : MonoBehaviour
{
    UIDocument uIDocument;
    VisualElement root;
    private void Awake() 
    {
        uIDocument = GetComponent<UIDocument>();
        root = uIDocument.rootVisualElement;
        AddDummyDropdownValue();
    }

    public void AddDummyDropdownValue()
    {
        DropdownField dropdown = root.Q<DropdownField>();
        dropdown.choices.Add("English");
        dropdown.choices.Add("Tiếng Việt");
        dropdown.choices.Add("日本語");
        dropdown.choices.Add("Español");
        dropdown.choices.Add("Português");
        dropdown.Query<Label>("unity-base-dropdown__label").ForEach(label => 
        {
            label.style.fontSize = 48;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
