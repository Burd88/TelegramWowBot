using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace TelegramBot
{
    class GuildLogs
    {
        private static List<Logs> logsall;
        private static string error = "false";
        private static string _dungeon = "<b>Рейд</b>: ";
        private static string _date = "<b>Начало рейда</b>: ";
        private static string _link = "";
        private static string _killsBoss = "<b>Боссов убито</b>: ";
        private static string _wipeBoss = "<b>Вайпов</b>: ";
        private static string[] charfull = new string[6];
        private static string _raid_time = "0";
        public static string[] GetLogsInfo()
        {
            logsall = new List<Logs>();
            error = "false";
            charfull = new string[7];
            _killsBoss = "<b>Боссов убито</b>: ";
            _dungeon = "<b>Рейд</b>: ";
            _date = "<b>Начало рейда</b>: ";
            _wipeBoss = "<b>Вайпов</b>: ";
            _link = "";
            _raid_time = "0";
            Update_warcraftlogs_data();
            charfull[0] = _dungeon;
            charfull[1] = _killsBoss;
            charfull[2] = _wipeBoss;
            charfull[3] = _date;
            charfull[4] = _link;
            charfull[5] = error;
            charfull[6] = _raid_time;
            return charfull;
        }
        private static void Update_warcraftlogs_data()
        {
            logsall = new List<Logs>();
            try
            {

                WebRequest request = WebRequest.Create("https://www.warcraftlogs.com/v1/reports/guild/сердце греха/howling-fjord/eu?api_key=c2c9093c70e642ac6ec003d4b0904c33");
                WebResponse responce = request.GetResponse();

                using (Stream stream = responce.GetResponseStream())

                {
                    using (StreamReader reader = new (stream))
                    {
                        string line = reader.ReadToEnd();



                        line = "{ \"logs\": " + line + "}";

                        //  List<Logs> logs_list = new List<Logs>();
                        Warcraftlogs logs = JsonConvert.DeserializeObject<Warcraftlogs>(line);


                        foreach (Logs_all log in logs.logs)
                        {

                            logsall.Add(new Logs() { ID = log.id, Dungeon = log.title.ToString(), Date_start = Functions.FromUnixTimeStampToDateTime(log.start.ToString()).ToString(), Downloader = log.owner.ToString(), Link = "https://ru.warcraftlogs.com/reports/" + log.id.ToString() });


                        }

                        GetLogInfo(logsall[0].ID, logsall[0].Dungeon, logsall[0].Date_start, logsall[0].Link);




                    }
                }
                responce.Close();
                error = "false";
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    error = "true";
                    Console.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                    Console.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                    Console.WriteLine("GetGuildLogs Error: " + e.Message);
                }
            }
            catch (Exception e)
            {
                error = "true";
                Console.WriteLine("GetGuildLogs Error: " + e.Message);
            }
        }

        private static void GetLogInfo(string id, string dungeon, string date, string link)
        {
            logsall = new List<Logs>();
            try
            {

                WebRequest request = WebRequest.Create("https://www.warcraftlogs.com/v1/report/fights/" + id + "?translate=false&api_key=c2c9093c70e642ac6ec003d4b0904c33");
                WebResponse responce = request.GetResponse();

                using (Stream stream = responce.GetResponseStream())

                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line = reader.ReadToEnd();


                        // int index = 1;
                        // line = "{ \"logs\": " + line + "}";

                        //  List<Logs> logs_list = new List<Logs>();
                        LogInfo log = JsonConvert.DeserializeObject<LogInfo>(line);

                        int kills = 0;
                        int wipe = 0;
                        foreach (Fight fight in log.fights)
                        {

                            if (fight.kill == true)
                            {
                                kills++;
                            }
                            else
                            {
                                wipe++;
                            }

                        }
                        TimeSpan ts = Functions.FromUnixTimeStampToDateTime(log.end.ToString()) - Functions.FromUnixTimeStampToDateTime(log.start.ToString());
                        _raid_time = $"<b>Продолжительность</b>: {ts.Hours} ч {ts.Minutes} м";
                        _date = _date + date;
                        _dungeon = _dungeon + dungeon;
                        _killsBoss = _killsBoss + kills.ToString();
                        _link = link;
                        _wipeBoss = _wipeBoss + wipe.ToString();



                    }
                }
                responce.Close();
                error = "false";
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    error = "true";
                    Console.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                    Console.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                    Console.WriteLine("GetGuildLogs Error: " + e.Message);
                }
            }
            catch (Exception e)
            {
                error = "true";
                Console.WriteLine("GetGuildLogs Error: " + e.Message);
            }
        }
    }

    public class Logs
    {

        public string Dungeon { get; set; }

        public string Date_start { get; set; }


        public string Link { get; set; }
        public string Downloader { get; set; }
        public string ID { get; set; }
    }
    public class Warcraftlogs
    {
        public List<Logs_all> logs { get; set; }

    }
    public class Logs_all
    {

        public string id { get; set; }
        public string title { get; set; }
        public string owner { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public int zone { get; set; }
    }

    public class Logsw
    {


        public string Dungeon { get; set; }

        public string Date_start { get; set; }

        public string Date_end { get; set; }
        public string Link { get; set; }
        public string Downloader { get; set; }
        public int ID { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Fight
    {
        public int id { get; set; }
        public int boss { get; set; }
        public int start_time { get; set; }
        public int end_time { get; set; }
        public string name { get; set; }
        public int zoneID { get; set; }
        public string zoneName { get; set; }
        public int zoneDifficulty { get; set; }
        public int size { get; set; }
        public int difficulty { get; set; }
        public bool kill { get; set; }
        public int partial { get; set; }
        public int bossPercentage { get; set; }
        public int fightPercentage { get; set; }
        public int lastPhaseForPercentageDisplay { get; set; }
        public List<int> maps { get; set; }
        public int instances { get; set; }
        public int groups { get; set; }
    }

    public class Friendly
    {
        public string name { get; set; }
        public int id { get; set; }
        public int guid { get; set; }
        public string type { get; set; }
        public string server { get; set; }
        public string icon { get; set; }
        public List<Fight> fights { get; set; }
    }

    public class Enemy
    {
        public string name { get; set; }
        public int id { get; set; }
        public int guid { get; set; }
        public string type { get; set; }
        public string icon { get; set; }
        public List<Fight> fights { get; set; }
    }

    public class FriendlyPet
    {
        public string name { get; set; }
        public int id { get; set; }
        public int guid { get; set; }
        public string type { get; set; }
        public string icon { get; set; }
        public int petOwner { get; set; }
        public List<Fight> fights { get; set; }
    }

    public class Phase
    {
        public int boss { get; set; }
        public List<string> phases { get; set; }
    }

    public class ExportedCharacter
    {
        public int id { get; set; }
        public string name { get; set; }
        public string server { get; set; }
        public string region { get; set; }
    }

    public class LogInfo
    {
        public List<Fight> fights { get; set; }
        public string lang { get; set; }
        public List<Friendly> friendlies { get; set; }
        public List<Enemy> enemies { get; set; }
        public List<FriendlyPet> friendlyPets { get; set; }
        public List<object> enemyPets { get; set; }
        public List<Phase> phases { get; set; }
        public int logVersion { get; set; }
        public int gameVersion { get; set; }
        public string title { get; set; }
        public string owner { get; set; }
        public long start { get; set; }
        public long end { get; set; }
        public int zone { get; set; }
        public List<ExportedCharacter> exportedCharacters { get; set; }
    }


}
