
using UnityEngine;
using UnityEngine.UIElements;

public class HealthInnerUI : VisualElement
{
	public HealthInnerUI() 
	{
		generateVisualContent += GenerateVisualContent;
	}
	
	Vector2 position = new Vector2(100, 100);
	float radius = 100;
	Color[] fillColors = new Color[] {Color.gray, Color.red};
	float[][] angle = new float[][]
	{
		new float[] {0, 360},
		new float[] {-90, 180}
	};
	public void GenerateVisualContent(MeshGenerationContext meshGenerationContext)
	{
		var painter2D = meshGenerationContext.painter2D;
		
		for (int i=0;i<fillColors.Length;i++)
		{
			painter2D.fillColor = fillColors[i];
			
			painter2D.BeginPath();
			painter2D.MoveTo(position);
			painter2D.Arc(position, radius, angle[i][0], angle[i][1], ArcDirection.Clockwise);
			painter2D.Fill();
		}
		
	}
	public new class UxmlFactory : UxmlFactory<HealthInnerUI> { }
}