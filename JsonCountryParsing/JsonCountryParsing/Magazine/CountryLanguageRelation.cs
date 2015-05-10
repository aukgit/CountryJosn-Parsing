using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazine.Models.POCO.IdentityCustomization {
    class CountryLanguageRelation {
        public int CountryLanguageRelationID { get; set; }
        public int CountryID { get; set; }
        public int CountryLanguageID { get; set; }
    }
}
