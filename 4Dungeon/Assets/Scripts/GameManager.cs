using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	public enum GameState
	{
		Playing,
		Paused,
		Completed,
	}

	[SerializeField] private GameState currentState = GameState.Playing;

	public float w = 0f;

	public float minW = -10f;
	public float maxW = 10f;


	public static event Action<float> OnWChanged;

	public static event Action<GameState> OnGameStateChanged;

	public float W => w;

	public GameState CurrentState => currentState;

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

	public void SetW(float newValue)
	{
        float clamped = Mathf.Clamp(newValue, minW, maxW);

		if (w != clamped)
		{
			w = clamped;
            OnWChanged?.Invoke(w);
		}
	}

	public void ChangeGameState(GameState newState)
	{
		if (currentState != newState)
		{
			currentState = newState;
			OnGameStateChanged?.Invoke(currentState);
		}
	}

	private void OnDestroy()
	{
		OnWChanged = null;
		OnGameStateChanged = null;
	}
}

