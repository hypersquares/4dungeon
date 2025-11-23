using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class Transform4D : MonoBehaviour
{
	[Header("Mesh4D")]
	
	public Mesh4D mesh4D;   // drag your Cube4D.asset here in the Inspector
	
	[ReadOnly]
	public Vector4[] vertices;

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

	/// <summary>
	/// Returns a new vector4 representing the input transformed by the current position and rotation, and scale.
	/// </summary>
	/// <param name="v"></param>
	/// <returns></returns>
	private Vector4 Transform(Vector4 v)
	{
		Vector4 tmp = RotationMatrix * v;

		tmp.x *= Scale.x;
		tmp.y *= Scale.y;
		tmp.z *= Scale.z;
		tmp.w *= Scale.w;

		tmp += Position;

		return tmp;
	}
	/// <summary>
	/// Returns a new Vector4 array representing the input transformed by the current position and rotation, and scale.
	/// Does not modify the input array.
	/// </summary>
	/// <param name="v"></param>
	/// <returns></returns>
	public Vector4[] Transform(Vector4[] v)
	{
		Vector4[] arr = new Vector4[v.Length];
		for (int i = 0; i < v.Length; i++)
		{
			arr[i] = Transform(v[i]);
		}
		return arr;
	}

    // Projects a 4D point to 3D space by dropping the W component
    public Vector3 ProjectTo3D(Vector4 v) {
		return v;
	}
}