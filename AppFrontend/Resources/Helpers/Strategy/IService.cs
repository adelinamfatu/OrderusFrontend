using App.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppFrontend.Resources.Helpers.Strategy
{
    public interface IService
    {
        void SendServiceData(CompanyServiceOptionDTO cso);
    }
}
