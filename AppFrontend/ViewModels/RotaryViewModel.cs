using System;
using System.Collections.Generic;
using System.Text;

namespace AppFrontend.ViewModels
{
    public class RotaryViewModel
    {
        public List<RotarySector> Sectors { get; set; }
    }

    public class RotarySector
    {
        public string Price { get; set; }
        
        public string Color { get; set; }
    }
}
