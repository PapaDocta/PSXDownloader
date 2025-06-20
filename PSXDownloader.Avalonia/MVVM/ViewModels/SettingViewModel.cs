using PSXDLL;
using PSXDownloader.MVVM.Commands;
using PSXDownloader.MVVM.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using System.Windows.Input;

namespace PSXDownloader.MVVM.ViewModels
{
    public class SettingViewModel : ViewModelBase
    {
        private ICommand? _filePath;
        private ICommand? _saveSetting;

        private readonly SettingRepository _repository;


        public SettingViewModel()
        {
            _repository = new();
            _repository.LoadSetting(AppConfig.Instance());
        }

        public Dictionary<int, string> BufferList => Enumerable.Range(2, 15)
            .ToDictionary(s => (int)Math.Pow(2, s), s => (int)Math.Pow(2, s) % 1024 == 0 ? $"{(int)Math.Pow(2, s) / 1024} MB" : $"{(int)Math.Pow(2, s)} KB");

        public string? Rule
        {
            get => AppConfig.Instance().Rule;
            set
            {
                AppConfig.Instance().Rule = value;
                OnPropertyChanged();
            }
        }

        public string? LocalFileDirectory
        {
            get => AppConfig.Instance().LocalFileDirectory;
            set
            {
                AppConfig.Instance().LocalFileDirectory = value;
                OnPropertyChanged();
            }
        }

        public bool IsAutoFind
        {
            get => AppConfig.Instance().IsAutoFindFile;
            set
            {
                AppConfig.Instance().IsAutoFindFile = value;
                OnPropertyChanged();
            }
        }

        public int BufferSize
        {
            get => AppConfig.Instance().BufferSize;
            set
            {
                AppConfig.Instance().BufferSize = value;
                OnPropertyChanged();
            }
        }

        public ICommand? FilePath
        {
            get
            {
                _filePath ??= new RelayCommand(FilePathCommand);
                return _filePath;
            }
        }

        private async void FilePathCommand(object? obj)
        {
            if (obj is Window parent)
            {
                string? path = await _repository.LocalFilePathAsync(parent);
                if (!string.IsNullOrEmpty(path))
                {
                    LocalFileDirectory = path;
                }
            }
        }

        public ICommand? SaveSetting
        {
            get
            {
                _saveSetting ??= new RelayCommand(SaveSettingCommand);
                return _saveSetting;
            }
        }

        private void SaveSettingCommand(object? obj)
        {
            _repository.SaveSetting(AppConfig.Instance());
        }
    }
}

