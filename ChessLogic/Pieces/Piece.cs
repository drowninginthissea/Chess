namespace ChessLogic
{
    /// <summary>
    /// Класс, представлющий шахматную фигуру
    /// </summary>
    public abstract class Piece
    {
        public abstract PieceType Type { get; }
        public abstract Player Color { get; }
        public bool HasMoved { get; set; } = false;
        public abstract Piece Copy();
        /// <summary>
        /// Возвращает все ходы, которые доступны у данной фигуры на данной позиции
        /// </summary>
        /// <param name="from">Откуда производиться ход</param>
        /// <param name="board">Доска, на которой производиться ход</param>
        /// <returns>Всевозможные ходы фигуры</returns>
        public abstract IEnumerable<Move> GetMoves(Position from, Board board);
        /// <summary>
        /// Нахождение всех ходов, которые может сделать фигура в определённом направлении
        /// </summary>
        /// <param name="from">Место, откуда ходит фигура</param>
        /// <param name="board">Доска, на которой ходит</param>
        /// <param name="dir">Направление, в которое ходит фигура</param>
        /// <returns></returns>
        protected IEnumerable<Position> MovePositionsInDir(Position from, Board board, Direction dir)
        {
            for (Position pos = from + dir; Board.IsInside(pos); pos += dir)
            {
                if (board.IsEmpty(pos))
                {
                    yield return pos;
                    continue;
                }

                Piece piece = board[pos];
                if (piece.Color != Color)
                {
                    yield return pos;
                }

                yield break;
            }
        }
        protected IEnumerable<Position> MovePositionsInDirs(Position from, Board board, Direction[] dirs)
        {
            return dirs.SelectMany(dir => MovePositionsInDir(from, board, dir));
        }
        public virtual bool CanCapruteOpponentKing(Position from, Board board)
        {
            return GetMoves(from, board).Any(move =>
            {
                Piece piece = board[move.ToPosition];
                return piece != null && piece.Type == PieceType.King;
            });
        }
    }
}
