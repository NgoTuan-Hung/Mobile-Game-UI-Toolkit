
using UnityEngine;
using UnityEngine.UIElements;

public partial class TestVisualElement : VisualElement
{
	public Color color = Color.white;
	
	public TestVisualElement()
	{
		generateVisualContent += GenerateVisualContent;
	}
	
	public void GenerateVisualContent(MeshGenerationContext meshGenerationContext)
	{
		var mesh = meshGenerationContext.Allocate(3, 3);
		
		var p0 = Vector2.zero;
		var p1 = new Vector2(layout.width, 0);
		var p2 = new Vector2(0, layout.height);
		
		mesh.SetAllVertices
		(
			new Vertex[] 
			{
				new Vertex() {position = new Vector3(p0.x, p0.y, Vertex.nearZ), tint = color},
				new Vertex() {position = new Vector3(p1.x, p1.y, Vertex.nearZ), tint = color},
				new Vertex() {position = new Vector3(p2.x, p2.y, Vertex.nearZ), tint = color}
			}
		);
		
		mesh.SetAllIndices(new ushort[] {0, 1, 2});
	}
	
	public new class UxmlFactory : UxmlFactory<TestVisualElement>
	{
		
	}
}