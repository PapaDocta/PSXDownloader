﻿using Microsoft.EntityFrameworkCore;
using PSXDownloader.MVVM.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace PSXDownloader.MVVM.Data
{
    public class PSXRepository
    {
        public async Task BulkAdd(string? path)
        {
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(path))
            {
                return;
            }
            IEnumerable<string> folders = Directory.EnumerateDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
            foreach (string folder in folders)
            {
                try
                {
                    IEnumerable<string> files = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith("_0.pkg", StringComparison.OrdinalIgnoreCase));
                    foreach (string file in files)
                    {
                        string? titleID = Path.GetFileNameWithoutExtension(file).Split('-')[1].Replace("_00", "");
                        string? title = Directory.GetParent(file)?.Name;
                        string? localPath = Directory.GetParent(file)?.FullName;
                        PSXDatabase? db = new()
                        {
                            Title = title,
                            TitleID = titleID,
                            LocalPath = localPath,
                        };
                        if (!IsExist(titleID))
                        {
                            await Create(db);
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        public async Task Create(PSXDatabase Entity)
        {
            using PSXDataContext dataContext = new();
            await dataContext.Set<PSXDatabase>().AddAsync(Entity);
            await dataContext.SaveChangesAsync();
        }

        public async Task Update(PSXDatabase Entity)
        {
            using PSXDataContext dataContext = new();
            dataContext.Set<PSXDatabase>().Update(Entity);
            await dataContext.SaveChangesAsync();
        }

        public async Task Delete(PSXDatabase Entity)
        {
            using PSXDataContext dataContext = new();
            dataContext.Set<PSXDatabase>().Remove(Entity);
            await dataContext.SaveChangesAsync();
        }

        public async Task<List<PSXDatabase>> GetAll()
        {
            using PSXDataContext dataContext = new();
            List<PSXDatabase> entities = await dataContext.Set<PSXDatabase>().ToListAsync();
            return await Task.FromResult(entities);
        }

        public bool IsExist(string? titleID)
        {
            using PSXDataContext dataContext = new();
            PSXDatabase? local = dataContext.Set<PSXDatabase>().FirstOrDefault(s => s.TitleID == titleID);
            return local != null;
        }

        public async Task<string> GetLocalPath(string? titleID)
        {
            using PSXDataContext dataContext = new();
            PSXDatabase? local = dataContext.Set<PSXDatabase>().FirstOrDefault(s => s.TitleID == titleID);
            if (local != null)
            {
                return await Task.FromResult(local.LocalPath!);
            }
            return await Task.FromResult(string.Empty);
        }

        public async Task<string> LocalDirectory(string url)
        {
            Uri uri = new(url);
            string titleID = Path.GetFileName(uri.LocalPath).Split('-')[1].Replace("_00", "");

            return await GetLocalPath(titleID);

        }

        public async Task<string?> LocalFilePathAsync(Window parent)
        {
            var dialog = new OpenFolderDialog();
            return await dialog.ShowAsync(parent);
        }

        public async Task<PSXDatabase?> SingleAddAsync(Window parent)
        {
            string? path = await LocalFilePathAsync(parent);

            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    string? file = Directory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly).FirstOrDefault(s => s.EndsWith("_0.pkg", StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrEmpty(file))
                    {
                        string? titleID = Path.GetFileNameWithoutExtension(file).Split('-')[1].Replace("_00", "");
                        string? title = Directory.GetParent(file)?.Name;
                        string? localPath = Directory.GetParent(file)?.FullName;
                        PSXDatabase? db = new()
                        {
                            Title = title,
                            TitleID = titleID,
                            LocalPath = localPath,
                        };
                        if (!IsExist(titleID))
                        {
                            return await Task.FromResult(db);
                        }
                    }
                }
                catch
                {
                    return await Task.FromResult(new PSXDatabase());
                }
            }
            return await Task.FromResult(new PSXDatabase());
        }

        public async Task Backup()
        {
            try
            {
                if (!Directory.Exists("Backup"))
                {
                    Directory.CreateDirectory("Backup");
                }
                using PSXDataContext dataContext = new();
                List<PSXDatabase> entities = await dataContext.Set<PSXDatabase>().ToListAsync();
                JsonSerializerOptions? options = new() { WriteIndented = true };
                string? json = JsonSerializer.Serialize(entities, options);
                string time = TimeOnly.FromDateTime(DateTime.Now).ToString().Replace(":", "-");
                string backup = $"Backup\\{time}.json";
                using StreamWriter sw = new(backup);
                sw.WriteLine(json);
            }
            catch
            {
            }
        }

        public async Task RestoreAsync(Window parent)
        {
            if (!Directory.Exists("Backup"))
            {
                Directory.CreateDirectory("Backup");
            }
            string? json = null;
            var dialog = new OpenFileDialog
            {
                Filters = new List<FileDialogFilter>{ new FileDialogFilter { Name = "Json", Extensions = {"json"} } },
                InitialFileName = "Backup"
            };
            var result = await dialog.ShowAsync(parent);
            if (result != null && result.Length > 0)
            {
                json = File.ReadAllText(result[0]);
                try
                {
                    List<PSXDatabase>? entities = JsonSerializer.Deserialize<List<PSXDatabase>>(json);
                    foreach (PSXDatabase? entity in entities!)
                    {
                        if (!IsExist(entity.TitleID))
                        {
                            await Create(entity);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
