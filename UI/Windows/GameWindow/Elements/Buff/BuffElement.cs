using Swat.Game.Services.WindowsService;
using UnityEngine;
using UnityEngine.UI;

namespace Swat.Game.UI.Windows.GameWindow.Elements.Buff
{
	public class BuffElement : BaseWindow
	{
		[SerializeField] private Image _perkImage;

		public void SetSprite(Sprite sprite)
		{
			_perkImage.sprite = sprite;
		}
	}
}