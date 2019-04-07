using System;
using System.Globalization;
using System.Windows.Data;

namespace GraphEditor.Ui.Converters
{
    public class CalculationConverter : IValueConverter
    {
        private static void PerformOp(string operation, ref double value)
        {
            var opKind = operation.Substring(0, 1);
            var strVal = operation.Substring(1).Replace(CultureInfo.InvariantCulture.NumberFormat.CurrencyDecimalSeparator, CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator);
            var opVal = Double.Parse(strVal);

            switch (opKind)
            {
                case "+":
                    value += opVal;
                    return;

                case "-":
                    value -= opVal;
                    return;

                case "*":
                    value *= opVal;
                    return;

                case "/":
                    value /= opVal;
                    return;
            }

            throw new InvalidOperationException();
        }

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var operations = ((string) parameter).Split('|');

            if (value is double val)
            {
                foreach (var op in operations)
                {
                    PerformOp(op, ref val);
                }
                return val;
            }

            throw new InvalidOperationException();
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
