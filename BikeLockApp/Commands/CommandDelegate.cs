using System;
using System.Windows.Input;

namespace BikeLockApp.Commands
{
    public class CommandDelegate : ICommand
    {
        Func<object, bool> func;
        Action<object> action;

        public CommandDelegate(Func<object, bool> func, Action<object> action)
        {
            this.func = func;
            this.action = action;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return this.func.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            this.action.Invoke(parameter);
        }
    }
}
