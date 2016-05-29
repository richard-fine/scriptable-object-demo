using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
	[Serializable]
	public class PlayerInfo
	{
		public string Name;
		public Color Color;
		public TankBrain Brain;

		public string GetColoredName()
		{
			return "<color=#" + ColorUtility.ToHtmlStringRGBA(Color) + ">" + Name + "</color>";
		}
	}

	public List<PlayerInfo> players;

	private static GameSettings _instance;
	public static GameSettings Instance
	{
		get
		{
			if (!_instance)
				_instance = FindObjectOfType<GameSettings>();
#if UNITY_EDITOR
			if (!_instance)
				_instance = Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameSettings>("Assets/Test game settings.asset"));
#endif
			return _instance;
		}
	}

	public int NumberOfRounds;

	public static void InitializeFromDefault(GameSettings settings)
	{
		if (_instance) DestroyImmediate(_instance);
		_instance = Instantiate(settings);
	}

#if UNITY_EDITOR
	[UnityEditor.MenuItem("Window/Game Settings")]
	public static void ShowGameSettings()
	{
		UnityEditor.Selection.activeObject = Instance;
	}
#endif

	public bool ShouldFinishGame()
	{
		return GameState.Instance.RoundNumber >= NumberOfRounds;
	}

	public void OnBeginRound()
	{
		++GameState.Instance.RoundNumber;
	}

	public TankThinker OnEndRound()
	{
		// Return the winner of the round, if there is one
		var winner = GameState.Instance.players.FirstOrDefault(t => t.IsAlive);

		if (winner != null)
			winner.TotalWins++;

		return winner != null ? winner.Tank : null;
	}

	public bool ShouldFinishRound()
	{
		return GameState.Instance.players.Count(p => p.IsAlive) <= 1;
	}

	public GameState.PlayerState GetGameWinner()
	{
		return GameState.Instance.GetPlayerWithMostWins();
	}
}
