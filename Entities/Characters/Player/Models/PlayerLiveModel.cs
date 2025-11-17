using System;
using Swat.Game.Entities.Core.Characters.Player.Models;
using UnityEngine;

namespace Swat.Game.Entities.Characters.Player.Models
{
	[Serializable]
	public class PlayerLiveModel : BaseModel, IPlayerLiveModel
	{
		[field: SerializeField]
		[field: Range(0.0f, 1.0f)]
		public float HitDecreasePercent { get; private set; }

		[field: SerializeField]
		[field: Tooltip("Delay before mission complete, after reborn while all enemies dead")]
		public float DelayAfterRebornWithAllEnemiesDead { get; private set; } = 3f;

		public float BaseHealth { get; private set; }

		public void InitializeBaseHealth(float health)
		{
			BaseHealth = health;
		}
	}
}