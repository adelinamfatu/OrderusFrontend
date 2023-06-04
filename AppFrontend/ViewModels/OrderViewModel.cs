using App.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace AppFrontend.ViewModels
{
    public class OrderViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int ID { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime FinishTime { get; set; }

        public int Duration { get; set; }

        public string ServiceName { get; set; }

        public float PaymentAmount { get; set; }

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

        public OrderViewModel(OrderDTO order)
        {
            this.ID = order.ID;
            this.StartTime = order.StartTime;
            this.FinishTime = order.FinishTime;
            this.Duration = order.Duration;
            this.ServiceName = order.ServiceName;
            this.PaymentAmount = order.PaymentAmount;
            this.Materials = new ObservableCollection<MaterialDTO>();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
