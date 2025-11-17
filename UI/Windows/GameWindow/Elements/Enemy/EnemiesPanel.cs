using Swat.Game.Services.WindowsService;
using TMPro;
using UnityEngine;



namespace Swat.Game.UI.Windows.GameWindow.Elements.Enemy
{
	public class EnemiesPanel : BaseWindow
	{
		[SerializeField] private TMP_Text enemiesCountDeathText;
		[SerializeField] private TMP_Text enemiesCountTotalText;


		public void SetEnemiesCount(int deathEnemiesCount, int totalEnemiesCount)
		{
			enemiesCountDeathText.text = deathEnemiesCount.ToString();
			enemiesCountTotalText.text = $"/{totalEnemiesCount}";
		}
	}
}