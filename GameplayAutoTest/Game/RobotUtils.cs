using Swat.Game.Entities;
using Swat.Game.Entities.Core.Characters.AI.Views;
using Swat.Game.Entities.Core.Environment;
using Swat.Game.Entities.Core.Environment.Views;
using UnityEngine;

namespace Swat.Game.GameplayRobot.Game
{
	public static class RobotUtils
	{
		public static bool IsEntityDead(ICharacterEntity entity)
		{
			entity.CurrentExecutionContext.TryResolveView(out IAiLiveView liveView);
			return !liveView.IsAlive;
		}

		public static bool IsEntityDead(IEnvironmentEntity entity)
		{
			entity.CurrentExecutionContext.TryResolveView(out IEnvironmentLiveView liveView);
			return !liveView.IsAlive;
		}

		public static Vector3 GetEnemyHeadPosition(ICharacterEntity entity)
		{
			entity.TryGetEntityView(out IAiTargetSelectionView targetSelectionView);
			return targetSelectionView.HeadTransform.position;
		}

		public static Vector3 GetEnemyPelvisPosition(ICharacterEntity entity)
		{
			entity.TryGetEntityView(out IAiTargetSelectionView targetSelectionView);
			return targetSelectionView.PelvisTransform.position;
		}
		
		public static Transform GetEnemyHead(ICharacterEntity entity)
		{
			entity.TryGetEntityView(out IAiTargetSelectionView targetSelectionView);
			return targetSelectionView.HeadTransform;
		}

		public static Transform GetEnemyPelvis(ICharacterEntity entity)
		{
			entity.TryGetEntityView(out IAiTargetSelectionView targetSelectionView);
			return targetSelectionView.PelvisTransform;
		}

		public static Transform GetEnemyChest(ICharacterEntity entity)
		{
			entity.TryGetEntityView(out IAiTargetSelectionView targetSelectionView);
			return targetSelectionView.ChestTransform;
		}
	}
}