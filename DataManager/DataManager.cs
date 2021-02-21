// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator

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

        internal void InitDataFiles(string[] fileNames, bool force = false)
        {
            foreach (var fileName in fileNames)
                if (!File.Exists(DataPath + fileName) || force)
                    CreateDataFile(DataPath + fileName);
        }

        internal void CreateDataFile(string fileName)
        {
            var file = File.Create(fileName);
            file.Close();
        }

        internal void SaveDataList(string fileName, List<string> data)
        {
            var dataFile = DataPath + fileName;
            var tempFile = DataPath + fileName + ".tmp";
            try
            {
                File.Delete(tempFile);
                CreateDataFile(tempFile);
                using (var sw = new StreamWriter(tempFile, false))
                {
                    foreach (var entry in data)
                        if (!string.IsNullOrEmpty(entry))
                            sw.WriteLine(entry);
                }

                File.Delete(dataFile);
                File.Move(tempFile, dataFile);
            }
            catch (Exception ex)
            {
                Plugin.LogError(ex, "Failed to write data file.");
            }
        }

        internal void SaveDataStr(string fileName, string data)
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


        internal string ReadDataStr(string fileName)
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


        internal List<string> ReadDataList(string fileName)
        {
            try
            {
                var data = new List<string> {string.Empty};
                using (var sr = new StreamReader(DataPath + fileName))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null) data.Add(line);
                }

                return data;
            }
            catch (Exception ex)
            {
                Plugin.LogError(ex, "Failed to read data file.");
            }

            return null;
        }

        internal void CreateBackup(string dirPrefix = "")
        {
            try
            {
                var backupDir = DataPath + dirPrefix + DateUtil.CurrentTime() + "/";
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
                        var dirName = new DirectoryInfo(dir).Name;
                        if (dirName.Any(char.IsLetter)) continue;
                        dirNames.Add(Convert.ToInt64(dirName));
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