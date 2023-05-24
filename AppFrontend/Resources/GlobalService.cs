using App.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AppFrontend.Resources
{
    public class GlobalService : INotifyPropertyChanged
    {
        private ClientDTO _client;

        public ClientDTO Client 
        { 
            get { return _client; }
            set
            {
                _client = value;
                OnPropertyChanged("Client");
            }
        }

        private CompanyDTO _company;

        public CompanyDTO Company
        {
            get { return _company; }
            set
            {
                _company = value;
                OnPropertyChanged("Company");
            }
        }

        private EmployeeDTO _employee;

        public EmployeeDTO Employee
        {
            get { return _employee; }
            set
            {
                _employee = value;
                OnPropertyChanged("Employee");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
