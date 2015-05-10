using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazine.Models.POCO.IdentityCustomization {
    class CountryCurrency {
        [Key]
        public int CountryCurrencyID { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(7)]
        [Required]
        public string CurrencyName { get; set; }

        public int CountryID { get; set; }

    }
}
