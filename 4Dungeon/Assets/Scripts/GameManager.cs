using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

    public InputAction wAction;

    // w-coordinate of global slicing plane
    public float W { get; } = 0;
	[SerializeField] private float minW = -10;
	[SerializeField] private float maxW = 10;

	public float w = 0f;
	public float minW = -10f;
	public float maxW = 10f;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

    private void Update()
    {
        W = wAction.ReadValue<float>();
        SlicingPlane = new Plane4D(SlicingPlane.normal, new Vector4(0, 0, 0, W));
    }
}
