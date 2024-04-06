using ChessLogic;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChessUI
{
    public partial class PromotionMenu : UserControl
    {
        private readonly Player color;
        public event Action<PieceType> PieceSelected;
        public PromotionMenu(Player pawnColor)
        {
            InitializeComponent();
            color = pawnColor;
            InitializeImages();
        }
        private void InitializeImages()
        {
            QueenImage.Source = Images.GetImage(PieceType.Queen, color);
            BishopImage.Source = Images.GetImage(PieceType.Bishop, color);
            KnightImage.Source = Images.GetImage(PieceType.Knight, color);
            RookImage.Source = Images.GetImage(PieceType.Rook, color);
        }

        private void QueenImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PieceSelected?.Invoke(PieceType.Queen);
        }

        private void BishopImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PieceSelected?.Invoke(PieceType.Bishop);
        }

        private void KnightImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PieceSelected?.Invoke(PieceType.Knight);
        }

        private void RookImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PieceSelected?.Invoke(PieceType.Rook);
        }
    }
}
