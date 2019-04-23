#region copyright
// Initial developer of the original code is Martin Lange.
//
// The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); you may not use this file except in
// compliance with the License. You may obtain a copy of the License at https://www.mozilla.org/MPL/
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
// License for the specific language governing rights and limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GraphEditor.Ui.Converters
{
    public class BooleanConverter<T> : IValueConverter
    {
        public T TrueValue { get; set; }
        public T FalseValue { get; set; }

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && (bool) value ? TrueValue : FalseValue;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is T && EqualityComparer<T>.Default.Equals((T) value, TrueValue);
        }
    }

    public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
    }
}
