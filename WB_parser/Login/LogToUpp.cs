using WB_parser.Color;

namespace WB_parser.Login
{
    public class LogToUpp
    {
        internal class Firewell
        {
            private Attempts attempts;

            public Firewell(Attempts attempts)
            {
                this.attempts = attempts;
            }

            /// <summary>
            /// Вывод сообщения в консоль
            /// </summary>
            internal void Say()
            {
                if (attempts.UserPassed())
                {
                    ConsoleColors.DrawColor("Green", $"Успешный вход! Работаем..");
                    Thread.Sleep(500);
                    ConsoleColors.Clear();
                }
                else
                {
                    ConsoleColors.DrawColor("Red", $"К сожалению, попытки закончились!");
                }
            }
        }

        internal class Attempts
        {
            private VerboseDiff verboseDiff;

            private int count;

            public Attempts(VerboseDiff verboseDiff, int count)
            {
                this.verboseDiff = verboseDiff;
                this.count = count;
            }

            /// <summary>
            /// Получение пароля и проверка на корректность
            /// </summary>
            /// <returns>Вывод результата</returns>
            public bool UserPassed()
            {
                var result = false;
                for (int i = 0; i < count; i++)
                {
                    if (verboseDiff.IsAttemptCorrect())
                    {
                        result = true;
                        break;
                    }
                }
                return result;
            }
        }

        internal class VerboseDiff
        {
            private Diff diff;
            public VerboseDiff(Diff diff)
            {
                this.diff = diff;
            }

            /// <summary>
            /// Проверка - верный пароль или нет
            /// </summary>
            /// <returns>Возвращает результат</returns>
            public bool IsAttemptCorrect()
            {
                var result = diff.IsInputCorrect();
                if (result)
                {
                    ConsoleColors.DrawColor("Green", $"Спасибо!");
                }
                else
                {
                    ConsoleColors.DrawColor("Red", $"К сожалению, пароль не верен!");
                }
                return result;
            }
        }

        internal class Diff
        {
            private Password password;

            private Input input;

            public Diff(Password password, Input input)
            {
                this.password = password;
                this.input = input;
            }

            /// <summary>
            /// Проверка корректности ввода
            /// </summary>
            /// <returns>Возвращает результат проверки</returns>
            public bool IsInputCorrect()
            {
                var userInput = input.GetUserInput();
                var currentPassword = password.GetPassword();

                return userInput.Equals(currentPassword);
            }
        }

        internal class Password
        {
            public Password()
            { }

            /// <summary>
            /// Получаем пароль
            /// </summary>
            /// <returns>Возвращаем тот пароль, который надо ввести для успешного входа</returns>
            public string GetPassword()
            {
                return "777";
            }
        }

        internal class Input
        {
            public Input()
            { }

            /// <summary>
            /// Получаем то, что ввел пользователь
            /// </summary>
            /// <returns>Возвращаем результат</returns>
            public string GetUserInput()
            {
                ConsoleColors.DrawColor("Cyan", $"Ваш пароль: ");
                return Console.ReadLine();
            }
        }
    }
}
