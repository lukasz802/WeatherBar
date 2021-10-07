using System;
using System.Windows.Input;

namespace WeatherBar.Core
{
    public class RelayCommand : ICommand
    {
        #region Fields

        private readonly Action<object> executeAction;

        private readonly Predicate<object> canExecuteFunc;

        #endregion

        #region Constructors

        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }

        public RelayCommand(Action execute) : this((o) => execute() , null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            executeAction = execute ?? throw new ArgumentNullException(nameof(execute));
            canExecuteFunc = canExecute;
        }

        #endregion

        #region Public methods

        public bool CanExecute(object parameter)
        {
            return canExecuteFunc == null || canExecuteFunc(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (canExecuteFunc != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (canExecuteFunc != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public void Execute(object parameter)
        {
            executeAction(parameter);
        }

        #endregion
    }
}
