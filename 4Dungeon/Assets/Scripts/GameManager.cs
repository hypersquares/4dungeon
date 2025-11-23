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

    public Plane4D SlicingPlane { get; } = new Plane4D();

	// public enum GameState
	// {
	// 	Playing,
	// 	Paused,
	// 	Completed,
	// }

	// [SerializeField] private GameState currentState = GameState.Playing;	

	// public static event Action<int> OnWChanged;

	// public static event Action<GameState> OnGameStateChanged;

	// public int W => w;

	// public GameState CurrentState => currentState;

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

	// private void OnValidate()
	// {
	// 	w = Mathf.Clamp(w, minW, maxW);

	// 	if (Application.isPlaying)
	// 	{
	// 		OnWChanged?.Invoke(w);
	// 	}
	// }


	// public void SetW(float newValue)
	// {
	// 	float clamped = Mathf.Clamp(newValue, minW, maxW);

	// 	if (w != clamped)
	// 	{
	// 		w = clamped;
	// 		// OnWChanged?.Invoke(w);
	// 	}
	// }

	// public void IncrementW(int amount = 1)
	// {
	// 	SetW(w + amount);
	// }

	// public void DecrementW(int amount = 1)
	// {
	// 	SetW(w - amount);
	// }

	// public void ChangeGameState(GameState newState)
	// {
	// 	if (currentState != newState)
	// 	{
	// 		currentState = newState;
	// 		OnGameStateChanged?.Invoke(currentState);
	// 	}
	// }

	// private void OnDestroy()
	// {
	// 	OnWChanged = null;
	// 	OnGameStateChanged = null;
	// }
}

