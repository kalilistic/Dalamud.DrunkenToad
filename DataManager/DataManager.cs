using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DalamudPluginCommon
{
	public class DataManager
	{
		protected readonly IPluginBase Plugin;

		public DataManager(IPluginBase plugin)
		{
			Plugin = plugin;
			DataPath = plugin.PluginFolder() + "/data/";
			CreateDataDirectory();
		}

		public string DataPath { get; set; }

		internal void CreateDataDirectory()
		{
			try
			{
				Directory.CreateDirectory(DataPath);
			}
			catch (Exception ex)
			{
				Plugin.LogError(ex, "Failed to create data directory.");
			}
		}

		internal void InitDataFiles(string[] fileNames)
		{
			foreach (var fileName in fileNames)
				if (!File.Exists(DataPath + fileName))
					SaveData(fileName, string.Empty);
		}

		internal void SaveData(string fileName, string data)
		{
			try
			{
				File.WriteAllText(DataPath + fileName, data);
			}
			catch (Exception ex)
			{
				Plugin.LogError(ex, "Failed to write data file.");
			}
		}

		internal string ReadData(string fileName)
		{
			try
			{
				return File.ReadAllText(DataPath + fileName);
			}
			catch (Exception ex)
			{
				Plugin.LogError(ex, "Failed to read data file.");
			}

			return null;
		}

		internal void CreateBackup()
		{
			try
			{
				var backupDir = DataPath + DateUtil.CurrentTime() + "/";
				Directory.CreateDirectory(backupDir);
				var files = Directory.GetFiles(DataPath);
				foreach (var file in files)
				{
					var fileName = Path.GetFileName(file);
					File.Copy(DataPath + fileName, backupDir + fileName, true);
				}
			}
			catch (Exception ex)
			{
				Plugin.LogError(ex, "Failed to create backup.");
			}
		}

		internal void DeleteBackups(int max)
		{
			if (max == 0) return;
			try
			{
				var dirs = Directory.GetDirectories(DataPath);
				var dirNames = new List<long>();
				foreach (var dir in dirs)
					try
					{
						dirNames.Add(Convert.ToInt64(new DirectoryInfo(dir).Name));
					}
					catch (Exception)
					{
						// ignored
					}

				if (dirs.Length <= max) return;
				dirNames.Sort();
				Directory.Delete(DataPath + dirNames.First(), true);
				DeleteBackups(max);
			}
			catch (Exception ex)
			{
				Plugin.LogError(ex, "Failed to delete backup.");
			}
		}
	}
}