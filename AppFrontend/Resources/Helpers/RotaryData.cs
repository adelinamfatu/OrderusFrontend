using AppFrontend.ViewModels;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppFrontend.Resources.Helpers
{
    public class RotaryData
    {
        public string Text => $"{Constants.EmptySpaces}{Sector.Price.ToString()}";
        public SKColor Color => Xamarin.Forms.Color.FromHex(Sector.Color).ToSKColor();

        public RotarySector Sector { get; set; }
    }
}
