/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace GameDataEditor
{
	public enum ImportExportType
	{
		Google,
		Local,
		None
	}

	public class GDEExcelManager
	{
		public static void ClearExcelSettings()
		{
			GDESettings settings = GDESettings.Instance;

			settings.ImportedGoogleSpreadsheetName = string.Empty;
			settings.ImportedLocalSpreadsheetName = string.Empty;

			settings.ExportedGoogleSpreadsheetPath = string.Empty;
			settings.ExportedLocalSpreadsheetName = string.Empty;

			settings.ImportType = ImportExportType.None;
			settings.ExportType = ImportExportType.None;
            settings.ExportFileMD5 = string.Empty;

			settings.Save();
		}

		public static void DoImport()
		{
			GDESettings settings = GDESettings.Instance;

			if (settings.ImportType.Equals(ImportExportType.Local) &&
			    !string.IsNullOrEmpty(settings.ImportedLocalSpreadsheetName))
			{
				ProcessSheet(settings.ImportedLocalSpreadsheetName);
			}
			else if (settings.ImportType.Equals(ImportExportType.Google) &&
			         !string.IsNullOrEmpty(settings.ImportedGoogleSpreadsheetName) &&
			         GDEDriveHelper.Instance.HasAuthenticated())
			{
				string path = GDEDriveHelper.Instance.DownloadSpreadSheet(settings.ImportedGoogleSpreadsheetName,
				                                                  "import_" + settings.ImportedGoogleSpreadsheetName + ".xlsx");
				ProcessSheet(path);
			}
			else
			{
				var window = EditorWindow.GetWindow<GDEImportExcel>(true, GDEConstants.ImportSpreadsheetMenu);
				window.LoadSettings();
				window.Show();
			}
		}

		public static void DoExport(bool newSheet = false)
		{
			GDESettings settings = GDESettings.Instance;

            if (!GDEItemManager.FileChangedOnDisk(GDEItemManager.DataFilePath, settings.ExportFileMD5))
            {
                Debug.Log("GDE Data hasn't changed, skipping export.");
                return;
            }

            if (settings.ExportType.Equals(ImportExportType.Local) &&
			    !string.IsNullOrEmpty(settings.ExportedLocalSpreadsheetName))
			{
				// take the local languages dictionary
				// write it out to an excel file
				GDEExcelDataHelper excelHelper = new GDEExcelDataHelper(settings.ExportedLocalSpreadsheetName, true);
				excelHelper.ExportToSheet(GDEItemManager.ItemListBySchema);
                settings.ExportFileMD5 = File.ReadAllText(GDEItemManager.DataFilePath).Md5Sum();
			}
			else if (settings.ExportType.Equals(ImportExportType.Google) &&
			         !string.IsNullOrEmpty(settings.ExportedGoogleSpreadsheetPath) &&
			         GDEDriveHelper.Instance.HasAuthenticated())
			{
                GDEDriveHelper.Instance.GetSpreadsheetList();

				string tempSheetPath = FileUtil.GetUniqueTempPathInProject() + "exportnewgoog_" + settings.ExportedGoogleSpreadsheetPath + ".xlsx";

				GDEExcelDataHelper excelHelper = new GDEExcelDataHelper(tempSheetPath, true);
				excelHelper.ExportToSheet(GDEItemManager.ItemListBySchema);

                if (newSheet)
				{
                    GDEDriveHelper.Instance.UploadNewSheet(tempSheetPath, settings.ExportedGoogleSpreadsheetPath);
                    settings.ExportFileMD5 = File.ReadAllText(GDEItemManager.DataFilePath).Md5Sum();
                }
                else
                {
					GDEDriveHelper.Instance.UploadToExistingSheet(settings.ExportedGoogleSpreadsheetPath, tempSheetPath);
                    settings.ExportFileMD5 = File.ReadAllText(GDEItemManager.DataFilePath).Md5Sum();
                }
            }
            else
            {
				var window = EditorWindow.GetWindow<GameDataEditor.GDEExportExcel>(true, GDEConstants.ExportSpreadsheetLbl);
				window.LoadSettings();
				window.Show();
			}
		}

		static void ProcessSheet(string path)
		{
			try
			{
				GDEExcelDataHelper excelDataHelper = new GDEExcelDataHelper(path);
				excelDataHelper.OnUpdateProgress += delegate (string title, string msg, float progress) {
					EditorUtility.DisplayProgressBar(title, msg, progress);
				};

				excelDataHelper.ProcessSheet();
			}
			catch(Exception ex)
			{
				Debug.LogError(ex);
			}
			finally
			{
				EditorUtility.ClearProgressBar();
			}
		}
	}
}
