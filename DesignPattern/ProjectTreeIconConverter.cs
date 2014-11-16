using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace DesignPattern
{
    [ValueConversion(typeof(string), typeof(bool))]
    class ProjectTreeIconConverter : IValueConverter
    {
        public static ProjectTreeIconConverter Instance = new ProjectTreeIconConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value as string).Contains(@"."))
            {
                Uri uri = new Uri("pack://application:,,,/Resources/file.gif");
                BitmapImage source = new BitmapImage(uri);
                return source;
            }
            else
            {
                Uri uri = new Uri("pack://application:,,,/Resources/folder.png");
                BitmapImage source = new BitmapImage(uri);
                return source;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
