using System;
using Unity.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class Transform4D : MonoBehaviour
{
	[Header("Mesh4D")]
	
	public Mesh4D mesh4D;   // drag your Cube4D.asset here in the Inspector
	[ReadOnly]
	public Vector4[] vertices;   // working copy after transforms

	[Header("Transform4D")]
	public Euler4 Rotation;
	public Vector4 Position;
	public Vector4 Scale = Vector4.one;
	[Header("Rotation")]
	[ReadOnly]
	private Matrix4x4 RotationMatrix;

	void Start()
	{
		RefreshMesh();
	}

	/// <summary>
	/// Reinitializes the vertices array and updates transforms. Call this when mesh4D changes.
	/// </summary>
	public void RefreshMesh()
	{
		if (mesh4D != null)
		{
			// Instantiates the vertices
			vertices = new Vector4[mesh4D.Vertices.Length];

			// Updates the mesh
			UpdateRotationMatrix();
			UpdateVertices();
		}
	}

	private void Update()
	{
		if (mesh4D != null)
		{
			UpdateRotationMatrix();
			UpdateVertices();
		}
	}

	private void UpdateVertices()
	{
		vertices = Transform(mesh4D.Vertices);
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

	private Vector4[] Transform(Vector4[] v)
	{
		for (int i = 0; i < v.Length; i++)
		{
			v[i] = Transform(v[i]);
		}
		return v; 
	}

    // Projects a 4D point to 3D space by dropping the W component
    public Vector3 ProjectTo3D(Vector4 v) {
		return v;
	}
}