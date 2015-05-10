using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Magazine.Models.POCO.IdentityCustomization;
using Magazine.Models.Context;
using System.Net;
using HtmlAgilityPack;

namespace JsonCountryParsing.CountryParsing {

    class CountryParser {



        #region Declaration
        string processingURL;
        double numberOfPageLengthCached = 0;
        string jsonCountry;
        string jsonLanguage;
        readonly DataContext db = new DataContext();
        List<JToken> allCountries;
        List<JToken> allLanguages;
        #endregion

        #region Programmatic Processes like Each word upper case , last slash find etc
        public string EachWordUpperCase(String s) {
            if (!string.IsNullOrEmpty(s)) {
                if (s.IndexOf(' ') > -1) {
                    //more than one word
                    var listOfWords = s.Split(' ');
                    var length = listOfWords.Length;
                    for (int i = 0; i < length; i++) {
                        listOfWords[i] = SingleFirstCharWordUpperCase(listOfWords[i]);
                    }
                    s = string.Join(" ", listOfWords);
                    return s;
                }
                return SingleFirstCharWordUpperCase(s);
            }
            return "";
        }

        public string SingleFirstCharWordUpperCase(string s) {
            if (!string.IsNullOrEmpty(s)) {
                StringBuilder sb = new StringBuilder(s.Length);
                sb.Append(char.ToUpper(s[0]));
                if (s.Length >= 2) {
                    sb.Append(s.Substring(1).ToLowerInvariant());
                }
                return sb.ToString();
            }
            return s;
        }
        public string LastSlashFileOrPageName(string s) {
            if (!string.IsNullOrEmpty(s)) {
                var slashFound = s.LastIndexOf('/');
                if (slashFound > -1) {
                    s = s.Substring(slashFound + 1);
                }
            }
            return s;
        }
        #endregion

        #region Process HTML
        /// <summary>
        /// After getting search should be like this htmlDocument.DocumentNode.Descendants("table")
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public HtmlDocument GetHTMLProcessed(string url) {
            HtmlWeb htmlWeb = new HtmlWeb() { AutoDetectEncoding = false, OverrideEncoding = Encoding.GetEncoding("iso-8859-2") };
            return htmlWeb.Load(url);
        }
        #endregion

        #region Parsing Geoname.org website

        public int GetPagesLength(string url, bool featureClassA = false) {
            HtmlWeb htmlWeb = new HtmlWeb() { AutoDetectEncoding = false, OverrideEncoding = Encoding.GetEncoding("iso-8859-2") };
            HtmlDocument htmlDocument = htmlWeb.Load(url);
            HtmlNode links = null;
            int pagesLength = 0;
            var tables = htmlDocument.DocumentNode.Descendants("table").ToList();
            if (tables.Count >= 2) {
                if (featureClassA) {
                    links = tables[2];
                } else {
                    links = tables[1];
                }
            }
            if (links == null) {
                DevMVCComponent.Starter.Mailer.QuickSend("devorg.bd@gmail.com", "table not found in table", "table not found in table");
                return -1;
            }
            var tableRows = links.ChildNodes.Where(x => x.Name == "tr").ToList();

            //generate the page length
            if (processingURL != null && processingURL.Equals(url)) {
                //have cached len
                pagesLength = (int)numberOfPageLengthCached;

            } else {
                //not cached have to generate and then again cached.
                var firstTrTd = tableRows[0].InnerText;
                var spaceExist = firstTrTd.IndexOf(' ');
                var number = firstTrTd.Substring(0, spaceExist);
                numberOfPageLengthCached = double.Parse(number);
                numberOfPageLengthCached = numberOfPageLengthCached / 50;
                numberOfPageLengthCached = Math.Ceiling(numberOfPageLengthCached);
                pagesLength = (int)numberOfPageLengthCached;
                processingURL = url;
            }

            return pagesLength;
        }

        /// <summary>
        /// Parse HTML only from GEONames org not from any other location.
        /// </summary>
        /// <param name="url">Give url like this : http://www.geonames.org/search.html?q=YourQuery</param>
        /// <param name="skipPage">Number of pages should skip, skipPage= skipPage * 50, so if 0 no page skip.</param>
        /// <param name="skipItems">Number of places skips from that page, highest 50. number 0 means nothing to skip. 1 means skip 1</param>
        /// <param name="EndingItemLength">-1 means end after how many exist, or stop after getting only the given number</param>

        public List<CountryTemp> ParseHTML(string url, int skipPage = 0, int skipItems = 0, int EndingItemLength = -1, bool featureClassA = false, Country countryEntity = null) {
            HtmlWeb htmlWeb = new HtmlWeb() { AutoDetectEncoding = false, OverrideEncoding = Encoding.GetEncoding("iso-8859-2") };
            //HtmlDocument htmlDocument = htmlWeb.Load("http://www.geonames.org/search.html?q=&startRow=0");
            //HtmlDocument htmlDocument = htmlWeb.Load("http://www.geonames.org/search.html?q=bangladesh+dhaka+division&country=");
            int pageNo = skipPage;
            skipPage *= 50;
            url += "&startRow=" + skipPage;
            HtmlDocument htmlDocument = htmlWeb.Load(url);
            HtmlNode links = null;
            int length = 0;
            var tables = htmlDocument.DocumentNode.Descendants("table").ToList();
            if (tables.Count >= 2) {
                if (featureClassA) {
                    links = tables[2];
                } else {
                    links = tables[1];
                }
            }
            if (links == null) {
                DevMVCComponent.Starter.Mailer.QuickSend("devorg.bd@gmail.com", "table not found in table", "table not found in table");
                return null;
            }
            var tableRows = links.ChildNodes.Where(x => x.Name == "tr").ToList();



            int startingNumber = 2 + skipItems;

            if (EndingItemLength > 0) {
                length = startingNumber + EndingItemLength;
            } else {
                length = tableRows.Count;
            }
            int itemsListRequired = length - startingNumber - 1;
            List<CountryTemp> list = new List<CountryTemp>(itemsListRequired + 1);
            for (int i = startingNumber; i < length; i++)
            //for (int i =51; i < length; i++)
            {
                var tableData = tableRows[i].ChildNodes.Where(x => x.Name == "td").ToList();
                //Console.WriteLine((i - 1) + " : ");
                if (tableData.Count >= 4) {
                    string country = "", place = "", typeOfPlace = "", area = "", geoMapLink = "";
                    float startingLating = -1, endingLating = -1;
                    string lastPageUrl = "";
                    string alternativePlaceNames;
                    string[] alternativePlaceNamesList = new string[1];
                    string wikiLink = "";

                    var aCountry = tableData[2].Descendants("a").FirstOrDefault();
                    if (aCountry != null) {
                        country = aCountry.InnerText;
                    } else {
                        if (countryEntity != null) {
                            country = countryEntity.CountryName;
                        }
                    }


                    area = tableData[0].Descendants("a").FirstOrDefault().Attributes["href"].Value;
                    lastPageUrl = LastSlashFileOrPageName(area);
                    if (!string.IsNullOrEmpty(lastPageUrl)) {
                        lastPageUrl = lastPageUrl.Replace("google_", "").Replace("_", " ").Replace("-", " ").Replace(".html", "");
                    }
                    area = lastPageUrl;
                    geoMapLink = tableData[1].Descendants("a").FirstOrDefault().Attributes["href"].Value;
                    lastPageUrl = LastSlashFileOrPageName(geoMapLink);
                    if (!string.IsNullOrEmpty(lastPageUrl)) {
                        lastPageUrl = lastPageUrl.Replace("google_", "").Replace("_", " ").Replace(".html", "");
                        var lating = lastPageUrl.Split(' ');
                        if (lating.Length >= 2) {
                            startingLating = float.Parse(lating[0]);
                            endingLating = float.Parse(lating[1]);
                        }
                    }
                    typeOfPlace = tableData[3].InnerHtml;
                    if (!string.IsNullOrEmpty(typeOfPlace)) {
                        typeOfPlace = typeOfPlace.Replace("<br>", " ").Replace("<small>", "").Replace("</small>", "").Trim();
                    }
                    var placeColumnData = tableData[1];
                    var aLinksInPlace = tableData[1].Descendants("a").ToList();
                    if (aLinksInPlace != null) {
                        place = aLinksInPlace.FirstOrDefault().InnerText;
                        if (aLinksInPlace.Count > 1) {
                            wikiLink = aLinksInPlace[1].GetAttributeValue("href", "");
                            wikiLink = LastSlashFileOrPageName(wikiLink);
                        }
                    }

                    var tempList = placeColumnData.Descendants("small").FirstOrDefault();
                    if (tempList != null) {
                        alternativePlaceNames = tempList.InnerText;
                        alternativePlaceNamesList = GetAlternativeNamesOfPlace(alternativePlaceNames);
                    } else {
                        alternativePlaceNamesList = null;
                    }

                    var c = new CountryTemp();
                    c.PageNo = pageNo;
                    c.ItemNumber = i - 1;
                    c.CountryName = country;

                    c.Area = EachWordUpperCase(area);
                    c.Place = place;
                    c.AlterNativePlaceNames = alternativePlaceNamesList;
                    c.PlaceType = typeOfPlace;
                    c.XLating = startingLating;
                    c.YLating = endingLating;
                    c.WikiLink = wikiLink;


                    ProcessDistrictState(c, tableData[2]);

                    list.Add(c);

                    //Console.WriteLine(" \t\r " + "Country : " + country);
                    //Console.WriteLine(" \t\r " + "District : " + district);
                    //Console.WriteLine(" \t\r " + "area : " + area);
                    //Console.WriteLine(" \t\r " + "place(in blue) : " + place);
                    //if (alternativePlaceNamesList != null)
                    //{
                    //    Console.WriteLine(" \t\r " + "Alternative Names :");
                    //    Console.Write(" \t\r\t  ");
                    //    foreach (var alt in alternativePlaceNamesList)
                    //    {
                    //        Console.Write(alt + " ");
                    //    }
                    //    Console.WriteLine("");

                    //}
                    //Console.WriteLine(" \t\r " + "type of place : " + typeOfPlace);
                    //Console.WriteLine(" \t\r " + "geomap starting : " + startingLating);
                    //Console.WriteLine(" \t\r " + "geomap ending : " + endingLating + "\n");
                    //Console.WriteLine(" \t\r " + "WikiLink : " + wikiLink + "\n");


                }
            }
            return list;

        }

        #endregion

        #region District State Processing
        public CountryTemp ProcessDistrictState(CountryTemp c, HtmlNode tdForCountry) {
            string district = null, innerHTML = tdForCountry.InnerHtml;


            if (!string.IsNullOrEmpty(innerHTML) && IsCommaExist(innerHTML)) {
                district = innerHTML.Split(',').ToList()[1];
                c.IsStateOrDistrict = true;



                if (!string.IsNullOrEmpty(district)) {
                    district = district.ToLower();
                    var indexOfBr = district.IndexOf("<br>");
                    if (indexOfBr > -1) {
                        district = district.Substring(0, indexOfBr);
                    }
                    district = district.Trim();
                    if (district.IndexOf("district") > -1) {
                        c.IsConfirmDistrict = true;
                    }
                    district = EachWordUpperCase(district);
                    c.District = district;
                }

                //state process
                var smallNode = tdForCountry.Descendants("small").FirstOrDefault();
                if (smallNode != null) {
                    var smallNodeText = smallNode.InnerText;
                    if (!string.IsNullOrEmpty(smallNodeText)) {
                        c.State = district;
                        c.IsState = true;
                        c.District = EachWordUpperCase(smallNodeText.Trim());
                        var districtLowercase = c.District.ToLower();

                        if (districtLowercase.IndexOf("county") > -1) {
                            c.IsCounty = true;
                        }

                        if (c.State.IndexOf("Division") > -1) {
                            c.IsDivision = true;
                        }
                    }
                }
            }

            return c;
        }

        #endregion


        #region Get Culture info from website
        /// <summary>
        /// http://timtrott.co.uk/culture-codes/
        /// </summary>
        /// <returns></returns>
        public List<CustomCultureInfo> ParseCultureFromWebsite() {
            string url = "http://timtrott.co.uk/culture-codes/";
            var cultures = new List<CustomCultureInfo>(255);
            var html = GetHTMLProcessed(url);
            var table = html.DocumentNode.Descendants("table").FirstOrDefault();
            var rows = table.Descendants("tr").ToList();
            int length = rows.Count;
            for (int i = 1; i < length; i++) {
                var culture = new CustomCultureInfo();
                var columns = rows[i].Descendants("td").ToList();
                culture.CountryName = columns[0].InnerText;
                culture.CountryCode2 = columns[1].InnerText;
                culture.Culture = columns[6].InnerText;
                cultures.Add(culture);
            }

            return cultures;
        }
        #endregion
        public bool IsCommaExist(string s) {
            if (!string.IsNullOrEmpty(s)) {
                return s.IndexOf(',') > -1;
            }
            return false;
        }
        public bool IsCommaExist(string s, ref int index) {
            if (!string.IsNullOrEmpty(s)) {
                index = s.IndexOf(',');
                return index > -1;
            }
            return false;
        }

        #region Getting things State and alternative place names

        public string[] GetState(string s) {
            return s.Split(',');
        }
        public string[] GetAlternativeNamesOfPlace(string s) {
            if (!string.IsNullOrEmpty(s)) {
                return s.Split(',');
            }
            return null;
        }
        #endregion


        #region Read Json File
        public string ReadJson(string loc) {
            return File.ReadAllText(loc);
        }
        #endregion

        #region Safe String Starter
        string GetSafeString(int index, string property) {
            if (allCountries != null) {
                if ((string)allCountries[index][property] == null) {
                    return "";
                } else {
                    return (string)allCountries[index][property];
                }
            }
            return "";
        }
        #endregion

        #region Safe Array
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">starts with 0</param>
        /// <param name="property"></param>
        /// <param name="ArrayIndex">starts with 0</param>
        /// <returns></returns>
        string GetSafeStringArray(int index, string property, int ArrayIndex) {
            if (allCountries != null) {
                if (!allCountries[index][property].HasValues || (allCountries[index][property].Count() - 1) < ArrayIndex) {
                    return "";
                } else {
                    return (string)allCountries[index][property][ArrayIndex];
                }
            }
            return "";
        }
        #endregion

        #region Safe 2
        /// <summary>
        /// If null return -1
        /// </summary>
        /// <param name="index"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        long GetSafeStringNumberSafe(int index, string property, int defReturn = -1) {
            if (allCountries != null) {
                if ((string)allCountries[index][property] == null) {
                    return defReturn;
                } else {
                    return long.Parse((string)allCountries[index][property]);
                }
            }
            return defReturn;
        }

        /// <summary>
        /// If null return -1
        /// </summary>
        /// <param name="index"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        float GetSafeStringNumberFloatSafe(int index, string property, float defReturn = -1) {
            if (allCountries != null) {
                if ((string)allCountries[index][property] == null) {
                    return defReturn;
                } else {
                    return float.Parse((string)allCountries[index][property]);
                }
            }
            return defReturn;
        }

        decimal GetSafeStringNumberDecimalSafe(int index, string property, decimal defReturn = -1) {
            if (allCountries != null) {
                if ((string)allCountries[index][property] == null) {
                    return defReturn;
                } else {
                    return decimal.Parse((string)allCountries[index][property]);
                }
            }
            return defReturn;
        }
        #endregion

        #region Safe Code 3
        /// <summary>
        /// Returns default =-1
        /// </summary>
        /// <param name="s"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        short GetSafeShort(string s, short def = -1) {
            if (string.IsNullOrEmpty(s)) {
                return def;
            } else {
                return short.Parse(s);
            }
        }

        int GetSafeInt(string s, int def = -1) {
            if (string.IsNullOrEmpty(s)) {
                return def;
            } else {
                return int.Parse(s);
            }
        }

        float GetSafeFloat(string s, float def = -1) {
            if (string.IsNullOrEmpty(s)) {
                return def;
            } else {
                return float.Parse(s);
            }
        }

        /// <summary>
        /// Get domain without the . and safely
        /// </summary>
        /// <param name="index"></param>
        /// <param name="insideItem"></param>
        /// <returns></returns>
        string GetTopDomain(int index, int insideItem) {
            string s = GetSafeStringArray(index, "topLevelDomain", insideItem);
            if (!String.IsNullOrEmpty(s)) {
                return s.Remove(0, 1);
            }
            return "";
        }

        string GetTimeZone(int index, int insideItem)
        {
            return GetSafeStringArray(index, "timezones", insideItem);            
        }
        #endregion

        #region Insert All Languages to the DB, Independent
        public void InsertAllLangauges(string locationOfJson) {
            jsonLanguage = ReadJson(locationOfJson);
            var jArray = JArray.Parse(jsonLanguage);
            allLanguages = jArray[0].ToList();
            foreach (var language in allLanguages) {
                string code = ((Newtonsoft.Json.Linq.JProperty)(language)).Name;
                string languageName = (string)language.First["name"];
                string nativeName = (string)language.First["nativeName"];
                var ln = new CountryLanguage();
                ln.Code = code;
                ln.Language = languageName;
                ln.NativeName = nativeName;
                db.CountryLanguages.Add(ln);

            }
            db.SaveChanges(null, "InsertAllLangauges");
            Console.WriteLine("Done Language Database adding.");

        }
        #endregion

        #region Country Additional Properties adding
        public void InsertLanguageCountryRelationship(Country country, int index) {
            string property = "languages";
            if (allCountries[index][property].HasValues) {
                country.CountryLanguageRelations = new List<CountryLanguageRelation>(4);
                int len = allCountries[index][property].Count();

                for (int i = 0; i < len; i++) {
                    string code = GetSafeStringArray(index, property, i);
                    var language = db.CountryLanguages.FirstOrDefault(n => n.Code == code);
                    if (language != null) {
                        var o = new CountryLanguageRelation();
                        o.CountryLanguageID = language.CountryLanguageID;
                        country.CountryLanguageRelations.Add(o);
                    } else {
                        var ln = new CountryLanguage();
                        ln.Code = code;
                        ln.Language = "Unknown";
                        ln.NativeName = "Unknown";
                        db.CountryLanguages.Add(ln);
                    }
                }
            }
        }



        public void InsertBorders(Country country) {

        }
        public void InsertAlternativeNames(Country country, int index) {
            string property = "altSpellings";
            if (allCountries[index][property].HasValues) {
                country.CountryAlternativeNames = new List<CountryAlternativeName>(5);
                int len = allCountries[index][property].Count();

                for (int i = 0; i < len; i++) {
                    var altName = new CountryAlternativeName();
                    altName.AlternativeName = GetSafeStringArray(index, property, i);
                    if (!string.IsNullOrEmpty(altName.AlternativeName)) {
                        country.CountryAlternativeNames.Add(altName);
                    }
                }
            }
        }

        public void InsertCurrencies(Country country, int index) {
            string property = "currencies";
            if (allCountries[index][property].HasValues) {
                country.CountryCurrencies = new List<CountryCurrency>(4);
                int len = allCountries[index][property].Count();

                for (int i = 0; i < len; i++) {
                    var o = new CountryCurrency();
                    o.CurrencyName = GetSafeStringArray(index, property, i);
                    if (!string.IsNullOrEmpty(o.CurrencyName)) {
                        country.CountryCurrencies.Add(o);
                    }
                }
            }
        }
        public void InsertDomain(Country country, int index) {
            string property = "topLevelDomain";
            if (allCountries[index][property].HasValues) {
                country.CountryDomains = new List<CountryDomain>(5);
                int len = allCountries[index][property].Count();

                for (int i = 0; i < len; i++) {
                    var domain = new CountryDomain();
                    domain.Domain = GetTopDomain(index, i);
                    if (!string.IsNullOrEmpty(domain.Domain)) {
                        country.CountryDomains.Add(domain);
                    }
                }
            }
        }
        public void InsertCountryTranslations(Country country, int index) {
            string property = "translations";
            if (allCountries[index][property].HasValues) {
                country.CountryTranslations = new List<CountryTranslation>(4);
                int len = allCountries[index][property].Count();
                var translations = allCountries[index]["translations"].ToList();

                for (int i = 0; i < len; i++) {
                    var single = (Newtonsoft.Json.Linq.JProperty)translations[i];
                    if (single != null) {
                        //translation exist
                        var lan = db.CountryLanguages.FirstOrDefault(n => n.Code == single.Name);
                        if (lan != null && !String.IsNullOrEmpty((string)single.Value)) {
                            var trans = new CountryTranslation();
                            trans.Translation = (string)single.Value;
                            trans.CountryLanguageID = lan.CountryLanguageID;
                            country.CountryTranslations.Add(trans);
                        }

                    }
                }
            }
        }
        #endregion



        #region Insert All Country to the Database. Depends on Language.

        private string GetUtcName(string displayZoneName)
        {
            int start = displayZoneName.IndexOf('(');
            int end = displayZoneName.IndexOf(')');
            int len = end - start -1;
            return displayZoneName.Substring(start + 1, len);
        }
        public void SyncTimeZones()
        {
                var zones = db.UserTimeZones.ToList();
                var change = false;
                var SystemTimeZones = TimeZoneInfo.GetSystemTimeZones();
                foreach (var timezone in SystemTimeZones) {
                    if (!zones.Any(n => n.InfoID == timezone.Id)) {
                        //not in the database.
                        
                        var timeZoneDb = new UserTimeZone() {
                            InfoID = timezone.Id,
                            Display = timezone.DisplayName,
                            UTCName = GetUtcName(timezone.DisplayName)
                        };
                        change = true;
                        db.UserTimeZones.Add(timeZoneDb);
                    }
                }
                if (change)
                    db.SaveChanges();
                //dbTimeZones = db.UserTimeZones.ToList();
        }

        public int GetUserTimeZoneID(string UTCName)
        {
            var userZone = db.UserTimeZones.FirstOrDefault(n => n.UTCName == UTCName);
            if (userZone != null)
            {
                return userZone.UserTimeZoneID;
            }
            else
            {
                return -1;
            }
        }

        public void InsertAllTimeZones(string locationOfJson)
        {
            SyncTimeZones();

            jsonCountry = ReadJson(locationOfJson);
            var jArray = JArray.Parse(jsonCountry);
            allCountries = jArray.ToList();
            int len = allCountries.Count;



            for (int index = 0; index < len; index++)
            {

                var alpha2Code = GetSafeString(index, "alpha2Code");
                var countryID = db.Countries.FirstOrDefault(n => n.Alpha2Code == alpha2Code).CountryID;

                string property = "timezones";
                if (allCountries[index][property].HasValues)
                {
                    int len2 = allCountries[index][property].Count();

                    for (int i = 0; i < len2; i++)
                    {
                        var timeZoneRel = new CountryTimezoneRelation();
                        var utc = GetTimeZone(index, i);
                        var utcID = GetUserTimeZoneID(utc);

                        if (utcID != -1)
                        {
                            timeZoneRel.CountryID = countryID;
                            timeZoneRel.UserTimeZoneID = utcID;
                            db.CountryTimezoneRelations.Add(timeZoneRel);

                        }
                    }
                }
        
            }
            int iError = db.SaveChanges(null, "TimeZoneSave");
            if (iError == -1)
            {
                Console.WriteLine("Error. ");
            }

            Console.WriteLine("Done Saving Time Zone Database adding.");
        }


        /// <summary>
        /// Depends on language insertion
        /// </summary>
        /// <param name="locationOfJson"></param>
        public void InsertAllCountries(string locationOfJson) {
            jsonCountry = ReadJson(locationOfJson);
            var jArray = JArray.Parse(jsonCountry);
            allCountries = jArray.ToList();

            int len = allCountries.Count;
            for (int i = 0; i < len; i++) {

                var dbCountry = new Country();
                dbCountry.CountryName = GetSafeString(i, "name");
                dbCountry.Capital = GetSafeString(i, "capital");
                dbCountry.Alpha2Code = GetSafeString(i, "alpha2Code");
                dbCountry.Alpha3Code = GetSafeString(i, "alpha3Code");
                dbCountry.NationalityName = GetSafeString(i, "demonym");
                dbCountry.Region = GetSafeString(i, "region");
                dbCountry.SubRegion = GetSafeString(i, "subregion");
                dbCountry.Population = GetSafeStringNumberSafe(i, "population");
                dbCountry.Area = GetSafeStringNumberFloatSafe(i, "area");
                dbCountry.GiniCoefficient = GetSafeStringNumberFloatSafe(i, "gini");
                dbCountry.Relevance = GetSafeStringNumberFloatSafe(i, "relevance");
                dbCountry.LatitudeStartingPoint = GetSafeFloat(GetSafeStringArray(i, "latlng", 0));
                dbCountry.LatitudeEndingPoint = GetSafeFloat(GetSafeStringArray(i, "latlng", 1));
                dbCountry.CallingCode = GetSafeInt(GetSafeStringArray(i, "callingCodes", 0));
                InsertDomain(dbCountry, i);
                InsertCurrencies(dbCountry, i);
                InsertAlternativeNames(dbCountry, i);
                InsertLanguageCountryRelationship(dbCountry, i);
                InsertCountryTranslations(dbCountry, i);
                db.Countries.Add(dbCountry);

            }
            int iError = db.SaveChanges(null, "InsertAllCountries");
            if (iError == -1) {
                Console.WriteLine("Error. ");
            }

            Console.WriteLine("Done Database adding.");
        }
        #endregion

        #region Insert Language and Country at once ( call both methods that's it)

        /// <summary>
        /// Insert all languages and then countries in the database
        /// </summary>
        /// <param name="languageLocation"></param>
        /// <param name="countryLocation"></param>
        public void InsertAllLanguageThenCountries(string languageLocation, string countryLocation) {
            InsertAllLangauges(languageLocation);
            InsertAllCountries(countryLocation);
        }

        #endregion
    }
}
