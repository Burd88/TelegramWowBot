using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace TelegramBot
{
    class GuildInfo
    {




        private static string guildAchiv = "";
        private static string memberCount = "";
        private static string guildFaction = "";


        private static string guildLeader = "";
        private static string guildTimeCreate = "";
        private static string guildName = "";

        private static string error = "false";
        // private static string CNguildraidprogress;
        private static string SODguildraidprogress = "";
        // private static string CNguildrankname;
        //  private static string CNguildrankworld;
        // private static string CNguildrankregion;
        // private static string CNguildrankrealm;
        private static string SODguildrankname;
        private static string SODguildrankworld;
        private static string SODguildrankregion;
        private static string SODguildrankrealm;
        public static string[] GetGuildInfo()
        {

            Guild_raid_progress();
            GetGuildOtherInfo();
            GetGuildRosterInfo();
            string[] charfull = new string[13];
            charfull[0] = guildName;
            charfull[1] = guildFaction;
            charfull[2] = guildLeader;
            charfull[3] = memberCount;
            charfull[4] = guildAchiv;
            charfull[5] = SODguildraidprogress;
            charfull[6] = guildTimeCreate;
            charfull[7] = error;
            charfull[8] = SODguildrankname;
            charfull[9] = SODguildrankrealm;

            return charfull;
        }
        private static void GetGuildRosterInfo()
        {

            try
            {
                WebRequest request = WebRequest.Create("https://eu.api.blizzard.com/data/wow/guild/howling-fjord/сердце-греха/roster?namespace=profile-eu&locale=ru_RU&access_token=" + Program.tokenWow);
                WebResponse responce = request.GetResponse();

                using (System.IO.Stream stream = responce.GetResponseStream())

                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line = "";
                        while ((line = reader.ReadLine()) != null)
                        {
                            MainGuild guild = JsonConvert.DeserializeObject<MainGuild>(line);
                            foreach (Member_guild character in guild.members)
                            {
                                if (character.rank == 0)
                                {
                                    guildLeader = "<b>Лидер</b>: " + character.character.name;
                                }
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
                    Console.WriteLine("GetGuildInfo Error Roster&Lead: " + e.Message);
                }
            }
            catch (Exception e)
            {
                error = "true";
                Console.WriteLine("GetGuildInfo Error Roster&Lead: " + e.Message);
            }


        }


        private static void GetGuildOtherInfo()
        {
            try
            {
                WebRequest requestchar = WebRequest.Create("https://eu.api.blizzard.com/data/wow/guild/howling-fjord/сердце-греха?namespace=profile-eu&locale=ru_RU&access_token=" + Program.tokenWow);

                WebResponse responcechar = requestchar.GetResponse();

                using (System.IO.Stream stream1 = responcechar.GetResponseStream())

                {

                    using (StreamReader reader1 = new(stream1))
                    {
                        string line = "";

                        while ((line = reader1.ReadLine()) != null)
                        {
                            // Console.WriteLine(line);
                            GuildMain guildmain = JsonConvert.DeserializeObject<GuildMain>(line);
                            guildTimeCreate = "<b>Основана</b>: " + Functions.FromUnixTimeStampToDateTime(guildmain.created_timestamp.ToString()).ToString();
                            guildName = "<b>" + guildmain.name + "</b>";
                            guildAchiv = "<b>Достижения</b>: " + guildmain.achievement_points.ToString();
                            memberCount = "<b>Членов гильдии</b>: " + guildmain.member_count.ToString();
                            guildFaction = "<b>Фракция</b>: " + guildmain.faction.name;

                        }

                    }

                }
                responcechar.Close();
                error = "false";
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    error = "true";
                    Console.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                    Console.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                    Console.WriteLine("GetGuildInfo Error OtherInfo: " + e.Message);
                }
            }
            catch (Exception e)
            {
                error = "true";
                Console.WriteLine("GetGuildInfo Error OtherInfo: " + e.Message);
            }
        }


        private static void Guild_raid_progress()
        {

            try
            {
                WebRequest requestchar = WebRequest.Create("https://raider.io/api/v1/guilds/profile?region=eu&realm=howling-fjord&name=сердце греха&fields=raid_progression%2Craid_rankings");

                WebResponse responcechar = requestchar.GetResponse();

                using (System.IO.Stream stream1 = responcechar.GetResponseStream())

                {
                    using StreamReader reader1 = new StreamReader(stream1);
                    string line = "";
                    while ((line = reader1.ReadLine()) != null)
                    {
                        GuildRaiderIO rio_guild = JsonConvert.DeserializeObject<GuildRaiderIO>(line);
                        /*   CNguildraidprogress = "Рейд прогресс: " + rio_guild.raid_progression.CastleNathria.summary;
                            if (rio_guild.raid_rankings.CastleNathria.mythic.world == 0)
                             {
                                 if (rio_guild.raid_rankings.CastleNathria.heroic.world == 0)
                                 {
                                     if (rio_guild.raid_rankings.CastleNathria.normal.world == 0)
                                     {
                                         CNguildrankname = "Сложность: " + "Обычный";
                                         CNguildrankworld = "Мир: " + "0";
                                         CNguildrankregion = "Регион: " + "0";
                                         CNguildrankrealm = "Сервер: " + "0";

                                     }
                                     else
                                     {
                                         CNguildrankname = "Сложность: " + "Обычный";
                                         CNguildrankworld = "Мир: " + rio_guild.raid_rankings.CastleNathria.normal.world.ToString();
                                         CNguildrankregion = "Регион: " + rio_guild.raid_rankings.CastleNathria.normal.region.ToString();
                                         CNguildrankrealm = "Сервер: " + rio_guild.raid_rankings.CastleNathria.normal.realm.ToString();
                                     }


                                 }
                                 else
                                 {
                                     CNguildrankname = "Сложность: " + "Героический";
                                     CNguildrankworld = "Мир: " + rio_guild.raid_rankings.CastleNathria.heroic.world.ToString();
                                     CNguildrankregion = "Регион: " + rio_guild.raid_rankings.CastleNathria.heroic.region.ToString();
                                     CNguildrankrealm = "Сервер: " + rio_guild.raid_rankings.CastleNathria.heroic.realm.ToString();
                                 }

                             }
                             else
                             {
                                 CNguildrankname = "Сложность: " + "Мифический";
                                 CNguildrankworld = "Мир: " + rio_guild.raid_rankings.CastleNathria.mythic.world.ToString();
                                 CNguildrankregion = "Регион: " + rio_guild.raid_rankings.CastleNathria.mythic.region.ToString();
                                 CNguildrankrealm = "Сервер: " + rio_guild.raid_rankings.CastleNathria.mythic.realm.ToString();
                             }
                             try
                             {
                                 main_info_worker.ReportProgress(50);
                             }
                             catch (Exception ex)
                             {
                                 Console.WriteLine("EXSA" + ex.Message);
                             } */
                        SODguildraidprogress = "<b>Рейд прогресс</b>: " + rio_guild.raid_progression.SanctumOfDomination.summary;
                        if (rio_guild.raid_rankings.SanctumOfDomination.mythic.world == 0)
                        {
                            if (rio_guild.raid_rankings.SanctumOfDomination.heroic.world == 0)
                            {
                                if (rio_guild.raid_rankings.SanctumOfDomination.normal.world == 0)
                                {
                                    SODguildrankname = "<b>Сложность</b>: " + "Обычный";
                                    SODguildrankworld = "Мир: " + "0";
                                    SODguildrankregion = "Регион: " + "0";
                                    SODguildrankrealm = "<b>Сервер</b>: " + "0";

                                }
                                else
                                {
                                    SODguildrankname = "<b>Сложность</b>: " + "Обычный";
                                    SODguildrankworld = "Мир: " + rio_guild.raid_rankings.SanctumOfDomination.normal.world.ToString();
                                    SODguildrankregion = "Регион: " + rio_guild.raid_rankings.SanctumOfDomination.normal.region.ToString();
                                    SODguildrankrealm = "<b>Сервер</b>: " + rio_guild.raid_rankings.SanctumOfDomination.normal.realm.ToString();
                                }


                            }
                            else
                            {
                                SODguildrankname = "<b>Сложность</b>: " + "Героический";
                                SODguildrankworld = "Мир: " + rio_guild.raid_rankings.SanctumOfDomination.heroic.world.ToString();
                                SODguildrankregion = "Регион: " + rio_guild.raid_rankings.SanctumOfDomination.heroic.region.ToString();
                                SODguildrankrealm = "<b>Сервер</b>: " + rio_guild.raid_rankings.SanctumOfDomination.heroic.realm.ToString();
                            }

                        }
                        else
                        {
                            SODguildrankname = "<b>Сложность<b>: " + "Мифический";
                            SODguildrankworld = "Мир: " + rio_guild.raid_rankings.SanctumOfDomination.mythic.world.ToString();
                            SODguildrankregion = "Регион: " + rio_guild.raid_rankings.SanctumOfDomination.mythic.region.ToString();
                            SODguildrankrealm = "<b>Сервер</b>: " + rio_guild.raid_rankings.SanctumOfDomination.mythic.realm.ToString();
                        }

                    }
                }
                responcechar.Close();
                error = "false";
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    error = "true";
                    Console.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                    Console.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                    Console.WriteLine("GetGuildInfo Error RaidProgress: " + e.Message);
                }
            }
            catch (Exception e)
            {
                error = "true";
                Console.WriteLine("GetGuildInfo Error RaidProgress: " + e.Message);

            }
        }
    }


    #region Guild Members Info
    public class Guild
    {

        public int ID { get; set; }
        public string Region { get; set; }
        public string Realm { get; set; }
        public string GuildName { get; set; }
        public string Local { get; set; }
        public string RealmSlug { get; set; }
        public string GuildSlug { get; set; }
        public string LocalSlug { get; set; }

    }


    public class GuildRosterMain
    {

        public int Level { get; set; }
        public string Race { get; set; }
        public string Class { get; set; }
        public string Rank { get; set; }
        public string Name { get; set; }

    }

    public class Self_guild
    {
        public string href { get; set; }
    }

    public class Links_guild
    {
        public Self_guild self { get; set; }
    }

    public class Key_guild
    {
        public string href { get; set; }
    }

    public class Realm_guild
    {
        public Key_guild key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string slug { get; set; }
    }

    public class Faction_guild
    {
        public string type { get; set; }
        public string name { get; set; }
    }

    public class Guild_guild
    {
        public Key_guild key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public Realm_guild realm { get; set; }
        public Faction_guild faction { get; set; }
    }

    public class PlayableClass_guild
    {
        public Key_guild key { get; set; }
        public int id { get; set; }
    }

    public class PlayableRace_guild
    {
        public Key_guild key { get; set; }
        public int id { get; set; }
    }

    public class Character_guild
    {
        public Key_guild key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public Realm_guild realm { get; set; }
        public int level { get; set; }
        public PlayableClass_guild playable_class { get; set; }
        public PlayableRace_guild playable_race { get; set; }
    }

    public class Member_guild
    {
        public Character_guild character { get; set; }
        public int rank { get; set; }
    }

    public class MainGuild
    {
        public Links_guild _links { get; set; }
        public Guild_guild guild { get; set; }
        public List<Member_guild> members { get; set; }
    }
    #endregion

    #region RaiderIo


    public class GuildNormal
    {
        public int world { get; set; }
        public int region { get; set; }
        public int realm { get; set; }
    }

    public class GuildHeroic
    {
        public int world { get; set; }
        public int region { get; set; }
        public int realm { get; set; }
    }

    public class GuildMythic
    {
        public int world { get; set; }
        public int region { get; set; }
        public int realm { get; set; }
    }

    public class GuildCastleNathria
    {
        public GuildNormal normal { get; set; }
        public GuildHeroic heroic { get; set; }
        public GuildMythic mythic { get; set; }
        public string summary { get; set; }
        public int total_bosses { get; set; }
        public int normal_bosses_killed { get; set; }
        public int heroic_bosses_killed { get; set; }
        public int mythic_bosses_killed { get; set; }
    }

    public class GuildSanctumOfDomination
    {
        public GuildNormal normal { get; set; }
        public GuildHeroic heroic { get; set; }
        public GuildMythic mythic { get; set; }
        public string summary { get; set; }
        public int total_bosses { get; set; }
        public int normal_bosses_killed { get; set; }
        public int heroic_bosses_killed { get; set; }
        public int mythic_bosses_killed { get; set; }
    }

    public class GuildRaidRankings
    {
        [JsonProperty("castle-nathria")]
        public GuildCastleNathria CastleNathria { get; set; }

        [JsonProperty("sanctum-of-domination")]
        public GuildSanctumOfDomination SanctumOfDomination { get; set; }
    }

    public class GuildRaidProgression
    {
        [JsonProperty("castle-nathria")]
        public GuildCastleNathria CastleNathria { get; set; }

        [JsonProperty("sanctum-of-domination")]
        public GuildSanctumOfDomination SanctumOfDomination { get; set; }
    }

    public class GuildRaiderIO
    {
        public string name { get; set; }
        public string faction { get; set; }
        public string region { get; set; }
        public string realm { get; set; }
        public DateTime last_crawled_at { get; set; }
        public string profile_url { get; set; }
        public GuildRaidRankings raid_rankings { get; set; }
        public GuildRaidProgression raid_progression { get; set; }
    }
    #endregion

    #region Main Guild Info
    public class GuildMainSelf
    {
        public string href { get; set; }
    }

    public class GuildMainLinks
    {
        public GuildMainSelf self { get; set; }
    }

    public class GuildMainFaction
    {
        public string type { get; set; }
        public string name { get; set; }
    }

    public class GuildMainKey
    {
        public string href { get; set; }
    }

    public class GuildMainRealm
    {
        public GuildMainKey key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string slug { get; set; }
    }

    public class GuildMainMedia
    {
        public GuildMainKey key { get; set; }
        public int id { get; set; }
    }

    public class GuildMainRgba
    {
        public int r { get; set; }
        public int g { get; set; }
        public int b { get; set; }
        public double a { get; set; }
    }

    public class GuildMainColor
    {
        public int id { get; set; }
        public GuildMainRgba rgba { get; set; }
    }

    public class GuildMainEmblem
    {
        public int id { get; set; }
        public GuildMainMedia media { get; set; }
        public GuildMainColor color { get; set; }
    }

    public class GuildMainBorder
    {
        public int id { get; set; }
        public GuildMainMedia media { get; set; }
        public GuildMainColor color { get; set; }
    }

    public class GuildMainBackground
    {
        public GuildMainColor color { get; set; }
    }

    public class GuildMainCrest
    {
        public GuildMainEmblem emblem { get; set; }
        public GuildMainBorder border { get; set; }
        public GuildMainBackground background { get; set; }
    }

    public class GuildMainRoster
    {
        public string href { get; set; }
    }

    public class GuildMainAchievements
    {
        public string href { get; set; }
    }

    public class GuildMainActivity
    {
        public string href { get; set; }
    }

    public class GuildMain
    {
        public GuildMainLinks _links { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public GuildMainFaction faction { get; set; }
        public int achievement_points { get; set; }
        public int member_count { get; set; }
        public GuildMainRealm realm { get; set; }
        public GuildMainCrest crest { get; set; }
        public GuildMainRoster roster { get; set; }
        public GuildMainAchievements achievements { get; set; }
        public long created_timestamp { get; set; }
        public GuildMainActivity activity { get; set; }
    }

    #endregion
}
