using UnityEngine;
using System;

[ExecuteInEditMode]
public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[SerializeField] private float minW = -10f;
	[SerializeField] private float maxW = 10f;
    [SerializeField] private Plane4D slicingPlane = new Plane4D();
    public Vector4 SlicingPlaneNormal { get => slicingPlane.normal; set => slicingPlane.normal = value; }
    public float SlicingPlaneOffset { get => slicingPlane.offset; set => slicingPlane.offset = Mathf.Clamp(value, minW, maxW); }
    public Vector4 SlicingPlanePoint { get => slicingPlane.point; }

	private void Start()
	{
		if (Instance == null)
		{
			Instance = this;
#if !UNITY_EDITOR
			DontDestroyOnLoad(gameObject);
#endif
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
