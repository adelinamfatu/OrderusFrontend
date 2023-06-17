using App.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace AppFrontend.Converters
{
    public class PercentageValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DiscountType discountType)
            {
                if (discountType == DiscountType.Value)
                {
                    return " lei";
                }
                else if (discountType == DiscountType.Percentage)
                {
                    return "%";
                } 
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
