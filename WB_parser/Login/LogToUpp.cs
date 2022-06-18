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

            internal void Say()
            {
                if (attempts.UserPassed())
                {
                    ConsoleColors.DrawColor("Green", $"Успешный вход! Работаем..");
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

            public bool UserPassed()
            {
                var result = false;
                for(int i = 0; i < count; i++)
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

            public string GetPassword()
            {
                return "777";
            }
        }

        internal class Input
        {
            public Input()
            { }

            public string GetUserInput()
            {
                ConsoleColors.DrawColor("Cyan", $"Ваш пароль: ");
                return Console.ReadLine();
            }
        }
    }
}
