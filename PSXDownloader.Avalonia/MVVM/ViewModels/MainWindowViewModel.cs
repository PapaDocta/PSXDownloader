using PSXDownloader.MVVM.Commands;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System.Windows.Input;

namespace PSXDownloader.MVVM.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ICommand? _exit;
        private ICommand? _maximize;
        private ICommand? _minimize;
        private WindowState _windowState;

        public double MaxWidth => 1024;
        public double MaxHeight => 768;

        public WindowState WindowState
        {
            get => _windowState;
            set
            {
                _windowState = value;
                OnPropertyChanged();
            }
        }

        public ICommand? Exit
        {
            get
            {
                _exit ??= new RelayCommand(ExitCommand);
                return _exit;
            }
        }

        private void ExitCommand(object? obj)
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                lifetime.Shutdown();
            }
        }

        public ICommand? Maximize
        {
            get
            {
                _maximize ??= new RelayCommand(MaximizeCommand);
                return _maximize;
            }
        }

        private void MaximizeCommand(object? obj)
        {
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        public ICommand? Minimize
        {
            get
            {
                _minimize ??= new RelayCommand(MinimizeCommand);
                return _minimize;
            }
        }

        private void MinimizeCommand(object? obj)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
