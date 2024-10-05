using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UIElements;

public class HelperLensDragAndDropManipulator : PointerManipulator
{
    VisualElement root, lens, skillTooltip, skillTooltipRoot;
    private VisualTreeAsset skillTooltipTemplate;
    public HelperLensDragAndDropManipulator(VisualElement target, VisualTreeAsset skillTooltipTemplate)
    {
        this.target = target;
        lens = target.Children().First();
        root = target.parent.parent;
        skillTooltipRoot = skillTooltipTemplate.Instantiate();
        skillTooltip = skillTooltipRoot.Q<VisualElement>("skill-tooltip");
        root.Add(skillTooltip);
        skillTooltip.PlaceBehind(target.parent);
        skillTooltip.styleSheets.Add(Resources.Load<StyleSheet>("SkillTooltipSS"));
        skillTooltip.style.left = 99999f;
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
    public VisualTreeAsset SkillTooltipTemplate { get => skillTooltipTemplate; set => skillTooltipTemplate = value; }

    private void OnPointerDown(PointerDownEvent evt)
    {
        
        targetStartPosition = target.transform.position;
        pointerStartPosition = evt.position;
        target.CapturePointer(evt.pointerId);
        target.AddToClassList("in-use");
        target.RemoveFromClassList("not-use");
    }

    private void OnPointerUp(PointerUpEvent evt)
    {
        target.ReleasePointer(evt.pointerId);
        target.RemoveFromClassList("in-use");
        target.AddToClassList("not-use");

        UQueryBuilder<VisualElement> allHelpers = root.Query<VisualElement>(className: "has-helper");
        UQueryBuilder<VisualElement> overlappingHelper = allHelpers.Where(OverlappingHelper);

        VisualElement clothestHelper = FindClosestHelper(overlappingHelper);

        if (clothestHelper != null)
        {
            skillTooltip.style.left = evt.position.x + skillTooltip.worldBound.width > root.worldBound.width ? evt.position.x - skillTooltip.worldBound.width : evt.position.x;
            skillTooltip.style.top = evt.position.y + skillTooltip.worldBound.height > root.worldBound.height ? evt.position.y - skillTooltip.worldBound.height : clothestHelper.worldBound.position.y;
            skillTooltip.AddToClassList("skill-tooltip-showup");
        }
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
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

    public VisualElement FindClosestHelper(UQueryBuilder<VisualElement> query)
    {
        var helpers = query.ToList();
        if (helpers.Count == 0) return null;

        var lensLocalPosition = GetRootLocalPosition(lens);
        var closestHelper = helpers.OrderBy(h => (GetRootLocalPosition(h) - lensLocalPosition).magnitude).First();

        return closestHelper;
    }

    public bool OverlappingHelper(VisualElement helper)
    {
        return lens.worldBound.Overlaps(helper.worldBound);
    }

    public Vector2 GetRootLocalPosition(VisualElement helper)
    {
        return root.WorldToLocal(helper.GetWorldPosition());
    }
}