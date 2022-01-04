using System;
using System.Net.Http;
using System.Web.Http;

using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace DemoAPI.Controllers
{
    /// <summary>
    /// Api about country holidays
    /// </summary>
    public class HolidaysController : ApiController
    {
        readonly HttpClient client = new HttpClient();

        //string connectionString = @"Server=tcp:holidaydb.database.windows.net,1433;Initial Catalog=holidayDB;Persist Security Info=False;User ID=sanddrifter;Password=Pranksta1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringName"].ConnectionString;
        private async Task<dynamic> GetJsonOnNet(string url)
        {
            string response = await client.GetStringAsync(url);
            dynamic json = JsonConvert.DeserializeObject(response);
            return json;
        }
    
        /// <summary>
        /// Returns JSON with supported countries and countrycodes
        /// </summary>
        /// <returns></returns>
        [Route("api/Holidays/GetCountries/")]
        [HttpGet]
        public  async Task<dynamic> GetCountries()
        {
            SqlConnection cnn;
            cnn = new SqlConnection(connectionString);
            cnn.Open();
            SqlCommand command = new SqlCommand("SELECT * FROM Countries;", cnn);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                dynamic countryList = new JObject();
                countryList.countries = new JArray();

                //if there is data in table, get countries from DB
                if (reader.Read() == true)
                {
                    while (reader.Read())
                    {
                        countryList.countries.Add(new JArray(reader.GetValue(0).ToString(), reader.GetValue(1).ToString()));
                    }
                }//if table is empty insert data into table
                else {

                    HolidaysController holidaysController = new HolidaysController();
                    dynamic jsonObj = await holidaysController
                        .GetJsonOnNet("https://kayaposoft.com/enrico/json/v2.0/?action=getSupportedCountries");

                    SqlConnection cnn2;
                    cnn2 = new SqlConnection(connectionString);
                    cnn2.Open();
                    SqlCommand command2 = new SqlCommand("SELECT * FROM Countries;", cnn2);
                    foreach (var country in jsonObj)
                    {
                        countryList.countries.Add(new JArray((string)country["fullName"], (string)country["countryCode"]));
                        command2.CommandText = $"INSERT INTO [dbo].[Countries] (Country, CountryCode) VALUES('{(string)country["fullName"]}', '{(string)country["countryCode"]}');";
                        
                        int result = command2.ExecuteNonQuery();

                        // Check Error
                        if (result < 0) 
                        {
                            Console.WriteLine("Error inserting data into Database!"); 
                        }
                        cnn2.Close();
                    }
                    
                }
                cnn.Close();
                //don't need to serialize since added json formatter in webapiConfig
                return countryList;
            }
        }


        /// <summary>
        /// Returns every month's holiday of a specific country and year
        /// </summary>
        /// <param name="countryCode">usually a 3 letter code of a country</param>
        /// <param name="year">year of country's holidays</param>
        /// <returns></returns>
        [Route("api/Holidays/EveryMonthHolidays/{countryCode}/{year:int}")]
        [HttpGet]
        public async Task<dynamic> EveryMonthHolidays(string countryCode, int year)
        {
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

            SqlConnection cnn;
            cnn = new SqlConnection(connectionString);
            cnn.Open();
            SqlCommand command = new SqlCommand($"SELECT * FROM [dbo].[MonthHolidays] WHERE Country='{countryCode}';", cnn);

            HolidaysController holidaysController = new HolidaysController();
            dynamic jsonObj;

            using (SqlDataReader reader = command.ExecuteReader())
            {
                System.Diagnostics.Debug.WriteLine("inside SqlDataReader");
                dynamic countryList = new JObject();
                countryList.countries = new JArray();

                //if there is data in table, get countries from DB
                if (reader.Read() == true)
                {
                    do
                    {
                        string monthNumStr = reader.GetValue(2).ToString();
                        int monthNum = Convert.ToInt32(monthNumStr);
                        string holidayName = reader.GetValue(3).ToString();

                        switch (monthNum)
                        {
                            case 1:
                                monthHolidays.January.Add(holidayName);
                                break;
                            case 2:
                                // code block
                                monthHolidays.February.Add(holidayName);
                                break;
                            case 3:
                                // code block
                                monthHolidays.March.Add(holidayName);
                                break;
                            case 4:
                                // code block
                                monthHolidays.April.Add(holidayName);
                                break;
                            case 5:
                                // code block
                                monthHolidays.May.Add(holidayName);
                                break;
                            case 6:
                                // code block
                                monthHolidays.June.Add(holidayName);
                                break;
                            case 7:
                                // code block
                                monthHolidays.July.Add(holidayName);
                                break;
                            case 8:
                                // code block
                                monthHolidays.August.Add(holidayName);
                                break;
                            case 9:
                                // code block
                                monthHolidays.September.Add(holidayName);
                                break;
                            case 10:
                                // code block
                                monthHolidays.October.Add(holidayName);
                                break;
                            case 11:
                                // code block
                                monthHolidays.November.Add(holidayName);
                                break;
                            case 12:
                                // code block
                                monthHolidays.December.Add(holidayName);
                                break;
                            default:
                                // code block
                                break;
                        }


                    } while (reader.Read());
                    return monthHolidays;
                }//if table is empty insert data into table
                else
                {
                    System.Diagnostics.Debug.WriteLine("reader.Read() == true");
                    //HolidaysController holidaysController = new HolidaysController();
                    jsonObj = await holidaysController
               .GetJsonOnNet("https://kayaposoft.com/enrico/json/v2.0/?action=getHolidaysForYear&year=" + year.ToString() + "&country=" + countryCode + "&holidayType=public_holiday");

                    SqlConnection cnn2;
                    cnn2 = new SqlConnection(connectionString);
                    cnn2.Open();
                    
                    foreach (var holiday in jsonObj)
                    {
                        int monthNum = Convert.ToInt32(Convert.ToString(holiday.date["month"]));
                        string holidayName = holiday.name[1]["text"].ToString();
                       string sqlStr = $"INSERT INTO [dbo].[MonthHolidays] (Country, MonthNum, Holiday) VALUES('{countryCode}', '{monthNum}', @param);";
                        SqlCommand command2 = new SqlCommand(sqlStr, cnn2);
                        command2.Parameters.AddWithValue("@param", holidayName);

                        int result = command2.ExecuteNonQuery();

                        // Check Error
                        if (result < 0)
                        {
                            Console.WriteLine("Error inserting data into Database!");
                        }
                        
                    }
                    cnn2.Close();

                }
                cnn.Close();
            }
            
            //jsonObj = await holidaysController
            //   .GetJsonOnNet("https://kayaposoft.com/enrico/json/v2.0/?action=getHolidaysForYear&year=" + year.ToString() + "&country=" + countryCode + "&holidayType=public_holiday");

            foreach (var holiday in jsonObj)
            {
                string monthStr = Convert.ToString(holiday.date["month"]);
                int monthNum = Convert.ToInt32(monthStr);

                dynamic jsonObj_ = holiday.name[1]["text"];

                switch (monthNum)
                {
                    case 1:
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

            return monthHolidays;
        }

        /// <summary>
        /// Returns if the day is a workday, freeday or a holiday
        /// </summary>
        /// <param name="date">day-month-year ex.: 25-12-2021</param>
        /// <param name="countryCode">usually a 3 letter code of a country</param>
        /// <returns></returns>
        [Route("api/Holidays/DayStatus/{date}/{countryCode}")]
        [HttpGet]
        public async Task<dynamic> DayStatus(string date, string countryCode)
        {
            SqlConnection cnn;
            cnn = new SqlConnection(connectionString);
            cnn.Open();

            Regex regex = new Regex(@"(\d{1,2})-(\d{1,2})-(\d{4})");
            Match match = regex.Match(date);
            string sqlDate = "";
            if (match.Success)
                sqlDate = $"{match.Groups[3].Value}-{match.Groups[2].Value}-{match.Groups[1].Value}";
            else {
                return "bad date format";
            }

            SqlCommand command = new SqlCommand(
                $"SELECT * FROM [dbo].[DayStatus] WHERE Country='{countryCode}' AND Date='{sqlDate}';", cnn);
            dynamic jsonObj; //= new JObject(); 
            dynamic dayStatus = new JObject();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                dynamic countryList = new JObject();
                countryList.countries = new JArray();

                //if there is data in table, get countries from DB
                if (reader.Read() == true)
                {
                    dayStatus.dayStatus = reader.GetValue(3).ToString();

                    return dayStatus;
                }//call external api and save data to DB
                else{
                    HolidaysController holidaysController = new HolidaysController();
                    jsonObj = await holidaysController
                      .GetJsonOnNet("https://kayaposoft.com/enrico/json/v2.0?action=isWorkDay&date=" + date + "&country=" + countryCode);

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
                    SqlConnection cnn2;
                    cnn2 = new SqlConnection(connectionString);
                    cnn2.Open();

                    string sqlStr = $"INSERT INTO [dbo].[DayStatus] (Country, Date, Status) VALUES('{countryCode}', '{sqlDate}', '{dayStatus.dayStatus}');";
                    SqlCommand command2 = new SqlCommand(sqlStr, cnn2);
                    int result = command2.ExecuteNonQuery();
                    cnn2.Close();

                    return dayStatus;
                }
            }
            
        }

        /// <summary>
        /// Returns the maximum number of free(free day + holiday) days in a row
        /// </summary>
        /// <param name="year">year of holidays</param>
        /// <param name="countryCode">usually a 3 letter code of a country</param>
        /// <returns></returns>
        [Route("api/Holidays/MaxFreeDaysInARow/{year}/{countryCode}")]
        [HttpGet]
        public async Task<dynamic> MaxFreeDaysInARow(string year, string countryCode)
        {
            dynamic maxFreeDaysInARow = new JObject();

            SqlConnection cnn;
            cnn = new SqlConnection(connectionString);
            cnn.Open();

            SqlCommand command = new SqlCommand($"SELECT Streak FROM [dbo].[DaysInARow] WHERE Country='{countryCode}' AND YearOfHolidays='{year}';", cnn);


            using (SqlDataReader reader = command.ExecuteReader())
            {
                dynamic countryList = new JObject();
                countryList.countries = new JArray();

                //if there is data in table, get free Day Streak from DB
                if (reader.Read() == true)
                {
                    maxFreeDaysInARow.longestFreeDayStreak  = reader.GetValue(0);
                    return maxFreeDaysInARow;
                }//call external api and save data to DB
                else
                {
                    HolidaysController holidaysController = new HolidaysController();
                    dynamic holidays = await holidaysController
                         .GetJsonOnNet("https://kayaposoft.com/enrico/json/v2.0/?action=getHolidaysForYear&year=" + year + "&country=" + countryCode + "&holidayType=public_holiday");

                    int holidayStreak;
                    maxFreeDaysInARow.longestFreeDayStreak = 0;

                    //check free days around holidays
                    foreach (var holiday in holidays)
                    {
                        int day = holiday["date"]["day"];
                        int month = holiday["date"]["month"];
                        int year_ = holiday["date"]["year"];

                        string fullDate = $"{day}-{month}-{year_}";

                        holidayStreak = -1;

                        dynamic jsonobj = await DayStatus(fullDate, countryCode);
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
                    SqlConnection cnn2;
                    cnn2 = new SqlConnection(connectionString);
                    cnn2.Open();

                    string sqlStr = $"INSERT INTO [dbo].[DaysInARow] ( Country, YearOfHolidays, Streak) VALUES('{countryCode}', '{year}', '{ maxFreeDaysInARow.longestFreeDayStreak}');";
                    SqlCommand command2 = new SqlCommand(sqlStr, cnn2);
                    command2.ExecuteNonQuery();
                    cnn2.Close();

                    return maxFreeDaysInARow;
                }
            }
        }

    }
}
