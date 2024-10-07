
using System.Collections.Generic;
using UnityEngine.UIElements;

public static class UIManager
{
    public static Dictionary<string, VisualElement> helpers = new Dictionary<string, VisualElement>();

    public static void AddHelper(string name, VisualElement helper)
    {
        helpers.Add(name, helper);
    }

    public static VisualElement GetHelper(string name)
    {
        return helpers[name];
    }
}