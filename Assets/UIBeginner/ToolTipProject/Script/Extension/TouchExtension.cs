using UnityEngine;
using UnityEngine.UIElements;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
public class TouchExtension
{
    public static Touch GetTouchOverlapVisualElement(VisualElement visualElement, IPanel panel)
    {
        foreach (var touch in Touch.activeTouches)
        {
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                /* Convert from screen space to panel space */
                var touchPosition = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(touch.screenPosition.x, Screen.height - touch.screenPosition.y));

                if (visualElement.worldBound.Overlaps(new Rect(touchPosition, new Vector2(1, 1)))) return touch;
            }
        }

        return new Touch();
    }

    public static Touch GetTouchOverlapRect(Rect rect, IPanel panel)
    {
        foreach (var touch in Touch.activeTouches)
        {
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                /* Convert from screen space to panel space */
                var touchPosition = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(touch.screenPosition.x, Screen.height - touch.screenPosition.y));

                if (rect.Overlaps(new Rect(touchPosition, new Vector2(1, 1)))) return touch;
            }
        }

        return new Touch();
    }
}