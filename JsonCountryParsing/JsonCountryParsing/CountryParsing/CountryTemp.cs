using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonCountryParsing.CountryParsing {
    public class CustomCultureInfo {
        public string CountryName { get; set; }
        public string CountryCode2 { get; set; }
        public string Culture { get; set; }
    }

    

    public class DisplayCountrytemp {
  
     
        #region Display CountryTemp
        public static void Print(CountryTemp c, int CountingIndex = -1) {
            if (CountingIndex > -1) {
                Console.WriteLine("[ " + CountingIndex + " ]Page No:" + c.PageNo + ", Item No:" + c.ItemNumber);
            } else {
                Console.WriteLine("Page No:" + c.PageNo + ", Item No:" + c.ItemNumber);
            }
            Console.WriteLine(" \t\r " + "Country : " + c.CountryName);
            if (c.IsState) {
                Console.WriteLine(" \t\r " + "State : " + c.State);
                Console.Write(" \t\r " + "District : " + c.District);

                if (c.IsCounty) {
                    Console.Write(" \t" + " : It's a county");
                }
                Console.WriteLine();
            } else {
                Console.WriteLine(" \t\r " + "District : " + c.District);
            }
            Console.WriteLine(" \t\r " + "area : " + c.Area);
            Console.WriteLine(" \t\r " + "place(in blue) : " + c.Place);
            if (c.AlterNativePlaceNames != null) {
                Console.WriteLine(" \t\r " + "Alternative Names :");
                Console.Write(" \t\r\t  ");
                foreach (var alt in c.AlterNativePlaceNames) {
                    Console.Write(alt + " ");
                }
                Console.WriteLine("");

            }
            Console.WriteLine(" \t\r " + "type of place : " + c.PlaceType);
            Console.WriteLine(" \t\r " + "WikiLink : " + c.WikiLink);
            Console.WriteLine(" \t\r " + "X Lating : " + c.XLating + ", YLating: " + c.YLating + "\n");
            if (c.ErrorsList != null) {
                Console.WriteLine(" \t\r " + "Errors : ");
                foreach (var err in c.ErrorsList) {
                    Console.Write(" \t\r\t  " + err + " ; ");
                }
                Console.WriteLine("");
            }
        }
        #endregion
    }
    [Serializable]
    public class CountryTemp {
        public int PageNo { get; set; }
        public int ItemNumber { get; set; }
        public string CountryName { get; set; }
        public string District { get; set; }
        public string State { get; set; }
        public string[] AlterNativePlaceNames { get; set; }
        public string Area { get; set; }
        public string Place { get; set; }
        public float XLating { get; set; }
        public float YLating { get; set; }
        public string PlaceType { get; set; }
        public string PopulationType { get; set; }
        public string WikiLink { get; set; }
        public bool IsCounty { get; set; }
        public bool IsState { get; set; }
        public bool IsStateOrDistrict { get; set; }

        public List<string> ErrorsList { get; set; }
        public bool IsErrorExist { get; set; }

        public bool IsDivision { get; set; }

        public bool IsConfirmDistrict { get; set; }


    }
}
