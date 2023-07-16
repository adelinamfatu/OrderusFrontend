using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace AppFrontend.Converters
{
    public class ExpirationDateColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime expirationDate)
            {
                DateTime currentDate = DateTime.Now.Date;

                if (expirationDate.Date < currentDate)
                {
                    return Color.PaleVioletRed;
                }
            }

            return Color.ForestGreen;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
