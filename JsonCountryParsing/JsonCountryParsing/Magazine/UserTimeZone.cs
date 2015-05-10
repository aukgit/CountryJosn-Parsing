using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;



namespace Magazine.Models.POCO.IdentityCustomization {
    class UserTimeZone {
        [Key]
        public int UserTimeZoneID { get; set; }
        /// <summary>
        /// Windows TimeInfo ID
        /// </summary>
        [Column(TypeName = "VARCHAR")]
        [Required]
        [StringLength(50)]
        public string InfoID { get; set; }

        [Column(TypeName = "VARCHAR")]
        [Required]
        [StringLength(70)]
        public string Display { get; set; }

        [Column(TypeName = "VARCHAR")]
        [Required]
        [StringLength(10)]
        public string UTCName { get; set; }
        [Required]
        public float UTCValue { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(10)]
        [Required]
        public string TimePartOnly { get; set; }

        public ICollection<CountryTimezoneRelation> CountryTimezoneRelations { get; set; }

    }
}