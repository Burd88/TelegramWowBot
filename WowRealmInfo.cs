using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace TelegramBot
{
    class WowRealmInfo
    {
        public static string[] GetRealmInfo()
        {
            RealmUpdateFunction();
            string[] charfull = new string[3];
            charfull[0] = realmname;
            charfull[1] = realmstatus;
            charfull[2] = error;


            return charfull;
        }
        private static string error = "false";
        private static string realmstatus = "null";
        private static string realmname = "null";
        public static string realmstatustype = "";

        public static string GetRealmInfoForTimer()
        {
            RealmUpdateFunction();
            Task<string> Tstr = ReadRealmStatusTypeText();
            string str = Tstr.Result;
            if (str == "Up")
            {
                WriteRealmStatusTypeInFile(realmstatustype);

                return $"<b>Тех. Обслуживание закончилось!</b>\nИгровой мир: <b>{realmname}</b> работает!\u2705";

            }
            else if (str == "Down")
            {
                WriteRealmStatusTypeInFile(realmstatustype);
                return $"<b>Тех. Обслуживание началось!</b>\nИгровой мир: <b>{realmname}</b> не работает!\u274c";

            }
            else if (str == "No work")
            {
                return null;
            }
            else if (str == "Work")
            {
                return null;
            }

            return null;
        }
        static void RealmUpdateFunction()
        {
            try
            {

                WebRequest request = WebRequest.Create("https://eu.api.blizzard.com/data/wow/connected-realm/1615?namespace=dynamic-eu&locale=ru_RU&access_token=" + Program.tokenWow);
                WebResponse responce = request.GetResponse();

                using (System.IO.Stream stream = responce.GetResponseStream())

                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line = "";
                        while ((line = reader.ReadLine()) != null)
                        {


                            RealmInfo realm = JsonConvert.DeserializeObject<RealmInfo>(line);
                            if (realm.status.type == "UP")
                            {
                                realmstatustype = realm.status.type;
                                realmstatus = "\u2705" + realm.status.name;
                            }
                            else
                            {
                                realmstatus = "\u274c" + realm.status.name;
                                realmstatustype = realm.status.type;
                            }

                            foreach (RealmInfoRealm realms in realm.realms)
                            {
                                realmname = realms.name;
                            }



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
                    Console.WriteLine("GetRealmInfo Error: " + e.Message);
                }
            }
            catch (Exception e)
            {
                error = "true";
                Console.WriteLine("GetRealmInfo Error: " + e.Message);
            }


        }

        static async Task<string> ReadRealmStatusTypeText()
        {
            string writePathtext = @"F:\TelegramWowBot\RealmStatusType.txt";

            try
            {

                using (StreamReader sr = new StreamReader(writePathtext))
                {
                    string statusbefore = await sr.ReadToEndAsync();
                    // Console.WriteLine(statusbefore);
                    //  Console.WriteLine(realmstatustype);
                    Thread.Sleep(2000);
                    if (statusbefore == realmstatustype)
                    {
                        if (realmstatustype == "UP")
                        {
                            // Console.WriteLine($"Work this realm : {realmname}");
                            return "Work";
                        }
                        else
                        {
                            // Console.WriteLine($"No work this realm : {realmname}");
                            return "No work";
                        }

                    }
                    else
                    {
                        if (realmstatustype == "UP")
                        {
                            Console.WriteLine($"Up this realm : {realmname}");
                            return "Up";
                        }
                        else
                        {
                            Console.WriteLine($"Down work this realm : {realmname}");
                            return "Down";
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "Error";
            }

        }
        private static async void WriteRealmStatusTypeInFile(string text)
        {
            string writePathtext = @"F:\TelegramWowBot\RealmStatusType.txt";





            try
            {
                using (StreamWriter sw = new StreamWriter(writePathtext, false, System.Text.Encoding.Default))
                {
                    await sw.WriteAsync(text);
                }



            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
    #region RealmInfo Classes
    public class RealmInfoSelf
    {
        public string href { get; set; }
    }

    public class RealmInfoLinks
    {
        public RealmInfoSelf self { get; set; }
    }

    public class RealmInfoStatus
    {
        public string type { get; set; }
        public string name { get; set; }
    }

    public class RealmInfoPopulation
    {
        public string type { get; set; }
        public string name { get; set; }
    }

    public class RealmInfoKey
    {
        public string href { get; set; }
    }

    public class RealmInfoRegion
    {
        public RealmInfoKey key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }

    public class RealmInfoConnectedRealm
    {
        public string href { get; set; }
    }

    public class RealmInfoType
    {
        public string type { get; set; }
        public string name { get; set; }
    }

    public class RealmInfoRealm
    {
        public int id { get; set; }
        public RealmInfoRegion region { get; set; }
        public RealmInfoConnectedRealm connected_realm { get; set; }
        public string name { get; set; }
        public string category { get; set; }
        public string locale { get; set; }
        public string timezone { get; set; }
        public RealmInfoType type { get; set; }
        public bool is_tournament { get; set; }
        public string slug { get; set; }
    }

    public class RealmInfoMythicLeaderboards
    {
        public string href { get; set; }
    }

    public class RealmInfoAuctions
    {
        public string href { get; set; }
    }

    public class RealmInfo
    {
        public RealmInfoLinks _links { get; set; }
        public int id { get; set; }
        public bool has_queue { get; set; }
        public RealmInfoStatus status { get; set; }
        public RealmInfoPopulation population { get; set; }
        public List<RealmInfoRealm> realms { get; set; }
        public RealmInfoMythicLeaderboards mythic_leaderboards { get; set; }
        public RealmInfoAuctions auctions { get; set; }
    }
    #endregion
}
