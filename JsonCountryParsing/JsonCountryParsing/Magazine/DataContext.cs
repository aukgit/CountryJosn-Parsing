using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magazine.Models.POCO.IdentityCustomization;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Magazine.Models.Context {
    class DataContext : DbContext{
        public DataContext():base("name=DefaultConnection") {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            base.Configuration.LazyLoadingEnabled = false;
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //modelBuilder.Entity<Country>().
            //  HasOptional(e => e.StartingBorder).
            //  WithMany().
            //  HasForeignKey(m => m.BorderBeginingCountryID);
            //modelBuilder.Entity<Country>().
            //  HasOptional(e => e.EndingBorder).
            //  WithMany().
            //  HasForeignKey(m => m.BorderEndingCountryID);

        
        }
            
           

        public static DataContext Create() {
            return new DataContext();
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<CountryAlternativeName> CountryAlternativeNames { get; set; }
        public DbSet<CountryCurrency> CountryCurrencies { get; set; }
        public DbSet<CountryDistrict> CountryDistricts { get; set; }
        public DbSet<CountryLanguage> CountryLanguages { get; set; }
        public DbSet<CountryLanguageRelation> CountryLanguageRelations { get; set; }
        public DbSet<CountryBorder> CountryBorders { get; set; }
        public DbSet<CountryDetectByIP> CountryDetectByIPs { get; set; }
        public ICollection<CountryDomain> CountryDomains { get; set; }
        public DbSet<CountryTimezoneRelation> CountryTimezoneRelations { get; set; }
        public DbSet<CountryTranslation> CountryTranslations { get; set; }

        public DbSet<CountrySate> CountrySates { get; set; }
        public DbSet<CountryStateDistrictRelation> CountryStateDistrictRelations { get; set; }
        public DbSet<CountryStateCountryRelation> CountryStateCountryRelations { get; set; }
        public DbSet<CountryPlace> CountryPlaces { get; set; }
        public DbSet<CountryPlaceType> CountryPlaceTypes { get; set; }
        public DbSet<CountryPlaceAlternative> CountryPlaceAlternatives { get; set; }
        public DbSet<UserTimeZone> UserTimeZones { get; set; }
        public DbSet<SampleTestTable> SampleTestTables { get; set; }

        // <summary>
        // Save changes and sends an email to the developer if any error occurred.
        // </summary>
        // <returns>>=0 :executed correctly. -1: error occurred.</returns>
        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges(); // Important!
            try
            {
                return base.SaveChanges();

            }
            catch (Exception ex)
            {
              //  async email
                DevMVCComponent.Starter.HanldeError.HandleBy(ex, "SaveChanges", "Error SaveChanges()");
                return -1;
            }

        }



        // <summary>
        // Save changes and sends an email to the developer if any error occurred.
        // </summary>
        // <param name="entity">A single entity while saving if any error occurred send the info to the developer as well.</param>
        // <returns>>=0 :executed correctly. -1: error occurred.</returns>
        public int SaveChanges(object entity)
        {
            ChangeTracker.DetectChanges(); // Important!
            try
            {
                return base.SaveChanges();
            }
            catch (Exception ex)
            {
               // async email
                DevMVCComponent.Starter.HanldeError.HandleBy(ex, "SaveChanges", "Error SaveChanges()", entity);
                return -1;
            }
        }

        public int SaveChanges(object entity, string RunnigMethodName)
        {
            ChangeTracker.DetectChanges(); // Important!
            try
            {
                return base.SaveChanges();
            }
            catch (Exception ex)
            {
               // async email
                DevMVCComponent.Starter.HanldeError.HandleBy(ex, "SaveChanges - " + RunnigMethodName, "Error SaveChanges -" + RunnigMethodName + "()", entity);
                return -1;
            }
        }
    }
}
