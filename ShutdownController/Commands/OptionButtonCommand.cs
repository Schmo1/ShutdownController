using System;
using System.Windows.Input;

namespace ShutdownController.Commands
{

    public class OptionButtonCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action _execute;

        public OptionButtonCommand(Action execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute.Invoke();
        }
    }
}
