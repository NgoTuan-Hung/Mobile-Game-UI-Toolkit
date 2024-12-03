
using UnityEngine;
using UnityEngine.UIElements;

public class RadialProgressUI : VisualElement
{
	public RadialProgressUI()
	{
		generateVisualContent += GenerateVisualContent;
	}
	
	Vector2 center;
	Color strokeColor;
	public void GenerateVisualContent(MeshGenerationContext meshGenerationContext)
	{
		center = new Vector2(contentRect.width / 2, contentRect.height / 2);
		var painter2D = meshGenerationContext.painter2D;
		
		painter2D.lineWidth = ProgressWidth;
		
		painter2D.strokeColor = Color.Lerp(startColor, endColor, progress);
		painter2D.BeginPath();
		painter2D.Arc(center, center.x, -90 + progress * 360, -90, ArcDirection.CounterClockwise);
		painter2D.Stroke();
	}
	
	static readonly CustomStyleProperty<float> cs_Progress = new CustomStyleProperty<float>("--progress");
	float progress = 0f;
	public float Progress
	{
		get => progress;
		set 
		{
			progress = value;
			MarkDirtyRepaint();
		}
	}
	
	float progressWidth = 30;
	public float ProgressWidth
	{
		get => progressWidth;
		set
		{
			progressWidth = value;
			MarkDirtyRepaint();
		}
	}
	
	Color startColor = Color.red, endColor = Color.green;
	public Color StartColor
	{
		get => startColor;
		set
		{
			startColor = value;
			MarkDirtyRepaint();
		}
	}
	
	public Color EndColor
	{
		get => endColor;
		set
		{
			endColor = value;
			MarkDirtyRepaint();
		}
	}
	public new class UxmlFactory : UxmlFactory<RadialProgressUI, UxmlTraits> { }
	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		// The progress property is exposed to UXML.
		UxmlFloatAttributeDescription m_ProgressAttribute = new UxmlFloatAttributeDescription()
		{
			name = "progress"
		};
		
		UxmlFloatAttributeDescription m_lineWidthAttribute = new UxmlFloatAttributeDescription()
		{
			name = "progress-width", defaultValue = 30
		};
		
		UxmlColorAttributeDescription m_strokeStartColorAttribute = new UxmlColorAttributeDescription()
		{
			name = "start-color", defaultValue = Color.red
		};
		
		UxmlColorAttributeDescription m_strokeEndColorAttribute = new UxmlColorAttributeDescription()
		{
			name = "end-color", defaultValue = Color.green
		};

		// Use the Init method to assign the value of the progress UXML attribute to the C# progress property.
		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			var rp = ve as RadialProgressUI;

			rp.Progress = m_ProgressAttribute.GetValueFromBag(bag, cc);
			rp.ProgressWidth = m_lineWidthAttribute.GetValueFromBag(bag, cc);
			rp.StartColor = m_strokeStartColorAttribute.GetValueFromBag(bag, cc);
			rp.EndColor = m_strokeEndColorAttribute.GetValueFromBag(bag, cc);
		}
	}
}