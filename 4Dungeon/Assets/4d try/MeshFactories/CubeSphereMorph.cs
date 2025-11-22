using UnityEngine;

[ExecuteAlways]
public class CubeSphereMorph : MonoBehaviour
{
	public Mesh4D morphAsset;
	[Range(0, 1)] public float morphFactor = 0f;
	public float radius = 1f;

	public void UpdateMorph()
	{
		if (morphAsset != null)
		{
			Cube4DInitializer.CreateCubeSphereMorph(morphAsset, radius: 1f, resolution: 5, t: morphFactor);
		}
	}

	private void Update()
	{
		UpdateMorph(); // works in Play and Edit Mode
	}

	private void OnValidate()
	{
		UpdateMorph(); // runs when inspector values change
	}
}
