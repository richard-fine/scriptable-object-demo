using UnityEngine;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{

	public UnityEngine.UI.Text winnerLabel;
	public UnityEngine.UI.Text scores;

	public void OnBackToMainMenu()
	{
		DestroyImmediate(GameState.Instance);
		SceneManager.LoadScene(0);
	}

	public void Start()
	{
		var winner = GameSettings.Instance.GetGameWinner();
		if (winner != null)
		{
			winnerLabel.text = string.Format("<color=#{0}>{1}</color> is the winner!",
				ColorUtility.ToHtmlStringRGBA(winner.PlayerInfo.Color), winner.PlayerInfo.Name);
		}
		else
		{
			winnerLabel.text = "It's a draw!";
		}

		var sb = new StringBuilder();
		foreach (var player in GameState.Instance.players.OrderByDescending(p => p.TotalWins))
		{
			sb.AppendFormat("<color=#{0}>{1} {2} wins</color>\n", ColorUtility.ToHtmlStringRGBA(player.PlayerInfo.Color),
				player.PlayerInfo.Name, player.TotalWins);
		}
		scores.text = sb.ToString();
	}
}
