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
            var po = new PossibleOrderDTO()
            {
                CompanyID = cso.Company.ID,
                ServiceID = cso.Service.ID,
                ClientEmail = clientEmail,
                DateTime = cso.DateTime,
            };

            if (cso.Service.Name == ServiceType.Curatenie.ToString())
            {
                po.Surface = cso.Surface;
                po.NbRooms = cso.NbRooms;
            }

            return po;
        }
    }
}
