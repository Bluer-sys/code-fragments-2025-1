using Swat.Game.Entities;
using Swat.Game.Entities.Core.Environment;
using UnityEngine;

namespace Swat.Game.GameplayRobot.Game.GameContext
{
	public class GameContextBlackboard
	{
		public ICharacterEntity EnemyTarget { get; set; }
		public Transform EnemyShootTarget { get; set; }
		
		public IEnvironmentEntity EnvironmentTarget { get; set; }
	}
}