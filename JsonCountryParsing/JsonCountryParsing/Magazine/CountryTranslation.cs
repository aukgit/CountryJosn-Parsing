using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazine.Models.POCO.IdentityCustomization {
    class CountryTranslation {
        public int CountryTranslationID { get; set; }
        public int CountryLanguageID { get; set; }
        [StringLength(50)]
        [Required]
        public string Translation { get; set; }

        public int CountryID { get; set; }
    }
}
