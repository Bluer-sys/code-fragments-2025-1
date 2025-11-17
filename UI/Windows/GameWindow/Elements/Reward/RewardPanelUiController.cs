using System;
using Swat.Game.Services.CurrencyService.Core;
using Swat.Game.Services.WindowsService;
using Swat.Game.UI.Windows.GameWindow.Core;
using Zenject;

namespace Swat.Game.UI.Windows.GameWindow.Elements.Reward
{
	[Serializable]
	public class RewardPanelUiController : BaseUiController<RewardPanel>, IRewardUiController
	{
		private ICurrencyService currencyService;

        [Inject]
        private void Construct(ICurrencyService currencyService)
        {
            this.currencyService = currencyService;

            currencyService.OnCurrencyValueChange += OnCurrencyValueChange;
        }
		public override void OnShowBegin()
		{
			base.OnShowBegin();

			OnCurrencyValueChange(CurrencyType.Money);
		}

        public override void RefreshViewAfterChangeOrientation()
        {
            base.RefreshViewBeforeChangeOrientation();

            OnCurrencyValueChange(CurrencyType.Money);
        }

		public void OnMoneyChanged(float money)
		{
			view.SetMoney(money);
		}

		private void OnCurrencyValueChange(CurrencyType type)
		{
			float value = currencyService.GetCurrencyValue(CurrencyType.Money);

			OnMoneyChanged(value);
		}
	}
}
