namespace ConsoleChess.Pieces
{
    public class Knight : Piece
    {
        public override char Char => 'N';

        public Knight(PieceColor color) : base(color)
        {
        }

        public Knight(Piece promote) : this(promote.Color)
        {
            HasMoved = promote.HasMoved;
        }

        public override IEnumerable<Move> PossibleMoves
        {
            get
            {
                if (Parent is null) yield break;

                for (int dr = -2; dr <= 2; dr++)
                {
                    for (int df = -2; df <= 2; df++)
                    {
                        if (Math.Abs(dr) + Math.Abs(df) == 3)
                        {
                            ChessBoard.Space? spaceInQuestion = Parent.RelativeSpace(dr, df);
                            if (spaceInQuestion == null) continue;
                            Piece? pieceAtSpace = spaceInQuestion.Piece;
                            if (pieceAtSpace == null) yield return new Move(spaceInQuestion, Parent, this, !HasMoved);
                            else if (pieceAtSpace.Color != Color) yield return new Move(spaceInQuestion, Parent, this, !HasMoved, pieceAtSpace);
                            // else continue
                        }
                    }
                }
            }
        }
    }
}