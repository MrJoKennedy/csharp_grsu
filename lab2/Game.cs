using System;
using System.IO;
using System.Text;

namespace CatAndMouse
{
    enum GameState { Running, End }

    class Game
    {
        // Пути к входному и выходному файлам задаются извне
        public static string InputFile = "ChaseData.txt";
        public static string OutFile = "PursuitLog.txt";

        private const string Line = "-------------------";

        private readonly int size;               // размер поля N
        private readonly Player cat;
        private readonly Player mouse;
        private GameState state = GameState.Running;
        private readonly StringBuilder log = new StringBuilder();

        public Game(int size)
        {
            this.size = size;
            cat = new Player("Cat", size);
            mouse = new Player("Mouse", size);
        }

        /// <summary>Запуск игры: чтение команд, выполнение, вывод результатов.</summary>
        public void Run()
        {
            string[] lines = File.ReadAllLines(InputFile);

            log.AppendLine("Cat and Mouse");
            log.AppendLine();
            log.AppendLine("Cat Mouse  Distance");
            log.AppendLine(Line);

            int i = 1; // строка 0 — размер поля, уже передан в конструктор
            while (state != GameState.End)
            {
                // пропускаем пустые строки
                while (i < lines.Length && lines[i].Trim() == "") i++;

                if (i >= lines.Length)           // команды исчерпаны
                {
                    state = GameState.End;
                    break;
                }

                string[] parts = lines[i++].Split(new[] { ' ', '\t' },
                                                  StringSplitOptions.RemoveEmptyEntries);
                char command = parts[0][0];

                if (command == 'P')
                    DoPrintCommand();
                else
                    DoMoveCommand(command, int.Parse(parts[1]));

                // если мышь поймана — конец игры
                if (cat.InGame && mouse.InGame && cat.location == mouse.location)
                {
                    cat.state = State.Winner;
                    mouse.state = State.Loser;
                    state = GameState.End;
                }
            }

            PrintResults();
        }

        private void DoMoveCommand(char command, int steps)
        {
            Player p = command == 'M' ? mouse : cat;
            if (p.state == State.NotInGame)
                p.SetStart(steps);               // первая команда — начальная позиция
            else
                p.Move(steps);
        }

        private void DoPrintCommand()
        {
            string dist = (cat.InGame && mouse.InGame) ? GetDistance().ToString() : "";
            log.AppendLine(string.Format("{0,3}{1,6}{2,10}",
                                         cat.LocationText, mouse.LocationText, dist).TrimEnd());
        }

        private int GetDistance()
        {
            return Math.Abs(cat.location - mouse.location);
        }

        private void PrintResults()
        {
            log.AppendLine(Line);
            log.AppendLine();
            log.AppendLine();
            log.AppendLine("Distance traveled:   Mouse    Cat");
            log.AppendLine(string.Format("{0,26}{1,7}", mouse.distanceTraveled, cat.distanceTraveled));
            log.AppendLine();

            if (mouse.state == State.Loser)
                log.AppendLine(string.Format("Mouse caught at: {0,2}", mouse.location));
            else
            {
                mouse.state = State.Winner;
                cat.state = State.Loser;
                log.AppendLine("Mouse evaded Cat");
            }

            File.WriteAllText(OutFile, log.ToString());
            Console.Write(log.ToString());       // вывод на экран
        }
    }
}
