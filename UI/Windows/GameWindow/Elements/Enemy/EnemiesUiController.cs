using System;
using Swat.Game.Entities;
using Swat.Game.Services.BattleService;
using Swat.Game.Services.WindowsService;
using Swat.Game.UI.Windows.GameWindow.Core;
using Zenject;

namespace Swat.Game.UI.Windows.GameWindow.Elements.Enemy
{
	[Serializable]
	public class EnemiesUiController : BaseUiController<EnemiesPanel>, IEnemyUiController
	{
		private IBattleService<BattleContext> _battleService;

		[Inject]
		public void Construct(IBattleService<BattleContext> battleService)
		{
			this._battleService = battleService;

			battleService.OnBattleReady += RefreshCounters;
			battleService.OnEnemyAuthorized += OnEnemyAuthorized;
		}

		
		public void OnEnemyDeath()
		{
			view.SetEnemiesCount(_battleService.DeadEnemiesCount, _battleService.TotalEnemiesCount);
		}


		public override void RefreshViewAfterChangeOrientation()
		{
			base.RefreshViewAfterChangeOrientation();

			if (_battleService.IsBattle)
				RefreshCounters();
		}


		private void RefreshCounters()
		{
			view.SetEnemiesCount(_battleService.DeadEnemiesCount, _battleService.TotalEnemiesCount);
		}

		private void OnEnemyAuthorized(ICharacterEntity entity)
		{
			RefreshCounters();
		}
	}
}