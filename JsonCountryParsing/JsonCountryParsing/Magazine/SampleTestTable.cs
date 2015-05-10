using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Magazine.Models.POCO.IdentityCustomization
{
    class SampleTestTable
    {
        public int SampleTestTableID { get; set; }
        [StringLength(200)]
        public string Title { get; set; }

        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int CountryID { get; set; }

        public bool IsSate { get; set; }
        public bool IsDistrict { get; set; }
        public bool IsCounty { get; set; }
        public bool IsDivision { get; set; }
        public int Population { get; set; }

    }
}
