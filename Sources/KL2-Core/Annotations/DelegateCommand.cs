using System;
using System.Windows.Input;

namespace AnnotationsLib
{
    public class DelegateCommand<T> : ICommand
    {
        protected Predicate<T> _canExecute;
        protected Action<T> _execute;

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public virtual bool CanExecute(object parameter) => _canExecute == null ? true : _canExecute.Invoke((T)parameter);

        public virtual void Execute(object parameter) => _execute?.Invoke((T)parameter);

        public DelegateCommand(Action<T> execute) : this(execute, null)
        { }

        public DelegateCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
    }

    public class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Action<object> execute) : this(execute, null)
        { }

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute) : base(execute, canExecute)
        { }
    }
}
