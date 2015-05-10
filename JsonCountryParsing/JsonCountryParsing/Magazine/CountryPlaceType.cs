using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazine.Models.POCO.IdentityCustomization {
    class CountryPlaceType {
        public int CountryPlaceTypeID { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(120)]
        public string PlaceType { get; set; }
        [ForeignKey("CountryPlaceTypeID")]
        public ICollection<CountryPlace> CountryPlaces { get; set; }
    }
}
