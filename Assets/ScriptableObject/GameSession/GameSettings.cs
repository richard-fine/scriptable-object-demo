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
				_instance = Resources.FindObjectsOfTypeAll<GameSettings>().FirstOrDefault();
#if UNITY_EDITOR
			if (!_instance)
				InitializeFromDefault(UnityEditor.AssetDatabase.LoadAssetAtPath<GameSettings>("Assets/Test game settings.asset"));
#endif
			return _instance;
		}
	}

	public int NumberOfRounds;

	public static void LoadFromJSON(string path)
	{
		if (!_instance) DestroyImmediate(_instance);
		_instance = ScriptableObject.CreateInstance<GameSettings>();
		JsonUtility.FromJsonOverwrite(System.IO.File.ReadAllText(path), _instance);
		_instance.hideFlags = HideFlags.HideAndDontSave;
	}

	public void SaveToJSON(string path)
	{
		Debug.LogFormat("Saving game settings to {0}", path);
		System.IO.File.WriteAllText(path, JsonUtility.ToJson(this, true));
	}

	public static void InitializeFromDefault(GameSettings settings)
	{
		if (_instance) DestroyImmediate(_instance);
		_instance = Instantiate(settings);
		_instance.hideFlags = HideFlags.HideAndDontSave;
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
