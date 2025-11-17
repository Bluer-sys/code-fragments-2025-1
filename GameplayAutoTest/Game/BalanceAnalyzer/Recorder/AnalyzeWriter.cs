#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Swat.Game.Utils;
using UnityEditor;
using UnityEngine;

namespace Swat.Game.GameplayRobot.Game.BalanceAnalyzer.Recorder
{
	public static class AnalyzeWriter
	{
		public static void WriteSheet(List<AnalyzeRecorder> recorders)
		{

			string path = EditorUtility.SaveFilePanel(
				"Save levels analyze",
				"",
				"levels_analyze.xlsx",
				"xlsx");

			if (string.IsNullOrEmpty(path))
				return;

			// Create Excel
			IWorkbook workbook = new XSSFWorkbook();

			// For all recorders
			for (int i = 0; i < recorders.Count; i++)
			{
				AnalyzeRecorder recorder = recorders[i];
				ISheet sheet = workbook.CreateSheet($"Sheet{i + 1}");

				WriteHeaders(sheet);
				WriteLevelsAnalyze(workbook, sheet, recorder);
				NpoiUtils.AutosizeColumns(sheet, 0, 50);
			}

			using (var fileStream = new FileStream(path, FileMode.Create))
			{
				workbook.Write(fileStream);
			}

			Debug.Log($"Levels analyze saved to {path}");
			AssetDatabase.Refresh();
		}

		private static void WriteLevelsAnalyze(IWorkbook workbook, ISheet sheet, AnalyzeRecorder recorder)
		{
			for (var i = 0; i < recorder.LevelRecords.Count; i++)
			{
				var record = recorder.LevelRecords[i];
				var runningCount = record.PlayTimes.Count;
				var row = sheet.CreateRow(i + 1);

				NpoiUtils.WriteFormattedCell(workbook, row.CreateCell(0), record.StageIndex);
				NpoiUtils.WriteFormattedCell(workbook, row.CreateCell(1), record.RaidIndex);
				NpoiUtils.WriteFormattedCell(workbook, row.CreateCell(2), record.LevelIndex);
				NpoiUtils.WriteFormattedCell(workbook, row.CreateCell(3), record.RemainingHealthPercent.Average() * 100f);
				NpoiUtils.WriteFormattedCell(workbook, row.CreateCell(4), record.RemainingAmmoPrimaryPercent.Average() * 100f);
				NpoiUtils.WriteFormattedCell(workbook, row.CreateCell(5), record.RemainingAmmoSecondaryPercent.Average() * 100f);
				NpoiUtils.WriteFormattedCell(workbook, row.CreateCell(6), record.BattleEarnMoney.Average());
				
				NpoiUtils.WriteFormattedCell(workbook, row.CreateCell(7), ((float)record.LooseCount / runningCount) * 100f);
				NpoiUtils.WriteFormattedCell(workbook, row.CreateCell(8), ((float)record.LooseCountSegments[0] / runningCount) * 100f);
				NpoiUtils.WriteFormattedCell(workbook, row.CreateCell(9), ((float)record.LooseCountSegments[1] / runningCount) * 100f);
				NpoiUtils.WriteFormattedCell(workbook, row.CreateCell(10), ((float)record.LooseCountSegments[2] / runningCount) * 100f);
				NpoiUtils.WriteFormattedCell(workbook, row.CreateCell(11), ((float)record.LooseCountSegments[3] / runningCount) * 100f);
				NpoiUtils.WriteFormattedCell(workbook, row.CreateCell(12), ((float)record.LooseCountSegments[4] / runningCount) * 100f);
				NpoiUtils.WriteFormattedCell(workbook, row.CreateCell(13), ((float)record.LooseCountSegments[5] / runningCount) * 100f);
				
				NpoiUtils.WriteFormattedCell(workbook, row.CreateCell(14), record.PlayTimes.Average());
				NpoiUtils.WriteFormattedCell(workbook, row.CreateCell(15), record.AverageFps.Average());
				NpoiUtils.WriteFormattedCell(workbook, row.CreateCell(16), record.SceneLoadTime.Average());
			}
		}

		private static void WriteHeaders(ISheet sheet)
		{
			IRow row = sheet.CreateRow(0);
			
			row.CreateCell(0).SetCellValue("Stage");
			row.CreateCell(1).SetCellValue("Raid");
			row.CreateCell(2).SetCellValue("Level");
			row.CreateCell(3).SetCellValue("AWG_HP_SAVE_in%");
			row.CreateCell(4).SetCellValue("AWG_AMMO_SAVE_1_in%");
			row.CreateCell(5).SetCellValue("AWG_AMMO_SAVE_2_in%");
			row.CreateCell(6).SetCellValue("AWG_BATTLE_$");
			row.CreateCell(7).SetCellValue("LOSERATE");
			row.CreateCell(8).SetCellValue("LOSE_1");
			row.CreateCell(9).SetCellValue("LOSE_2");
			row.CreateCell(10).SetCellValue("LOSE_3");
			row.CreateCell(11).SetCellValue("LOSE_4");
			row.CreateCell(12).SetCellValue("LOSE_5");
			row.CreateCell(13).SetCellValue("LOSE_6");
			row.CreateCell(14).SetCellValue("AWG_time");
			row.CreateCell(15).SetCellValue("AWG_FPS");
			row.CreateCell(16).SetCellValue("SCENE_LOAD_TIME");
		}
	}
}
#endif