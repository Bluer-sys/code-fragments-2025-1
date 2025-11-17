using System.Collections.Generic;
using Swat.Game.Services.WindowsService;
using UnityEngine;

namespace Swat.Game.UI.Windows.GameWindow.Elements.Buff
{
	public class BuffPanel : BaseWindow
	{
		[SerializeField] private List<BuffElement> _perkContainers = new();


		public void DeactivateAll()
		{
			foreach (var perkContainer in _perkContainers)
			{
				perkContainer.gameObject.SetActive(false);
			}
		}
		
		public void Activate(Sprite sprite)
		{
			foreach (var perkContainer in _perkContainers)
			{
				if(perkContainer.gameObject.activeSelf)
					continue;
				
				perkContainer.SetSprite(sprite);
				perkContainer.gameObject.SetActive(true);
				return;
			}
			
			Debug.LogWarning("Not enough perk containers");
		}
	}
}