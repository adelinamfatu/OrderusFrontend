using App.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms.MultiSelectListView;

namespace AppFrontend
{
    public class CompanyViewModel : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string StreetNumber { get; set; }

        public string Building { get; set; }

        public string Staircase { get; set; }

        public int? ApartmentNumber { get; set; }

        public int? Floor { get; set; }

        public string Logo { get; set; }

        public string Site { get; set; }

        public string Description { get; set; }

        public string RepresentativeEmail { get; set; }

        public string RepresentativeName { get; set; }

        public string RepresentativeSurname { get; set; }

        public List<CompanyServiceOptionDTO> services = new List<CompanyServiceOptionDTO>();

        public MultiSelectObservableCollection<CompanyServiceOptionDTO> Services { get; set; }

        public CompanyViewModel(CompanyDTO company)
        {
            this.ID = company.ID;
            this.Name = company.Name;
            this.City = company.City;
            this.Street = company.Street;
            this.StreetNumber = company.StreetNumber;
            this.Building = company.Building;
            this.Staircase = company.Staircase;
            this.ApartmentNumber = company.ApartmentNumber;
            this.Floor = company.Floor;
            this.Logo = company.Logo;
            this.Site = company.Site;
            this.Description = company.Description;
            this.RepresentativeEmail = company.RepresentativeEmail;
            this.RepresentativeName = company.RepresentativeName;
            this.RepresentativeSurname = company.RepresentativeSurname;
            this.Services = new MultiSelectObservableCollection<CompanyServiceOptionDTO>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
