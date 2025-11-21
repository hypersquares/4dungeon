using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Mesh4D", menuName = "Scriptable Objects/Mesh4D")]
public class Mesh4D : ScriptableObject
{
	public Vector4[] Vertices;
	public Edge[] Edges;
}