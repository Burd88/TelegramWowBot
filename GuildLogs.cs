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
        public static string[] GetLogsInfo()
        {



            string[] charfull = new string[4];
            Update_warcraftlogs_data();
            charfull[0] = logsall[0].Dungeon;
            charfull[1] = logsall[0].Date_start;
            charfull[2] = logsall[0].Link;
            charfull[3] = error;
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
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line = reader.ReadToEnd();


                        int index = 1;
                        line = "{ \"logs\": " + line + "}";

                        //  List<Logs> logs_list = new List<Logs>();
                        Warcraftlogs logs = JsonConvert.DeserializeObject<Warcraftlogs>(line);


                        foreach (Logs_all log in logs.logs)
                        {

                            logsall.Add(new Logs() { ID = index, Dungeon = log.title.ToString(), Date_start = Functions.FromUnixTimeStampToDateTime(log.start.ToString()).ToString(), Downloader = log.owner.ToString(), Link = "https://ru.warcraftlogs.com/reports/" + log.id.ToString() });


                        }






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
        public int ID { get; set; }
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
}
