using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SomethingCoolListElementController
{
    Label label;

    public void SetVisualElement(VisualElement visualElement)
    {
        label = visualElement.Q<Label>("character-name");
    }
    
    // This function receives the character whose name this list 
    // element is supposed to display. Since the elements list 
    // in a `ListView` are pooled and reused, it's necessary to 
    // have a `Set` function to change which character's data to display.
    public void SetCharacterData(SomethingCoolDataHolder somethingCoolDataHolder)
    {
        label.text = somethingCoolDataHolder.name;
    }
}
