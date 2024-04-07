using System.Data;
using System.Runtime.CompilerServices;

namespace ChessLogic
{
    public class Board
    {
        private readonly Piece[,] pieces = new Piece[8, 8];

        private readonly Dictionary<Player, Position> pawnSkipPosition = new()
        {
            { Player.White, null },
            { Player.Black, null },
        };

        public Piece this[int row, int col]
        {
            get { return pieces[row, col]; }
            set { pieces[row, col] = value; }
        }
        public Piece this[Position pos]
        {
            get { return this[pos.Row, pos.Column]; }
            set { this[pos.Row, pos.Column] = value; }
        }
        public static Board Initial()
        {
            Board board = new Board();
            board.AddStartPieces();
            return board;
        }
        public Position GetPawnSkipPosition(Player player)
        {
            return pawnSkipPosition[player];
        }
        public void SetPawnSkipPosition(Player player, Position pos)
        {
            pawnSkipPosition[player] = pos;
        }
        private void AddStartPieces()
        {
            this[0, 0] = new Rook(Player.Black);
            this[0, 1] = new Knight(Player.Black);
            this[0, 2] = new Bishop(Player.Black);
            this[0, 3] = new Queen(Player.Black);
            this[0, 4] = new King(Player.Black);
            this[0, 5] = new Bishop(Player.Black);
            this[0, 6] = new Knight(Player.Black);
            this[0, 7] = new Rook(Player.Black);

            this[7, 0] = new Rook(Player.White);
            this[7, 1] = new Knight(Player.White);
            this[7, 2] = new Bishop(Player.White);
            this[7, 3] = new Queen(Player.White);
            this[7, 4] = new King(Player.White);
            this[7, 5] = new Bishop(Player.White);
            this[7, 6] = new Knight(Player.White);
            this[7, 7] = new Rook(Player.White);

            for (int i = 0; i < 8; i++)
            {
                this[1, i] = new Pawn(Player.Black);
                this[6, i] = new Pawn(Player.White);
            }
        }
        public static bool IsInside(Position pos)
        {
            return pos.Row >= 0 && pos.Row < 8 && pos.Column >= 0 && pos.Column < 8;
        }
        public bool IsEmpty(Position pos)
        {
            return this[pos] == null;
        }
        public IEnumerable<Position> PiecePositions()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Position pos = new Position(r, c);
                    if (!IsEmpty(pos))
                    {
                        yield return pos;
                    }
                }
            }
        }
        public IEnumerable<Position> PiecePositionsFor(Player color)
        {
            return PiecePositions().Where(pos => this[pos].Color == color);
        }
        public bool IsInCheck(Player player)
        {
            return PiecePositionsFor(player.Opponent()).Any(pos => this[pos].CanCapruteOpponentKing(pos, this));
        }
        public Board Copy()
        {
            Board copy = new Board();
            foreach (Position pos in PiecePositions())
            {
                copy[pos] = this[pos].Copy();
            }
            return copy;
        }
        public Counting CountPieces()
        {
            Counting count = new Counting();

            foreach (Position pos in PiecePositions())
            {
                Piece piece = this[pos];
                count.Increment(piece.Color, piece.Type);
            }

            return count;
        }

        public bool InsufficientMaterial()
        {
            Counting count = CountPieces();
            return IsKingVsKing(count) ||
                IsKingVsKingBishop(count) ||
                IsKingVsKingKnight(count) ||
                IsKingBishopVsKingBishop (count);
        }

        private static bool IsKingVsKing(Counting count)
        {
            return count.TotalCount == 2;
        }

        private static bool IsKingVsKingBishop(Counting count)
        {
            return count.TotalCount == 3 && 
                (count.White(PieceType.Bishop) == 1 || count.Black(PieceType.Bishop) == 1);
        }

        private static bool IsKingVsKingKnight(Counting count)
        {
            return count.TotalCount == 3 &&
                (count.White(PieceType.Knight) == 1 || count.Black(PieceType.Knight) == 1);
        }

        private bool IsKingBishopVsKingBishop(Counting count)
        {
            if (count.TotalCount != 4)
            {
                return false;
            }
            if (count.White(PieceType.Bishop) != 1 || count.Black(PieceType.Bishop) != 1)
            {
                return false;
            }
            Position wBishopPos = FindPiece(Player.White, PieceType.Bishop);
            Position bBishopPos = FindPiece(Player.White, PieceType.Bishop);

            return wBishopPos.SquareColor() == bBishopPos.SquareColor();
        }
        private Position FindPiece(Player color, PieceType type)
        {
            return PiecePositionsFor(color).First(piece => this[piece].Type == type);
        }
    }
}
