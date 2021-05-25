using System;
using System.Windows.Input;

namespace Thismaker.Goro.Commands
{
    /// <summary>
    /// A command to allow for binding WPF buttons etc to ViewModel methods.
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Private Fields
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;
        private readonly Action _nsExecute;
        private readonly Func<bool> _nsCanExecute;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a <see cref="RelayCommand"/> that will always allow execution
        /// </summary>
        /// <param name="execute"></param>
        public RelayCommand(Action<object> execute)
        {
            _execute = execute;
            _canExecute = DefaultCanExecute;
        }

        /// <summary>
        /// Creates a <see cref="RelayCommand"/> with the specified execution and execution test values
        /// </summary>
        /// <param name="execute">The <see cref="Action{T}"/> to be called whenever the command is invoked</param>
        /// <param name="canExecute">The see <see cref="Func{T, TResult}"/> to test whether execution is allowed</param>
        public RelayCommand(Action<object>execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Creates a relay command that does not require a parameter/sender parameter to work.
        /// Use in situations where knowing the sender of the request is not necessary.
        /// </summary>
        /// <param name="execute">The <see cref="Action"/> to be called whenever the command is invoked</param>
        /// <param name="canExecute">The see <see cref="Func{TResult}"/> to test whether execution is allowed</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _nsExecute = execute;
            _nsCanExecute = canExecute;
        }

        /// <summary>
        /// Creates a relay command that will always be executable and does not require a parameter/sender to work
        /// </summary>
        /// <param name="execute">The <see cref="Action"/> to be called whenever the command is invoked</param>
        public RelayCommand(Action execute)
        {
            _nsExecute = execute;
            _canExecute = DefaultCanExecute;
        }
        #endregion

        #region ICommand implements
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Checks if command can be executed
        /// </summary>
        /// <param name="parameter">The sender of the request</param>
        /// <returns><see cref="true"/> if can execute</returns>
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null && _nsCanExecute == null) return true;

            return _canExecute == null ? _nsCanExecute() : _canExecute(parameter);
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="parameter">The sender of the request</param>
        public void Execute(object parameter)
        {
            if (_execute == null)
            {
                _nsExecute();
            }
            else
            {
                _execute(parameter);
            }
        }

        private bool DefaultCanExecute(object parameter) => true;
        #endregion
    }
}
