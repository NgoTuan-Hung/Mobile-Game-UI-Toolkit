using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DragAndDropManipulator : PointerManipulator
{
    public DragAndDropManipulator(VisualElement target)
    {
        this.target = target;
    }
    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        target.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        target.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
    }

    private Vector2 targetStartPosition { get; set; }

    private Vector3 pointerStartPosition { get; set; }
    private void OnPointerDown(PointerDownEvent evt)
    {
        Debug.Log("OnPointerDown");
        target.CapturePointer(evt.pointerId);
    }

    private void OnPointerUp(PointerUpEvent evt)
    {
        Debug.Log("OnPointerUp");
        target.ReleasePointer(evt.pointerId);
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        Debug.Log("OnPointerMove");

        if (target.HasPointerCapture(evt.pointerId))
        {
            Vector3 pointerDelta = evt.position - pointerStartPosition;

            target.transform.position = new Vector2(
                Mathf.Clamp(targetStartPosition.x + pointerDelta.x, 0, target.panel.visualTree.worldBound.width),
                Mathf.Clamp(targetStartPosition.y + pointerDelta.y, 0, target.panel.visualTree.worldBound.height));
        }
    }

    private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
    {

    }
}