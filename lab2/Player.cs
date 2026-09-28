using System;

namespace CatAndMouse
{
    /// <summary>Состояние игрока.</summary>
    enum State
    {
        Winner,     // победитель
        Loser,      // проигравший
        Playing,    // играет
        NotInGame   // не в игре (позиция ещё не задана)
    }

    /// <summary>Игрок (кот или мышь) на линейном поле из N клеток, замкнутом в круг.</summary>
    class Player
    {
        public string name;                      // имя игрока
        public int location;                     // позиция на поле (1..N), -1 если не в игре
        public State state = State.NotInGame;    // состояние
        public int distanceTraveled = 0;         // пройденное расстояние

        private readonly int fieldSize;          // размер поля N

        public Player(string name, int fieldSize)
        {
            this.name = name;
            this.fieldSize = fieldSize;
            this.location = -1;                  // не в игре
        }

        /// <summary>Задать начальную позицию (это не ход, расстояние не считается).</summary>
        public void SetStart(int cell)
        {
            location = Normalize(cell);
            state = State.Playing;
        }

        /// <summary>Переместиться на steps клеток (отрицательное — назад).
        /// За границей поля игрок оказывается на противоположном конце (поле — круг).</summary>
        public void Move(int steps)
        {
            if (state != State.Playing) return;  // вне игры ходить нельзя
            location = Normalize(location + steps);
            distanceTraveled += Math.Abs(steps);
        }

        /// <summary>Приводит номер клетки к диапазону 1..N (циклически).</summary>
        private int Normalize(int cell)
        {
            int zeroBased = ((cell - 1) % fieldSize + fieldSize) % fieldSize;
            return zeroBased + 1;
        }

        public bool InGame => state == State.Playing;

        /// <summary>Позиция для вывода: число или "??".</summary>
        public string LocationText => InGame ? location.ToString() : "??";
    }
}
