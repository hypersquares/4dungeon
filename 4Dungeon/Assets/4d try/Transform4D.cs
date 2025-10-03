using MIConvexHull;
using System;
using UnityEngine;


[Serializable]
public struct Euler4
{
	[Range(-180, +180)]
	public float XY; // Z (W)
	[Range(-180, +180)]
	public float YZ; // X (w)
	[Range(-180, +180)]
	public float XZ; // Y (W)
	[Range(-180, +180)]
	public float XW; // Y Z
	[Range(-180, +180)]
	public float YW; // X Z
	[Range(-180, +180)]
	public float ZW; // X Y
}

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



public class Transform4D : MonoBehaviour
{
	[Header("Mesh4D")]
	public Mesh4D mesh4D;   // drag your Cube4D.asset here in the Inspector

	private Vector4[] vertices;   // working copy after transforms
	public Euler4 Rotation;
	public Vector4 Position;
	public Vector4 Scale = Vector4.one;

	private Matrix4x4 RotationMatrix;

	private void Update()
	{
		UpdateRotationMatrix();
		UpdateVertices();
	}

	private void UpdateVertices()
	{
		// Make sure asset exists and has verts
		if (mesh4D == null || mesh4D.Vertices == null) return;

		// Allocate local array if needed
		if (vertices == null || vertices.Length != mesh4D.Vertices.Length)
			vertices = new Vector4[mesh4D.Vertices.Length];

		for (int i = 0; i < mesh4D.Vertices.Length; i++)
		{
			vertices[i] = Transform(mesh4D.Vertices[i]);
		}
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


public static class Matrix4x4Extensions
{
	public static Matrix4x4 RotateXY(this Matrix4x4 m, float a)
	{
		return m * RotateXY(a);
	}

	public static Matrix4x4 RotateYZ(this Matrix4x4 m, float a)
	{
		return m * RotateYZ(a);
	}

	public static Matrix4x4 RotateXZ(this Matrix4x4 m, float a)
	{
		return m * RotateXZ(a);
	}

	public static Matrix4x4 RotateXW(this Matrix4x4 m, float a)
	{
		return m * RotateXW(a);
	}

	public static Matrix4x4 RotateYW(this Matrix4x4 m, float a)
	{
		return m * RotateYW(a);
	}

	public static Matrix4x4 RotateZW(this Matrix4x4 m, float a)
	{
		return m * RotateZW(a);
	}

	public static Matrix4x4 RotateXY(float a)
	{
		float c = Mathf.Cos(a);
		float s = Mathf.Sin(a);
		Matrix4x4 m = new Matrix4x4();
		m.SetColumn(0, new Vector4(c, -s, 0, 0));
		m.SetColumn(1, new Vector4(s, c, 0, 0));
		m.SetColumn(2, new Vector4(0, 0, 1, 0));
		m.SetColumn(3, new Vector4(0, 0, 0, 1));
		return m;
	}

	public static Matrix4x4 RotateYZ(float a)
	{
		float c = Mathf.Cos(a);
		float s = Mathf.Sin(a);
		Matrix4x4 m = new Matrix4x4();
		m.SetColumn(0, new Vector4(1, 0, 0, 0));
		m.SetColumn(1, new Vector4(0, c, -s, 0));
		m.SetColumn(2, new Vector4(0, s, c, 0));
		m.SetColumn(3, new Vector4(0, 0, 0, 1));
		return m;
	}

	public static Matrix4x4 RotateXZ(float a)
	{
		float c = Mathf.Cos(a);
		float s = Mathf.Sin(a);
		Matrix4x4 m = new Matrix4x4();
		m.SetColumn(0, new Vector4(c, 0, s, 0));
		m.SetColumn(1, new Vector4(0, 1, 0, 0));
		m.SetColumn(2, new Vector4(-s, 0, c, 0));
		m.SetColumn(3, new Vector4(0, 0, 0, 1));
		return m;
	}

	public static Matrix4x4 RotateXW(float a)
	{
		float c = Mathf.Cos(a);
		float s = Mathf.Sin(a);
		Matrix4x4 m = new Matrix4x4();
		m.SetColumn(0, new Vector4(c, 0, 0, -s));
		m.SetColumn(1, new Vector4(0, 1, 0, 0));
		m.SetColumn(2, new Vector4(0, 0, 1, 0));
		m.SetColumn(3, new Vector4(s, 0, 0, c));
		return m;
	}

	public static Matrix4x4 RotateYW(float a)
	{
		float c = Mathf.Cos(a);
		float s = Mathf.Sin(a);
		Matrix4x4 m = new Matrix4x4();
		m.SetColumn(0, new Vector4(1, 0, 0, 0));
		m.SetColumn(1, new Vector4(0, c, 0, s));
		m.SetColumn(2, new Vector4(0, 0, 1, 0));
		m.SetColumn(3, new Vector4(0, -s, 0, c));
		return m;
	}

	public static Matrix4x4 RotateZW(float a)
	{
		float c = Mathf.Cos(a);
		float s = Mathf.Sin(a);
		Matrix4x4 m = new Matrix4x4();
		m.SetColumn(0, new Vector4(1, 0, 0, 0));
		m.SetColumn(1, new Vector4(0, 1, 0, 0));
		m.SetColumn(2, new Vector4(0, 0, c, s));
		m.SetColumn(3, new Vector4(0, 0, -s, c));
		return m;
	}
}