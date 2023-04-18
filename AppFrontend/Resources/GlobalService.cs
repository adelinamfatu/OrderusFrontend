using App.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AppFrontend.Resources
{
    public class GlobalService
    {
        public event EventHandler<PropertyChangedEventArgs> PropertyChanged;

        private ClientDTO _client = null;

        public ClientDTO Client 
        { 
            get { return _client; }
            set
            {
                _client = value;
                OnPropertyChanged(nameof(Client));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
