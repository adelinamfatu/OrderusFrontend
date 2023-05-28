using App.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace AppFrontend.Converters
{
    public class TimeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderDTO order)
            {
                DateTime currentTime = DateTime.Now;
                DateTime startTime = order.StartTime;
                DateTime finishTime = order.FinishTime;

                if (currentTime < startTime)
                {
                    return Color.SlateGray;
                }
                else if (startTime <= currentTime && currentTime <= finishTime)
                {
                    return Color.PaleGreen;
                }
                else
                {
                    return Color.PaleVioletRed;
                }
            }

            return Color.PaleGreen;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
