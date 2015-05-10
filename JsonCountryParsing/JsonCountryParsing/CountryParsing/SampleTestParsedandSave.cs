using Magazine.Models.Context;
using Magazine.Models.POCO.IdentityCustomization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonCountryParsing.CountryParsing {
    class SampleTestParsedandSave
    {

        private readonly DataContext db = new DataContext();




        public void MainProcessing(string[] lines, bool isCounty = false, bool isState = false, bool isDivision = false, bool isDistrict = false) {
            int processing = 0;
            int previousThousandNumberSaved = 0;
            //lines.AsParallel().ForAll(line=> {
            foreach (var line in lines) {


                var overThousand = 0;
                var remainder  = processing / 1000;
                
                float latitude, longitude;
                int countryId = -1;
                int population = 0;
                int titleCol = 0, latitudeCol = 1,
                    longitudeCol = 2, countryCol = 3,
                    populationCol = 4;

                var columns = line.Split(';');
                var title = columns[titleCol];


                latitude = float.Parse(columns[latitudeCol]);

                longitude = float.Parse(columns[longitudeCol]);


                var alphaCode2 = columns[countryCol];
                population = int.Parse(columns[populationCol]);


                var country = db.Countries.FirstOrDefault(n => n.Alpha2Code == alphaCode2);
                if (country != null) {
                    countryId = country.CountryID;
                } else {
                    throw new Exception("Country not found");
                }


                var sampleTest = new SampleTestTable();
                sampleTest.Title = title;
                sampleTest.IsCounty = isCounty;
                sampleTest.IsDistrict = isDistrict;
                sampleTest.IsSate = isState;
                sampleTest.IsDivision = isDivision;
                sampleTest.Latitude = latitude;
                sampleTest.Longitude = longitude;
                sampleTest.Population = population;
                sampleTest.CountryID = countryId;


                if (sampleTest.Title.Length > 199) {
                    sampleTest.Title = sampleTest.Title.Substring(0, 199);
                }

                //bool existBefore = db.SampleTestTables.Any(n => n.Longitude == longitude && n.Latitude == latitude);
                //if (existBefore) {
                //    Console.WriteLine(sampleTest.Title + " already exist");
                //    return;
                //} else {
                db.SampleTestTables.Add(sampleTest);
                if (previousThousandNumberSaved != remainder) {
                    previousThousandNumberSaved = remainder;
                    Console.WriteLine(++overThousand + "K processed.");
                }
                //Console.WriteLine(++processing + ". '" + sampleTest.Title + "' processed.");
                //}
            }
            //});
        }
        /// <summary>
        /// Process everything based on the route value
        /// </summary>
        /// <param name="isCounty"></param>
        /// <param name="isState"></param>
        /// <param name="isDivision"></param>
        /// <param name="isDistrict"></param>
        public void ParseRoute(bool isCounty = false, bool isState = false, bool isDivision = false, bool isDistrict = false) {

            string[] lines = null;
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            string fileLocation = "";
            try {
                if (isCounty) {
                    fileLocation = "County";
                } else if (isDivision) {
                    fileLocation = "Division";
                } else if (isDistrict) {
                    fileLocation = "District";
                } else if (isState) {
                    fileLocation = "State";
                }

                lines = File.ReadAllLines(appDir + @"Processing Db Records\" + fileLocation + ".txt").AsParallel().ToArray();
            } catch (Exception ex) {
                DevMVCComponent.Starter.HanldeError.HandleBy(ex);
            }
            if (lines != null) {
                MainProcessing(lines, isCounty, isState, isDivision, isDistrict);
                Console.WriteLine("Saving " + fileLocation + ".");
                db.SaveChanges();
                Console.WriteLine("Saved " + fileLocation + " Successfully.");

            } else {
                Console.WriteLine("Lines are null.");
            }

        }

    }
}
