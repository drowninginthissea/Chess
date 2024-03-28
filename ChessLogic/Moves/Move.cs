namespace ChessLogic
{
    /// <summary>
    /// Класс, олицетворяющий ход
    /// </summary>
    public abstract class Move
    {
        public abstract MoveType Type { get; }
        public abstract Position FromPosition { get; }
        public abstract Position ToPosition { get; }
        /// <summary>
        /// Выполнить текущий ход
        /// </summary>
        /// <param name="board">Доска, на которой собирается произойти ход</param>
        public abstract void Execute(Board board);
        /// <summary>
        /// Проверка легальности хода. Если после данного хода оппонент имеет шах на нас, то ход невозможен
        /// </summary>
        /// <param name="board">Доска для проверки, где происходит ход</param>
        /// <returns>true, если ход возможен, false, если мы походим и на нашего короля будет атака</returns>
        public virtual bool IsLegal(Board board)
        {
            Player color = board[FromPosition].Color;
            Board copy = board.Copy();
            Execute(copy);
            return !copy.IsInCheck(color);
        }
    }
}
