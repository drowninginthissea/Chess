using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ChessLogic;

namespace ChessUI
{
    public partial class MainWindow : Window
    {
        private readonly Image[,] pieceImages = new Image[8, 8];
        private readonly Rectangle[,] highlights = new Rectangle[8, 8];
        private readonly Dictionary<Position, Move> moveCache = new Dictionary<Position, Move>();

        private GameState gameState;
        private Position selectedPos;
        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();
            gameState = new GameState(Player.White, Board.Initial());
            DrawBoard(gameState.Board);
            SetCursor(gameState.CurrentPlayer);
        }
        private void InitializeBoard()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Image image = new Image();
                    pieceImages[r, c] = image;
                    PieceGrid.Children.Add(image);

                    Rectangle highlight = new Rectangle();
                    highlights[r, c] = highlight;
                    HighlighGrid.Children.Add(highlight);
                }
            }
        }
        private void DrawBoard(Board board)
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Piece piece = board[r, c];
                    pieceImages[r, c].Source = Images.GetImage(piece);
                }
            }
        }
        private void BoardGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsMenuOnScreen())
                return;

            Point point = e.GetPosition(BoardGrid);
            Position pos = ToSquarePosition(point);

            if (selectedPos == null)
            {
                OnFromPositionSelected(pos);
            }
            else
            {
                OnToPositionSelected(pos);
            }
        }
        private void OnFromPositionSelected(Position pos)
        {
            IEnumerable<Move> moves = gameState.LegalMovesForPiece(pos);
            if (moves.Any())
            {
                selectedPos = pos;
                CacheMoves(moves);
                ShowHighlights();
            }
        }
        private void OnToPositionSelected(Position pos)
        {
            selectedPos = null;
            HideHighlights();
            if (moveCache.TryGetValue(pos, out Move move))
            {
                if (move.Type == MoveType.PawnPromotion)
                {
                    HandlePromotion(move.FromPosition, move.ToPosition);
                }
                else
                {
                    HandleMove(move);
                }
            }
        }
        private void HandlePromotion(Position from, Position to)
        {
            pieceImages[from.Row, from.Column].Source = null;
            pieceImages[to.Row, to.Column].Source = Images.GetImage(PieceType.Pawn, gameState.CurrentPlayer);

            PromotionMenu menu = new PromotionMenu(gameState.CurrentPlayer);
            MenuContainer.Content = menu;
            menu.PieceSelected += piece =>
            {
                MenuContainer.Content = null;
                Move promMove = new PawnPromotion(from, to, piece);
                HandleMove(promMove);
            };
        }
        private void HandleMove(Move move)
        {
            gameState.MakeMove(move);
            DrawBoard(gameState.Board);
            SetCursor(gameState.CurrentPlayer);

            if (gameState.IsGameOver())
            {
                ShowGameOver();
            }
        }
        private Position ToSquarePosition(Point point)
        {
            double squareSize = BoardGrid.ActualWidth / 8;
            int column = (int)(point.X / squareSize);
            int row = (int)(point.Y / squareSize);
            return new Position(row, column);
        }
        private void CacheMoves(IEnumerable<Move> moves)
        {
            moveCache.Clear();
            foreach (Move move in moves)
            {
                moveCache[move.ToPosition] = move;
            }
        }
        private void ShowHighlights()
        {
            Color color = Color.FromArgb(150, 125, 255, 125);

            foreach (Position to in moveCache.Keys)
            {
                highlights[to.Row, to.Column].Fill = new SolidColorBrush(color);
            }
        }
        private void HideHighlights()
        {
            foreach (Position to in moveCache.Keys)
            {
                highlights[to.Row, to.Column].Fill = Brushes.Transparent;
            }
        }
        private void SetCursor(Player player)
        {
            if (player == Player.White)
            {
                Cursor = ChessCursors.WhiteCursor;
            }
            else
            {
                Cursor = ChessCursors.BlackCursor;
            }
        }
        private bool IsMenuOnScreen()
        {
            return MenuContainer.Content != null;
        }
        private void ShowGameOver()
        {
            GameOverMenu menu = new GameOverMenu(gameState);
            MenuContainer.Content = menu;

            menu.OptionSelected += option =>
            {
                if (option == Option.Restart)
                {
                    MenuContainer.Content = null;
                    RestartGame();
                }
                else
                {
                    Application.Current.Shutdown();
                }
            };
        }
        private void RestartGame()
        {
            selectedPos = null;
            HideHighlights();
            moveCache.Clear();
            gameState = new GameState(Player.White, Board.Initial());
            DrawBoard(gameState.Board);
            SetCursor(gameState.CurrentPlayer);
        }
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!IsMenuOnScreen() && e.Key == Key.Escape)
            {
                ShowPauseMenu();
            }
        }
        private void ShowPauseMenu()
        {
            PauseMenu menu = new PauseMenu();
            MenuContainer.Content = menu;
            menu.OptionSelected += option =>
            {
                MenuContainer.Content = null;
                if (option == Option.Restart)
                {
                    RestartGame();
                }
            };
        }
    }
}