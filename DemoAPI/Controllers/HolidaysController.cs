using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;

using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Text.RegularExpressions;

namespace DemoAPI.Controllers
{
    public class HolidaysController : ApiController
    {

        HttpClient client = new HttpClient();

        private async Task<dynamic> GetJsonOnNet(string url)
        {
            string response = await client.GetStringAsync(url);
            //System.Diagnostics.Debug.WriteLine(response);
            dynamic json = JsonConvert.DeserializeObject(response);
            return json;
        }
    
        [Route("api/Holidays/GetCountries/")]
        [HttpGet]
        public  async Task<dynamic> GetCountries()
        {
            //List<string> output = new List<string>();
            dynamic countryList = new JObject();
            countryList.countries = new JArray();
            System.Diagnostics.Debug.WriteLine("Testing api route api/Holidays/GetCountries/");

            HolidaysController holidaysController = new HolidaysController();
            dynamic jsonObj = await holidaysController
                .GetJsonOnNet("https://kayaposoft.com/enrico/json/v2.0/?action=getSupportedCountries");

            foreach (var country in jsonObj) {
               // countryList.countries.Add((string)country["fullName"]);
               countryList.countries.Add(new JArray( (string)country["fullName"], (string)country["countryCode"])  );
            }

            //don't need to serialize since added json formatter in webapiConfig
            //string json = JsonConvert.SerializeObject(countryList, Formatting.Indented);
            
            return countryList;
        }

        [Route("api/Holidays/EveryMonthHolidays/{countryCode}/{year:int}")]
        [HttpGet]
        public async Task<dynamic> EveryMonthHolidays(string countryCode, int year)
        {
            //dynamic holidays = new JObject();
            dynamic monthHolidays = new JObject();
            monthHolidays.January = new JArray();
            monthHolidays.February = new JArray();
            monthHolidays.March = new JArray();
            monthHolidays.April = new JArray();
            monthHolidays.May = new JArray();
            monthHolidays.June = new JArray();
            monthHolidays.July = new JArray();
            monthHolidays.August = new JArray();
            monthHolidays.September = new JArray();
            monthHolidays.October = new JArray();
            monthHolidays.November = new JArray();
            monthHolidays.December = new JArray();

            System.Diagnostics.Debug.WriteLine("Testing api route api/Holidays/EveryMonthHolidays/");

            System.Diagnostics.Debug.WriteLine(countryCode);
            System.Diagnostics.Debug.WriteLine(year);

            HolidaysController holidaysController = new HolidaysController();
            dynamic jsonObj = await holidaysController
               .GetJsonOnNet("https://kayaposoft.com/enrico/json/v2.0/?action=getHolidaysForYear&year=" + year.ToString() + "&country=" + countryCode + "&holidayType=public_holiday");

           // dynamic jsonObj = await holidaysController
            //   .GetJsonOnNet("https://kayaposoft.com/enrico/json/v2.0/?action=getHolidaysForYear&year=2022&country=est&holidayType=public_holiday");


            foreach (var holiday in jsonObj)
            {
                //output.Add((string)country["fullName"]);
                //countryList.countries.Add((string)country["fullName"]);
               // System.Diagnostics.Debug.WriteLine("month");
                
                string monthStr = Convert.ToString(holiday.date["month"]);
              //  System.Diagnostics.Debug.WriteLine(monthStr);
                int monthNum = Convert.ToInt32(monthStr);
                //dynamic jsonObj_ = new JObject();
                //jsonObj_.name = holiday.name[1]["text"];
                //jsonObj_.date = holiday.date;

               // dynamic holidayName = holiday.name[1]["text"];
                dynamic jsonObj_ = holiday.name[1]["text"];

                switch (monthNum)
                {
                    case 1:
                        // code block
                      //  System.Diagnostics.Debug.WriteLine(holiday.name[1]["text"].ToString());
                        monthHolidays.January.Add(jsonObj_);
                        break;
                    case 2:
                        // code block
                        monthHolidays.February.Add(jsonObj_);
                        break;
                    case 3:
                        // code block
                        monthHolidays.March.Add(jsonObj_);
                        break;
                    case 4:
                        // code block
                        monthHolidays.April.Add(jsonObj_);
                        break;
                    case 5:
                        // code block
                        monthHolidays.May.Add(jsonObj_);
                        break;
                    case 6:
                        // code block
                        monthHolidays.June.Add(jsonObj_);
                        break;
                    case 7:
                        // code block
                        monthHolidays.July.Add(jsonObj_);
                        break;
                    case 8:
                        // code block
                        monthHolidays.August.Add(jsonObj_);
                        break;
                    case 9:
                        // code block
                        monthHolidays.September.Add(jsonObj_);
                        break;
                    case 10:
                        // code block
                        monthHolidays.October.Add(jsonObj_);
                        break;
                    case 11:
                        // code block
                        monthHolidays.November.Add(jsonObj_);
                        break;
                    case 12:
                        // code block
                        monthHolidays.December.Add(jsonObj_);
                        break;
                    default:
                        // code block
                        break;
                }
            }

            //don't need to serialize since added json formatter in webapiConfig
            //string json = JsonConvert.SerializeObject(countryList, Formatting.Indented);

            return monthHolidays;
        }

        [Route("api/Holidays/DayStatus/{date}/{countryCode}")]
        [HttpGet]
        public async Task<dynamic> DayStatus(string date, string countryCode)
        {
            dynamic dayStatus = new JObject();
            //dayStatus.January = new JArray();

            HolidaysController holidaysController = new HolidaysController();
            dynamic jsonObj = await holidaysController
              .GetJsonOnNet("https://kayaposoft.com/enrico/json/v2.0?action=isWorkDay&date=" + date + "&country="+ countryCode);


            //dynamic jsonObj = await holidaysController
            //  .GetJsonOnNet("https://kayaposoft.com/enrico/json/v2.0?action=isWorkDay&date=30-07-2022&country=hun");
           

            if (jsonObj["isWorkDay"] == null) 
            {
                //return empty json if day is not valid
                return dayStatus;
            }


            if (jsonObj["isWorkDay"] == false)
            {
                //check if holiday
                jsonObj = await holidaysController
                    .GetJsonOnNet("https://kayaposoft.com/enrico/json/v2.0?action=isPublicHoliday&date=" + date + "&country=" + countryCode);
                if (jsonObj["isPublicHoliday"] == true)
                {
                    dayStatus.dayStatus = "holiday";
                }
                else 
                {
                    dayStatus.dayStatus = "free day";
                }
            }
            else 
            {
                dayStatus.dayStatus = "workday";
            }

            return dayStatus;
        }

        [Route("api/Holidays/MaxFreeDaysInARow/{date}/{countryCode}")]
        [HttpGet]
        public async Task<dynamic> MaxFreeDaysInARow(string date, string countryCode)
        {
            dynamic maxFreeDaysInARow = new JObject();

            string expr = @"\d{4}";
            MatchCollection mc = Regex.Matches(date, expr);
            //string year = Convert.ToString(mc[0]);
            int year = Convert.ToInt32(Convert.ToString(mc[0]));
            // System.Diagnostics.Debug.WriteLine(year);

            //get all holidays
            HolidaysController holidaysController = new HolidaysController();
            dynamic holidays = await holidaysController
                 .GetJsonOnNet("https://kayaposoft.com/enrico/json/v2.0/?action=getHolidaysForYear&year=" + year.ToString() + "&country=" + countryCode + "&holidayType=public_holiday");
            //dynamic holidays = await EveryMonthHolidays(countryCode, year);

            int holidayStreak;
            maxFreeDaysInARow.longestFreeDayStreak = 0;

            //check free days around holidays
            foreach (var holiday in holidays)
            {
                int day = holiday["date"]["day"];
                int month = holiday["date"]["month"];
                int year_ = holiday["date"]["year"];

                string fullDate = $"{day}-{month}-{year_}";
                System.Diagnostics.Debug.WriteLine(fullDate);

                holidayStreak = -1;

                //request self api UNFINISHED *******************************************************************************
                // dynamic jsonobj = await holidaysController
                //.GetJsonOnNet("api/holidays/daystatus/" + day + "-" + month + "-" + year + "/" + countryCode);
                
                dynamic jsonobj = await DayStatus(date, countryCode);
                DateTime holidayDate = new DateTime(year_, month, day);
                string dateStatus = jsonobj["dayStatus"];

                while (!dateStatus.Equals("workday"))
                {
                    holidayStreak++;
                    holidayDate = holidayDate.AddDays(1);
                    int year2 = holidayDate.Year;
                    int month2 = holidayDate.Month;
                    int day2 = holidayDate.Day;
                    string date2 = $"{day2}-{month2}-{year2}";
                    jsonobj = await DayStatus(date2, countryCode);
                    dateStatus = jsonobj["dayStatus"];
                    //jsonobj = await holidaysController
                    //    .GetJsonOnNet(url);
                }

                //reset
                holidayDate = new DateTime(year_, month, day);
                dateStatus = "holiday";

                while (!dateStatus.Equals("workday"))
                {
                    holidayStreak++;
                    holidayDate = holidayDate.AddDays(-1);
                    int year2 = holidayDate.Year;
                    int month2 = holidayDate.Month;
                    int day2 = holidayDate.Day;
                    string date2 = $"{day2}-{month2}-{year2}";
                    jsonobj = await DayStatus(date2, countryCode);
                    dateStatus = jsonobj["dayStatus"];
                }

                if (holidayStreak > Convert.ToInt16(maxFreeDaysInARow.longestFreeDayStreak))
                {
                    maxFreeDaysInARow.longestFreeDayStreak = holidayStreak;
                }
            }

            return maxFreeDaysInARow;
        }

            // GET: api/Holidays
            public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Holidays/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Holidays
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Holidays/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Holidays/5
        public void Delete(int id)
        {
        }
    }
}
