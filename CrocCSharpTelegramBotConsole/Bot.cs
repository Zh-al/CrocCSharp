using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace CrocCSharpTelegramBotConsole
{
    /// <summary>
    /// Основной модуль бота
    /// </summary>
    public class Bot
    {
        /// <summary>
        /// Клиент Telegram
        /// </summary>
        private TelegramBotClient client;

        /// <summary>
        /// Конструктор без параметров
        /// </summary>
        public Bot()
        {
            // Создание клиента для Telegram
            client = new TelegramBotClient("1391020211:AAHd8MDVR11PihUcvsJ84_65mG1JPtXg9_o");
            client.OnMessage += MessageProcessor;
        }

        /// <summary>
        /// Обработка входящего сообщения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageProcessor(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            switch (e.Message.Type)
            {
                case Telegram.Bot.Types.Enums.MessageType.Contact: // телефон
                    string phone = e.Message.Contact.PhoneNumber;
                    client.SendTextMessageAsync(e.Message.Chat.Id, $"Твой телефон: {phone}");
                    Console.WriteLine(phone);
                    break;

                case Telegram.Bot.Types.Enums.MessageType.Text: // текстовое сообщение
                    if (e.Message.Text.Substring(0, 1) == "/")
                    {
                        CommandProcessor(e);
                    }
                    else
                    {
                        client.SendTextMessageAsync(e.Message.Chat.Id, $"Ты сказал мне: {e.Message.Text}");
                        Console.WriteLine(e.Message.Text);
                    }
                    break;

                default:
                    client.SendTextMessageAsync(e.Message.Chat.Id, $"Ты прислал мне {e.Message.Type}, но я это пока не понимаю");
                    Help(e); // вывод возможностей
                    Console.WriteLine(e.Message.Type);
                    break;
            }
        }

        /// <summary>
        /// Выводит информацию о доступных командах
        /// </summary>
        private void Help(Telegram.Bot.Args.MessageEventArgs e)
        {
            string messageToSend = "Я умею:\n";
            foreach (var command in Enum.GetValues(typeof(Commands)))
            {
                messageToSend += $"/{command}\n";
            }

            client.SendTextMessageAsync(e.Message.Chat.Id, messageToSend);
        }

        /// <summary>
        /// Обработка команды
        /// </summary>
        /// <param name="message"></param>
        private void CommandProcessor(Telegram.Bot.Args.MessageEventArgs e)
        {
            // Отрезаем первый символ (который должен быть '/')
            string string_command = e.Message.Text.Substring(1).ToLower();

            Commands command;
            if (Enum.TryParse(string_command, out command))
                switch (command)
                {
                    case Commands.start:
                        var button = new KeyboardButton("Поделись телефоном");
                        button.RequestContact = true;
                        var array = new KeyboardButton[] { button };
                        var reply = new ReplyKeyboardMarkup(array, true, true);
                        client.SendTextMessageAsync(e.Message.Chat.Id, $"Привет, {e.Message.Chat.FirstName}, скажи мне свой телефон", replyMarkup: reply);
                        break;
                    case Commands.help:
                        Help(e);
                        break;
                    default:
                        client.SendTextMessageAsync(e.Message.Chat.Id, $"Это сообщение никогда не должно появиться. \nЕсли оно вывелось, бот написан неккоректно.");
                        break;
                }
            else
            {
                client.SendTextMessageAsync(e.Message.Chat.Id, $"Я пока не понимаю команду {string_command}");
                Help(e);
            }
        }

        /// <summary>
        /// Запуск бота
        /// </summary>
        public void Run()
        {
            // Запуск приема сообщений
            client.StartReceiving();
        }
    }
}