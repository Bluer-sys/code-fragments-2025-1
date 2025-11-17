using Swat.Game.Entities;
using Swat.Game.Entities.Core.Environment;
using UnityEngine;

namespace Swat.Game.GameplayRobot.Core.Interfaces
{
	public interface IGameServiceAdapter
	{
		bool CanAimAtEnemies { get; }
		bool CanShootAtEnemies { get; }
		float PlayerHealthPercent { get; }
		bool IsPlayerAlive { get; }

		void ClickReviveOrRetry();
		void ClickStartLevel();
		void ClickGoToArsenal();
		void ClickUpgradeCurrentWeapon();
		void GetRewardAndContinue();
		bool IsEnoughMoneyForCurrentUpgrade();

		ICharacterEntity FindTarget();
		IEnvironmentEntity FindEnvironmentTarget();
		void AimAtTarget(ICharacterEntity target);
		void AimAtTarget(Vector3 targetPosition);
		int ShootEnemiesNowCount();
		
		bool CalculateAndSetBestWeapon(ICharacterEntity enemyEntity);
		Transform CalculateBestShootTarget(ICharacterEntity enemyEntity);
		
		void UpgradeWeaponsToCurrentLevelPower();
		void ResetHealthAmmoAndBattlePoints();
		
		void EnterAimState();
		void ExitAimState();
		void SetAutoAimLock(bool isLocked);
		void TryUseMedKit();
		bool IsHealingProcess();
		bool IsMeaningfulTarget(ICharacterEntity entity);
	}
}