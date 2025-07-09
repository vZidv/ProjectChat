using ChatClient.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ChatClient.Converters
{
    public class Base64ToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string base64String && !string.IsNullOrEmpty(base64String))
            {
                try
                {
                    return AvatarService.Base64ToBitmapImage(base64String);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка конвертации: {ex.Message}");
                    return null; 
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
