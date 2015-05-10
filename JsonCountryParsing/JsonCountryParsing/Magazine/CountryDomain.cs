using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazine.Models.POCO.IdentityCustomization {
    class CountryDomain {
        public int CountryDomainID { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(6)]
        public string Domain { get; set; }

        public int CountryID { get; set; }
    }
}
