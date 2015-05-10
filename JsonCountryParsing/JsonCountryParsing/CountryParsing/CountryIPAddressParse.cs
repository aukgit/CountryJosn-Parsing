using Magazine.Models.Context;
using Magazine.Models.POCO.IdentityCustomization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace JsonCountryParsing.CountryParsing
{
    class CountryIpAddressParse
    {


        List<Country> _countries;

        private string _lastCountryAlphaCode2;
        private int _lastCountryId;

        public string GetValueWithOutQuote(string s) {
            int len = s.Length - 1; // for first quote
            len--; // for last quote;
            return s.Substring(1,len);

        }
        public void MainProcessing(DataContext _db,string[] lines)
        {
            int processing = 0;

            //lines.AsParallel().ForAll(line=> {
            var overThousand = 0;
            foreach (var line in lines)
            {


                int remainder = processing % 500;

                long beginingRange = 0, endingRange = 0;
                int beginingCol = 0,
                    endingCol = 1, countryCol = 2;


                var columns = line.Split(',');

                //int findDot = columns[beginingCol].IndexOf('.');
                beginingRange = long.Parse(GetValueWithOutQuote(columns[beginingCol]));
                //findDot = columns[endingCol].IndexOf('.');
                endingRange = long.Parse(GetValueWithOutQuote(columns[endingCol]));


                var alphaCode2 = GetValueWithOutQuote(columns[countryCol]);



                var o = new CountryDetectByIP();
                o.BeginingIP = beginingRange;
                o.EndingIP = endingRange;
                o.CountryID = GetCountry(alphaCode2);


                //bool existBefore = db.SampleTestTables.Any(n => n.Longitude == longitude && n.Latitude == latitude);
                //if (existBefore) {
                //    Console.WriteLine(sampleTest.Title + " already exist");
                //    return;
                //} else {
                processing++;
                if (o.CountryID != -1)
                {
                    _db.CountryDetectByIPs.Add(o);
                }
                //Thread t = new Thread(() =>
                //{
                //    using (DataContext db2 = new DataContext())
                //    {
                //        db2.SaveChangesAsync();
                //    }
                //});
                //t.Start();
                if (remainder == 0 && processing != 1)
                {
                    Console.WriteLine(++overThousand * .5 + "K processed.");
                }
                //Console.WriteLine(++processing + ". '" + sampleTest.Title + "' processed.");
                //}
            }
            //});
        }

        public int GetCountry(string alpha2)
        {
            if (_lastCountryAlphaCode2 == alpha2)
            {
                return _lastCountryId;
            }
            else
            {
                _lastCountryAlphaCode2 = alpha2;
                var country = _countries.FirstOrDefault(n => n.Alpha2Code == alpha2);
                if (country != null)
                {
                    _lastCountryId = country.CountryID;
                    return _lastCountryId;
                }
                else
                {
                    //throw new Exception("Country not found." + alpha2);

                    Console.WriteLine("Country Alpha2 doesn't exist : " + alpha2);
                    return -1;
                }

            }
        }

        public void ParseIpAddress(int startPage = 0, int pagesLen = 3000)
        {

            string[] lines = null;
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            string fileLocation = "allCountryWithIP.csv";
            try
            {
                DataContext _db2 = new DataContext();

                _countries = _db2.Countries.ToList();
                lines = File.ReadAllLines(appDir + @"Processing Db Records\" + fileLocation).ToArray();
            }
            catch (Exception ex)
            {
                DevMVCComponent.Starter.HanldeError.HandleBy(ex);
            }
            if (lines != null)
            {
                int countPages = lines.Count();
                int totalPages = countPages / pagesLen;
                for (int i = startPage; i < totalPages; i++) {
                    var i1 = i;
                    Thread t = new Thread(() => {
                        DataContext _db = new DataContext();
                        Console.WriteLine("Thread/Page Running: " + i1 + ".");
                        Console.WriteLine("Skips :" + i1*pagesLen);
                        Console.WriteLine("Take :" + pagesLen);
                        var currentPageItems = lines.Skip(i1*pagesLen).Take(pagesLen).ToArray();
                        MainProcessing(_db, currentPageItems);
                        Console.WriteLine("Saving " + fileLocation + " Page " + i1 + ".");
                        _db.SaveChanges();
                        Console.WriteLine("Saved " + fileLocation + " Successfully" + " Page/Thread : " + i1 + ".");
                    });
                    t.Start();
                }
            }
            else
            {
                Console.WriteLine("Lines are null.");
            }

        }

    }
}
