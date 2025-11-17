using System;
using Swat.Game.Services.AudioService;
using Swat.Game.Services.WindowsService.Core.Common;
using UnityEngine;

namespace Swat.Game.Services.WindowsService.Windows
{
	[Serializable]
	public class GameWindowUiModel : IUiModel
	{
		[field: SerializeField] public SoundId[] StartLevelTalking { get; private set; }
		[field: SerializeField] public SoundId[] StartLevelTalkingChase { get; private set; }
		[field: SerializeField] public SoundId[] StartLevelTalkingSniper { get; private set; }
		[field: SerializeField] public SoundId[] VictoryLevelTalking { get; private set; }
		[field: SerializeField] public float StartTalkingDelay { get; private set; }
	}
}