using Swat.Game.Entities.Core.Characters.AI.Models;
using UnityEngine;

namespace Swat.Game.Entities.Characters.AI.Models
{
	public class AiLayeredArmorModel : BaseModel, IAiLayeredArmorModel
	{
		[field: SerializeField] public bool HideLayerOnBroken { get; private set; }

		[Header("Break Layer Slow Motion")]
		[field: SerializeField]
		public float SlowMotionStartDelay { get; private set; }

		[field: SerializeField] public float SlowMotionTimeScale { get; private set; }
		[field: SerializeField] public float SlowMotionFixedTimeScale { get; private set; }
		[field: SerializeField] public float SlowMotionDuration { get; private set; }
		[field: SerializeField] public float[] LayersBaseHealth { get; set; }
		
		public int CurrentLayerIndex { get; set; }
		public float CurrentLayerHealth { get; set; }


		public void ResetHealth()
		{
			if (LayersBaseHealth != null && CurrentLayerIndex >= 0 && CurrentLayerIndex < LayersBaseHealth.Length)
				CurrentLayerHealth = LayersBaseHealth[CurrentLayerIndex];
		}


		public void DecreaseHealth(float value)
		{
			CurrentLayerHealth -= value;
		}


		public bool IsHealthOver()
		{
			return CurrentLayerHealth <= 0;
		}
	}
}