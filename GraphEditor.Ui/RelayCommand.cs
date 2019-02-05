// Copyright 2018 © CSA Computer & Antriebstechnik GmbH - all rights reserved.
// Unauthorized copying of this file (RelayCommand.cs) via any medium is strictly prohibited.
// Proprietary and confidential.
// Created 14:43 27.04.2018 by Robert Pejas 

using System;
using System.Windows.Input;

namespace GraphEditor
{
    /// <summary>
    /// To register commands in MMVM pattern
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        /// <inheritdoc />
        /// <summary>
        /// Constructer takes Execute events to register in CommandManager.
        /// </summary>
        /// <param name="execute">Execute method as action.</param>
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
            _execute = execute ?? throw new NotImplementedException("Not implemented");
        }

        /// <summary>
        /// Constructer takes Execute and CanExcecute events to register in CommandManager.
        /// </summary>
        /// <param name="execute">Execute method as action.</param>
        /// <param name="canExecute">CanExecute method as return bool type.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            try
            {
                if (null == execute)
                {
                    _execute = null;
                    throw new NotImplementedException("Not implemented");
                }
                _execute = execute;
                _canExecute = canExecute;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// Can Executed Changed Event
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Execute method.
        /// </summary>
        /// <param name="parameter">Method parameter.</param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// CanExecute method.
        /// </summary>
        /// <param name="parameter">Method parameter.</param>
        /// <returns>Return true if can execute.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// InvalidateCanExecute method will initiate validation of the Command.
        /// </summary>
        public void InvalidateCanExecute()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}