using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    class Program
    {
        private static string token { get; set; } = "2022351481:AAFAZI299dXGG9KEvS3W4bfiDjkvr_Ezh54";
        private static TelegramBotClient client;

        [Obsolete]
        static void Main(string[] args)
        {
            int num = 0;
            AutorizationsBattleNet();
            Thread.Sleep(3000);
            client = new TelegramBotClient(token);
            client.StartReceiving();
            client.OnMessage += OnMessageHandler;

            TimerCallback tm = new TimerCallback(OnTimerHandler);

            Timer timer = new Timer(tm, num, 0, 1000);
            TimerCallback tmactivity = new TimerCallback(OnTimerHandlerActivity);

            Timer timerActivity = new Timer(tmactivity, num, 0, 300000);
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

        private static async void OnTimerHandlerActivity(object obj)
        {
            AutorizationsBattleNet();
            GuildActivity.GetGuildActivity();
            Thread.Sleep(3000);
            try
            {
                string writePathJSON = @"F:\TelegramWowBot\Activity.json";
                AllActivitys activitys = new AllActivitys() { activity = new List<Activity>() };

                using (FileStream fs = new(writePathJSON, FileMode.OpenOrCreate))
                {

                    activitys = await System.Text.Json.JsonSerializer.DeserializeAsync<AllActivitys>(fs);

                }

                foreach (Activity activ in activitys.activity)
                {
                    if (activ != null)
                    {
                        await client.SendTextMessageAsync(-1001596135768, activ.Name + "\n" + activ.Mode + "\n" + activ.Time);
                        Console.WriteLine("Отправленна активность гильдии!");
                    }

                }



            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static async void OnTimerHandler(object obj)
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


                            UsersIdForTelegram users = new UsersIdForTelegram() { members = new List<User>() };



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

        }
        [Obsolete]
        private static async void OnMessageHandler(object sender, MessageEventArgs e)
        {
            var msg = e.Message;

            if (msg.Type == Telegram.Bot.Types.Enums.MessageType.Text)
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
                            text = "Ошибка\nПроблема на сервере.\nПопробуй позже.";
                        }
                        Console.WriteLine($"Пришло сообщение с текстом: {msg.Text}");
                        await client.SendTextMessageAsync(msg.Chat.Id, text, replyToMessageId: msg.MessageId);

                    }
                    else if (mesg.Contains("/char "))
                    {
                        AutorizationsBattleNet();
                        string name = mesg.Replace("/char ", "").Trim();
                        string[] fullInfo = CharInfo.GetCharInfo(name);
                        string text = "";
                        if (fullInfo[10] == "false")
                        {
                            text = fullInfo[0] + "\n" + fullInfo[1] + "\n" + fullInfo[2] + "\n" + fullInfo[3] + "\n" + fullInfo[4] + "\n" + fullInfo[5] + "\n" +
                             fullInfo[6] + "\n" + fullInfo[7] + "\n" + fullInfo[8] + "\n" + fullInfo[9];
                        }
                        else if (fullInfo[10] == "true")
                        {
                            text = "Ошибка\nНе корректное имя: " + name + ".\nЛибо проблемы на сервере.";
                        }

                        Console.WriteLine($"Пришло сообщение с текстом: {msg.Text}");
                        await client.SendTextMessageAsync(msg.Chat.Id, text, replyToMessageId: msg.MessageId);

                    }
                    else if (mesg.Contains("/guild") || mesg.Contains("/гильдия"))
                    {
                        AutorizationsBattleNet();

                        string[] fullInfo = GuildInfo.GetGuildInfo();
                        string text = "";
                        if (fullInfo[7] == "false")
                        {
                            text = fullInfo[0] + "\n" + fullInfo[1] + "\n" + fullInfo[2] + "\n" + fullInfo[3] + "\n" + fullInfo[4] + "\n" + fullInfo[5] + "\n" +
                             fullInfo[6];
                        }
                        else if (fullInfo[7] == "true")
                        {
                            text = "Ошибка\nПроблема на сервере.\nПопробуй позже.";
                        }

                        Console.WriteLine($"Пришло сообщение с текстом: {msg.Text}");
                        await client.SendTextMessageAsync(msg.Chat.Id, text, replyToMessageId: msg.MessageId);

                    }
                    else if (mesg.Contains("/help"))
                    {
                        Console.WriteLine($"Пришло сообщение с текстом: {msg.Text}");
                        string text = "Список команд: \n/realm - показывавет состояние игрового мира.\n/char Имя персонажа - показывает информацию о персонаже.\n/guild - информация о гильдии.\n/help - выводит доступные команды";
                        await client.SendTextMessageAsync(msg.Chat.Id, text, replyToMessageId: msg.MessageId);
                    }
                    else if (mesg.Contains("/sub "))
                    {
                        if (msg.Chat.Id != -1001184245832)
                        {
                            Console.WriteLine($"Пришло сообщение с текстом: {msg.Text} от чата {msg.Chat.Id}");
                            WriteUserIdInFile(msg.Text.Replace("/sub ", ""), msg.Chat.Id.ToString());

                            await client.SendTextMessageAsync(msg.Chat.Id, "Вы подписанны на рассылку информации от статик Бота");
                        }
                        else
                        {
                            await client.SendTextMessageAsync(msg.Chat.Id, "Напиши данную команду мне в личку", replyToMessageId: msg.MessageId);
                        }

                    }
                    else if (mesg.Contains("/lastlog"))
                    {
                        string[] fullInfo = GuildLogs.GetLogsInfo();
                        string text = "";
                        Console.WriteLine($"Пришло сообщение с текстом: {msg.Text}");
                        if (fullInfo[3] == "false")
                        {
                            text = fullInfo[0] + "\n" + fullInfo[1];
                            var keyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Просмотр", fullInfo[2]));
                            await client.SendTextMessageAsync(msg.Chat, text, replyMarkup: keyboard);
                        }
                        else if (fullInfo[3] == "true")
                        {
                            text = "Ошибка\nПроблема на сервере.\nПопробуй позже.";
                            await client.SendTextMessageAsync(msg.Chat.Id, text + "", replyToMessageId: msg.MessageId);
                        }




                    }

                }
                else if (msg.Type == Telegram.Bot.Types.Enums.MessageType.ChatMembersAdded)
                {
                    Console.WriteLine($"Пришло сообщение с текстом: {msg.Text}");
                    foreach (Telegram.Bot.Types.User users in msg.NewChatMembers)
                    {
                        Console.WriteLine($"Пришло сообщение с текстом: {users.Username}");
                        await client.SendTextMessageAsync(-1001596135768, "Добро пожаловать в наш статик " + users.Username, replyToMessageId: msg.MessageId);
                    }

                }

            }
        }

        private static async void WriteUserIdInFile(string name, string id)
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
                        users.members.Add(new User() { Name = name, Id = id });
                        await System.Text.Json.JsonSerializer.SerializeAsync(fs, users);

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
