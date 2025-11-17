using Swat.Game.Services.WindowsService;
using TMPro;
using UnityEngine;

namespace Swat.Game.UI.Windows.GameWindow.Elements.Reward
{
	public class RewardPanel : BaseWindow
	{
		[SerializeField] private TextMeshProUGUI moneyText;


		public void SetMoney(float money)
		{
			moneyText.text = $"<color=#3EFC3E>$</color> {money}";
		}
	}
}