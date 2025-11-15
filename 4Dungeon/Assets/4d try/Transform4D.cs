using MIConvexHull;
using Unity.Collections;
using UnityEngine;

public class Vertex : IVertex
{
	public double[] Position { get; }

	public Vector3 position; // For Unity mesh construction

	public Vertex(Vector4 v)
	{
		Position = new double[] { v.x, v.y, v.z };
		position = new Vector3(v.x, v.y, v.z);
	}
}


[ExecuteInEditMode]
public class Transform4D : MonoBehaviour
{
	[Header("Mesh4D")]
	public Mesh4D mesh4D;   // drag your Cube4D.asset here in the Inspector
	[ReadOnly]
	private Vector4[] vertices;   // working copy after transforms

	[Header("Transform4D")]
    public Euler4 Rotation;
	public Vector4 Position;
	public Vector4 Scale = Vector4.one;

	[Header("Rotation")]
	[ReadOnly]
    private Matrix4x4 RotationMatrix;

    void Start()
    {
        // No mesh!
        if (mesh4D == null)
            return;

        // Instantiates the vertices
        vertices = new Vector4[mesh4D.Vertices.Length];

        // Updates the mesh
        UpdateRotationMatrix();
        UpdateVertices();
    }

    private void Update()
	{
		UpdateRotationMatrix();
		UpdateVertices();
	}

	private void UpdateVertices()
	{
		for (int i = 0; i < mesh4D.Vertices.Length; i++)
			vertices[i] = Transform(mesh4D.Vertices[i]);
	}

	private void UpdateRotationMatrix()
	{
		RotationMatrix =
			Matrix4x4.identity
			.RotateXY(Rotation.XY * Mathf.Deg2Rad)
			.RotateYZ(Rotation.YZ * Mathf.Deg2Rad)
			.RotateXZ(Rotation.XZ * Mathf.Deg2Rad)
			.RotateXW(Rotation.XW * Mathf.Deg2Rad)
			.RotateYW(Rotation.YW * Mathf.Deg2Rad)
			.RotateZW(Rotation.ZW * Mathf.Deg2Rad);
	}

    private Vector4 Transform(Vector4 v)
    {
        v = RotationMatrix * v;

        v.x *= Scale.x;
        v.y *= Scale.y;
        v.z *= Scale.z;
        v.w *= Scale.w;

        v += Position;

        return v;
    }

    private void OnDrawGizmos()
	{
		if (mesh4D == null || mesh4D.Vertices == null || mesh4D.Edges == null) return;

		UpdateRotationMatrix();
		UpdateVertices();

		if (vertices == null) return;

		Gizmos.color = Color.green;

		foreach (var edge in mesh4D.Edges)
		{
			// double check indices are in range
			if (edge.Index0 < vertices.Length && edge.Index1 < vertices.Length)
			{
				Vector3 p0 = new Vector3(vertices[edge.Index0].x, vertices[edge.Index0].y, vertices[edge.Index0].z);
				Vector3 p1 = new Vector3(vertices[edge.Index1].x, vertices[edge.Index1].y, vertices[edge.Index1].z);

				Gizmos.DrawLine(p0, p1);
			}
		}
	}
}