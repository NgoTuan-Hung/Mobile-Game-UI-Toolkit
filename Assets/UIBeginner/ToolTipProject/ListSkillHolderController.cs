using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ListSkillHolderController : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset skillHolderTemplate;

    ScrollView skillScrollView;
    VisualElement root;
    private void OnEnable() 
    {
        var uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        skillScrollView = root.Q<ScrollView>();

        InitializeSkillHolderList();
    }

    public void InitializeSkillHolderList()
    {
        for (int i = 0; i < 100; i++)
        {
            // var newSkillHolder = skillHolderTemplate.Instantiate();

            // skillScrollView.Add(newSkillHolder);

            Button button = new Button();
            button.text = "Skill";
            button.style.width = 150;
            button.style.height = 150;
            skillScrollView.Add(button);
        }
    }

}
