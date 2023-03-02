namespace ConsoleChess.Pieces
{
    public class Pawn : Piece
    {
        public override char Char => 'P'; // when generating a move, we will leave this off. its just for display in the console

        public Pawn(PieceColor color) : base(color)
        {
        }

        // Need to implement en passant and promotion
        public override IEnumerable<Move> PossibleMoves
        {
            get
            {
                if (Parent is null) yield break;

                int dir = Color == PieceColor.White ? 1 : -1;

                // Normal moves
                ChessBoard.Space? inFront = Parent.RelativeSpace(dir, 0);
                if (inFront is not null && inFront.Piece is null)
                {
                    yield return new Move(inFront, Parent, this, !HasMoved);

                    ChessBoard.Space? twoInFront = Parent.RelativeSpace(2 * dir, 0);
                    if (!HasMoved && twoInFront is not null && twoInFront.Piece is null) yield return new Move(twoInFront, Parent, this, !HasMoved);
                }

                // Diagonal captures
                ChessBoard.Space? inFrontDiag1 = Parent.RelativeSpace(dir, -1);
                if (inFrontDiag1 is not null)
                {
                    if (inFrontDiag1.Piece is not null && inFrontDiag1.Piece.Color != Color) yield return new Move(inFrontDiag1, Parent, this, !HasMoved, inFrontDiag1.Piece);
                }
                ChessBoard.Space? inFrontDiag2 = Parent.RelativeSpace(dir, 1);
                if (inFrontDiag2 is not null)
                {
                    if (inFrontDiag2.Piece is not null && inFrontDiag2.Piece.Color != Color) yield return new Move(inFrontDiag2, Parent, this, !HasMoved, inFrontDiag2.Piece);
                }
            }
        }
    }
}