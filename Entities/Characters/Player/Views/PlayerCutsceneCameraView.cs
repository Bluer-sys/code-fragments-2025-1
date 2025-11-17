using System;
using Swat.Game.Entities.Core.Characters.Player.Views;
using UnityEngine;

namespace Swat.Game.Entities.Characters.Player.Views
{
	[Serializable]
	public class PlayerCutsceneCameraView : BaseView, IPlayerCutsceneCameraView
	{
		[field: SerializeField] public Transform Root { get; private set; }
		[field: SerializeField] public Transform CameraRoot { get; private set; }
	}
}