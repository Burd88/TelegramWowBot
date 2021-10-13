using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace TelegramBot
{
    class CharInfo
    {
        private static string nameChar = "";
        private static string itemLVL = "Илвл: ";
        private static string classString = "Класс: нет";
        private static string spec = "Спек: нет";
        private static string coven = "Ковен: нет";

        private static string coven_soul = "Медиум: нет";
        private static string last_login = "Вход в игру: хз";
        private static string guild = "Гильдия: нет";
        private static string raid_progress = "Рейд: 0/0";
        private static string mythic_score = "Мифик: 0.0";
        private static string error = "false";

        public static string[] GetCharInfo(string name)
        {
            nameChar = "";
            itemLVL = "Илвл: ";
            classString = "Класс: нет";
            spec = "Спек: нет";
            coven = "Ковен: нет";
            coven_soul = "Медиум: нет";
            last_login = "Вход в игру: хз";
            guild = "Гильдия: нет";
            raid_progress = "Рейд: 0/0";
            mythic_score = "Мифик: 0.0";
            error = "false";

            Character_info(name);
            string[] charfull = new string[11];
            charfull[0] = nameChar;
            charfull[1] = guild;
            charfull[2] = itemLVL;
            charfull[3] = classString;
            charfull[4] = spec;
            charfull[5] = coven;
            charfull[6] = coven_soul;
            charfull[7] = raid_progress;
            charfull[8] = mythic_score;
            charfull[9] = last_login;
            charfull[10] = error;
            return charfull;
        }
        private static void Character_info(string name)
        {

            try
            {
                WebRequest requestchar = WebRequest.Create("https://eu.api.blizzard.com/profile/wow/character/howling-fjord/" + name.ToLower() + "?namespace=profile-eu&locale=ru_RU&access_token=" + Program.tokenWow);

                WebResponse responcechar = requestchar.GetResponse();

                using (Stream stream1 = responcechar.GetResponseStream())

                {
                    using (StreamReader reader1 = new StreamReader(stream1))
                    {
                        string line = "";
                        while ((line = reader1.ReadLine()) != null)
                        {
                            Root_charackter_full_info character = JsonConvert.DeserializeObject<Root_charackter_full_info>(line);


                            if (character.equipped_item_level.ToString() == "error")
                            {
                                itemLVL = "Илвл: 0";
                            }
                            else
                            {
                                itemLVL = "Илвл: " + character.equipped_item_level.ToString();
                            }
                            nameChar = character.name;
                            if (character.active_spec != null)
                            {
                                spec = "Спек: " + character.active_spec.name;

                            }
                            if (character.character_class != null)
                            {
                                classString = "Класс: " + character.character_class.name;
                            }
                            if (character.guild != null)
                            {
                                guild = "Гильдия: " + character.guild.name;
                            }


                            last_login = "Вход в игру: " + Functions.relative_time(Functions.FromUnixTimeStampToDateTime(character.last_login_timestamp.ToString()));
                            if (line.Contains("\"covenant_progress\":"))
                            {
                                coven = "Ковен: " + GetCoven(character.covenant_progress.chosen_covenant.id.ToString()) + " (" + character.covenant_progress.renown_level.ToString() + ")";
                            }

                            GetSoulbindsCharacter(character.name);
                            Character_raid_progress(character.name);

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
                    Console.WriteLine("CharInfo Error MainInfo: " + e.Message);
                }
            }
            catch (Exception e)
            {
                error = "true";
                Console.WriteLine("CharInfo Error MainInfo: " + e.Message);
            }
        }
        private static string GetCoven(string id)
        {
            if (id == "1")
            {
                return "Кирии";
            }
            else if (id == "2")
            {
                return "Вентиры";
            }
            else if (id == "3")
            {
                return "Ночной народец";
            }
            else if (id == "4")
            {
                return "Некролорды";
            }
            return "";
        }

        private static void GetSoulbindsCharacter(string name)
        {

            try
            {
                WebRequest request = WebRequest.Create("https://eu.api.blizzard.com/profile/wow/character/howling-fjord/" + name.ToLower() + "/soulbinds?namespace=profile-eu&locale=ru_RU&access_token=" + Program.tokenWow);
                WebResponse responce = request.GetResponse();

                using (Stream stream = responce.GetResponseStream())

                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line = "";
                        while ((line = reader.ReadLine()) != null)
                        {

                            CharacterSoulbindsAll allSoulbinds = JsonConvert.DeserializeObject<CharacterSoulbindsAll>(line);
                            if (allSoulbinds.soulbinds != null)
                            {
                                foreach (SoulbindSoulbinds soulbinds in allSoulbinds.soulbinds)
                                {

                                    if (soulbinds.is_active == true)
                                    {
                                        coven_soul = "Медиум: " + soulbinds.soulbind.name;





                                    }

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
                    Console.WriteLine("CharInfo Error Soulbind: " + e.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("CharInfo Error Soulbind: " + e.Message);
                error = "true";
            }


        }
        private static void Character_raid_progress(string name)
        {

            try
            {
                WebRequest requestchar = WebRequest.Create("https://raider.io/api/v1/characters/profile?region=eu&realm=howling-fjord&name=" + name + "&fields=mythic_plus_scores%2Craid_progression");

                WebResponse responcechar = requestchar.GetResponse();

                using (Stream stream1 = responcechar.GetResponseStream())

                {
                    using (StreamReader reader1 = new StreamReader(stream1))
                    {
                        string line = "";
                        while ((line = reader1.ReadLine()) != null)
                        {
                            RaiderIO_info character = JsonConvert.DeserializeObject<RaiderIO_info>(line);
                            raid_progress = "Рейд: " + character.raid_progression.CastleNathria.summary;
                            mythic_score = "Мифик: " + character.mythic_plus_scores.all;
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
                    Console.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                    Console.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                    Console.WriteLine("CharInfo Error RaidProgress: " + e.Message);
                    error = "true";
                }
            }
            catch (Exception e)
            {
                error = "true";
                Console.WriteLine("CharInfo Error RaidProgress: " + e.Message);
            }
        }


    }
    #region SoulBindClass
    public class SelfSoulbinds
    {
        public string href { get; set; }
    }

    public class LinksSoulbinds
    {
        public SelfSoulbinds self { get; set; }
    }

    public class KeySoulbinds
    {
        public string href { get; set; }
    }

    public class RealmSoulbinds
    {
        public KeySoulbinds key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string slug { get; set; }
    }

    public class CharacterSoulbinds
    {
        public KeySoulbinds key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public RealmSoulbinds realm { get; set; }
    }

    public class ChosenCovenantSoulbinds
    {
        public KeySoulbinds key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }

    public class Soulbind2Soulbinds
    {
        public KeySoulbinds key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }

    public class Trait2Soulbinds
    {
        public KeySoulbinds key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }

    public class TypeSoulbinds
    {
        public string type { get; set; }
        public string name { get; set; }
    }

    public class ConduitSoulbinds
    {
        public KeySoulbinds key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }

    public class SocketSoulbinds
    {
        public ConduitSoulbinds conduit { get; set; }
        public int rank { get; set; }
    }

    public class ConduitSocketSoulbinds
    {
        public TypeSoulbinds type { get; set; }
        public SocketSoulbinds socket { get; set; }
    }

    public class TraitSoulbinds
    {
        public Trait2Soulbinds trait { get; set; }
        public int tier { get; set; }
        public int display_order { get; set; }
        public ConduitSocketSoulbinds conduit_socket { get; set; }
    }

    public class SoulbindSoulbinds
    {
        public Soulbind2Soulbinds soulbind { get; set; }
        public List<TraitSoulbinds> traits { get; set; }
        public bool? is_active { get; set; }
    }

    public class CharacterSoulbindsAll
    {
        public LinksSoulbinds _links { get; set; }
        public CharacterSoulbinds character { get; set; }
        public ChosenCovenantSoulbinds chosen_covenant { get; set; }
        public int renown_level { get; set; }
        public List<SoulbindSoulbinds> soulbinds { get; set; }
    }
    #endregion
    #region CharInfoClass
    public class Self_charackter_full_info
    {
        public string href { get; set; }
    }

    public class Links_charackter_full_info
    {
        public Self_charackter_full_info self { get; set; }
    }

    public class Gender_charackter_full_info
    {
        public string type { get; set; }
        public string name { get; set; }
    }

    public class Faction_charackter_full_info
    {
        public string type { get; set; }
        public string name { get; set; }
    }

    public class Key_charackter_full_info
    {
        public string href { get; set; }
    }

    public class Race_charackter_full_info
    {
        public Key_charackter_full_info key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }

    public class CharacterClass_charackter_full_info
    {
        public Key_charackter_full_info key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }

    public class ActiveSpec_charackter_full_info
    {
        public Key_charackter_full_info key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }

    public class Realm_charackter_full_info
    {
        public Key_charackter_full_info key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string slug { get; set; }
    }

    public class Guild_charackter_full_info
    {
        public Key_charackter_full_info key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public Realm_charackter_full_info realm { get; set; }
        public Faction_charackter_full_info faction { get; set; }
    }

    public class Achievements_charackter_full_info
    {
        public string href { get; set; }
    }

    public class Titles_charackter_full_info
    {
        public string href { get; set; }
    }

    public class PvpSummary_charackter_full_info
    {
        public string href { get; set; }
    }

    public class Encounters_charackter_full_info
    {
        public string href { get; set; }
    }

    public class Media_charackter_full_info
    {
        public string href { get; set; }
    }

    public class Specializations_charackter_full_info
    {
        public string href { get; set; }
    }

    public class Statistics_charackter_full_info
    {
        public string href { get; set; }
    }

    public class MythicKeystoneProfile_charackter_full_info
    {
        public string href { get; set; }
    }

    public class Equipment_charackter_full_info
    {
        public string href { get; set; }
    }

    public class Appearance_charackter_full_info
    {
        public string href { get; set; }
    }

    public class Collections_charackter_full_info
    {
        public string href { get; set; }
    }

    public class Reputations_charackter_full_info
    {
        public string href { get; set; }
    }

    public class Quests_charackter_full_info
    {
        public string href { get; set; }
    }

    public class AchievementsStatistics_charackter_full_info
    {
        public string href { get; set; }
    }

    public class Professions_charackter_full_info
    {
        public string href { get; set; }
    }

    public class ChosenCovenant_charackter_full_info
    {
        public Key_charackter_full_info key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }

    public class Soulbinds_charackter_full_info
    {
        public string href { get; set; }
    }

    public class CovenantProgress_charackter_full_info
    {
        public ChosenCovenant_charackter_full_info chosen_covenant { get; set; }
        public int renown_level { get; set; }
        public Soulbinds_charackter_full_info soulbinds { get; set; }
    }

    public class Root_charackter_full_info
    {
        public Links_charackter_full_info _links { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public Gender_charackter_full_info gender { get; set; }
        public Faction_charackter_full_info faction { get; set; }
        public Race_charackter_full_info race { get; set; }
        public CharacterClass_charackter_full_info character_class { get; set; }
        public ActiveSpec_charackter_full_info active_spec { get; set; }
        public Realm_charackter_full_info realm { get; set; }
        public Guild_charackter_full_info guild { get; set; }
        public int level { get; set; }
        public int experience { get; set; }
        public int achievement_points { get; set; }
        public Achievements_charackter_full_info achievements { get; set; }
        public Titles_charackter_full_info titles { get; set; }
        public PvpSummary_charackter_full_info pvp_summary { get; set; }
        public Encounters_charackter_full_info encounters { get; set; }
        public Media_charackter_full_info media { get; set; }
        public long last_login_timestamp { get; set; }
        public int average_item_level { get; set; }
        public int equipped_item_level { get; set; }
        public Specializations_charackter_full_info specializations { get; set; }
        public Statistics_charackter_full_info statistics { get; set; }
        public MythicKeystoneProfile_charackter_full_info mythic_keystone_profile { get; set; }
        public Equipment_charackter_full_info equipment { get; set; }
        public Appearance_charackter_full_info appearance { get; set; }
        public Collections_charackter_full_info collections { get; set; }
        public Reputations_charackter_full_info reputations { get; set; }
        public Quests_charackter_full_info quests { get; set; }
        public AchievementsStatistics_charackter_full_info achievements_statistics { get; set; }
        public Professions_charackter_full_info professions { get; set; }
        public CovenantProgress_charackter_full_info covenant_progress { get; set; }
    }
    #endregion
    #region RaiderIOClass
    public class MythicPlusScores
    {
        public string all { get; set; }
        public string dps { get; set; }
        public string healer { get; set; }
        public string tank { get; set; }
        public string spec_0 { get; set; }
        public string spec_1 { get; set; }
        public string spec_2 { get; set; }
        public string spec_3 { get; set; }
    }
    public class CastleNathria
    {
        public string summary { get; set; }
        public string total_bosses { get; set; }
        public string normal_bosses_killed { get; set; }
        public string heroic_bosses_killed { get; set; }
        public string mythic_bosses_killed { get; set; }
    }

    public class RaidProgression
    {
        [JsonProperty("sanctum-of-domination")]
        public CastleNathria CastleNathria { get; set; }
    }

    public class RaiderIO_info
    {
        public string name { get; set; }
        public string race { get; set; }
        public string @class { get; set; }
        public string active_spec_name { get; set; }
        public string active_spec_role { get; set; }
        public string gender { get; set; }
        public string faction { get; set; }
        public string achievement_points { get; set; }
        public string honorable_kills { get; set; }
        public string thumbnail_url { get; set; }
        public string region { get; set; }
        public string realm { get; set; }
        public DateTime last_crawled_at { get; set; }
        public string profile_url { get; set; }
        public string profile_banner { get; set; }
        public MythicPlusScores mythic_plus_scores { get; set; }
        public RaidProgression raid_progression { get; set; }
    }
    #endregion
}
