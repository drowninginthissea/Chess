using ChessLogic;
using System.Windows;
using System.Windows.Controls;

namespace ChessUI
{
    public partial class GameOverMenu : UserControl
    {
        public event Action<Option> OptionSelected;
        public GameOverMenu(GameState gameState)
        {
            InitializeComponent();

            Result result = gameState.Result;
            WinnerText.Text = GetWinnerText(result.Winner);
            ReasonText.Text = GetReasonText(result.Reason, gameState.CurrentPlayer);
        }
        private static string GetWinnerText(Player winner) =>
            winner switch
            {
                Player.White => "ПОБЕДА БЕЛЫХ",
                Player.Black => "ПОБЕДА ЧЁРНЫХ",
                _ => "НИЧЬЯ"
            };
        private static string PlayerString(Player player) =>
            player switch
            {
                Player.White => "БЕЛЫЕ",
                Player.Black => "ЧЁРНЫЕ",
                _ => ""
            };
        private static string GetReasonText(EndReason reason, Player player) =>
            reason switch
            {
                EndReason.Checkmate => $"ШАХ И МАТ - {PlayerString(player)} НЕ МОГУТ ДВИГАТЬСЯ",
                EndReason.Stalemate => $"ПАТ - {PlayerString(player)} НЕ МОГУТ ДВИГАТЬСЯ",
                EndReason.FiftyMoveRule => $"ПРАВИЛО 50 ХОДОВ",
                EndReason.InsufficientMaterial => $"НЕДОСТАТОК МАТЕРИАЛОВ",
                EndReason.ThreefoldRepetition => $"ТРОЕКРАТНОЕ ПОВТОРЕНИЕ ПОЗИЦИИ",
                _ => ""
            };
        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Option.Restart);
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Option.Exit);
        }
    }
}
