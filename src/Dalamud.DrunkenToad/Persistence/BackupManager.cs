using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dalamud.DrunkenToad
{
    /// <summary>
    /// Backup manager for use with Repository and LiteDB.
    /// </summary>
    public class BackupManager
    {
        private readonly string dataPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackupManager"/> class.
        /// </summary>
        /// <param name="pluginService">plugin service.</param>
        public BackupManager(PluginService pluginService)
        {
            this.dataPath = pluginService.PluginFolder() + "/data/";
        }

        /// <summary>
        /// Create new backup.
        /// </summary>
        /// <param name="prefix">optional prefix used for backup dir names (e.g. upgrade version).</param>
        public void CreateBackup(string prefix = "")
        {
            try
            {
                var backupDir = $"{this.dataPath}{prefix}{DateUtil.CurrentTime()}/";
                Directory.CreateDirectory(backupDir);
                var files = Directory.GetFiles(this.dataPath);
                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    File.Copy(this.dataPath + fileName, backupDir + fileName, true);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to create backup.");
            }
        }

        /// <summary>
        /// Delete backups based on maximum number.
        /// </summary>
        /// <param name="max">max number of backups.</param>
        public void DeleteBackups(int max)
        {
            // don't delete everything
            if (max == 0)
            {
                return;
            }

            try
            {
                // loop through directories and get those without prefix
                var dirs = Directory.GetDirectories(this.dataPath);
                var dirNames = new List<long>();
                foreach (var dir in dirs)
                {
                    try
                    {
                        var dirName = new DirectoryInfo(dir).Name;
                        if (dirName.Any(char.IsLetter))
                        {
                            continue;
                        }

                        dirNames.Add(Convert.ToInt64(dirName));
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                // if don't exceed max then out
                if (dirs.Length <= max)
                {
                    return;
                }

                dirNames.Sort();
                Directory.Delete(this.dataPath + dirNames.First(), true);
                this.DeleteBackups(max);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to delete backup.");
            }
        }
    }
}
