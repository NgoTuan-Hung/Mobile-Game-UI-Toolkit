
using UnityEngine;
using UnityEngine.UIElements;

public class TestVisualElementVectorApi : VisualElement
{
	public Color color = Color.white;
	public TestVisualElementVectorApi()
	{
		generateVisualContent += GenerateVisualContent;
	}
	
	public void GenerateVisualContent(MeshGenerationContext meshGenerationContext)
	{
		var p0 = Vector2.zero;
		var p1 = new Vector2(layout.width, 0);
		var p2 = new Vector2(0, layout.height);
		
		var painter2D = meshGenerationContext.painter2D;
		
		painter2D.strokeColor = color;
		painter2D.lineWidth = 10;
		painter2D.lineCap = LineCap.Round;
		painter2D.lineJoin = LineJoin.Round;
		
		painter2D.BeginPath();
		painter2D.MoveTo(p0);
		painter2D.LineTo(p1);
		painter2D.LineTo(p2);
		painter2D.LineTo(p0);
		painter2D.Stroke();
	}
	
	public new class UxmlFactory : UxmlFactory<TestVisualElementVectorApi>
	{
		
	}
}