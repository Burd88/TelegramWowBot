using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace TelegramBot
{
    class GuildAchievements
    {

        public static AllAchievements achievements = new AllAchievements() { Achievements = new List<Achievement>() };
        private static string error = "false";
        public static AllAchievements GetGuildAchievements()
        {

            try
            {
                achievements = new () { Achievements = new List<Achievement>() };
                WebRequest requesta = WebRequest.Create("https://eu.api.blizzard.com/data/wow/guild/howling-fjord/%D1%81%D0%B5%D1%80%D0%B4%D1%86%D0%B5-%D0%B3%D1%80%D0%B5%D1%85%D0%B0/achievements?namespace=profile-eu&locale=ru_RU&access_token=" + Program.tokenWow);
                WebResponse responcea = requesta.GetResponse();

                using (Stream stream = responcea.GetResponseStream())

                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line = "";
                        while ((line = reader.ReadLine()) != null)
                        {



                            GuildAchievement achievementsAll = JsonConvert.DeserializeObject<GuildAchievement>(line);


                            if (achievementsAll.recent_events != null)
                            {

                                for (int i = 0; i < achievementsAll.recent_events.Count; i++)
                                {
                                    TimeSpan ts = DateTime.Now - Functions.FromUnixTimeStampToDateTime(achievementsAll.recent_events[i].timestamp);
                                    if ((int)ts.TotalMinutes < 5)
                                    {
                                        GetGuildAchievementsRU(achievementsAll.recent_events[i].achievement.id.ToString());
                                    }


                                }

                            }
                            WriteAchievementsInFile();

                        }
                    }
                }
                responcea.Close();
                error = "false";
                return achievements;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    error = "true";
                    Console.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                    Console.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                    Console.WriteLine("GetGuildAchievements Error: " + e.Message);
                    return achievements;
                }
            }
            catch (Exception e)
            {
                error = "true";
                Console.WriteLine("GetGuildAchievements Error: " + e.Message);
                return achievements;
            }
            return achievements;
        }
        public static void GetGuildAchievementsRU(string id)
        {

            try
            {
                
                WebRequest requesta = WebRequest.Create("https://eu.api.blizzard.com/data/wow/achievement/"+ id + "?namespace=static-9.1.5_40764-eu&locale=ru_RU&access_token=" + Program.tokenWow);
                WebResponse responcea = requesta.GetResponse();

                using (Stream stream = responcea.GetResponseStream())

                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line = "";
                        while ((line = reader.ReadLine()) != null)
                        {



                            GuildAchievementMedia achievement = JsonConvert.DeserializeObject<GuildAchievementMedia>(line);

                            achievements.Achievements.Add(new Achievement { Category = achievement.category.name, Name = achievement.name });






                        }
                    }
                }
                responcea.Close();
                error = "false";

            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    error = "true";
                    Console.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                    Console.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                    Console.WriteLine("GetGuildAchievements Error: " + e.Message);

                }
            }
            catch (Exception e)
            {
                error = "true";
                Console.WriteLine("GetGuildAchievements Error: " + e.Message);

            }

        }

        private static async void WriteAchievementsInFile()
        {
            string writePathJSON = @"F:\TelegramWowBot\Achievements.json";






            try
            {



                using (FileStream fs = new(writePathJSON, FileMode.Create))
                {

                    //  var result = users.members.SingleOrDefault(a => a.Id == id);

                    //  if (result == null)
                    //  {
                    //     users.members.Add(new User() { Name = name, Id = id });
                    var options = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                        WriteIndented = true
                    };
                    await System.Text.Json.JsonSerializer.SerializeAsync(fs, achievements, options);



                    // }
                    //  else
                    //  {
                    //      Console.WriteLine("Есть такой уже");
                    //   }




                }



            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class AllAchievements
    {
        public List<Achievement> Achievements { get; set; }
    }

    public class Achievement
    {
        public string Category { get; set; }
        public string Name { get; set; }
    }

    public class SelfAchiev
    {
        public string href { get; set; }
    }

    public class LinksAchiev
    {
        public SelfAchiev self { get; set; }
    }

    public class KeyAchiev
    {
        public string href { get; set; }
    }

    public class RealmAchiev
    {
        public KeyAchiev key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string slug { get; set; }
    }

    public class FactionAchiev
    {
        public string type { get; set; }
        public string name { get; set; }
    }

    public class GuildAchiev
    {
        public KeyAchiev key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public RealmAchiev realm { get; set; }
        public FactionAchiev faction { get; set; }
    }

    public class Achievement2Achiev
    {
        public KeyAchiev key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }

    public class ChildCriteriaAchiev
    {
        public int id { get; set; }
        public object amount { get; set; }
        public bool is_completed { get; set; }
    }

    public class CriteriaAchiev
    {
        public int id { get; set; }
        public bool is_completed { get; set; }
        public List<ChildCriteriaAchiev> child_criteria { get; set; }
        public int? amount { get; set; }
    }

    public class AchievementAchiev
    {
        public int id { get; set; }
        public Achievement2Achiev achievement { get; set; }
        public CriteriaAchiev criteria { get; set; }
        public object completed_timestamp { get; set; }
    }

    public class CategoryAchiev
    {
        public KeyAchiev key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }

    public class CategoryProgressAchiev
    {
        public CategoryAchiev category { get; set; }
        public int quantity { get; set; }
        public int points { get; set; }
    }

    public class RecentEventAchiev
    {
        public AchievementAchiev achievement { get; set; }
        public string timestamp { get; set; }
    }

    public class GuildAchievement
    {
        public LinksAchiev _links { get; set; }
        public GuildAchiev guild { get; set; }
        public int total_quantity { get; set; }
        public int total_points { get; set; }
        public List<AchievementAchiev> achievements { get; set; }
        public List<CategoryProgressAchiev> category_progress { get; set; }
        public List<RecentEventAchiev> recent_events { get; set; }
    }
    public class SelfAchievMedia
    {
        public string href { get; set; }
    }

    public class LinksAchievMedia
    {
        public SelfAchievMedia self { get; set; }
    }

    public class KeyAchievMedia
    {
        public string href { get; set; }
    }

    public class CategoryAchievMedia
    {
        public KeyAchievMedia key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }

    public class OperatorAchievMedia
    {
        public string type { get; set; }
        public string name { get; set; }
    }

    public class ChildCriteriaAchievMedia
    {
        public int id { get; set; }
        public string description { get; set; }
        public int amount { get; set; }
    }

    public class CriteriaAchievMedia
    {
        public int id { get; set; }
        public string description { get; set; }
        public int amount { get; set; }
        public OperatorAchievMedia @operator { get; set; }
        public List<ChildCriteriaAchievMedia> child_criteria { get; set; }
    }

    public class MediaAchievMedia
    {
        public KeyAchievMedia key { get; set; }
        public int id { get; set; }
    }

    public class GuildAchievementMedia
    {
        public LinksAchievMedia _links { get; set; }
        public int id { get; set; }
        public CategoryAchievMedia category { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int points { get; set; }
        public bool is_account_wide { get; set; }
        public CriteriaAchievMedia criteria { get; set; }
        public MediaAchievMedia media { get; set; }
        public int display_order { get; set; }
    }
}
