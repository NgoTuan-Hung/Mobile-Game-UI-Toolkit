
using UnityEngine.UIElements;

public static class VisualElementExtension
{
    public static VisualElement GetLayer(this VisualElement visualElement)
    {
        return visualElement.parent.name.Equals("Layer") ? visualElement.parent : GetLayer(visualElement.parent);
    }
}