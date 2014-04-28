using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TcpConnectionsViewer.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    internal class FullFilePathToParentDirectoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var fullPath = value as string;
            if (string.IsNullOrWhiteSpace(fullPath))
                return null;

            var fi = new FileInfo(fullPath);
            if (fi != null && fi.Directory != null && !string.IsNullOrWhiteSpace(fi.Directory.FullName))
                return fi.Directory.FullName;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
