using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;


namespace WB_parser.TelegramJob
{
    public class TelegramSendCard
    {
        private static string token = "5467885087:AAEu4YDiz3mWmA06EpYwQ60oXbrQiAHAHrs";
        private static List<long> userLstnrList = new List<long>();
        private static TelegramBotClient bot;

        public static async void BotStart()
        {
            bot = new TelegramBotClient(token);
            TeleJob();
        }

        public static async void TeleJob()
        {
            try
            {
                GetMessages();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка в телеграм боте: " + ex);
            }
        }

        static async Task GetMessages()
        {
            int offset = 0;
            int timeout = 0;

            try
            {
                await bot.SetWebhookAsync("");
                while (true)
                {
                    var updates = await bot.GetUpdatesAsync(offset, timeout);

                    foreach (var update in updates)
                    {
                        var message = update.Message;

                        switch (message.Text.ToLower())
                        {
                            case "join":
                                if (!userLstnrList.Contains(message.Chat.Id))
                                {
                                    userLstnrList.Add(message.Chat.Id);
                                    Console.WriteLine(message.Chat.Username + " записался на получение изменений цен");
                                    await bot.SendTextMessageAsync(message.Chat.Id, "Вы записаны на получение изменений цен");
                                }
                                break;
                            case "leave":
                                if (userLstnrList.Contains(message.Chat.Id))
                                {
                                    userLstnrList.Remove(message.Chat.Id);
                                    Console.WriteLine(message.Chat.Username + " отписался");
                                    await bot.SendTextMessageAsync(message.Chat.Id, "Вы отписаны от рассылок бота");
                                }
                                break;
                        }
                        offset = update.Id + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка в телеграм боте: " + ex);
            }
        }

        public static async void SendMessages(string name, int difference, string vendor_code)
        {
            foreach (var userId in userLstnrList)
            {
                await bot.SendTextMessageAsync(userId, $"Цена по товару {name} ({vendor_code}) {(difference > 0 ? "повысилась" : "понизилась")} на {Math.Abs(difference)}" +
                    $"\n{DateTime.Now.ToString("g")}");
            }
        }
    }
}
