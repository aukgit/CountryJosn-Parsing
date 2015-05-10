using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazine.Models.POCO.IdentityCustomization {
    class CountryLanguage {
        public int CountryLanguageID { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string Language { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(3)]
        public string Code { get; set; }

        [StringLength(60)]
        public string NativeName { get; set; }

        [ForeignKey("CountryLanguageID")]
        public ICollection<CountryLanguageRelation> CountryLanguageRelations { get; set; }


        [ForeignKey("CountryLanguageID")]
        public ICollection<CountryTranslation> CountryTranslations { get; set; }

    }
}
