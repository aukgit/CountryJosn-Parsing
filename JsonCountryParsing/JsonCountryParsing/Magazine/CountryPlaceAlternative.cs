using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazine.Models.POCO.IdentityCustomization
{
    class CountryPlaceAlternative
    {
        public int CountryPlaceAlternativeID { get; set; }
        [Required]
        [StringLength(40)]
        public string AlternativeName { get; set; }
        public int CountryPlaceID { get; set; }
    }
}
