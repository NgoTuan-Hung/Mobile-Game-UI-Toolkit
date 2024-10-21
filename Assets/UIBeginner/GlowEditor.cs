using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GlowEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/UI Toolkit/GlowEditor")]
    public static void ShowExample()
    {
        GlowEditor wnd = GetWindow<GlowEditor>();
        wnd.titleContent = new GUIContent("GlowEditor");
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

        //Doesn't work
        Debug.Assert(true);
    }
}
