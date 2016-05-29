using UnityEngine;
using System.Collections;

public class PlayerInfoController : MonoBehaviour
{
	public int PlayerIndex;
	public UnityEngine.UI.InputField PlayerName;
	public UnityEngine.UI.Button PlayerColor;
	public UnityEngine.UI.Button PlayerBrain;

	private MainMenuController _mainMenu;
	private GameSettings.PlayerInfo _player;
	public void Awake()
	{
		_mainMenu = GetComponentInParent<MainMenuController>();
	}

	public void Refresh()
	{
		_player = GameSettings.Instance.players[PlayerIndex];

		PlayerName.text = _player.Name;

		var colorBlock = PlayerColor.colors;
		colorBlock.normalColor = _player.Color;
		colorBlock.highlightedColor = _player.Color;
		PlayerColor.colors = colorBlock;

		PlayerBrain.GetComponentInChildren<UnityEngine.UI.Text>().text = (_player.Brain != null)
			? _player.Brain.name
			: "None";
	}

	public void OnNameChanged()
	{
		_player.Name = PlayerName.text;
	}

	public void OnCycleColor()
	{
		_player.Color = _mainMenu.GetNextColor(_player.Color);
		Refresh();
	}

	public void OnCycleBrain()
	{
		_player.Brain = _mainMenu.GetNextBrain(_player.Brain);
		Refresh();
	}
}
