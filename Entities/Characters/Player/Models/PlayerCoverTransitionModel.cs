using DG.Tweening;
using Swat.Data;
using Swat.Game.Entities.Core.Characters.Player.Models;
using UnityEngine;

namespace Swat.Game.Entities.Characters.Player.Models
{
	public class PlayerCoverTransitionModel : BaseModel, IPlayerCoverTransitionModel
	{
		[Header("Cover Override")]
		[field: SerializeField] public float CoverOverrideRotationTweenDuration { get; set; }
		[field: SerializeField] public Ease CoverOverrideRotationTweenEase { get; set; }
		
		public PlayerCoverData CurrentCoverData { get; set; }
		public bool IsCoverDestructed { get; set; }
		
		public bool HasCover => !IsCoverDestructed 
								&& CurrentCoverData != null 
								&& CurrentCoverData.HasCover;
	}
}