﻿using App.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace AppFrontend.ViewModels
{
    public class EmployeeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string email { get; set; }
        public string Email
        {
            get { return email; }
            set
            {
                if (email != value)
                {
                    email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public string Surname { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public bool isConfirmed { get; set; }
        public bool IsConfirmed
        {
            get { return isConfirmed; }
            set
            {
                if (isConfirmed != value)
                {
                    isConfirmed = value;
                    OnPropertyChanged(nameof(IsConfirmed));
                }
            }
        }

        public string Picture { get; set; }

        public ObservableCollection<ServiceDTO> Services { get; set; }

        public EmployeeViewModel(EmployeeDTO employee, List<ServiceDTO> services = null)
        {
            this.Email = employee.Email;
            this.Name = employee.Name;
            this.Surname = employee.Surname;
            this.Phone = employee.Phone;
            this.IsConfirmed = employee.IsConfirmed;
            this.Picture = employee.Picture;
            this.Services = new ObservableCollection<ServiceDTO>();
            if (this.IsConfirmed == false)
            {
                foreach(var service in services)
                {
                    this.Services.Add(service);
                }
            }
            else
            {
                foreach(var service in employee.Services)
                {
                    this.Services.Add(service);
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
