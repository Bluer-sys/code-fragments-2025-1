using System;
using System.Collections.Generic;
using System.Linq;
using DevMenu.Common;
using GoogleSheetsWrapper;
using SafeConvert;
using Sirenix.OdinInspector;
using Swat.Game.Data.Weapon;
using Swat.Game.Entities.Core.Characters.AI.Models;
using Swat.Game.GoogleSheets;
using Swat.Game.GoogleSheets.Core;
using Swat.Game.GoogleSheets.Data;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Swat.Game.Entities.Characters.AI.Models
{
	[Serializable]
	public partial class AiLiveModel : BaseModel, IAiLiveModel
	{
		public event Action<BoneType> OnReceiveHit;
		public event Action OnDeath;
		
		
		[field: SerializeField] public float BaseHealth { get; private set; }
		[field: SerializeField] public int DeathSoundEveryEnemiesCount { get; private set; }
		[field: SerializeField] public float DeathCallbackDelay { get; private set; }
		[field: SerializeField] public SerializableDictionary<BoneType, float> DamageMultiplierMap { get; private set; }
		[field: Range(0.0f, 1.0f), SerializeField]
		public float PlayerAccuracy { get; private set; } = 0.7f;
		[field: Range(0.0f, 1.0f), SerializeField]
		public float PlayerHeadShotChance { get; private set; } = 0.6f;
		
		
		[field: SerializeField] public bool OverrideBonus { get; private set; } = false;
		
		[field: SerializeField, ShowIf(nameof(OverrideBonus))] 
		public int MoneyBonusForHeadshot { get; private set; } = 10;

		[field: SerializeField, ShowIf(nameof(OverrideBonus))] 
		public int MoneyBonusForKill { get; private set; } = 10;


		[field: SerializeField]
		public float SoftPenaltyPercent { get; private set; } = 1;

		[field: SerializeField]
		public float SoftBonusPercent { get; private set; } = 1;
		
		[field: SerializeField] public float DespawnDuration { get; private set; }

		[field: Header("Mission fail")]
		[field: SerializeField]
		public bool MissionFailOnDeath { get; private set; }

		[field: SerializeField] public float MissionFailDelay { get; private set; }
		
		public void TriggerReceiveHit(BoneType boneType) => OnReceiveHit?.Invoke(boneType);
		
		public void TriggerDeath() => OnDeath?.Invoke();
	}

	// Editor
#if UNITY_EDITOR
	
	public partial class AiLiveModel
	{
		[SerializeField] private SpreadsheetConfig _sheetConfig;
		
		private AiLiveModelRecord _currentRecord;
		
		
		// TODO: get rid of ImportFromGoogle code duplication
		[Button]
		public void ImportFromGoogle()
		{
			ImportSheet(new GoogleSheetLoader<AiLiveModelRecord>(_sheetConfig));
		}

		[Button, PropertySpace(5)]
		private void ImportFromCSV()
		{
			string path = EditorUtility.OpenFilePanel("Import CSV", Application.dataPath, "csv");
			ImportSheet(new CsvSheetLoader<AiLiveModelRecord>(path));
		}

		private void ImportSheet(ISheetLoader<AiLiveModelRecord> sheetLoader)
		{
			EditorUtility.DisplayProgressBar($"Refreshing {nameof(WeaponStaticData)} from Google Sheet", "Loading values...", 0.2f);

			_currentRecord = null;
			try
			{
				HandleRecords(sheetLoader.Load());
			}
			catch (Exception e)
			{
				if (_currentRecord != null)
					Debug.LogError($"Google Sheet parse failed: stopped on row {_currentRecord.RowId}");

				EditorUtility.ClearProgressBar();
				Debug.LogError(e);
				throw;
			}

			EditorUtility.ClearProgressBar();
		}

		private void HandleRecords(IList<AiLiveModelRecord> records)
		{
			var record = records.First();

			BaseHealth = record.Health.ToFloat();

			var dmgMplHead = record.DamageMultiplierHead.ToFloat();
			var dmgMplBody = record.DamageMultiplierBody.ToFloat();
			var dmgMplOther = record.DamageMultiplierOther.ToFloat();

			DamageMultiplierMap.Clear();
			foreach (BoneType boneType in Enum.GetValues(typeof(BoneType)))
			{
				switch (boneType)
				{
					case BoneType.None:
						continue;

					case BoneType.Head:
					case BoneType.Neck:
						DamageMultiplierMap.Add(boneType, dmgMplHead);
						break;

					case BoneType.Pelvis:
					case BoneType.Chest:
					case BoneType.Spine:
						DamageMultiplierMap.Add(boneType, dmgMplBody);
						break;

					default:
						DamageMultiplierMap.Add(boneType, dmgMplOther);
						break;
				}
			}
			
			SoftPenaltyPercent = record.SoftPenaltyPercent.ToFloat();
			SoftBonusPercent = record.SoftBonusPercent.ToFloat();
			
		}
		
		public class AiLiveModelRecord : BaseRecord
		{
			[SheetField(
				DisplayName = "Name", 
				ColumnID = 1,
				FieldType = SheetFieldType.String)]
			public string Name { get; set; }
			
			[SheetField(
				DisplayName = "Hp", 
				ColumnID = 2,
				FieldType = SheetFieldType.String)]
			public string Health { get; set; }
			
			[SheetField(
				DisplayName = "Multiply_head", 
				ColumnID = 3,
				FieldType = SheetFieldType.String)]
			public string DamageMultiplierHead { get; set; }
			
			[SheetField(
				DisplayName = "Multiply_body", 
				ColumnID = 4,
				FieldType = SheetFieldType.String)]
			public string DamageMultiplierBody { get; set; }
			
			[SheetField(
				DisplayName = "Multiply_other", 
				ColumnID = 5,
				FieldType = SheetFieldType.String)]
			public string DamageMultiplierOther { get; set; }
			
			[SheetField(
				DisplayName = "Soft_penalty", 
				ColumnID = 6,
				FieldType = SheetFieldType.String)]
			public string SoftPenaltyPercent { get; set; }
			
			[SheetField(
				DisplayName = "Soft_bonus", 
				ColumnID = 7,
				FieldType = SheetFieldType.String)]
			public string SoftBonusPercent { get; set; }

			public AiLiveModelRecord()
			{
			}

			// This constructor signature is required to define!
			public AiLiveModelRecord(IList<object> row, int rowId, int minColumnId = 1) : base(row, rowId, minColumnId)
			{
			}
		}
	}
	
#endif
}