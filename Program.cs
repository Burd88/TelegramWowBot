using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    class Program
    {
        private static string token { get; set; } = "2022351481:AAH9bJNYPDG5ytB5SMidMwnYuemusZtn4xk";
        private static TelegramBotClient client;
        private static bool additionalRT = false;

        [Obsolete]
        static void Main(string[] args)
        {
            int num = 0;
            AutorizationsBattleNet();
            Thread.Sleep(3000);
            client = new TelegramBotClient(token);
            client.StartReceiving();
            ReadAdditionalRTText();
            Functions.LoadRealmAll();
            client.OnMessage += OnMessageHandler;

            TimerCallback tmPoolRT = new(OnTimerHandlerPoolRT);
            Timer timerPoolRT = new(tmPoolRT, num, 0, 1000);

            TimerCallback tmCheckReboot = new(OnTimerHandlerheckReboot);
            Timer timerheckReboot = new(tmCheckReboot, num, 0, 30000);

            TimerCallback tmactivity = new(OnTimerHandlerActivity);
            Timer timerActivity = new(tmactivity, num, 0, 300000);

            TimerCallback tmachieve = new(OnTimerHandlerAchievements);
            Timer timerAchievements = new(tmachieve, num, 0, 300000);
            Console.ReadLine();

            client.StopReceiving();

        }

        private static async void StopBot()
        {
            await client.SendTextMessageAsync(-1001596135768, "Я диактивируюсь на какое-то время!");
        }
        private static async void StartBot()
        {
            await client.SendTextMessageAsync(-1001596135768, "Я снова в деле!");
        }
        private static async void OnTimerHandlerheckReboot(object obj)
        {
            AutorizationsBattleNet();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday)
            {
                string text = WowRealmInfo.GetRealmInfoForTimer();
                if (text != null)
                {
                    await client.SendTextMessageAsync(-1001184245832, text, parseMode: ParseMode.Html);
                    Console.WriteLine("Отправленно оповещение о Тех.Работах!");
                }


            }

        }
        private static async void OnTimerHandlerActivity(object obj)
        {



            try
            {
                // string writePathJSON = @"F:\TelegramWowBot\Activity.json";
                //  AllActivitys activitys = new(){ activity = new List<Activity>() };

                //  using (FileStream fs = new(writePathJSON, FileMode.OpenOrCreate))
                // {

                //     activitys = await System.Text.Json.JsonSerializer.DeserializeAsync<AllActivitys>(fs);

                // }
                AutorizationsBattleNet();
                var activ = GuildActivity.GetGuildActivity();
                if (activ.activity != null)
                {
                    foreach (Activity activs in activ.activity)
                    {
                        if (activ != null)
                        {
                            await client.SendTextMessageAsync(-1001184245832, activs.Name + "\n" + activs.Mode, parseMode: ParseMode.Html);
                            Console.WriteLine("Отправленна активность гильдии!");
                        }

                    }
                }




            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {

                    Console.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                    Console.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                    Console.WriteLine("GetGuildActicity Error: " + e.Message);

                }
            }
            catch (Exception e)
            {

                Console.WriteLine("GetGuildActicity Error: " + e.Message);

            }
        }

        private static async void OnTimerHandlerAchievements(object obj)
        {



            try
            {
                // string writePathJSON = @"F:\TelegramWowBot\Activity.json";
                //  AllActivitys activitys = new(){ activity = new List<Activity>() };

                //  using (FileStream fs = new(writePathJSON, FileMode.OpenOrCreate))
                // {

                //     activitys = await System.Text.Json.JsonSerializer.DeserializeAsync<AllActivitys>(fs);

                // }
                AutorizationsBattleNet();
                var achieves = GuildAchievements.GetGuildAchievements();
                if (achieves.Achievements != null)
                {
                    foreach (Achievement achieve in achieves.Achievements)
                    {
                        if (achieve != null)
                        {
                            await client.SendTextMessageAsync(-1001184245832, "<b>Гильдия получила достижение!</b>\n" + "Категория:<b>" + achieve.Category + "</b>\n" + "Название:<b>" + achieve.Name +"</b>", parseMode: ParseMode.Html);
                            Console.WriteLine("Отправленна достижение гильдии!");
                        }

                    }
                }




            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {

                    Console.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                    Console.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                    Console.WriteLine("GetGuildAchievements Error: " + e.Message);

                }
            }
            catch (Exception e)
            {

                Console.WriteLine("GetGuildAchievements Error: " + e.Message);

            }
        }
        private static async void OnTimerHandlerPoolRT(object obj)
        {

            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday || DateTime.Now.DayOfWeek == DayOfWeek.Wednesday || DateTime.Now.DayOfWeek == DayOfWeek.Thursday)
            {
                if (DateTime.Now.Hour == 21)
                {
                    if (DateTime.Now.Minute == 45)
                    {
                        if (DateTime.Now.Second == 00)
                        {
                            string writePathJSON = @"F:\TelegramWowBot\Userid.json";


                            UsersIdForTelegram users = new() { members = new List<User>() };



                            try
                            {

                                using (FileStream fs = new(writePathJSON, FileMode.OpenOrCreate))
                                {

                                    users = await System.Text.Json.JsonSerializer.DeserializeAsync<UsersIdForTelegram>(fs);

                                }

                                foreach (User usr in users.members)
                                {
                                    await client.SendTextMessageAsync(usr.Id, usr.Name + " через 15 минут РТ заходи в игру!");
                                }


                                await client.SendTextMessageAsync(-1001184245832, "<b>Всем Внимание!</b>\nРТ начнется через 15 минут!\nЗаходим все в игру!", parseMode: ParseMode.Html);
                                Console.WriteLine("Отправлено напоминание о рейде!");




                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                }


                if (DateTime.Now.Hour == 21)
                {
                    if (DateTime.Now.Minute == 00)
                    {
                        if (DateTime.Now.Second == 00)
                        {
                            Console.WriteLine($"Сработал таймер для опроса о рт: {DateTime.Now}");
                            string[] options = new string[3];
                            options[0] = "Да";
                            options[1] = "Нет";
                            options[2] = "Опоздаю";
                            await client.SendPollAsync(-1001184245832, "Через час начнется РТ, ты будешь?", options, false);
                            // await client.GetChatAsync(-1001596135768);
                        }
                        // -1001184245832 сердце греха
                        // -1001596135768 test
                    }

                }
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Tuesday)
            {

                if (additionalRT)
                {
                    if (DateTime.Now.Hour == 21)
                    {
                        if (DateTime.Now.Minute == 45)
                        {
                            if (DateTime.Now.Second == 00)
                            {


                                await client.SendTextMessageAsync(-1001184245832, "<b>Всем Внимание!</b>\nДополниетельное РТ начнется через 15 минут!", parseMode: ParseMode.Html);
                                Console.WriteLine("Отправлено напоминание о дополнительном рейде!");
                                additionalRT = false;
                            }
                        }
                    }
                    if (DateTime.Now.Hour == 21)
                    {
                        if (DateTime.Now.Minute == 00)
                        {
                            if (DateTime.Now.Second == 00)
                            {
                                Console.WriteLine($"Сработал таймер для опроса о доп рт: {DateTime.Now}");
                                string[] options = new string[3];
                                options[0] = "Да";
                                options[1] = "Нет";
                                options[2] = "Опоздаю";
                                await client.SendPollAsync(-1001184245832, "Через час начнется дополнительное РТ, ты будешь?", options, false);
                                // await client.GetChatAsync(-1001596135768);
                            }
                            // -1001184245832 сердце греха
                            // -1001596135768 test
                        }

                    }
                }

            }

        }
        [Obsolete]
        private static async void OnMessageHandler(object sender, MessageEventArgs e)
        {
            var msg = e.Message;

            if (msg.Type == MessageType.Text)
            {
                string mesg = msg.Text.ToLower().Trim();
                if (mesg != null)
                {
                    if (mesg.Contains("/realm"))
                    {
                        AutorizationsBattleNet();
                        string[] fullInfo = WowRealmInfo.GetRealmInfo();
                        string text = "";
                        if (fullInfo[2] == "false")
                        {
                            text = fullInfo[0] + "\n" + fullInfo[1];
                        }
                        else if (fullInfo[2] == "true")
                        {
                            text = "<b>Ошибка</b>\nПроблема на сервере.\nПопробуй позже.";
                        }
                        Console.WriteLine($"Пришло сообщение с текстом: {msg.Text} от {msg.Chat.Title}");
                        await client.SendTextMessageAsync(msg.Chat.Id, text, replyToMessageId: msg.MessageId, parseMode: ParseMode.Html);

                    }
                    else if (mesg.Contains("/char ") || mesg.Contains("/чар "))
                    {
                        try
                        {
                            AutorizationsBattleNet();
                            string name = mesg.Replace("/char ", "").Replace("/чар ", "").Trim();

                            string[] fullInfo = CharInfo.GetCharInfo(name);
                            string text = "";
                            if (fullInfo[10] == "false")
                            {
                                text = fullInfo[0] + "\n" + fullInfo[1] + "\n" + fullInfo[2] + "\n" + fullInfo[3] + "\n" + fullInfo[4] + "\n" + fullInfo[5] + "\n" +
                                 fullInfo[6] + "\n" + fullInfo[12] + "\n" + fullInfo[7] + "\n" + fullInfo[8] + "\n" + fullInfo[9] + "\n" + fullInfo[13];
                                Console.WriteLine(fullInfo[11]);
                                await client.SendPhotoAsync(msg.Chat.Id, fullInfo[11], text, replyToMessageId: msg.MessageId, parseMode: ParseMode.Html);
                            }
                            else if (fullInfo[10] == "true")
                            {
                                text = "<b>Ошибка</b>\nНе корректное имя: " + name + ".\nЛибо проблемы на сервере.";
                                await client.SendTextMessageAsync(msg.Chat.Id, text, parseMode: ParseMode.Html);
                            }
                        }
                        catch (Exception x)
                        {
                            string name = mesg.Replace("/char ", "").Replace("/чар ", "").Trim();
                            string text = "<b>Ошибка</b>\nНе корректное имя: " + name + ".\nЛибо проблемы на сервере.";
                            await client.SendTextMessageAsync(msg.Chat.Id, text, parseMode: ParseMode.Html);
                        }
                        Console.WriteLine($"Пришло сообщение с текстом: {msg.Text} от {msg.Chat.Title}");


                    }
                    else if (mesg.Contains("/guild") || mesg.Contains("/гильдия"))
                    {
                        AutorizationsBattleNet();

                        string[] fullInfo = GuildInfo.GetGuildInfo();
                        string text = "";
                        if (fullInfo[7] == "false")
                        {
                            text = $"{fullInfo[0]}\n{fullInfo[1]}\n{fullInfo[2]}\n{fullInfo[3]}\n{fullInfo[4]}\n{fullInfo[5]}\n{fullInfo[9]} место\n{fullInfo[6]}";
                        }
                        else if (fullInfo[7] == "true")
                        {
                            text = "<b>Ошибка</b>\nПроблема на сервере.\nПопробуй позже.";
                        }
                        Console.WriteLine($"Пришло сообщение с текстом: {msg.Text} от {msg.Chat.Title}");
                        await client.SendTextMessageAsync(msg.Chat.Id, text, replyToMessageId: msg.MessageId, parseMode: ParseMode.Html);
                    }
                    else if (mesg.Contains("/допрт "))
                    {
                        string command = mesg.Replace("/допрт ", "");
                        if (command == "назначить")
                        {
                            additionalRT = true;
                            WriteAdditionalRTInFile("true");
                            Console.WriteLine($"Пришло сообщение с текстом: {msg.Text} от {msg.Chat.Title} значение {additionalRT}");
                            await client.SendTextMessageAsync(-1001184245832, "<b>Всем Внимание!</b>\nНазначено дополнительное РТ на вторник!\nПрошу в игре в календаре отметится!", parseMode: ParseMode.Html);
                            await client.SendTextMessageAsync(msg.Chat.Id, "Доп рт назначено, статик оповещен, во вторник будет опрос перед рт", replyToMessageId: msg.MessageId, parseMode: ParseMode.Html);
                        }
                        else if (command == "отменить")
                        {
                            additionalRT = false;
                            WriteAdditionalRTInFile("false");
                            Console.WriteLine($"Пришло сообщение с текстом: {msg.Text} от {msg.Chat.Title} значение {additionalRT}");

                            await client.SendTextMessageAsync(-1001184245832, "<b>Всем Внимание!</b>\nДополниетельное РТ во вторник отменено!\nОтдыхаем", parseMode: ParseMode.Html);
                            await client.SendTextMessageAsync(msg.Chat.Id, "Доп рт отменено, статик оповещен, во вторник не будет опроса перед рт", replyToMessageId: msg.MessageId, parseMode: ParseMode.Html);
                        }
                        else
                        {
                            Console.WriteLine($"Пришло сообщение с текстом: {msg.Text} от {msg.Chat.Title} значение {additionalRT}");
                            await client.SendTextMessageAsync(msg.Chat.Id, "<b>Внимание!</b>\nНеверная команда: \n" + command + "\n<b>Корректные команды</b>:\nназначить\nотменить", replyToMessageId: msg.MessageId, parseMode: ParseMode.Html);
                        }
                    }
                    else if (mesg.Contains("/help"))
                    {
                        Console.WriteLine($"Пришло сообщение с текстом: {msg.Text}");
                        string text = "Список команд: \n/realm - показывавет состояние игрового мира.\n/char Имя персонажа - показывает информацию о персонаже.\n/guild - информация о гильдии.\n/lastlog - показывает последний загруженный лог РТ.\n/help - выводит доступные команды";
                        await client.SendTextMessageAsync(msg.Chat.Id, text, replyToMessageId: msg.MessageId);
                    }
                    else if (mesg.Contains("/sub "))
                    {
                        if (msg.Chat.Id != -1001184245832)
                        {
                            Console.WriteLine($"Пришло сообщение с текстом: {msg.Text} от {msg.Chat.Title}");
                            WriteUserIdInFile(msg.Chat.FirstName, msg.Text.Replace("/sub ", ""), msg.Chat.Id.ToString());

                            await client.SendTextMessageAsync(msg.Chat.Id, "Вы подписанны на рассылку информации от статик Бота", parseMode: ParseMode.Html);
                        }
                        else
                        {
                            await client.SendTextMessageAsync(msg.Chat.Id, "Напиши данную команду мне в личку", replyToMessageId: msg.MessageId, parseMode: ParseMode.Html);
                        }
                    }
                    else if (mesg.Contains("/lastlog"))
                    {
                        string[] fullInfo = GuildLogs.GetLogsInfo();
                        string text = "";
                        Console.WriteLine($"Пришло сообщение с текстом: {msg.Text}  от {msg.Chat.Title}");
                        if (fullInfo[5] == "false")
                        {
                            text = $"{fullInfo[0]}\n{fullInfo[1]}\n{fullInfo[2]}\n{fullInfo[3]}\n{fullInfo[6]}\n<b><a href =\"https://ru.warcraftlogs.com/guild/reports-list/47723/\">Все логи</a></b>";
                            var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Просмотр", fullInfo[4]));
                            await client.SendTextMessageAsync(msg.Chat.Id, text, replyMarkup: keyboard, parseMode: ParseMode.Html);
                        }
                        else if (fullInfo[5] == "true")
                        {
                            text = "Ошибка\nПроблема на сервере.\nПопробуй позже.";
                            await client.SendTextMessageAsync(msg.Chat.Id, text + "", replyToMessageId: msg.MessageId, parseMode: ParseMode.Html);
                        }
                    }
                    else if (mesg.Contains("/tactics") || mesg.Contains("/тактики"))
                    {
                        try
                        {

                            var keyboard = new InlineKeyboardMarkup(
                                new[]
                                {
                           new[]
    {
                        //       InlineKeyboardButton.WithCallbackData("Tarrag", "CallTarrag"),
        InlineKeyboardButton.WithUrl("Таррагр", "https://youtu.be/mGo44rWB00U"),
         InlineKeyboardButton.WithUrl("Око Тюремщика", "https://youtu.be/KQCR59KlMxg"),
         InlineKeyboardButton.WithUrl("Девять", "https://youtu.be/qNNAYP9V-ZQ"),
    },
                           new[]
    {
        InlineKeyboardButton.WithUrl("Душа Нер'зула", "https://youtu.be/cPKmRLZ3gg8"),
         InlineKeyboardButton.WithUrl("Дормацайн", "https://youtu.be/QxGSsfQkWm8"),
         InlineKeyboardButton.WithUrl("Разнал", "https://youtu.be/rHHyECuEA-w"),
    },
                           new[]
    {
        InlineKeyboardButton.WithUrl("Стражница Предвечных", "https://youtu.be/CrIKE9HRApg"),
         InlineKeyboardButton.WithUrl("Писарь Судьбы Ро-Кало", "https://youtu.be/UrotPeZxNmc"),
         InlineKeyboardButton.WithUrl("Кел'Тузад", "https://youtu.be/yahBQ8Ek0rc"),
    },
                           new[]
    {
        InlineKeyboardButton.WithUrl("Сильвана Ветрокрылая", "https://youtu.be/YUA9NmBDSRg"),
    },


                                });

                            /*  client.OnCallbackQuery += async (object sc, Telegram.Bot.Args.CallbackQueryEventArgs ev) =>
                              {
                                  var message = ev.CallbackQuery.Message;
                                  if (ev.CallbackQuery.Data == "CallTarrag")
                                  {
                                      await client.SendTextMessageAsync(msg.Chat.Id, "https://youtu.be/mGo44rWB00U");
                                  }

                              };*/


                            Console.WriteLine($"Пришло сообщение с текстом: {msg.Text}  от {msg.Chat.Title}");


                            await client.SendTextMessageAsync(msg.Chat, "Тактики на Святилище Господства от гильдии Банхаммер", replyMarkup: keyboard, parseMode: ParseMode.Html);

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

                    else if (mesg.Contains("/updaterealm"))
                    {
                        Functions.LoadRealmAll();
                        await client.SendTextMessageAsync(msg.Chat.Id, "Realms Update", replyToMessageId: msg.MessageId, parseMode: ParseMode.Html);
                    }
                    else if (mesg.Contains("/achieve"))
                    {
                        GuildAchievements.GetGuildAchievements();
                        await client.SendTextMessageAsync(msg.Chat.Id, "Achieve Update", replyToMessageId: msg.MessageId, parseMode: ParseMode.Html);
                    }
                }
            }
        }
        private static async void WriteUserIdInFile(string title, string name, string id)
        {
            string writePathJSON = @"F:\TelegramWowBot\Userid.json";


            UsersIdForTelegram users = new UsersIdForTelegram() { members = new List<User>() };



            try
            {

                using (FileStream fs = new(writePathJSON, FileMode.OpenOrCreate))
                {

                    users = await System.Text.Json.JsonSerializer.DeserializeAsync<UsersIdForTelegram>(fs);

                }

                using (FileStream fs = new(writePathJSON, FileMode.OpenOrCreate))
                {
                    var result = users.members.SingleOrDefault(a => a.Id == id);

                    if (result == null)
                    {
                        var options = new JsonSerializerOptions
                        {
                            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                            WriteIndented = true
                        };
                        users.members.Add(new User() { Title = title, Name = name, Id = id });
                        await System.Text.Json.JsonSerializer.SerializeAsync(fs, users, options);

                        Console.WriteLine("Запись персонажа " + name + " выполнена");
                    }
                    else
                    {
                        Console.WriteLine("Есть такой уже");
                    }




                }



            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static async void WriteAdditionalRTInFile(string text)
        {
            string writePathtext = @"F:\TelegramWowBot\AdditionalRT.txt";





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
        static async void ReadAdditionalRTText()
        {
            string writePathtext = @"F:\TelegramWowBot\AdditionalRT.txt";

            try
            {

                using (StreamReader sr = new StreamReader(writePathtext))
                {
                    string addRT = await sr.ReadToEndAsync();
                    additionalRT = Convert.ToBoolean(addRT);
                    Console.WriteLine(additionalRT);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static string tokenWow;

        class Token_for_api
        {
            public string access_token { get; set; }
            public string expires_in { get; set; }
            public string token_type { get; set; }
        }
        class UsersIdForTelegram
        {
            public List<User> members { get; set; }
        }

        class User
        {
            public string Title { get; set; }
            public string Name { get; set; }
            public string Id { get; set; }
        }
        private static async void AutorizationsBattleNet()
        {


            try
            {

                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.ConnectionClose = true;
                    using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("POST"), "https://eu.battle.net/oauth/token"))
                    {

                        string base64authorization = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("5adf246bde3d4a41a3792c229a6e425c:sBbit3bF6v0hSUzpPDPJIzy36dZhVwFq"));
                        request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

                        request.Content = new StringContent("grant_type=client_credentials");
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                        HttpResponseMessage response = await httpClient.SendAsync(request);
                        Token_for_api my_token = JsonConvert.DeserializeObject<Token_for_api>(response.Content.ReadAsStringAsync().Result);

                        tokenWow = my_token.access_token;







                    }

                }

            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {

                    Console.WriteLine(e.Message);
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }



        }

    }
}
