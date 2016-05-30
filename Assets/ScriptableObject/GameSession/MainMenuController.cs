using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
	public GameSettings GameSettingsTemplate;

	public Color[] AvailableColors;
	public TankBrain[] AvailableTankBrains;

	public UnityEngine.UI.Button PanelSwitcher;
	public GameObject PlayersPanel;
	public GameObject SettingsPanel;

	public string SavedSettingsPath {
		get {
			return System.IO.Path.Combine(Application.persistentDataPath, "tanks-settings.json");
		}
	}

	void Start () {
		if (System.IO.File.Exists(SavedSettingsPath))
			GameSettings.LoadFromJSON(SavedSettingsPath);
		else
			GameSettings.InitializeFromDefault(GameSettingsTemplate);

		foreach(var info in GetComponentsInChildren<PlayerInfoController>())
			info.Refresh();

		NumberOfRoundsSlider.value = GameSettings.Instance.NumberOfRounds;
	}

	public void Play()
	{
		GameSettings.Instance.SaveToJSON(SavedSettingsPath);
		GameState.CreateFromSettings(GameSettings.Instance);
		SceneManager.LoadScene(1, LoadSceneMode.Single);
	}

	public Color GetNextColor(Color color)
	{
		int existingColor = Array.FindIndex(AvailableColors, c => c == color);
		existingColor = (existingColor + 1)%AvailableColors.Length;
		return AvailableColors[existingColor];
	}

	public TankBrain GetNextBrain(TankBrain brain)
	{
		if (brain == null)
			return AvailableTankBrains[0];

		int index = Array.FindIndex(AvailableTankBrains, b => b == brain);
		index++;
		return (index < AvailableTankBrains.Length) ? AvailableTankBrains[index] : null;
	}

	public UnityEngine.UI.Text NumberOfRoundsLabel;
	public UnityEngine.UI.Slider NumberOfRoundsSlider;

	public void OnChangeNumberOfRounds(float value)
	{
		GameSettings.Instance.NumberOfRounds = (int) value;

		NumberOfRoundsLabel.text = GameSettings.Instance.NumberOfRounds.ToString();
	}

	public void OnSwitchPanels()
	{
		PlayersPanel.SetActive(!PlayersPanel.activeSelf);
		SettingsPanel.SetActive(!SettingsPanel.activeSelf);
		PanelSwitcher.GetComponentInChildren<UnityEngine.UI.Text>().text = PlayersPanel.activeSelf ? "Settings" : "Players";
	}
}
