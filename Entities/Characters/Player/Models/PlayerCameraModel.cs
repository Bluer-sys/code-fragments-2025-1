using Swat.Game.Entities.Core.Characters.Player.Models;
using UnityEngine;

namespace Swat.Game.Entities.Characters.Player.Models
{
	public class PlayerCameraModel : BaseModel, IPlayerCameraModel
	{
		public float CurrentStepImpulseAmplitudeMultiplier { get; set; }
		
		public float CameraHeightAddon { get; set; }
		public float FovAddon { get; set; }
	}
}