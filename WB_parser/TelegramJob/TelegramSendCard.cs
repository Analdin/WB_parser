using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;


namespace WB_parser.TelegramJob
{
    public class TelegramSendCard
    {
        private static string token = "5467885087:AAEu4YDiz3mWmA06EpYwQ60oXbrQiAHAHrs";

        public static void TeleJob(string name, int difference, int price_lower, int price_higher, int vendor_code)
        {
            while (true)
            {
                try
                {
                    GetMessages().Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка в телеграм боте: " + ex);
                }
            }
        }

        static async Task GetMessages()
        {
            TelegramBotClient bot = new TelegramBotClient(token);
            int offset = 0;
            int timeout = 0;

            try
            {
                await bot.SetWebhookAsync("");
                while (true)
                {
                    var updates = await bot.GetUpdatesAsync(offset, timeout);

                    foreach(var update in updates)
                    {
                        var message = update.Message;

                        if(message.Text == "BotTest")
                        {
                            Console.WriteLine("Бот принял информацию: " + message.Text);
                            await bot.SendTextMessageAsync(message.Chat.Id, "Тестовый привет !" + message.Chat.Username);
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
    }
}
