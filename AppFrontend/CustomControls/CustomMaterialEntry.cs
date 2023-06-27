using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AppFrontend.CustomControls
{
    public class CustomMaterialEntry : Entry
    {
        public static readonly BindableProperty MaximumValueProperty =
        BindableProperty.Create(nameof(MaximumValue), typeof(int), typeof(CustomEntry), default(int));

        public int MaximumValue
        {
            get { return (int)GetValue(MaximumValueProperty); }
            set { SetValue(MaximumValueProperty, value); }
        }

        public CustomMaterialEntry()
        {
            TextChanged += OnTextChanged;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(e.NewTextValue, out int enteredValue) && enteredValue > MaximumValue)
            {
                Text = MaximumValue.ToString();
            }
        }
    }
}
