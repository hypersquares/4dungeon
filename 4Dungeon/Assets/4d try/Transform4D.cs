using System;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Stores and applies 4D transform data (rotation, position, scale).
/// Does not own mesh data - that belongs to MeshRenderer4D.
/// </summary>
[ExecuteInEditMode]
public class Transform4D : MonoBehaviour
{
	[Header("Transform4D")]
	[SerializeField, HideInInspector] private string m_Label = "";
	public Euler4 Rotation;
	public Vector4 Position;
	public Vector4 Scale = Vector4.one;

	/// <summary>
	/// Optional label to identify this Transform4D in the inspector.
	/// </summary>
	public string label
	{
		get => m_Label;
		set => m_Label = value;
	}
	
	private Matrix4x4 m_RotationMatrix;

	private void Start()
	{
		UpdateRotationMatrix();
	}

	private void Update()
	{
		UpdateRotationMatrix();
		if (gameObject.transform.hasChanged && !gameObject.transform.worldToLocalMatrix.Equals(Matrix4x4.identity))
        {
			gameObject.transform.hasChanged = false;
            Debug.LogError("Node with Transform4D component has a non-identity Transform3D -- make sure to fix this. ");
		}
	}

	private void UpdateRotationMatrix()
	{
		m_RotationMatrix =
			Matrix4x4.identity
			.RotateXY(Rotation.XY * Mathf.Deg2Rad)
			.RotateYZ(Rotation.YZ * Mathf.Deg2Rad)
			.RotateXZ(Rotation.XZ * Mathf.Deg2Rad)
			.RotateXW(Rotation.XW * Mathf.Deg2Rad)
			.RotateYW(Rotation.YW * Mathf.Deg2Rad)
			.RotateZW(Rotation.ZW * Mathf.Deg2Rad);
	}

	/// <summary>
	/// Returns a new Vector4 representing the input transformed by the current position, rotation, and scale.
	/// </summary>
	public Vector4 Transform(Vector4 v)
	{
		Vector4 tmp = m_RotationMatrix * v;

		tmp.x *= Scale.x;
		tmp.y *= Scale.y;
		tmp.z *= Scale.z;
		tmp.w *= Scale.w;

		tmp += Position;

		return tmp;
	}

	/// <summary>
	/// Returns a new Vector4 array representing the input transformed by the current position, rotation, and scale.
	/// Does not modify the input array.
	/// </summary>
	public Vector4[] Transform(Vector4[] v)
	{
		Vector4[] arr = new Vector4[v.Length];
		for (int i = 0; i < v.Length; i++)
		{
			arr[i] = Transform(v[i]);
		}
		return arr;
	}

	/// <summary>
	/// Projects a 4D point to 3D space by dropping the W component.
	/// </summary>
	public Vector3 ProjectTo3D(Vector4 v)
	{
		return v;
	}
}
