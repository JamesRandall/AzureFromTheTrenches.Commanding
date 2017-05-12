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
                Console.WriteLine("");
                Console.WriteLine("Esc - quit");
                keyInfo = Console.ReadKey();
                Console.WriteLine();
                switch (keyInfo.Key)
                {
                    case ConsoleKey.D1:
                        ExecuteSimpleCommand.Run();
                        break;
                    case ConsoleKey.D2:
                        PushToStackWithDispatcher.Run();
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