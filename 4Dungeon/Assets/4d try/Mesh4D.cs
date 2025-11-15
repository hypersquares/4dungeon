using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Mesh4D", menuName = "Scriptable Objects/Mesh4D")]
public class Mesh4D : ScriptableObject
{
	public Vector4[] Vertices;
	public Edge[] Edges;
}


// Add this extension method to convert Vertex to Vector4
public static class VertexExtensions
{
	public static Vector4 ToVector4(this Vertex vertex)
	{
		// Assuming the Position array has at least 4 elements
		return new Vector4(
			(float)vertex.Position[0],
			(float)vertex.Position[1],
			(float)vertex.Position[2],
			(float)vertex.Position[3]
		);
	}
}

