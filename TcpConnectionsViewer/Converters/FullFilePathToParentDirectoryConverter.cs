using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace TcpConnectionsViewer.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    internal class FullFilePathToParentDirectoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value as string))
                return DependencyProperty.UnsetValue;

            var fi = new FileInfo(value as string);

            if (fi.Directory == null)
                return DependencyProperty.UnsetValue;

            if (fi != null && fi.Directory != null && !string.IsNullOrWhiteSpace(fi.Directory.FullName))
                return fi.Directory.FullName;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
