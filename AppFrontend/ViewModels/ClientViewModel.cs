using App.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AppFrontend.ViewModels
{
    public class ClientViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Email { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Phone { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string StreetNumber { get; set; }

        public string Building { get; set; }

        public string Staircase { get; set; }

        public int? ApartmentNumber { get; set; }

        public int? Floor { get; set; }

        public string Picture { get; set; }

        public ClientViewModel(ClientDTO client)
        {
            this.Email = client.Email;
            this.Name = client.Name;
            this.Surname = client.Surname;
            this.Phone = client.Phone;
            this.City = client.City;
            this.Street = client.Street;
            this.StreetNumber = client.StreetNumber;
            this.Building = client.Building;
            this.Staircase = client.Staircase;
            this.ApartmentNumber = client.ApartmentNumber;
            this.Floor = client.Floor;
            this.Picture = client.Picture;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
