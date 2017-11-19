using System;

namespace InMemoryCommanding
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleKeyInfo keyInfo;
            do
            {
                Console.Clear();
                Console.WriteLine("1. Execute simple command");
                Console.WriteLine("2. Push to stack with dispatcher");
                Console.WriteLine("3. Simple auditing example (write to console)");
                Console.WriteLine("4. Simple auditing example, root command only (write to console)");
                Console.WriteLine("5. Execute simple command with no result");
                Console.WriteLine("");
                Console.WriteLine("Esc - quit");
                keyInfo = Console.ReadKey();
                Console.WriteLine();
                switch (keyInfo.Key)
                {
                    case ConsoleKey.D1:
#pragma warning disable 4014
                        ExecuteSimpleCommand.Run();
#pragma warning restore 4014
                        break;
                    case ConsoleKey.D2:
#pragma warning disable 4014
                        PushToStackWithDispatcher.Run();
#pragma warning restore 4014
                        break;

                    case ConsoleKey.D3:
#pragma warning disable 4014
                        ConsoleAuditing.Run(false);
#pragma warning restore 4014
                        break;

                    case ConsoleKey.D4:
#pragma warning disable 4014
                        ConsoleAuditing.Run(true);
#pragma warning restore 4014
                        break;

                    case ConsoleKey.D5:
#pragma warning disable 4014
                        ExecuteCommandWithoutResult.Run();
#pragma warning restore 4014
                        break;
                }
                if (keyInfo.Key != ConsoleKey.Escape)
                {
                    Console.ReadKey();
                }
            } while (keyInfo.Key != ConsoleKey.Escape);
        }
    }
}