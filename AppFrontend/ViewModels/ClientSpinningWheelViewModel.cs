using AppFrontend.Resources.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AppFrontend.ViewModels
{
    public class ClientSpinningWheelViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        protected bool SetProperty<T>(ref T backingStore, T value,
          [CallerMemberName] string propertyName = "",
          Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Events
        public event EventHandler DataReady;

        #endregion

        #region Properties
        private double _refershRate;
        public double RefreshRate
        {
            get => _refershRate; set
            {
                SetProperty(ref _refershRate, value);
            }
        }
        private bool _isSpinning;
        public bool IsSpinning
        {
            get => _isSpinning; set
            {
                SetProperty(ref _isSpinning, value);
            }
        }

        private bool _enableHaptic;
        public bool EnableHaptic
        {
            get => _enableHaptic; set
            {
                SetProperty(ref _enableHaptic, value);
            }
        }


        private RotaryViewModel _rotaryModel;
        public RotaryViewModel RotaryModel
        {
            get => _rotaryModel; set
            {
                SetProperty(ref _rotaryModel, value);
            }
        }
        private RotarySector _prize;
        public RotarySector Prize
        {
            get => _prize; set
            {
                SetProperty(ref _prize, value);
            }
        }
        private List<RotaryData> _chartData;
        public List<RotaryData> ChartData
        {
            get => _chartData; set
            {
                SetProperty(ref _chartData, value);
            }
        }
        private List<string> _colors;
        public List<string> Colors
        {
            get => _colors; set
            {
                SetProperty(ref _colors, value);
            }
        }
        private List<int> _invalidPoints;
        public List<int> InvalidPoints
        {
            get => _invalidPoints; set
            {
                SetProperty(ref _invalidPoints, value);
            }
        }

        #endregion
        public ClientSpinningWheelViewModel()
        {
            LoadData();
        }

        #region Methods

        private void LoadData()
        {
            Colors = new List<string>() { "#FF5A5F", "#7FBA00", "#FF5A5F", "#4267B2", "#FF5A5F", "#FFB900", "#FF5A5F", "#0F9D58", "#FF5A5F", "#737373", "#FF5A5F", "#00A4EF" };
            RotaryModel = new RotaryViewModel
            {
                Sectors = new List<RotarySector>()
            };

            int value = 5;
            int percentage = 10;
            for (int i = 0; i < 12; i++)
            {
                var price = "ghinion";
                if (i == 1 || i == 5 || i == 9)
                {
                    price = value.ToString() + " lei";
                    value += 5;
                }
                else if(i == 3 || i == 7 || i == 11)
                {
                    price = percentage.ToString() + "%";
                    percentage += 5;
                }

                RotaryModel.Sectors.Add(new RotarySector()
                {
                    Color = Colors[i],
                    Price = price
                });
            }

            ChartData = new List<RotaryData>();
            foreach (var sector in RotaryModel.Sectors)
            {
                ChartData.Add(new RotaryData() { Sector = sector });
            }

            int total = 360;
            int sides = ChartData.Count;
            double divisor = total / sides;
            double counter = divisor;
            InvalidPoints = new List<int>();
            while (counter <= total)
            {
                InvalidPoints.Add((int)Math.Round(counter, MidpointRounding.AwayFromZero));
                counter += divisor;
            }

            DataReady?.Invoke(this, null);
        }

        #endregion
    }
}
