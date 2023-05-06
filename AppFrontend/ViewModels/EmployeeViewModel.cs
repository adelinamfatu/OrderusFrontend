using App.DTO;
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

        public string Email { get; set; }

        public string Surname { get; set; }

        public string Name { get; set; }

        public bool IsConfirmed { get; set; }

        public ObservableCollection<ServiceDTO> Services { get; set; }

        public EmployeeViewModel(EmployeeDTO employee, List<ServiceDTO> services = null)
        {
            this.Email = employee.Email;
            this.Name = employee.Name;
            this.Surname = employee.Surname;
            this.IsConfirmed = employee.IsConfirmed;
            if(this.IsConfirmed == false)
            {
                this.Services = new ObservableCollection<ServiceDTO>();
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
