using System;
using System.Windows.Input;

namespace MigrationTool
{
    public class DelegateCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action<T> _execute;
        private Func<T, bool> _canExecute;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public bool CanExecute(T parameter) => _canExecute?.Invoke(parameter) ?? true;
        public bool CanExecute(object parameter) => CanExecute((T)parameter);

        public void Execute(T parameter) => _execute?.Invoke(parameter);
        public void Execute(object parameter) => Execute((T)parameter);

        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public DelegateCommand(Action<T> execute) : this(execute, null)
        { }
    }
}
