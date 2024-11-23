using UnityEngine;
using UnityEngine.UIElements;

public class HealthUI : VisualElement
{
	public HealthUI()
	{
		generateVisualContent += GenerateVisualContent;
	}
	
	Vector2 center = Vector2.zero;
	Color fillColor;
	void GenerateVisualContent(MeshGenerationContext meshGenerationContext)
	{
		var painter2D = meshGenerationContext.painter2D;
		center = new Vector2(resolvedStyle.width / 2, resolvedStyle.height / 2);
		
		painter2D.fillColor = fillColor;
		painter2D.BeginPath();
		painter2D.MoveTo(center);
		painter2D.Arc(center, center.x, -90 + progress * 360, -90, ArcDirection.CounterClockwise);
		painter2D.Fill();
	}
	
	float progress = 0f;
	public float Progress
	{
		get => progress;
		set 
		{
			progress = value;
			fillColor = Color.Lerp(Color.red, Color.green, progress);
			MarkDirtyRepaint();
		}
	}
	public new class UxmlFactory : UxmlFactory<HealthUI, UxmlTraits> { }
	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		// The progress property is exposed to UXML.
		UxmlFloatAttributeDescription m_ProgressAttribute = new UxmlFloatAttributeDescription()
		{
			name = "progress"
		};

		// Use the Init method to assign the value of the progress UXML attribute to the C# progress property.
		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);

			(ve as HealthUI).Progress = m_ProgressAttribute.GetValueFromBag(bag, cc);
		}
	}
}