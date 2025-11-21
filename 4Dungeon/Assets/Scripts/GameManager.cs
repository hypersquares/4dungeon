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

	public int w = 0;

	public int minW = -10;
	public int maxW = 10;


	public static event Action<int> OnWChanged;

	public static event Action<GameState> OnGameStateChanged;

	public int W => w;

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

	private void OnValidate()
	{
		w = Mathf.Clamp(w, minW, maxW);

		if (Application.isPlaying)
		{
			OnWChanged?.Invoke(w);
		}
	}


	public void SetW(int newValue)
	{
		int clamped = Mathf.Clamp(newValue, minW, maxW);

		if (w != clamped)
		{
			w = clamped;
			OnWChanged?.Invoke(w);
		}
	}


	public void IncrementW(int amount = 1)
	{
		SetW(w + amount);
	}

	public void DecrementW(int amount = 1)
	{
		SetW(w - amount);
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

