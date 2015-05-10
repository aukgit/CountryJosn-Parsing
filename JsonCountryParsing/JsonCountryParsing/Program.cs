using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonCountryParsing.CountryParsing;
using Magazine.Models.Context;
using System.Globalization;
using System.IO;
namespace JsonCountryParsing {
    class Program {

        static void Intial() {
            CountryParser parser = new CountryParser();
            CountryProcessingToDatabase dbProcessor = new CountryProcessingToDatabase();
            parser.InsertAllLanguageThenCountries("allLanguages.json", "countryjson.json");
            ProcessCulture2();
            dbProcessor.InsertPageNumbersInCountryTable();
        }

        static void CultureTest() {

            CultureInfo[] cinfo = CultureInfo.GetCultures(CultureTypes.AllCultures);
            CountryProcessingToDatabase dbProcessor = new CountryProcessingToDatabase();
            foreach (CultureInfo cul in cinfo) {
                RegionInfo ri = null;
                try {
                    ri = new RegionInfo(cul.Name);
                    string TwoLetterISORegionName = ri.TwoLetterISORegionName;
                    string ThreeLetterISORegionName = ri.ThreeLetterISORegionName;
                    string TwoLetterISOLanguageName = cul.TwoLetterISOLanguageName;
                    string ThreeLetterISOLanguageName = cul.ThreeLetterISOLanguageName;
                    var country = dbProcessor.Countries.FirstOrDefault(n => n.Alpha2Code == TwoLetterISORegionName);

                    string RegionName = cul.Name;
                    if (country != null) {
                        if (string.IsNullOrEmpty(country.Culture)) {
                            country.Culture = RegionName;
                        }
                        Console.WriteLine("RegionName :" + RegionName + " ;        Country:" + country.CountryName);
                    } else {
                        Console.WriteLine("Country not found for : " + TwoLetterISORegionName);
                    }
                    dbProcessor.SaveChanges();
                } catch {
                    continue;
                }
            }
        }
        static void ProcessCountries() {
            CountryParser parser = new CountryParser();
            CountryProcessingToDatabase dbProcessor = new CountryProcessingToDatabase();

            string fileloc = "Log.txt";

            //-----
            Console.Write("Input Country 2 Digit Alpha Code:");
            var inputCountryCode = Console.ReadLine();
            var countryX = dbProcessor.Countries.FirstOrDefault(n => n.Alpha2Code == inputCountryCode);
            int countryid = 0;
            if (countryX != null) {
                countryid = countryX.CountryID;
            }
            var countries = dbProcessor.Countries.Where(n => n.CountryID >= countryid).ToList();
            foreach (var country in countries) {
                //string url = "http://www.geonames.org/search.html?q=" + country.CountryName;
                //string url = "http://www.geonames.org/advanced-search.html?q=&country=BD&featureClass=A&continentCode=";
                string url = "http://www.geonames.org/advanced-search.html?q=&country=" + country.Alpha2Code + "&featureClass=A";
                File.AppendAllText(fileloc, DateTime.Now + " :: " + url + "\n");
                bool isFeatured = true;
                Console.WriteLine(url);

                int length = 101;//183189;//183189
                length = parser.GetPagesLength(url, isFeatured);

                Console.WriteLine("Length : " + length);
                //length = 1;
                if (length >= 101) {
                    length = 101;
                }
                for (int i = 0; i < length; i++) {

                    Console.Write("Page : " + (i + 1) + " downloading...");
                    var items = parser.ParseHTML(url, i, 0, -1, true, country);

                    //var items = parser.ParseHTML(url, i, 24, 1);
                    Console.Write(" downloaded... processing...");
                    if (items != null) {
                        foreach (var item in items) {
                            //DisplayCountrytemp.Print(item, -1);
                            dbProcessor.EntryWholeCountryFromCounrtyTemp(item);
                        }
                        Console.Write(" done.\n");
                        File.AppendAllText(fileloc, DateTime.Now + " :: Done Page ::" + (i + 1) + "\n");
                    } else {
                        Console.Write(" none found.\n");
                        File.AppendAllText(fileloc, DateTime.Now + " :: Error not found ::" + url + " ; page no :" + (i + 1) + "\n");
                    }
                }
            }


            dbProcessor.SaveErrorList();
        }
        static void ProcessCulture2() {
            CountryParser parser = new CountryParser();
            CountryProcessingToDatabase dbProcessor = new CountryProcessingToDatabase();
            var cultures = parser.ParseCultureFromWebsite();
            dbProcessor.InsertCultureforCountries(cultures);
        }


        static void ParseDistrictANDSates() {

            SampleTestParsedandSave sampleTesting = new SampleTestParsedandSave();
            //sampleTesting.ParseAndSaveInDbCounty();

            //sampleTesting.ParseRoute(isCounty: true);
            //sampleTesting.ParseRoute(isState: true);
            //sampleTesting.ParseRoute(isDivision: true);
            sampleTesting.ParseRoute(isDistrict: true);
        }
        static void ParseIPAddress() {

            CountryIpAddressParse ip = new CountryIpAddressParse();

            ip.ParseIpAddress(startPage: 0);
        }
        static void ParseIPAddress2() {

            CountryIpAddressParse ip = new CountryIpAddressParse();

            //Console.WriteLine(ip.GetValueWithOutQuote("\"Hello World\""));
            ip.ParseIpAddress();

        }

        static void TimeZonesProcess() {
            CountryParser parser = new CountryParser();
            parser.InsertAllTimeZones("countryjson.json");
        }

        static void InsertSingleTimezonetoCountryTable() {
            DataContext db = new DataContext();
            Console.WriteLine("Start processing country zone.");
            var countryTimezoneRels = db.CountryTimezoneRelations.ToList();
            foreach (var timeZoneRel in countryTimezoneRels) {
                var multiple = countryTimezoneRels.Count(n => n.CountryID == timeZoneRel.CountryID) > 1;
                if (!multiple) {
                    //single
                    var country = db.Countries.Find(timeZoneRel.CountryID);
                    country.RelatedTimeZoneID = timeZoneRel.UserTimeZoneID;
                    country.IsSingleTimeZone = true;
                    db.Entry(country).State = System.Data.Entity.EntityState.Modified;

                }
            }

            db.SaveChanges();
            Console.WriteLine("Done processing country zone.");
        }


        /// <summary>
        /// input as 1:00,2:30 -> 2.3 , 1 and so on.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        static float TimeToValue(string time) {
            var parts = time.Split(':');
            float f = float.Parse(parts[0]);
            string w = parts[1];
            float w2 = float.Parse(w);
            if (f >= 0) {
                w = "." + parts[1];
                w2 = float.Parse(w);

                return f + w2;
            } else {
                if (w2 > 2) {
                    var str2 = f.ToString();

                }
                var str = f.ToString();
                if (w2 > 0) {
                    str += "." + w2.ToString();
                }
                return float.Parse(str);
            }

        }
        static void PopulateTimeZoneValueField() {
            DataContext db = new DataContext();
            Console.WriteLine("Start processing timezone value field.");
            var zones = db.UserTimeZones.ToList();
            foreach (var zone in zones) {
                string valueStr = zone.UTCName.Replace("UTC+", "");
                valueStr = zone.UTCName.Replace("UTC-", "");
                valueStr = zone.UTCName.Replace("UTC", "");
                if (string.IsNullOrWhiteSpace(valueStr)) {
                    valueStr = "0";
                    zone.UTCValue = 0;
                } else {
                    //convert 1:00 to value
                    zone.UTCValue = TimeToValue(valueStr);
                }
                zone.TimePartOnly = valueStr;


            }

            db.SaveChanges();
            Console.WriteLine("Done processing timezone value field.");
        }
        static void PopulateCountryDisplayName() {
            DataContext db = new DataContext();
            Console.WriteLine("Start processing country display name.");
            var countries = db.Countries.ToList();
            foreach (var country in countries) {
                string callingCode = (country.CallingCode > 0) ? "+" + country.CallingCode.ToString() : country.CallingCode.ToString();
                callingCode = "(" + callingCode + ") ";
                //var zoneName = db.UserTimeZones.Find(country.RelatedTimeZoneID);
                country.DisplayCountryName = callingCode + country.CountryName;

            }

            db.SaveChanges();
            Console.WriteLine("Done processing country display name.");
        }

        //ProcessCountries();

        //Console.WriteLine("Done all processing.! hit enter to stop.");
        static void Main(string[] args) {
            DevMVCSetup setup = new DevMVCSetup();

            //PopulateCountryDisplayName();
            ParseIPAddress2();
            Console.ReadKey();
        }
    }
}
