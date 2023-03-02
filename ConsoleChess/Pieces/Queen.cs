namespace ConsoleChess.Pieces
{
    public class Queen : Piece
    {
        public override char Char => 'Q';

        public Queen(PieceColor color) : base(color)
        {
        }

        public Queen(Piece promote) : this(promote.Color)
        {
            HasMoved = promote.HasMoved;
        }

        public override IEnumerable<Move> PossibleMoves
        {
            get
            {
                if (Parent is null) yield break;

                for (int rDir = -1; rDir <= 1; rDir += 1)
                {
                    for (int fDir = -1; fDir <= 1; fDir += 1)
                    {
                        if (rDir == 0 && fDir == 0) continue;

                        int dr = rDir;
                        int df = fDir;
                        while (dr >= -7 && dr <= 7 && df >= -7 && df <= 7)
                        {
                            ChessBoard.Space? spaceInQuestion = Parent.RelativeSpace(dr, df);
                            if (spaceInQuestion == null) break;
                            Piece? pieceAtSpace = spaceInQuestion.Piece;
                            if (pieceAtSpace != null)
                            {
                                if (pieceAtSpace.Color != Color) yield return new Move(spaceInQuestion, Parent, this, !HasMoved, pieceAtSpace);
                                break;
                            }
                            yield return new Move(spaceInQuestion, Parent, this, !HasMoved);

                            dr += rDir;
                            df += fDir;
                        }
                    }
                }
            }
        }
    }
}