using Magazine.Models.Context;
using Magazine.Models.POCO.IdentityCustomization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonCountryParsing.CountryParsing
{
    class CountryProcessingToDatabase
    {


        #region Constructors
        public CountryProcessingToDatabase()
        {
            Countries = db.Countries.ToList();
            ErrorFileLocation = "DefaultError.txt";

        }
        public CountryProcessingToDatabase(string errorFileLocation)
        {
            Countries = db.Countries.ToList();
            ErrorFileLocation = errorFileLocation;
        }
        #endregion

        #region Declarations
        private string ErrorFileLocation = "DefaultError.txt";
        private readonly DataContext db = new DataContext();
        public List<Country> Countries;
        private Country LastCountryCached;
        /// <summary>
        /// List of items which are failed to save into the database.
        /// </summary>
        private List<CountryTemp> ErrorItemsList;
        private CountryTemp LastErrorCached;
        ReadWriteJsonFile<List<CountryTemp>> readerWriter = new ReadWriteJsonFile<List<CountryTemp>>();

        #endregion

        #region Culture entry in database
        public void InsertCultureforCountries(List<CustomCultureInfo> cultures)
        {
            foreach (var culture in cultures)
            {
                var country = Countries.FirstOrDefault(n => n.Alpha2Code == culture.CountryCode2 || n.CountryName == culture.CountryName);
                if (country != null)
                {
                    country.Culture = culture.Culture;
                }
            }
            SaveChanges();
            Console.WriteLine("All cultures are updated in country table.");
        }

        #endregion

        #region Alternative Place names
        
        #endregion

        #region Pages update in country Table
        public void InsertPageNumbersInCountryTable()
        {
            foreach (var country in Countries)
            {
                string url = "http://www.geonames.org/advanced-search.html?q=&country=" + country.Alpha2Code + "&featureClass=A";
                CountryParser parser = new CountryParser();
                int page = parser.GetPagesLength(url, true);
                //country.PagesAvailableOnGeolocationSite = (short)page;

            }
            if (SaveChanges() > -1)
            {
                Console.WriteLine("Successfully pages updated.");
            }
            else
            {
                Console.WriteLine("There are some bugs in the page update.");
            }
        }
        #endregion

        #region Search and Get Place
        public CountryPlace GetPlace(CountryTemp c, Country country)
        {
            return db.CountryPlaces.FirstOrDefault(n => n.Name == c.Place && n.CountryID == country.CountryID && (c.XLating >= (n.XLating - 1) && c.XLating <= (n.XLating + 1)) && (c.YLating >= (n.YLating - 1) && c.YLating <= (n.YLating + 1)));
        }
        #endregion

        #region Search and Get Country
        public Country GetCountry(string countryName)
        {
            if (!string.IsNullOrEmpty(countryName))
            {
                if (LastCountryCached != null && countryName.ToLower().Equals(LastCountryCached.CountryName.ToLower()))
                {
                    return LastCountryCached;
                }
                LastCountryCached = Countries.FirstOrDefault(n => n.CountryName == countryName);
                return LastCountryCached;
            }
            return null;
        }
        #endregion

        #region Saving and Retriving Error List

        public void SaveErrorList()
        {
            try
            {
                readerWriter.WriteToJsonFile<List<CountryTemp>>(ErrorFileLocation, ErrorItemsList, true);
            }
            catch (Exception ex)
            {
                DevMVCComponent.Starter.HanldeError.HandleBy(ex, "SaveErrorList");
            }
        }

        //public void SaveErrorList() {
        //    try {
        //        readerWriter.WriteToJsonFile<List<CountryTemp>>(ErrorFileLocation, ErrorItemsList, true);
        //    } catch (Exception ex) {
        //        DevMVCComponent.Starter.HanldeError.HandleBy(ex, "SaveErrorList");
        //    }
        //}

        #endregion

        #region Error Maintain in Country temp
        public void AddError(CountryTemp c, string error)
        {
            if (c.ErrorsList == null)
            {
                c.ErrorsList = new List<string>(10);
            }
            c.ErrorsList.Add(error);
            c.IsErrorExist = true;
            AddErrorToErrorList(c);
        }

        /// <summary>
        /// Add items to the error list of CountryTemp
        /// </summary>
        /// <param name="c"></param>
        public void AddErrorToErrorList(CountryTemp c)
        {
            if (ErrorItemsList == null)
            {
                ErrorItemsList = new List<CountryTemp>(2000);
            }
            if (LastErrorCached != null && LastErrorCached.PageNo == c.PageNo && LastErrorCached.ItemNumber == c.ItemNumber)
            {
                return; //already added
            }

            if (!ErrorItemsList.Any(n => n.PageNo == c.PageNo && n.ItemNumber == c.ItemNumber))
            {
                //not exist in the error list.
                LastErrorCached = c;
                ErrorItemsList.Add(c);
            }
        }



        #endregion

        #region Adding to Database
        public CountrySate AddState(CountryTemp c)
        {
            bool isFailed;
            if (c.IsState)
            {
                if (IsStringNullOrWhite(c.State))
                {
                    return null;
                }
                if (!IsStateExist(c.State))
                {
                    var state = new CountrySate();

                    state.StateName = c.State;
                    state.IsDivision = c.IsDivision;
                    db.CountrySates.Add(state);
                    var failed = db.SaveChanges(state, "AddState");
                    //var failed = db.SaveChanges();
                    isFailed = (failed == -1) ? true : false;
                    if (isFailed)
                    {
                        AddError(c, "State");
                    }
                    return state;
                }
            }
            var x = db.CountrySates.FirstOrDefault(n => n.StateName == c.State);
            if (x == null)
            {
                isFailed = true;
                AddError(c, "State");
            }
            return x;
        }


        public CountryPlaceType AddPlaceType(CountryTemp c)
        {
            bool isFailed;

            if (IsStringNullOrWhite(c.PlaceType))
            {
                return null;
            }
            if (!IsPlaceTypeExist(c.PlaceType))
            {
                if (!string.IsNullOrWhiteSpace(c.PlaceType.Trim()))
                {
                    var o = new CountryPlaceType();

                    o.PlaceType = c.PlaceType;

                    db.CountryPlaceTypes.Add(o);
                    var failed = db.SaveChanges(o, "AddPlaceType");
                    //var failed = db.SaveChanges();
                    isFailed = (failed == -1) ? true : false;
                    if (isFailed)
                    {
                        AddError(c, "AddPlaceType");
                    }
                    return o;
                }
                return null;
            }
            else
            {
                var x = db.CountryPlaceTypes.FirstOrDefault(n => n.PlaceType == c.PlaceType);
                if (x == null)
                {
                    isFailed = true;
                    AddError(c, "PlaceType");
                }
                return x;

            }


        }

        public CountryPlace AddPlace(CountryTemp c, Country country, CountryPlaceType cp)
        {
            bool isFailed;
            if (IsStringNullOrWhite(c.Place))
            {
                return null;
            }
            if (!IsPlaceExist(c))
            {
                var o = new CountryPlace();
                o.Name = c.Place;
                o.XLating = c.XLating;
                o.YLating = c.YLating;
                o.WikiLink = c.WikiLink;
                o.CountryID = country.CountryID;
                o.Area = c.Area;
                if (cp != null)
                    o.CountryPlaceTypeID = cp.CountryPlaceTypeID;
                db.CountryPlaces.Add(o);
                var failed = db.SaveChanges(o, "AddPlace");
                //var failed = db.SaveChanges();
                isFailed = (failed == -1) ? true : false;
                if (isFailed)
                {
                    AddError(c, "AddPlace");
                }
                return o;
            }
            else
            {
                var x = GetPlace(c, country);
                if (x == null)
                {
                    isFailed = true;
                    AddError(c, "Place");
                }
                return x;
            }
        }

        public CountryDistrict AddDistrict(CountryTemp c, Country country)
        {
            bool isFailed;
            if (IsStringNullOrWhite(c.District))
            {
                return null;
            }
            if (c.IsStateOrDistrict)
            {
                if (!IsDistrictExist(c.District))
                {
                    var district = new CountryDistrict();

                    district.DistrictName = c.District;
                    district.CountryID = country.CountryID;
                    district.IsCounty = c.IsCounty;
                    district.IsConfirmDistrict = c.IsConfirmDistrict;
                    db.CountryDistricts.Add(district);
                    var failed = db.SaveChanges(district, "AddDistrict");
                    //var failed = db.SaveChanges();
                    isFailed = (failed == -1) ? true : false;
                    if (isFailed)
                    {
                        AddError(c, "District");
                    }
                    return district;

                }
                else
                {
                    var x = db.CountryDistricts.FirstOrDefault(n => n.DistrictName == c.District);
                    if (x == null)
                    {
                        isFailed = true;
                        AddError(c, "District");
                    }
                    return x;
                }
            }

            return null;
        }
        #endregion

        #region Relation Adding
        public CountryStateCountryRelation AddRelStateCountry(CountryTemp c, CountrySate p1, Country p2)
        {
            bool isFailed;
            if (p1 != null)
            {
                if (!IsStateCountryExist(p1, p2))
                {
                    var o = new CountryStateCountryRelation();

                    o.CountrySateID = p1.CountrySateID;
                    o.CountryID = p2.CountryID;


                    db.CountryStateCountryRelations.Add(o);
                    var failed = db.SaveChanges(o, "AddRelStateCountry");
                    //var failed = db.SaveChanges();
                    isFailed = (failed == -1) ? true : false;
                    if (!isFailed)
                    {
                        return o;
                    }
                    else
                    {
                        AddError(c, "StateCountryRel");
                        return null;
                    }
                }
            }

            return null;
        }
        public CountryStateDistrictRelation AddRelStateDistrict(CountryTemp c, CountrySate p1, CountryDistrict p2)
        {
            bool isFailed;
            if (p1 != null)
            {
                if (!IsStateDistrictExist(p1, p2))
                {
                    var o = new CountryStateDistrictRelation();

                    o.CountrySateID = p1.CountrySateID;
                    o.CountryDistrictID = p2.CountryDistrictID;


                    db.CountryStateDistrictRelations.Add(o);
                    var failed = db.SaveChanges(o, "AddRelStateDistrict");
                    //var failed = db.SaveChanges();
                    isFailed = (failed == -1) ? true : false;
                    if (!isFailed)
                    {
                        return o;
                    }
                    else
                    {
                        AddError(c, "StateDistrictRel");
                        return null;
                    }
                }
            }
            return null;

        }
        public void AddRelPlaceAlternatives(CountryTemp p1, CountryPlace p2, ref bool isFailed)
        {
            isFailed = false;
            if (p1.AlterNativePlaceNames != null)
            {
                foreach (var item in p1.AlterNativePlaceNames)
                {
                    var o = new CountryPlaceAlternative();
                    o.AlternativeName = item;
                    o.CountryPlaceID = p2.CountryPlaceID;
                    var failed = db.SaveChanges(o, "AddRelPlaceAlternatives");
                    //var failed = db.SaveChanges();
                    isFailed = (failed == -1) ? true : false;
                    if (isFailed)
                    {
                        AddError(p1, "Add Alter Place name :" + item);
                    }
                }
            }
        }
        #endregion

        #region Whole Processing and Adding to Database

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns>False means any of these have any error. Returns true means successful</returns>
        public bool EntryWholeCountryFromCounrtyTemp(CountryTemp c)
        {

            if (IsCountryExist(c.CountryName))
            {
                var country = GetCountry(c.CountryName);
                if (country != null)
                {
                    if (!IsPlaceExist(c, country))
                    {
                        var district = AddDistrict(c, country);
                        var placeType = AddPlaceType(c);
                        var place = AddPlace(c, country, placeType);
                        if (c.IsState)
                        {
                            var state = AddState(c);
                            var stateCountry = AddRelStateCountry(c, state, country);
                            var stateDistrict = AddRelStateDistrict(c, state, district);
                        }
                        if (c.IsErrorExist)
                        {
                            Console.WriteLine("\nError Exist on:");
                            DisplayCountrytemp.Print(c);
                        }
                        return !c.IsErrorExist;
                    }
                }
                //country exist
                return true;

            }
            else
            {
                AddError(c, "Country not found.");
            }
            return false;


        }

        #endregion

        #region Is Exist Queries
        public bool IsStateCountryExist(CountrySate s, Country c)
        {
            return db.CountryStateCountryRelations.Any(n => n.CountrySateID == s.CountrySateID && n.CountryID == c.CountryID);
        }

        public bool IsStateDistrictExist(CountrySate s, CountryDistrict d)
        {
            return db.CountryStateDistrictRelations.Any(n => n.CountrySateID == s.CountrySateID && n.CountryDistrictID == d.CountryDistrictID);
        }
        public bool IsStringNullOrWhite(string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }
        public bool IsCountryExist(string d)
        {
            var x = GetCountry(d);
            if (x != null)
            {
                return true;
            }
            return false;
            //return Countries.Any(n => n.CountryName == d);
        }
        public bool IsDistrictExist(string d)
        {
            return db.CountryDistricts.Any(n => n.DistrictName == d);
        }

        public bool IsStateExist(string d)
        {
            return db.CountrySates.Any(n => n.StateName == d);
        }
        public bool IsPlaceExist(CountryTemp c)
        {
            return db.CountryPlaces.Any(n => n.Name == c.Place && n.XLating == c.XLating && n.YLating == c.YLating);
        }
        public bool IsPlaceExist(CountryTemp c, Country country)
        {
            return db.CountryPlaces.Any(n => n.Name == c.Place && n.CountryID == country.CountryID && (c.XLating >= (n.XLating - 1) && c.XLating <= (n.XLating + 1)) && (c.YLating >= (n.YLating - 1) && c.YLating <= (n.YLating + 1)));
        }

        public bool IsPlaceTypeExist(string d)
        {
            return db.CountryPlaceTypes.Any(n => n.PlaceType == d);
        }
        public bool IsPlaceAltExist(string d)
        {
            return db.CountryPlaceAlternatives.Any(n => n.AlternativeName == d);
        }



        #endregion

        #region Save Data
        public int SaveChanges()
        {
            return db.SaveChanges();
        }
        #endregion
    }
}
