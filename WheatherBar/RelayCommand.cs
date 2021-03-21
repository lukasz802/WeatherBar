using System;
using System.Diagnostics;
using System.Windows.Input;

namespace WeatherBar
{
    public class RelayCommand : ICommand
    {
        #region Fields

        private readonly Action<object> _execute;

        private readonly Predicate<object> _canExecute;

        #endregion

        #region Constructors

        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        #endregion

        #region Methods

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null) { CommandManager.RequerySuggested += value; }
            }
            remove
            {
                if (_canExecute != null) { CommandManager.RequerySuggested -= value; }
            }
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        #endregion
    }
}
