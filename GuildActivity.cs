using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace TelegramBot
{
    class GuildActivity
    {
        public static AllActivitys activitys = new AllActivitys() { activity = new List<Activity>() };
        private static string error = "false";
        public static AllActivitys GetGuildActivity()
        {

            try
            {
                activitys = new AllActivitys() { activity = new List<Activity>() };
                WebRequest requesta = WebRequest.Create("https://eu.api.blizzard.com/data/wow/guild/howling-fjord/сердце-греха/activity?namespace=profile-eu&locale=ru_RU&access_token=" + Program.tokenWow);
                WebResponse responcea = requesta.GetResponse();

                using (Stream stream = responcea.GetResponseStream())

                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line = "";
                        while ((line = reader.ReadLine()) != null)
                        {



                            ActivityAll activity = JsonConvert.DeserializeObject<ActivityAll>(line);


                            if (activity.activities != null)
                            {

                                for (int i = 0; i < activity.activities.Count; i++)
                                {
                                    TimeSpan ts = DateTime.Now - Functions.FromUnixTimeStampToDateTime(activity.activities[i].timestamp);
                                    if ((int)ts.TotalMinutes < 5)
                                    {
                                        if (activity.activities[i].activity.type == "CHARACTER_ACHIEVEMENT")
                                        {
                                            activitys.activity.Add(new Activity() { Name = "<b>Персонаж</b>: " + activity.activities[i].character_achievement.character.name.ToString(), Mode = "<b>Получил достижение</b>: " + activity.activities[i].character_achievement.achievement.name.ToString(), Time = Functions.relative_time(Functions.FromUnixTimeStampToDateTime(activity.activities[i].timestamp)) });


                                        }
                                        else if (activity.activities[i].activity.type == "ENCOUNTER")
                                        {

                                            activitys.activity.Add(new Activity() { Name = "<b>Гильдия победила</b>: " + activity.activities[i].encounter_completed.encounter.name.ToString(), Mode = "<b>Режим</b>: " + activity.activities[i].encounter_completed.mode.name.ToString(), Time = Functions.relative_time(Functions.FromUnixTimeStampToDateTime(activity.activities[i].timestamp)) });

                                        }
                                    }


                                }

                            }
                            WriteActivityInFile();

                        }
                    }
                }
                responcea.Close();
                error = "false";
                return activitys;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    error = "true";
                    Console.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                    Console.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                    Console.WriteLine("GetGuildActicity Error: " + e.Message);
                    return activitys;
                }
            }
            catch (Exception e)
            {
                error = "true";
                Console.WriteLine("GetGuildActicity Error: " + e.Message);

            }
            return activitys;
        }

        private static async void WriteActivityInFile()
        {
            string writePathJSON = @"F:\TelegramWowBot\Activity.json";






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
                    await System.Text.Json.JsonSerializer.SerializeAsync(fs, activitys, options);



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

    #region Classes Activity
    class AllActivitys
    {
        public List<Activity> activity { get; set; }
    }
    class Activity
    {

        public string Name { get; set; }
        public string Mode { get; set; }
        public string Time { get; set; }
    }

    public class ActivityAllSelf
    {
        public string href { get; set; }
    }

    public class ActivityAllLinks
    {
        public ActivityAllSelf self { get; set; }
    }

    public class ActivityAllKey
    {
        public string href { get; set; }
    }

    public class ActivityAllRealm
    {
        public ActivityAllKey key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string slug { get; set; }
    }

    public class ActivityAllFaction
    {
        public string type { get; set; }
        public string name { get; set; }
    }

    public class ActivityAllGuild
    {
        public ActivityAllKey key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public ActivityAllRealm realm { get; set; }
        public ActivityAllFaction faction { get; set; }
    }

    public class ActivityAllEncounter
    {
        public ActivityAllKey_encou key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }
    public class ActivityAllKey_encou
    {
        public string href { get; set; }
    }
    public class ActivityAllMode
    {
        public string type { get; set; }
        public string name { get; set; }
    }

    public class ActivityAllEncounterCompleted
    {
        public ActivityAllEncounter encounter { get; set; }
        public ActivityAllMode mode { get; set; }
    }

    public class ActivityAllActivity2
    {
        public string type { get; set; }
    }

    public class ActivityAllKey_charakter
    {
        public string href { get; set; }
    }
    public class ActivityAllCharacter
    {
        public ActivityAllKey_charakter key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public ActivityAllRealm_char realm { get; set; }
    }
    public class ActivityAllRealm_char
    {
        public ActivityAllKey_realm_achiv key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string slug { get; set; }
    }
    public class ActivityAllKey_realm_achiv
    {
        public string href { get; set; }
    }
    public class ActivityAllKey_achiv
    {
        public string href { get; set; }
    }
    public class ActivityAllAchievement
    {
        public ActivityAllKey_achiv key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }

    public class ActivityAllCharacterAchievement
    {
        public ActivityAllCharacter character { get; set; }
        public ActivityAllAchievement achievement { get; set; }
    }

    public class ActivityAllActivity
    {
        public ActivityAllEncounterCompleted encounter_completed { get; set; }
        public ActivityAllActivity2 activity { get; set; }
        public string timestamp { get; set; }
        public ActivityAllCharacterAchievement character_achievement { get; set; }
    }

    public class ActivityAll
    {
        public ActivityAllLinks _links { get; set; }
        public ActivityAllGuild guild { get; set; }
        public List<ActivityAllActivity> activities { get; set; }
    }
    #endregion
}
