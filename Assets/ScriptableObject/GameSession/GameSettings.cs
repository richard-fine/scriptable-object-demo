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

		// Serializing an object reference directly to JSON doesn't do what we want - we just get an InstanceID
		// which is not stable between sessions. So instead we serialize the string name of the object, and
		// look it back up again after deserialization
		private TankBrain _cachedBrain;
		public TankBrain Brain
		{
			get
			{
				if (!_cachedBrain && !String.IsNullOrEmpty(BrainName))
				{
					TankBrain[] availableBrains;

					#if UNITY_EDITOR
					// When working in the Editor and launching the game directly from the play scenes, rather than the
					// main menu, the brains may not be loaded and so Resources.FindObjectsOfTypeAll will not find them.
					// Instead, use the AssetDatabase to find them. At runtime, all available brains get loaded by the
					// MainMenuController so it's not a problem outside the editor.
					availableBrains = UnityEditor.AssetDatabase.FindAssets("t:TankBrain")
									.Select(guid => UnityEditor.AssetDatabase.GUIDToAssetPath(guid))
									.Select(path => UnityEditor.AssetDatabase.LoadAssetAtPath<TankBrain>(path))
									.Where(b => b).ToArray();
					#else
					availableBrains = Resources.FindObjectsOfTypeAll<TankBrain>();
					#endif

					_cachedBrain = availableBrains.FirstOrDefault(b => b.name == BrainName);
				}
				return _cachedBrain;
			}
			set
			{
				_cachedBrain = value;
				BrainName = value ? value.name : String.Empty;
			}
		}

		[SerializeField] private string BrainName;

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
