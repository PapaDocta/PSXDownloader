﻿using System;
using System.Windows.Input;

namespace PSXDownloader.MVVM.Commands
{
    public class RelayCommand : ICommand
    {
        private Action<object?> _execute;
        private Predicate<object?>? _canExecute;

        public RelayCommand(Action<object?> execute) : this(execute, null) { }

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute?.Invoke(parameter);
        }
    }
}
