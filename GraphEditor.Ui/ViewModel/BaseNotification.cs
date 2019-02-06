﻿// Copyright 2018 © CSA Computer & Antriebstechnik GmbH - all rights reserved.
// Unauthorized copying of this file (BaseNotification.cs) via any medium is strictly prohibited.
// Proprietary and confidential.
// Created 15:20 11.11.2018 by Robert Pejas 

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;

namespace GraphEditor.ViewModel
{
    /// <summary>
    /// Base PropertyChanged implementation
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class BaseNotification : INotifyPropertyChanged
    {
        protected BaseNotification()
        {
            CurrentDispatcher = Dispatcher.CurrentDispatcher;
        }

        protected Dispatcher CurrentDispatcher { get; }

        /// <summary>
        /// Sets the storage (of a property) to the value and fires property changed event only if the storage value is changed
        /// </summary>
        /// <typeparam name="T">Value type of the storage</typeparam>
        /// <param name="storage">The storage reference</param>
        /// <param name="value">The new value</param>
        /// <param name="propertyName">The property name</param>
        /// <returns>True if the value was changed</returns>
        protected bool SetProperty<T>(ref T storage, T value, string propertyName)
        {
            if (Equals(storage, value))
                return false;

            storage = value;

            CurrentDispatcher.Invoke(() =>
            {
                CommandManager.InvalidateRequerySuggested();
                FirePropertyChanged(propertyName);
            });

            return true;
        }

        /// <summary>
        /// Sets the storage (of a property) to the value and fires property changed event only if the storage value is changed
        /// </summary>
        /// <typeparam name="T">Value type of the storage</typeparam>
        /// <param name="doStore">The storage action</param>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        /// <param name="propertyNames">The property names list the properyt changed event is fired for</param>
        /// <returns>True if the value was changed</returns>
        protected bool SetProperty<T>(Action<T> doStore, T oldValue, T newValue, params string[] propertyNames)
        {
            if (Equals(oldValue, newValue))
                return false;

            doStore(newValue);

            CurrentDispatcher.Invoke(() => FirePropertiesChanged(propertyNames));

            return true;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void FirePropertyChanged([CallerMemberName] string propertyName = null)
        {
            FirePropertiesChanged(propertyName);
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyNames">Name of the property.</param>
        public void FirePropertiesChanged(params string[] propertyNames)
        {
            foreach (string propName in propertyNames)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}