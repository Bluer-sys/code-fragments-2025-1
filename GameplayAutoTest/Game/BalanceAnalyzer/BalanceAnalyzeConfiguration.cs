using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Swat.Game.Entities.Weapons.Core;
using Swat.Game.GameplayRobot.Core.Data;
using Swat.Game.Services.LevelService;
using Swat.Game.Utils;
using UnityEngine;

namespace Swat.Game.GameplayRobot.Game.BalanceAnalyzer
{
	[CreateAssetMenu(fileName = "BalanceAnalyzerConfiguration", menuName = "Configs/BalanceAnalyzerConfiguration")]
	public class BalanceAnalyzeConfiguration : RobotConfiguration
	{
		[field: SerializeField] public int CycleCount { get; private set; }
		
		[field: SerializeField] 
		[field: ValueDropdown(nameof(GetLevelsNames))] 
		public string StartLevelName { get; private set; }

		[field: SerializeField]
		[field: ValueDropdown(nameof(GetLevelsNames))]
		public string EndLevelName { get; private set; }

		[field: SerializeField]
		public List<Preset> Presets { get; private set; }

		private string[] GetLevelsNames()
		{
#if UNITY_EDITOR
			var ldb = EditorExtras.FindSingleAsset<LevelDatabase>();

			return ldb != null 
				? ldb.Stages.SelectMany(x => x.LevelsMap.Keys).ToArray() 
				: Array.Empty<string>();
#else
			return Array.Empty<string>();
#endif
		}

		[Serializable]
		public class Preset
		{
			[field: SerializeField] public WeaponType PrimaryWeaponType { get; set; }
			[field: SerializeField] public WeaponType SecondaryWeaponType { get; set; }
		}
	}
}