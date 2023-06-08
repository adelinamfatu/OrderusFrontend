using App.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppFrontend.Resources.Helpers
{
    public class ConvertCSOToPO
    {
        public static PossibleOrderDTO Convert(CompanyServiceOptionDTO cso, string clientEmail)
        {
            return new PossibleOrderDTO()
            {
                CompanyID = cso.Company.ID,
                ServiceID = cso.Service.ID,
                ClientEmail = clientEmail,
                DateTime = cso.DateTime,
                Surface = cso.Surface,
                NbRooms = cso.NbRooms
            };
        }
    }
}
