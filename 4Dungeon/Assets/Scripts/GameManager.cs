using UnityEngine;
using System;

[ExecuteInEditMode]
public class GameManager : MonoBehaviour
{
	private static GameManager s_Instance;

	public static GameManager Instance
	{
		get
		{
			// Lazy initialization handles OnValidate() running before Awake()
			if (s_Instance == null)
			{
				s_Instance = FindAnyObjectByType<GameManager>();
			}
			return s_Instance;
		}
		private set => s_Instance = value;
	}

	[SerializeField] private float minW = -10f;
	[SerializeField] private float maxW = 10f;
    [SerializeField] public Plane4D slicingPlane = new Plane4D();
    public Vector4 SlicingPlaneNormal { get => slicingPlane.normal; set => slicingPlane.normal = value; }
    public float SlicingPlaneOffset { get => slicingPlane.offset; set => slicingPlane.offset = Mathf.Clamp(value, minW, maxW); }
    public Vector4 SlicingPlanePoint { get => slicingPlane.point; }

	private void Awake()
	{
		if (s_Instance == null || s_Instance == this)
		{
			s_Instance = this;
#if !UNITY_EDITOR
			DontDestroyOnLoad(gameObject);
#endif
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void OnDestroy()
	{
		if (s_Instance == this)
		{
			s_Instance = null;
		}
	}
}
