using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TcpConnectionsViewer.Properties;

namespace TcpConnectionsViewer.Converters
{
    public class PendingLoadingStringsToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value as string))
                return DependencyProperty.UnsetValue;

            var cellText = value as string;

            if (cellText == Settings.Default.AsyncPropertyPendingText)
                return GetBrush(Settings.Default.AsyncPropertyPendingTextColour);

            if (cellText == Settings.Default.AsyncPropertyLoadingText)
                return GetBrush(Settings.Default.AsyncPropertyLoadingTextColour);

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static Brush GetBrush(string code)
        {
            return (Brush)new BrushConverter().ConvertFromString("#" + code);
        }
    }
}
