﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Wox.Infrastructure.Logger;

namespace Wox.Plugin.Program
{
    internal static class FileChangeWatcher
    {
        private static readonly List<string> WatchedPath = new List<string>();

        public static void AddWatch(string path, string[] programSuffixes, bool includingSubDirectory = true)
        {
            if (WatchedPath.Contains(path)) return;
            if (!Directory.Exists(path))
            {
                Log.Warn($"FileChangeWatcher: {path} doesn't exist");
                return;
            }

            WatchedPath.Add(path);
            foreach (string fileType in programSuffixes)
            {
                FileSystemWatcher watcher = new FileSystemWatcher
                {
                    Path = path,
                    IncludeSubdirectories = includingSubDirectory,
                    Filter = $"*.{fileType}",
                    EnableRaisingEvents = true
                };
                watcher.Changed += FileChanged;
                watcher.Created += FileChanged;
                watcher.Deleted += FileChanged;
                watcher.Renamed += FileChanged;
            }
        }

        private static void FileChanged(object source, FileSystemEventArgs e)
        {
            Task.Run(() =>
            {
                Main.IndexPrograms();
            });
        }
    }
}
