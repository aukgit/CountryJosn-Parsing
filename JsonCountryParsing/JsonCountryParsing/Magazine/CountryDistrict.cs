using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazine.Models.POCO.IdentityCustomization
{
    class CountryDistrict {
        public int CountryDistrictID { get; set; }
        
        [Required]
        //[Column(TypeName = "VARCHAR")]
        [StringLength(150)]
        public string DistrictName { get; set; }

        public int CountryID { get; set; }


        public bool IsConfirmDistrict { get; set; }

        public bool  IsCounty { get; set; }
        [ForeignKey("CountryDistrictID")]
        public ICollection<CountryStateDistrictRelation> CountryStateDistrictRelations { get; set; }


    }
}
