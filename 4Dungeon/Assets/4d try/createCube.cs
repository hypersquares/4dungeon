using UnityEngine;

public class Cube4DInitializer : MonoBehaviour
{
	public Mesh4D cube4DAsset;

	[ContextMenu("Init Cube4D")]
	private void Init()
	{
		if (cube4DAsset != null)
		{
			Mesh4D.CreateCube4(cube4DAsset, -1, 1, -1, 1, -1, 1, -1, 1);
			Debug.Log("Initialized Cube4D with tesseract geometry!");
		}
	}

	public Mesh4D convexHull;

	[ContextMenu("Create Convex Hull")]
	private void CreateConvexHull()
	{
		if (convexHull != null)
		{
			var points = new Vector4[]
			{
				new Vector4(1,1,1,1),
				new Vector4(1,1,-1,1),
				new Vector4(1,-1,1,1),
				new Vector4(1,-1,-1,1),
			};
			Mesh4D.ConstructConvexHull(convexHull, points);
			Debug.Log("Constructed Convex Hull in 4D!");
		}
	}

	public Mesh4D tesseract;
	[ContextMenu("Create Tesseract")]
	private void CreateTesseract()
	{
		if (tesseract != null)
		{
			Mesh4D.CreateTesseract(tesseract, -1, 1, -1, 1, -1, 1, -1, 1);
			Debug.Log("Created Tesseract in 4D!");
		}
	}

	public Mesh4D sphere4D;

	[ContextMenu("Create Sphere4D")]
	private void CreateSphere4D()
	{
		if (sphere4D != null)
		{
			Mesh4D.CreateSphere4(sphere4D, radius: 1f, resolution: 6);
			Debug.Log("Created Sphere4D in 4D!");
		}
	}

}
