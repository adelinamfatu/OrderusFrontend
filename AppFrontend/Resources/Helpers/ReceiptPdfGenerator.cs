using App.DTO;
using AppFrontend.ViewModels;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace AppFrontend.Resources.Helpers
{
    public class ReceiptPdfGenerator
    {
        public async Task GeneratePdf(OrderViewModel Order, CompanyDTO Company)
        {
            var document = new PdfDocument();

            var page = document.AddPage();

            var gfx = XGraphics.FromPdfPage(page);

            var bigFont = new XFont("Arial", 16);
            var smallFont = new XFont("Arial", 12);

            var centerX = page.Width / 2;
            var centerY = 10;

            var companyName = $"S.C. {Company.Name} S.R.L.";
            var companyNameSize = gfx.MeasureString(companyName, bigFont);
            var companyNameX = centerX - (companyNameSize.Width / 2);
            var companyNameY = centerY + (companyNameSize.Height / 2);
            gfx.DrawString(companyName, bigFont, XBrushes.Black, new XPoint(companyNameX, companyNameY));

            var companyAddress = $"{Company.Street} {Company.StreetNumber}";
            var companyAddressSize = gfx.MeasureString(companyAddress, bigFont);
            var companyAddressX = centerX - (companyAddressSize.Width / 2);
            var companyAddressY = companyNameY + (2 * companyAddressSize.Height);
            gfx.DrawString(companyAddress, bigFont, XBrushes.Black, new XPoint(companyAddressX, companyAddressY));

            var city = "Bucuresti";
            var citySize = gfx.MeasureString(city, bigFont);
            var cityX = centerX - (citySize.Width / 2);
            var cityY = companyAddressY + (2 * citySize.Height);
            gfx.DrawString(city, bigFont, XBrushes.Black, new XPoint(cityX, cityY));

            var emptyRowHeight = 10;

            var serviceName = Order.ServiceName;
            var amount = $"{Order.PaymentAmount:F2}";
            var margin = 20;
            var amountSize = gfx.MeasureString(amount, smallFont);
            var amountSizeWidth = amountSize.Width;
            var serviceRowX = margin;
            var serviceRowY = cityY + (2 * amountSize.Height) + emptyRowHeight;
            var amountX = page.Width - margin - amountSizeWidth;
            gfx.DrawString(serviceName, smallFont, XBrushes.Black, new XPoint(serviceRowX, serviceRowY));
            gfx.DrawString(amount, smallFont, XBrushes.Black, new XPoint(amountX, serviceRowY));

            var lastY = serviceRowY;

            foreach(var material in Order.Materials)
            {
                var materialQuantity = $"{material.Quantity:F2} BUC x {material.Price:F2}";
                var materialSize = gfx.MeasureString(materialQuantity, smallFont);
                var materialX = margin + 30;
                var materialY = lastY + (2 * materialSize.Height);
                gfx.DrawString(materialQuantity, smallFont, XBrushes.Black, new XPoint(materialX, materialY));
                lastY = materialY;

                var materialName = material.Name;
                var materialAmount = $"{material.Total:F2}";
                var materialNameSize = gfx.MeasureString(materialName, smallFont);
                var materialAmountSize = gfx.MeasureString(materialAmount, smallFont);
                var materialAmountSizeWidth = materialAmountSize.Width;
                var materialNameX = margin;
                var materialAmountX = page.Width - margin - materialAmountSizeWidth;
                var materialNameY = lastY + (2 * materialNameSize.Height);
                gfx.DrawString(materialName, smallFont, XBrushes.Black, new XPoint(materialNameX, materialNameY));
                gfx.DrawString(materialAmount, smallFont, XBrushes.Black, new XPoint(materialAmountX, materialNameY));
                lastY = materialNameY;
            }

            var total = "Total";
            var orderTotal = $"{Order.Total:F2}";
            var totalSize = gfx.MeasureString(orderTotal, bigFont);
            var totalSizeWidth = totalSize.Width;
            var totalX = margin;
            var totalY = lastY + (2 * totalSize.Height);
            var orderTotalX = page.Width - margin - totalSizeWidth;
            gfx.DrawString(total, bigFont, XBrushes.Black, new XPoint(totalX, totalY));
            gfx.DrawString(orderTotal, bigFont, XBrushes.Black, new XPoint(orderTotalX, totalY));

            var totalTVA = "Total TVA";
            var orderTotalTVA = $"{Order.TotalTVA:F2}";
            var totalTVASize = gfx.MeasureString(orderTotalTVA, smallFont);
            var totalTVASizeWidth = totalTVASize.Width;
            var totalTVAX = margin;
            var totalTVAY = totalY + (2 * totalTVASize.Height);
            var orderTotalTVAX = page.Width - margin - totalTVASizeWidth;
            gfx.DrawString(totalTVA, smallFont, XBrushes.Black, new XPoint(totalTVAX, totalTVAY));
            gfx.DrawString(orderTotalTVA, smallFont, XBrushes.Black, new XPoint(orderTotalTVAX, totalTVAY));

            var documentsPath = GetDocumentsFolderPath();

            var fileName = $"{Company.Name}_{Order.ServiceName}_{Order.StartTime.ToString("ddMMyyyy")}.pdf";

            var filePath = Path.Combine(documentsPath, fileName);
            document.Save(filePath);

            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }

        private string GetDocumentsFolderPath()
        {
            string documentsPath;

            if (Device.RuntimePlatform == Device.Android)
            {
                documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            else
            {
                throw new NotSupportedException("Platform not supported.");
            }

            return documentsPath;
        }
    }
}
