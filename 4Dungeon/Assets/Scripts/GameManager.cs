using UnityEngine;
using System;

[ExecuteInEditMode]
public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[SerializeField] private float minW = -10f;
	[SerializeField] private float maxW = 10f;
    public Plane4D slicingPlane = new Plane4D();

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
}
