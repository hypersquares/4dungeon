using System;
using UnityEngine;

/// <summary>
/// Represents a plane in 4D space defined by a normal vector, a base point, and an offset.
/// </summary>
[Serializable]
public class Plane4D
{
    [SerializeField]
    [Tooltip("The normal vector of the plane in 4D space")]
    private Vector4 m_Normal = new Vector4(0, 0, 0, 1);

    [SerializeField]
    [Tooltip("A base point on the plane before offset is applied")]
    private Vector4 m_BasePoint = Vector4.zero;

    [SerializeField]
    [Tooltip("Offset distance along the normal from the base point")]
    private float m_Offset = 0f;

    /// <summary>
    /// The normal vector of the plane in 4D space.
    /// </summary>
    public Vector4 normal
    {
        get => m_Normal;
        set => m_Normal = value.normalized;
    }

    /// <summary>
    /// The offset distance along the normal from the base point.
    /// </summary>
    public float offset
    {
        get => m_Offset;
        set => m_Offset = value;
    }

    /// <summary>
    /// Returns the actual point on the plane (base point + normal * offset).
    /// This is read-only; modify basePoint or offset to change the plane position.
    /// </summary>
    public Vector4 point
    {
        get => m_BasePoint + m_Normal.normalized * m_Offset;
    }

    public Plane4D()
    {
        m_Normal = new Vector4(0, 0, 0, 1);
        m_BasePoint = Vector4.zero;
        m_Offset = 0f;
    }

    public Plane4D(Vector4 normal, Vector4 basePoint, float offset = 0f)
    {
        m_Normal = normal.normalized;
        m_BasePoint = basePoint;
        m_Offset = offset;
    }
}
