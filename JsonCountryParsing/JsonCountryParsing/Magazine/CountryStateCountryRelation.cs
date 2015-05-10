using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazine.Models.POCO.IdentityCustomization
{
    class CountryStateCountryRelation
    {
        public int CountryStateCountryRelationID { get; set; }
        public int CountryID { get; set; }
        public int CountrySateID { get; set; }
    }
}
