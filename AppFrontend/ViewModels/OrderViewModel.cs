﻿using App.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace AppFrontend.ViewModels
{
    public class OrderViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int ID { get; set; }

        public DateTime startTime { get; set; }
        public DateTime StartTime
        {
            get { return startTime; }
            set
            {
                if (startTime != value)
                {
                    startTime = value;
                    OnPropertyChanged(nameof(StartTime));
                }
            }
        }

        public DateTime finishTime { get; set; }
        public DateTime FinishTime
        {
            get { return finishTime; }
            set
            {
                if (finishTime != value)
                {
                    finishTime = value;
                    OnPropertyChanged(nameof(FinishTime));
                }
            }
        }

        public TimeSpan time { get; set; }
        public TimeSpan Time
        {
            get { return time; }
            set
            {
                if (time != value)
                {
                    time = value;
                    OnPropertyChanged(nameof(Time));
                }
            }
        }

        private int duration;
        public int Duration
        {
            get { return duration; }
            set
            {
                if (duration != value)
                {
                    duration = value;
                    OnPropertyChanged(nameof(Duration));
                }
            }
        }

        public string ServiceName { get; set; }

        private float paymentAmount;
        public float PaymentAmount
        {
            get { return paymentAmount; }
            set
            {
                if (paymentAmount != value)
                {
                    paymentAmount = value;
                    OnPropertyChanged(nameof(PaymentAmount));
                }
            }
        }

        public Dictionary<string, string> Details { get; set; }

        public ObservableCollection<MaterialDTO> Materials { get; set; }

        public float Total
        {
            get
            {
                float sum = PaymentAmount;
                if(Materials != null)
                {
                    foreach(var material in Materials)
                    {
                        sum += material.Total;
                    }
                }
                return sum;
            }
        }

        public double TotalTVA
        {
            get
            {
                return Total * 0.19;
            }
        }

        public bool isCurrentOrder { get; set; }
        public bool IsCurrentOrder
        {
            get { return isCurrentOrder; }
            set
            {
                if (isCurrentOrder != value)
                {
                    isCurrentOrder = value;
                    OnPropertyChanged(nameof(IsCurrentOrder));
                }
            }
        }

        public bool IsFinished { get; set; }

        public bool IsNotFinished
        {
            get { return !IsFinished && DateTime.Now <= StartTime; }
        }

        public Color Color { get; set; }

        public OrderViewModel(OrderDTO order)
        {
            this.ID = order.ID;
            this.StartTime = order.StartTime;
            this.FinishTime = order.FinishTime;
            this.Duration = order.Duration;
            this.ServiceName = order.ServiceName;
            this.PaymentAmount = order.PaymentAmount;
            this.IsFinished = order.IsFinished;
            this.Materials = new ObservableCollection<MaterialDTO>();
            Time = new TimeSpan(10, 0, 1);
            SetOrderColor();
            SetCurrentOrder();
        }

        private void SetOrderColor()
        {
            DateTime currentTime = DateTime.Now;
            DateTime startTime = this.StartTime;
            DateTime finishTime = this.FinishTime;

            if (currentTime < startTime)
            {
                this.Color = Color.PaleVioletRed;
            }
            else if (startTime <= currentTime && currentTime <= finishTime)
            {
                this.Color = Color.PaleGreen;
            }
            else
            {
                this.Color = Color.SlateGray;
            }
        }

        private void SetCurrentOrder()
        {
            var currentTime = DateTime.Now;
            if(this.startTime <= currentTime && currentTime <= this.finishTime)
            {
                IsCurrentOrder = true;
            }
            else
            {
                isCurrentOrder = false;
            }
        }

        public OrderViewModel()
        {
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
