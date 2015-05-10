using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazine.Models.POCO.IdentityCustomization {
    class CountryTimezoneRelation {
        public int CountryTimezoneRelationID { get; set; }

        public int UserTimeZoneID { get; set; }

        public int CountryID { get; set; }

    }
}
