using System;
using System.IO;
using System.Linq;

namespace CatAndMouse
{
    class Program
    {
        static void Main(string[] args)
        {
            // Тесты 1..3: файлы вида 1_ChaseData.txt -> 1_PursuitLog.txt
            // (если у вас имена с точкой, как в задании, замените "_" на ".")
            for (int t = 1; t <= 3; t++)
            {
                Game.InputFile = t + ".ChaseData.txt";
                Game.OutFile = t + ".PursuitLog.txt";

                // первая строка входного файла — размер поля
                int n = int.Parse(File.ReadLines(Game.InputFile).First().Trim());

                Game game = new Game(n);
                game.Run();                      // запуск игры и вывод результатов
                Console.WriteLine();
            }
        }
    }
}
