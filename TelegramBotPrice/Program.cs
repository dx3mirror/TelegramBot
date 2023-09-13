using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotPrice
{
    public class TelegramBot
    {
        private readonly string _botToken;
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, Func<string, Task<string>>> _commands;

        public TelegramBot(string botToken)
        {
            _botToken = botToken;
            _httpClient = new HttpClient();
            _commands = new Dictionary<string, Func<string, Task<string>>>
            {
                { "/price", GetPriceAsync }
            };
        }

        public async Task ProcessMessageAsync(string chatId, string message)
        {
            try
            {
                if (_commands.TryGetValue(message, out var command))
                {
                    var response = await command(chatId);
                    await SendMessageAsync(chatId, response);
                }
                else
                {
                    await SendMessageAsync(chatId, "Неизвестная команда");
                }
            }
            catch (Exception ex)
            {
                await SendMessageAsync(chatId, "Ошибка: " + ex.Message);
            }
        }

        private async Task<string> GetPriceAsync(string chatId)
        {
            // Здесь можно добавить логику для получения ценника продукта
            return "Примерный ценник продукта: $100";
        }

        private async Task SendMessageAsync(string chatId, string message)
        {
            string url = $"https://api.telegram.org/bot{_botToken}/sendMessage";
            string json = $"{{\"chat_id\":\"{chatId}\",\"text\":\"{message}\"}}";

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            response.EnsureSuccessStatusCode();
        }
    }

    public class Program
    {
        static async Task Main(string[] args)
        {
            string botToken = "YOUR_BOT_TOKEN"; // Замените на свой токен

            TelegramBot bot = new TelegramBot(botToken);

            string chatId = "YOUR_CHAT_ID"; // Замените на свой идентификатор чата

            while (true)
            {
                Console.WriteLine("Введите команду:");
                string command = Console.ReadLine();

                await bot.ProcessMessageAsync(chatId, command);
            }
        }
    }
}
