using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazine.Models.POCO.IdentityCustomization
{
    class CountrySate
    {
        public int CountrySateID { get; set; }
        //[Column(TypeName = "NVARCHAR")]
        [StringLength(80)]
        public string  StateName { get; set; }

        public bool IsDivision { get; set; }

        [ForeignKey("CountrySateID")]
        public ICollection<CountryStateDistrictRelation> CountryStateDistrictRelations { get; set; }

        [ForeignKey("CountrySateID")]
        public ICollection<CountryStateCountryRelation> CountryStateCountryRelations { get; set; }

    }
}
