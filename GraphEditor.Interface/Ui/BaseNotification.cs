#region copyright
/* MIT License

Copyright (c) 2019 Martin Lange (martin_lange@web.de)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */
#endregion

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;

namespace GraphEditor.Interface.Ui
{
    /// <summary>
    /// Base PropertyChanged implementation
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
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
        /// <typeparam name="S">Value type of the view model class</typeparam>
        /// <typeparam name="T">Value type of the storage variable</typeparam>
        /// <param name="storage">The storage reference</param>
        /// <param name="value">The new value</param>
        /// <param name="propertyName">The property name</param>
        /// <param name="onChangedEvent">Extra changed event fired when value is changed</param>
        /// <returns>True if the value was changed</returns>
        protected bool SetProperty<S, T>(ref T storage, T value, string propertyName, Action<S, T> onChangedEvent = null) where S : BaseNotification
        {
            if (Equals(storage, value))
                return false;

            storage = value;

            onChangedEvent?.Invoke((S) this, value);

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
        /// <typeparam name="S">Value type of the view model class</typeparam>
        /// <typeparam name="T">Value type of the storage variable</typeparam>
        /// <param name="onStore">The storage action</param>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        /// <param name="onChangedEvent">Extra changed event fired when value is changed</param>
        /// <param name="propertyNames">The property names list the properyt changed event is fired for</param>
        /// <returns>True if the value was changed</returns>
        protected bool SetProperty<S, T>(Action<T> onStore, T oldValue, T newValue, Action<S, T> onChangedEvent = null, params string[] propertyNames) where S : BaseNotification
        {
            if (Equals(oldValue, newValue))
                return false;

            onStore(newValue);
            onChangedEvent?.Invoke((S) this, newValue);

            CurrentDispatcher.Invoke(() =>
            {
                CommandManager.InvalidateRequerySuggested();
                FirePropertiesChanged(propertyNames);
            });

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