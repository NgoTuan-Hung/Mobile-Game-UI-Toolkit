using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SimpleCustomEditorShit : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/UI Toolkit/SimpleCustomEditorShit")]
    public static void ShowExample()
    {
        SimpleCustomEditorShit wnd = GetWindow<SimpleCustomEditorShit>();
        wnd.titleContent = new GUIContent("SimpleCustomEditorShit");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIBeginner/NewUXMLShit.uxml");
        VisualElement labelFromUXML_uxml = visualTree.Instantiate();
        root.Add(labelFromUXML_uxml);

        Label label1 = new Label("Fuck it");
        label1.AddToClassList("fack");
        root.Add(label1);

        Button button1 = new Button(); button1.text = "CLICK ME MOTHERFACKER";
        button1.AddToClassList("fack");
        root.Add(button1);

        UnityEngine.UIElements.RectField rect = new UnityEngine.UIElements.RectField();
        rect.AddToClassList("fack");
        root.Add(rect);
        
        SetupButtonHandler();
    }

    //Functions as the event handlers for your button click and number counts
    private void SetupButtonHandler()
    {
        VisualElement root = rootVisualElement;

        var buttons = root.Query<Button>();
        buttons.ForEach(RegisterHandler);
    }

    private void RegisterHandler(Button button)
    {
        button.RegisterCallback<ClickEvent>(PrintClickMessage);
    }

    private int m_ClickCount = 0;
    private void PrintClickMessage(ClickEvent evt)
    {
        VisualElement root = rootVisualElement;

        ++m_ClickCount;

        //Because of the names we gave the buttons and toggles, we can use the
        //button name to find the toggle name.
        Button button = evt.currentTarget as Button;
        string toggleName = "ToggleBitch";
        Toggle toggle = root.Q<Toggle>(toggleName);
        toggle.value = !toggle.value;

        Debug.Log("Button name: " + button.name + "  Toggle name: " + toggle.name + "  Click count: " + m_ClickCount);
    }
}
