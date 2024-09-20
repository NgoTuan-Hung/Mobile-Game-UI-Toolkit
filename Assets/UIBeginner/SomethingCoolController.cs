using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class SomethingCoolController : MonoBehaviour
{
    private Button button;
    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();

        button = uiDocument.rootVisualElement.Q<Button>();
        button.RegisterCallback<ClickEvent>(OnButtonClick);
        // show a popup window on button hover
        button.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
    }

    private void OnButtonClick(ClickEvent evt)
    {
        Debug.Log("Button clicked");
    }

    private void OnMouseEnter(MouseEnterEvent evt)
    {
        UnityEditor.PopupWindow.Show(new Rect(evt.localMousePosition.x, evt.localMousePosition.y, 0, 0), new SomethingCoolPopup());
    }
}

class SomethingCoolPopup : PopupWindowContent
{
    public override Vector2 GetWindowSize()
    {
        return new Vector2(200, 200);
    }

    public override void OnGUI(Rect rect)
    {
        Debug.Log("SomethingCoolPopup: OnGUI");
    }
}
