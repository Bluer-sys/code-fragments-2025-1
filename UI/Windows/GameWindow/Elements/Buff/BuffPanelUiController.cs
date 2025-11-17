using System;
using Swat.Game.Services.AddressableService.Core;
using Swat.Game.Services.BuffService.Core;
using Swat.Game.Services.BuffService.Data;
using Swat.Game.Services.PerkService.Core;
using Swat.Game.Services.WindowsService;
using Swat.Game.UI.Windows.GameWindow.Core;
using UnityEngine;
using Zenject;

namespace Swat.Game.UI.Windows.GameWindow.Elements.Buff
{
	[Serializable]
	public class BuffPanelUiController : BaseUiController<BuffPanel>, IBuffPanelUiController
	{
		private IBuffService _buffService;
		private IAddressableService _addressableService;


		[Inject]
		private void Construct(IPerkService perkService, IBuffService buffService, IAddressableService addressableService)
		{
			_addressableService = addressableService;
			_buffService = buffService;

			_buffService.OnActiveBuffsChanged += RefreshBuffs;
		}

		public override void OnShowBegin()
		{
			base.OnShowBegin();

			RefreshBuffs();
		}

		public override void RefreshViewAfterChangeOrientation()
		{
			base.RefreshViewBeforeChangeOrientation();

			RefreshBuffs();
		}

		private void RefreshBuffs()
		{
			view.DeactivateAll();
			
			foreach (var buff in _buffService.ActiveBuffs)
				view.Activate(_addressableService.Load<Sprite>(buff.Config.IconReference));
		}
	}
}