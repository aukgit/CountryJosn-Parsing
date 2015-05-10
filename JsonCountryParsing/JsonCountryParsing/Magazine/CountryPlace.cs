using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazine.Models.POCO.IdentityCustomization {
    class CountryPlace {
        public int CountryPlaceID { get; set; }
        [StringLength(120)]
        public string Name { get; set; }
        [StringLength(120)]
        public string Area { get; set; }
        public float XLating { get; set; }
        public float YLating { get; set; }
        [StringLength(120)]

        public string WikiLink { get; set; }

        public int CountryID { get; set; }
        public int? CountryPlaceTypeID { get; set; }
        [ForeignKey("CountryPlaceID")]
        public ICollection<CountryPlaceAlternative> CountryPlaceAlternatives { get; set; }
    }
}
