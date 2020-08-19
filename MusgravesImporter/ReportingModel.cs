using System;
using System.Collections.Generic;
using System.Text;

namespace MusgravesImporter
{
    public class ReportingModel
    {
        public string Location { get; set; }
        public string Product { get; set; }
        public string ProductType { get; set; }
        public string Week { get; set; }
        public string Sales { get; set; }
    }

    public class LocationString
    {
        public string PickupFileLocation { get; set; }
        public string CreateFileLocation { get; set; }
    }
}
