
using System.Linq;
using UnityEngine.UIElements;

public static class VisualElementExtension
{
    public static VisualElement GetLayer(this VisualElement visualElement)
    {
        return visualElement.parent.GetClasses().Contains("layer") ? visualElement.parent : GetLayer(visualElement.parent);
    }
}