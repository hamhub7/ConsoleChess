namespace ConsoleChess
{
    public abstract class Piece
    {
        public PieceColor Color { get; private set; }

        public bool HasMoved { get; set; }

        public abstract IEnumerable<Move> PossibleMoves { get; }

        public ChessBoard.Space? Parent { get; private set; }

        public abstract char Char { get; }

        public Piece(PieceColor color)
        {
            Color = color;
            HasMoved = false;
        }

        public void OnPlace(ChessBoard.Space space)
        {
            Parent = space;
        }

        public void OnMove(ChessBoard.Space space)
        {
            Parent = space;
            HasMoved = true;
        }

        public override bool Equals(object? obj)
        {
            return obj is Piece piece &&
                   Color == piece.Color &&
                   HasMoved == piece.HasMoved &&
                   EqualityComparer<ChessBoard.Space?>.Default.Equals(Parent, piece.Parent);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Color, HasMoved, Parent);
        }
    }

    public enum PieceColor
    { Black, White }
}