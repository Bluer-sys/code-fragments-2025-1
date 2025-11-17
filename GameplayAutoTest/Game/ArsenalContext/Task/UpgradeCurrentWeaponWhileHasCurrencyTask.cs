using Swat.Game.GameplayRobot.Core.Interfaces;
using Swat.Game.GameplayRobot.Core.Task;
using Swat.Game.Utils;
using UnityEngine;

namespace Swat.Game.GameplayRobot.Game.ArsenalContext
{
	public class UpgradeCurrentWeaponWhileHasCurrencyTask : RobotTask
	{
		private const float UpgradesDelay = 0.4f;
		
		private float _lastUpgradeTime;
		
		public UpgradeCurrentWeaponWhileHasCurrencyTask(IGameServiceAdapter game) : base(game)
		{
		}

		public override bool CanExecute()
		{
			return _game.IsEnoughMoneyForCurrentUpgrade();
		}

		public override void Execute()
		{
			if(TimeUtils.IsTimeExpired(_lastUpgradeTime, UpgradesDelay)) 
				return;
			
			_game.ClickUpgradeCurrentWeapon();

			_lastUpgradeTime = Time.time;
		}
	}
}