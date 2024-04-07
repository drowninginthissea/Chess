namespace ChessLogic
{
    public class GameState
    {
        public Board Board { get; }
        public Player CurrentPlayer { get; private set; }
        public Result Result { get; private set; } = null;
        public GameState(Player player, Board board)
        {
            CurrentPlayer = player;
            Board = board;
        }
        public IEnumerable<Move> LegalMovesForPiece(Position pos)
        {
            if (Board.IsEmpty(pos) || Board[pos].Color != CurrentPlayer)
                return Enumerable.Empty<Move>();
            Piece piece = Board[pos];
            return piece.GetMoves(pos, Board).Where(move => move.IsLegal(Board));
        }
        public void MakeMove(Move move)
        {
            Board.SetPawnSkipPosition(CurrentPlayer, null);
            move.Execute(Board);
            CurrentPlayer = CurrentPlayer.Opponent();
            CheckForGameOver();

        }
        public IEnumerable<Move> AllLegalMovesFor(Player player)
        {
            IEnumerable<Move> moveCandidates = Board.PiecePositionsFor(player).SelectMany(pos => Board[pos].GetMoves(pos, Board));
            // не используем метод LegalMovesForPiece, потому что там идёт сравнение на то, что фигура не текущего игрока (что не нужно)
            return moveCandidates.Where(move => move.IsLegal(Board));
        }
        private void CheckForGameOver()
        {
            if (!AllLegalMovesFor(CurrentPlayer).Any())
            {
                if (Board.IsInCheck(CurrentPlayer))
                {
                    Result = Result.Win(CurrentPlayer.Opponent());
                }
                else
                {
                    Result = Result.Draw(EndReason.Stalemate);
                }
            }
            else if (Board.InsufficientMaterial())
            {
                Result = Result.Draw(EndReason.InsufficientMaterial);
            }
        }
        public bool IsGameOver()
        {
            return Result != null;
        }
    }
}
