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
        private static string itemLVL = "<b>Илвл</b>: ";
        private static string classString = "<b>Класс</b>: нет";
        private static string spec = "<b>Спек</b>: нет";
        private static string coven = "<b>Ковен</b>: нет";
        private static string stats = "<b>Статы</b>: ";
        private static string coven_soul = "<b>Медиум</b>: нет";
        private static string last_login = "<b>Вход в игру</b>: хз";
        private static string guild = "<b>Гильдия</b>: нет";
        private static string raid_progress = "<b>Рейд</b>: 0/0";
        private static string mythic_score = "<b>Мифик: 0.0</b>";
        private static string image_char = "";
        private static string error = "false";
        private static string link_bNet = "";
        private static string linkForChar = "";

        public static string[] GetCharInfo(string name)
        {
            nameChar = "";
            itemLVL = "<b>Илвл</b>: ";
            classString = "<b>Класс</b>: нет";
            spec = "<b>Спек</b>: нет";
            coven = "<b>Ковен</b>: нет";
            stats = "<b>Статы</b>: ";
            coven_soul = "<b>Медиум</b>: нет";
            last_login = "<b>Вход в игру</b>: хз";
            guild = "<b>Гильдия</b>: нет";
            raid_progress = "<b>Рейд</b>: 0/0";
            mythic_score = "<b>Мифик: 0.0</b>";
            image_char = "";
            error = "false";
            link_bNet = "";
            GetLinkForChar(name);

            string[] charfull = new string[14];
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
            charfull[11] = image_char;
            charfull[12] = stats;
            charfull[13] = link_bNet;
            return charfull;
        }
        private static string name = "";
        private static string realm = "";
        private static void GetLinkForChar(string text)
        {
            if (text.Contains("-"))
            {
                string[] str = text.Split("-");
                foreach (RealmList rlm in Functions.Realms)
                {
                    if (rlm.Name.ToLower() == str[1].ToLower())
                    {
                        realm = rlm.Slug;
                        Character_info(str[0]);


                    }
                }

            }
            else
            {
                realm = "howling-fjord";
                Character_info(text);


            }
        }
        private static void Character_info(string name)
        {

            try
            {
                Console.WriteLine("https://eu.api.blizzard.com/profile/wow/character/" + realm + "/" + name.ToLower() + "?namespace=profile-eu&locale=ru_RU&access_token=" + Program.tokenWow);
                WebRequest requestchar = WebRequest.Create("https://eu.api.blizzard.com/profile/wow/character/" + realm + "/" + name.ToLower() + "?namespace=profile-eu&locale=ru_RU&access_token=" + Program.tokenWow);

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
                                itemLVL = "<b>Илвл</b>: 0";
                            }
                            else
                            {
                                itemLVL = "<b>Илвл</b>: " + character.equipped_item_level.ToString();
                            }
                            nameChar = "<b>" + character.name + "</b>";
                            if (character.active_spec != null)
                            {
                                spec = "<b>Спек</b>: " + character.active_spec.name;

                            }
                            if (character.character_class != null)
                            {
                                classString = "<b>Класс</b>: " + character.character_class.name;
                            }
                            if (character.guild != null)
                            {
                                guild = "<b>Гильдия</b>: " + character.guild.name;
                            }


                            last_login = "<b>Вход в игру</b>: " + Functions.relative_time(Functions.FromUnixTimeStampToDateTime(character.last_login_timestamp.ToString()));
                            if (line.Contains("\"covenant_progress\":"))
                            {
                                coven = "<b>Ковен</b>: " + GetCoven(character.covenant_progress.chosen_covenant.id.ToString()) + " (" + character.covenant_progress.renown_level.ToString() + ")";
                            }

                            GetSoulbindsCharacter(character.name);
                            Character_raid_progress(character.name);
                            GetCharMedia(character.name);
                            GetCharStats(character.name);
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
                WebRequest request = WebRequest.Create("https://eu.api.blizzard.com/profile/wow/character/" + realm + "/" + name.ToLower() + "/soulbinds?namespace=profile-eu&locale=ru_RU&access_token=" + Program.tokenWow);
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
                                        coven_soul = "<b>Медиум</b>: " + soulbinds.soulbind.name;





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
                WebRequest requestchar = WebRequest.Create("https://raider.io/api/v1/characters/profile?region=eu&realm=" + realm + "&name=" + name + "&fields=mythic_plus_scores%2Craid_progression");

                WebResponse responcechar = requestchar.GetResponse();

                using (Stream stream1 = responcechar.GetResponseStream())

                {
                    using (StreamReader reader1 = new StreamReader(stream1))
                    {
                        string line = "";
                        while ((line = reader1.ReadLine()) != null)
                        {
                            RaiderIO_info character = JsonConvert.DeserializeObject<RaiderIO_info>(line);
                            raid_progress = "<b>Рейд</b>: " + character.raid_progression.CastleNathria.summary;
                            mythic_score = "<b>Мифик</b>: " + character.mythic_plus_scores.all;
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
        private static void GetCharMedia(string name)
        {


            try
            {
                WebRequest request = WebRequest.Create("https://eu.api.blizzard.com/profile/wow/character/" + realm + "/" + name.ToLower() + "/character-media?namespace=profile-eu&locale=ru_RU&access_token=" + Program.tokenWow);
                WebResponse responce = request.GetResponse();

                using (Stream stream = responce.GetResponseStream())

                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line = "";
                        while ((line = reader.ReadLine()) != null)
                        {


                            CharMedia charMedia = JsonConvert.DeserializeObject<CharMedia>(line);


                            foreach (AssetCM media in charMedia.assets)
                            {
                                if (media.key == "inset")
                                {

                                    image_char = media.value;
                                    string lnk = "https://worldofwarcraft.com/ru-ru/character/eu/howling-fjord/" + name.ToLower();
                                    link_bNet = $"<a href=\"{lnk}\">Просмотр на офф сайте</a>";

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
                    Console.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                    Console.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                    Console.WriteLine("CharInfo Error Media: " + e.Message);
                    error = "true";
                }
            }
            catch (Exception e)
            {
                error = "true";
                Console.WriteLine("CharInfo Error Media: " + e.Message);
            }


        }
        private static void GetCharStats(string name)
        {


            try
            {
                WebRequest request = WebRequest.Create("https://eu.api.blizzard.com/profile/wow/character/" + realm + "/" + name.ToLower() + "/statistics?namespace=profile-eu&locale=ru_RU&access_token=" + Program.tokenWow);
                WebResponse responce = request.GetResponse();

                using (Stream stream = responce.GetResponseStream())

                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line = "";
                        while ((line = reader.ReadLine()) != null)
                        {


                            CharStats charStats = JsonConvert.DeserializeObject<CharStats>(line);
                            double crit = charStats.melee_crit.value;
                            crit = Math.Round(crit, 1);
                            double haste = charStats.melee_haste.value;
                            haste = Math.Round(haste, 1);
                            double mastery = charStats.mastery.value;
                            mastery = Math.Round(mastery, 1);
                            double versality_damage = charStats.versatility_damage_done_bonus;
                            versality_damage = Math.Round(versality_damage, 1);
                            double versality_healing = charStats.versatility_damage_taken_bonus;
                            versality_healing = Math.Round(versality_healing, 1);
                            stats = stats + "\n" + "  <b>Критический удар</b>:  " + crit.ToString() + "%\n  <b>Скорость</b>:  " + haste.ToString() + "%\n"
                                + "  <b>Искусность</b>:  " + mastery.ToString() + "%\n  <b>Универсальность</b>:  " + versality_damage.ToString() + "% / " + versality_healing.ToString() + "%";





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
                    Console.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                    Console.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                    Console.WriteLine("CharInfo Error Stats: " + e.Message);
                    error = "true";
                }
            }
            catch (Exception e)
            {
                error = "true";
                Console.WriteLine("CharInfo Error Stats: " + e.Message);
            }


        }
    }
    #region Char Stats
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Self
    {
        public string href { get; set; }
    }

    public class Links
    {
        public Self self { get; set; }
    }

    public class Key
    {
        public string href { get; set; }
    }

    public class PowerType
    {
        public Key key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }

    public class Speed
    {
        public double rating { get; set; }
        public double rating_bonus { get; set; }
    }

    public class Strength
    {
        public int @base { get; set; }
        public int effective { get; set; }
    }

    public class Agility
    {
        public int @base { get; set; }
        public int effective { get; set; }
    }

    public class Intellect
    {
        public int @base { get; set; }
        public int effective { get; set; }
    }

    public class Stamina
    {
        public int @base { get; set; }
        public int effective { get; set; }
    }

    public class MeleeCrit
    {
        public double rating { get; set; }
        public double rating_bonus { get; set; }
        public double value { get; set; }
    }

    public class MeleeHaste
    {
        public double rating { get; set; }
        public double rating_bonus { get; set; }
        public double value { get; set; }
    }

    public class Mastery
    {
        public double rating { get; set; }
        public double rating_bonus { get; set; }
        public double value { get; set; }
    }

    public class Lifesteal
    {
        public double rating { get; set; }
        public double rating_bonus { get; set; }
        public double value { get; set; }
    }

    public class Avoidance
    {
        public double rating { get; set; }
        public double rating_bonus { get; set; }
    }

    public class SpellCrit
    {
        public double rating { get; set; }
        public double rating_bonus { get; set; }
        public double value { get; set; }
    }

    public class Armor
    {
        public int @base { get; set; }
        public int effective { get; set; }
    }

    public class Dodge
    {
        public double rating { get; set; }
        public double rating_bonus { get; set; }
        public double value { get; set; }
    }

    public class Parry
    {
        public double rating { get; set; }
        public double rating_bonus { get; set; }
        public double value { get; set; }
    }

    public class Block
    {
        public double rating { get; set; }
        public double rating_bonus { get; set; }
        public double value { get; set; }
    }

    public class RangedCrit
    {
        public double rating { get; set; }
        public double rating_bonus { get; set; }
        public double value { get; set; }
    }

    public class RangedHaste
    {
        public double rating { get; set; }
        public double rating_bonus { get; set; }
        public double value { get; set; }
    }

    public class SpellHaste
    {
        public double rating { get; set; }
        public double rating_bonus { get; set; }
        public double value { get; set; }
    }

    public class Realm
    {
        public Key key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string slug { get; set; }
    }

    public class Character
    {
        public Key key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public Realm realm { get; set; }
    }

    public class CharStats
    {
        public Links _links { get; set; }
        public double health { get; set; }
        public double power { get; set; }
        public PowerType power_type { get; set; }
        public Speed speed { get; set; }
        public Strength strength { get; set; }
        public Agility agility { get; set; }
        public Intellect intellect { get; set; }
        public Stamina stamina { get; set; }
        public MeleeCrit melee_crit { get; set; }
        public MeleeHaste melee_haste { get; set; }
        public Mastery mastery { get; set; }
        public double bonus_armor { get; set; }
        public Lifesteal lifesteal { get; set; }
        public double versatility { get; set; }
        public double versatility_damage_done_bonus { get; set; }
        public double versatility_healing_done_bonus { get; set; }
        public double versatility_damage_taken_bonus { get; set; }
        public Avoidance avoidance { get; set; }
        public double attack_power { get; set; }
        public double main_hand_damage_min { get; set; }
        public double main_hand_damage_max { get; set; }
        public double main_hand_speed { get; set; }
        public double main_hand_dps { get; set; }
        public double off_hand_damage_min { get; set; }
        public double off_hand_damage_max { get; set; }
        public double off_hand_speed { get; set; }
        public double off_hand_dps { get; set; }
        public double spell_power { get; set; }
        public double spell_penetration { get; set; }
        public SpellCrit spell_crit { get; set; }
        public double mana_regen { get; set; }
        public double mana_regen_combat { get; set; }
        public Armor armor { get; set; }
        public Dodge dodge { get; set; }
        public Parry parry { get; set; }
        public Block block { get; set; }
        public RangedCrit ranged_crit { get; set; }
        public RangedHaste ranged_haste { get; set; }
        public SpellHaste spell_haste { get; set; }
        public Character character { get; set; }
    }


    #endregion
    #region Char Media
    public class SelfCM
    {
        public string href { get; set; }
    }

    public class LinksCM
    {
        public SelfCM self { get; set; }
    }

    public class KeyCM
    {
        public string href { get; set; }
    }

    public class RealmCM
    {
        public KeyCM key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public string slug { get; set; }
    }

    public class CharacterCM
    {
        public KeyCM key { get; set; }
        public string name { get; set; }
        public int id { get; set; }
        public RealmCM realm { get; set; }
    }

    public class AssetCM
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class CharMedia
    {
        public LinksCM _links { get; set; }
        public CharacterCM character { get; set; }
        public List<AssetCM> assets { get; set; }
    }
    #endregion
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
