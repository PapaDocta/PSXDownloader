using PSXDLL;
using System.IO;
using System.Text.Json;
using Microsoft.Win32;

namespace PSXDownloader.MVVM.Data
{
    public class SettingRepository
    {
        public string? LocalFilePath()
        {
            OpenFileDialog ofd = new()
            {
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Select Folder"
            };

            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                return Path.GetDirectoryName(ofd.FileName);
            }
            return null;
        }

        public void SaveSetting(AppConfig? config)
        {
            if (!Directory.Exists("Settings"))
            {
                Directory.CreateDirectory("Settings");
            }

            string fileName = Path.Combine("Settings", "Settings.json");
            JsonSerializerOptions? options = new() { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(config, options);
            File.WriteAllText(fileName, jsonString);
        }

        public void LoadSetting(AppConfig? config)
        {
            string fileName = Path.Combine("Settings", "Settings.json");
            if (!File.Exists(fileName))
            {
                SaveSetting(config);
            }
            string? json = File.ReadAllText(fileName);
            AppConfig? settings = JsonSerializer.Deserialize<AppConfig>(json);
            AppConfig.Instance().Rule = settings!.Rule;
            AppConfig.Instance().Host = settings!.Host;
            AppConfig.Instance().IsAutoFindFile = settings!.IsAutoFindFile;
            AppConfig.Instance().LocalFileDirectory = settings?.LocalFileDirectory;
            AppConfig.Instance().BufferSize = settings!.BufferSize;
        }
    }
}
