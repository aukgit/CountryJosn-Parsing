using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazine.Models.POCO.IdentityCustomization {
    class CountryAlternativeName {
        [Key]
        public int CountryAlternativeNameID { get; set; }
        [StringLength(80)]
        [Required]
        public string AlternativeName { get; set; }

        public int CountryID { get; set; }

    }
}
