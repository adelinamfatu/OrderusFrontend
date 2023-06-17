using App.DTO;
using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using AppFrontend.Resources.Helpers;
using AppFrontend.ViewModels;
using Newtonsoft.Json;
using Plugin.Toast;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppFrontend.ContentPages.Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientSpinningWheelPage : ContentPage
    {
        protected ClientSpinningWheelViewModel viewModel;
        Stopwatch stopwatch = new Stopwatch();

        bool _pageIsActive;
        float _degrees;

        private readonly Random random = new Random();

        public GlobalService globalService { get; set; }

        public ClientSpinningWheelPage()
        {
            InitializeComponent();
            globalService = DependencyService.Get<GlobalService>();
            viewModel = new ClientSpinningWheelViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.DataReady += ViewModel_DataReady;
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _pageIsActive = false;
            viewModel.DataReady -= ViewModel_DataReady;
        }
        private void ViewModel_DataReady(object sender, EventArgs e)
        {
            skiaView.InvalidateSurface();
            //UpdateButtons();
        }

        private async Task AnimationLoop(int extension = 0)
        {
            if (viewModel.IsSpinning)
                return;
            viewModel.IsSpinning = true;
            priceBox.Text = string.Empty;
            stopwatch.Reset();
            stopwatch.Start();

            double nextDuration = (random.NextDouble() * 10) + 10;
            if (extension != 0)
                nextDuration = extension;
            else
                viewModel.RefreshRate = Constants.DefaultRefreshRate;
            while (_pageIsActive && stopwatch.Elapsed < TimeSpan.FromSeconds(nextDuration))
            {
                skiaView.InvalidateSurface();
                await Task.Delay(TimeSpan.FromMilliseconds(viewModel.RefreshRate));

                if (stopwatch.Elapsed.TotalSeconds > nextDuration * 3 / 4)
                {
                    viewModel.EnableHaptic = true;
                    viewModel.RefreshRate += viewModel.RefreshRate / 20;
                    if (viewModel.RefreshRate > Constants.DefaultRefreshRate * 25)
                    {
                        viewModel.RefreshRate = Constants.DefaultRefreshRate;
                    }
                }

            }
            stopwatch.Stop();

            int rounder = (int)Math.Round(_degrees + 3.6f, MidpointRounding.AwayFromZero);
            if (viewModel.InvalidPoints.Contains(rounder))
            {
                await Task.Delay(100);
                viewModel.IsSpinning = false;
                AnimationLoop(5);
            }
            else
            {
                GetWinner();
                viewModel.IsSpinning = false;
                viewModel.EnableHaptic = false;
            }
        }

        private void GetWinner()
        {
            priceBox.Text = viewModel.Prize.Price.ToString();
            if(priceBox.TextColor == Color.Red)
            {
                CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.SpinningWheelNoPrize);
            }
            else
            {
                CrossToastPopUp.Current.ShowToastSuccess(ToastDisplayResources.SpinningWheelPrize);
                int discount = 0;
                string text = priceBox.Text;
                if(text.Contains("%"))
                {
                    text = text.Replace(" ", "").Replace("%", "");
                    discount = int.Parse(text);
                    var offer = BuildPercentageDTO(discount);
                    SendOfferData(offer);
                }
                else
                {
                    text = text.Replace(" ", "").Replace("lei", "");
                    discount = int.Parse(text);
                    var offer = BuildValueDTO(discount);
                    SendOfferData(offer);
                }
            }
        }

        private OfferDTO BuildValueDTO(int value)
        {
            return new OfferDTO()
            {
                DiscountValue = value,
                ClientEmail = globalService.Client.Email,
                ExpirationDate = DateTime.Now.AddDays(30)
            };
        }

        private OfferDTO BuildPercentageDTO(int percentage)
        {
            return new OfferDTO()
            {
                DiscountPercentage = percentage,
                ClientEmail = globalService.Client.Email,
                ExpirationDate = DateTime.Now.AddDays(30)
            };
        }

        private async void SendOfferData(OfferDTO offer)
        {
            string url = RestResources.ConnectionURL + RestResources.ClientsURL + RestResources.OfferURL + RestResources.CreateAccountURL;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient httpClient = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("client_token").Result;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var json = JsonConvert.SerializeObject(offer);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(url, content);
            }
        }

        private void OnCanvasViewPaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs args)
        {
            if (viewModel.ChartData == null)
                return;
            if (viewModel.ChartData.Count == 0)
                return;

            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            var y = info.Height / 2;

            canvas.Clear();

            SKPoint center = new SKPoint(info.Width / 2, info.Height / 2);
            float radius = Math.Min(info.Width / 2, info.Height / 2) - 2 * Constants.ExplodeOffset;
            SKRect rect = new SKRect(center.X - radius, center.Y - radius,
                                     center.X + radius, center.Y + radius);

            float startAngle = -90;  

            float xCenter = info.Width / 2;
            float yCenter = info.Height / 2;

            foreach (RotaryData item in viewModel.ChartData)
            {
                float sweepAngle = 360f / viewModel.ChartData.Count;

                using (SKPath path = new SKPath())
                using (SKPaint fillPaint = new SKPaint())
                using (SKPaint outlinePaint = new SKPaint())
                using (SKPaint textPaint = new SKPaint())
                {
                    path.MoveTo(center);
                    path.ArcTo(rect, startAngle, sweepAngle, false);
                    path.Close();

                    fillPaint.Style = SKPaintStyle.Fill;
                    fillPaint.Color = item.Color;

                    outlinePaint.Style = SKPaintStyle.Stroke;
                    outlinePaint.StrokeWidth = 5;
                    outlinePaint.Color = SKColors.White;

                    #region Text Writer
                    textPaint.TextSize = 40;
                    textPaint.StrokeWidth = 1;
                    textPaint.Color = SKColors.White;

                    SKRect textBounds = new SKRect();
                    textPaint.MeasureText(item.Text, ref textBounds);
                    float yText = yCenter - textBounds.Height / 2 - textBounds.Top;

                    // Adjust TextSize property so text is 95% of the ARC
                    // float textWidth = textPaint.MeasureText(item.Text);
                    // textPaint.TextSize = 0.95f * info.Width * textPaint.TextSize / textWidth;

                    #endregion 

                    canvas.Save();

                    DrawRotatedWithMatrices(canvas, path, fillPaint, outlinePaint, item, _degrees, (int)center.X, y);

                    var test_angle = _degrees + (360 / viewModel.ChartData.Count / 2) - (360 / viewModel.ChartData.Count * 2);

                    float sweepAngleText = 360f / viewModel.ChartData.Count;
                    float startAngleText = sweepAngleText - sweepAngleText / 2;
                    foreach (RotaryData itemer in viewModel.ChartData)
                    {
                        canvas.Save();
                        canvas.RotateDegrees(startAngleText + _degrees - 90, xCenter, yCenter);

                        /*if (itemer.Text.Trim().Length > 6)
                            textPaint.TextSize = 30;
                        else
                            textPaint.TextSize = 40;*/
                        textPaint.TextSize = 50;
                        canvas.DrawText(itemer.Text, xCenter, yText, textPaint);
                        canvas.Restore();
                        test_angle += 360 / viewModel.ChartData.Count;

                        if (test_angle > 360)
                            test_angle = test_angle - 360;

                        if (startAngleText > 360)
                            startAngleText = startAngleText - 360;
                        startAngleText += sweepAngleText;
                    }
                    canvas.Restore();
                }

                startAngle += sweepAngle;
            }

            #region Marker
            using (SKPaint fillMarkCirclePaint = new SKPaint())
            using (SKPaint fillMarkCirclePaintOuter = new SKPaint())
            using (SKPaint fillMarkTrianglePaint = new SKPaint())
            {
                fillMarkCirclePaint.Style = SKPaintStyle.StrokeAndFill;
                fillMarkCirclePaintOuter.Style = SKPaintStyle.StrokeAndFill;
                fillMarkCirclePaintOuter.Color = Color.FromHex("#FFF180").ToSKColor();

                // Define an array of rainbow colors
                List<SKColor> colors = new List<SKColor>();

                foreach (var col in viewModel.Colors)
                {
                    colors.Add(Color.FromHex(col).ToSKColor());
                }

                canvas.DrawCircle(args.Info.Width / 2, args.Info.Height / 2, 60, fillMarkCirclePaintOuter); //outer

                fillMarkTrianglePaint.Style = SKPaintStyle.StrokeAndFill;
                fillMarkTrianglePaint.Color = Color.FromHex("#FFF180").ToSKColor();
                SKPath trianglePath = new SKPath();
                trianglePath.MoveTo((args.Info.Width / 2) - 55, args.Info.Height / 2);
                trianglePath.LineTo((args.Info.Width / 2) - 55, args.Info.Height / 2);
                trianglePath.LineTo((args.Info.Width / 2) + 55, args.Info.Height / 2);
                trianglePath.LineTo(args.Info.Width / 2, (float)(args.Info.Height / 2.5));
                trianglePath.Close();
                canvas.DrawPath(trianglePath, fillMarkTrianglePaint);

                SKPoint circle_center = new SKPoint(info.Rect.MidX, info.Rect.MidY);
                fillMarkCirclePaint.Shader = SKShader.CreateSweepGradient(circle_center, colors.ToArray());
                canvas.DrawCircle(args.Info.Width / 2, args.Info.Height / 2, 50, fillMarkCirclePaint);
            }
            #endregion

            float prize_degree = _degrees + (360 / viewModel.ChartData.Count / 2);

            if (_degrees == 0 || Math.Round(_degrees, MidpointRounding.AwayFromZero) == 360)
                prize_degree = _degrees;

            var segment = prize_degree / 360f * viewModel.ChartData.Count;
            var int_segment2 = Math.Round(segment, MidpointRounding.AwayFromZero);
            var realIndex = viewModel.ChartData.Count == viewModel.ChartData.Count - (int)int_segment2 ? 0 : viewModel.ChartData.Count - (int)int_segment2;
            viewModel.Prize = viewModel.ChartData[realIndex].Sector;
            priceBox.Text = viewModel.Prize?.Price.ToString();
            if (viewModel.Prize?.Price.ToString() == "ghinion")
            {
                priceBox.TextColor = Color.Red;
            }
            else
            {
                priceBox.TextColor = Color.Green;
            }
            
            if (viewModel.EnableHaptic)
            {
                TryHaptic();
            }
            IncrementDegrees();
        }

        private void TryHaptic()
        {
            int rounder = (int)Math.Round(_degrees, MidpointRounding.AwayFromZero);
            if (viewModel.InvalidPoints.Contains(rounder))
            {
                HapticHelper.DoHaptic(HapticFeedbackType.Click);
            }
        }

        private void IncrementDegrees()
        {
            if (_degrees >= 360)
            {
                _degrees = _degrees - 360;
            }
            _degrees += 3.6f;
        }

        void DrawRotatedWithMatrices(SKCanvas canvas, SKPath path, SKPaint fill, SKPaint outline, RotaryData item, float degrees, int cx, int cy)
        {
            var identity = SKMatrix.CreateIdentity();
            var translate = SKMatrix.CreateTranslation(-cx, -cy);
            var rotate = SKMatrix.CreateRotationDegrees(degrees);

            //angleBox.Text = degrees.ToString();
            var translate2 = SKMatrix.CreateTranslation(cx, cy);

            SKMatrix.PostConcat(ref identity, translate);
            SKMatrix.PostConcat(ref identity, rotate);
            SKMatrix.PostConcat(ref identity, translate2);


            path.Transform(identity);
            canvas.DrawPath(path, fill);
            canvas.DrawPath(path, outline);
        }

        private void SpinTheWheel(object sender, EventArgs e)
        {
            _pageIsActive = true;
            AnimationLoop();
        }
    }
}