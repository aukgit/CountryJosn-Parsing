using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazine.Models.POCO.IdentityCustomization
{
    class CountryStateDistrictRelation
    {
        public int CountryStateDistrictRelationID { get; set; }
        public int CountrySateID { get; set; }
        public int CountryDistrictID { get; set; }

    }
}
