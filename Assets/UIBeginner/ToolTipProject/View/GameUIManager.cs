
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUIManager : MonoBehaviour
{
    public static Dictionary<string, VisualElement> helpers = new Dictionary<string, VisualElement>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        helpers = new Dictionary<string, VisualElement>();    
    }

    public static void AddHelper(string name, VisualElement helper)
    {
        helpers.Add(name, helper);
    }

    public static VisualElement GetHelper(string name)
    {
        return helpers[name];
    }

    public static void ChangeAllHelperOpacity(float opacity)
    {
        foreach (var helper in helpers.Values)
        {
            helper.style.opacity = opacity;
        }
    }

    UIDocument mainUIDocument;
    VisualElement root;
    List<VisualElement> layers;
    private void Awake() 
    {
        mainUIDocument = GetComponent<UIDocument>();
        root = mainUIDocument.rootVisualElement;
        layers = root.Query<VisualElement>(classes: "layer").ToList();
        layers.Sort((ve1, ve2) => ve1.name.CompareTo(ve2.name));
        
        InitDefaultLayer();
        HandleSafeArea();
    }

    public void HandleSafeArea()
    {
        /* Calculate the safe area so UIs don't touch unreachable parts of the screen */
        Rect safeArea = Screen.safeArea;
        Vector2 leftTop = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(safeArea.xMin, Screen.height - safeArea.yMax));
        Vector2 rightBottom = RuntimePanelUtils.ScreenToPanel(root.panel, new Vector2(Screen.width - safeArea.xMax, safeArea.yMin));
        root.Query<VisualElement>(classes: "safe-area").ForEach((ve) => 
        {
            ve.style.paddingLeft = leftTop.x;
            ve.style.paddingTop = leftTop.y;
            ve.style.paddingRight = rightBottom.x;
            ve.style.paddingBottom = rightBottom.y;
            /*  */
        });
    }

    public void InitDefaultLayer()
    {
        layers[0].style.left = 0;
        layers[0].style.top = 0;

        for (int i = 1; i < layers.Count; i++)
        {
            layers[i].style.left = 99999f;
            layers[i].style.top = 99999f;
        }
    }
}