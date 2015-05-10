using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazine.Models.POCO.IdentityCustomization {
    class CountryBorder {
        public int CountryBorderID { get; set; }

        public int BorderCountryID { get; set; }

        public int CountryID { get; set; }
        //[ForeignKey("CountryDistrictID")]
        //public ICollection<Country> CountryStateDistrictRelations { get; set; }
    }
}
