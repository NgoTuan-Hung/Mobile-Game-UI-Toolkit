using UnityEngine;
using UnityEngine.UIElements;

public class TestVisualElementArc : VisualElement
{
	public TestVisualElementArc() 
	{
		generateVisualContent += GenerateVisualContent;
	}
	
	Vector2 center = Vector2.zero;
	void GenerateVisualContent(MeshGenerationContext meshGenerationContext)
	{
		var painter2D = meshGenerationContext.painter2D;
		center = new Vector2(resolvedStyle.width / 2, resolvedStyle.height / 2);
		
		painter2D.fillColor = Color.red;
		painter2D.BeginPath();
		painter2D.MoveTo(center);
		painter2D.Arc(center, center.y, 0, 360, ArcDirection.Clockwise);
		
		painter2D.BeginPath();
		painter2D.MoveTo(center);
		painter2D.Arc(center, center.y / 2, 0, 360, ArcDirection.Clockwise);
		painter2D.Fill(FillRule.OddEven);
	}
	public new class UxmlFactory : UxmlFactory<TestVisualElementArc>{}
}