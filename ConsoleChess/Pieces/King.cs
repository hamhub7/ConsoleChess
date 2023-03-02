namespace ConsoleChess.Pieces
{
    public class King : Piece
    {
        public override char Char => 'K';

        public King(PieceColor color) : base(color)
        {
        }

        public override IEnumerable<Move> PossibleMoves
        {
            get
            {
                if (Parent is null) yield break;

                // Normal Moves
                for (int rDir = -1; rDir <= 1; rDir += 1)
                {
                    for (int fDir = -1; fDir <= 1; fDir += 1)
                    {
                        if (rDir == 0 && fDir == 0) continue;
                        ChessBoard.Space? spaceInQuestion = Parent.RelativeSpace(rDir, fDir);
                        if (spaceInQuestion == null) continue;
                        Piece? pieceAtSpace = spaceInQuestion.Piece;
                        if (pieceAtSpace != null)
                        {
                            if (pieceAtSpace.Color != Color) yield return new Move(spaceInQuestion, Parent, this, !HasMoved, pieceAtSpace);
                        }
                        else yield return new Move(spaceInQuestion, Parent, this, !HasMoved);
                    }
                }

                // Queenside Castle - a file
                ChessBoard.Space? rookSpaceQ = Parent.Parent.GetSpace((Color == PieceColor.White) ? 0 : 7, 0);
                if (!HasMoved && rookSpaceQ != null && rookSpaceQ.Piece is Rook && rookSpaceQ.Piece.Color == Color && !rookSpaceQ.Piece.HasMoved)
                {
                    // neither us or the rook have moved, we can castle if nothing is in the way
                    bool wayClear = true;
                    for (int i = 1; i <= 3; i++)
                    {
                        ChessBoard.Space? intermediateSpace = Parent.Parent.GetSpace((Color == PieceColor.White) ? 0 : 7, i);
                        if (intermediateSpace is null || intermediateSpace.Piece is not null) { wayClear = false; break; }
                    }

                    ChessBoard.Space? castleTo = Parent.Parent.GetSpace((Color == PieceColor.White) ? 0 : 7, 2);
                    if (wayClear && castleTo is not null)
                    {
                        yield return new Move(castleTo, Parent, this, !HasMoved, null, rookSpaceQ.Piece, castleTo.RelativeSpace(0, 1), rookSpaceQ);
                    }
                }

                // Kingside Castle - h file
                ChessBoard.Space? rookSpaceK = Parent.Parent.GetSpace((Color == PieceColor.White) ? 0 : 7, 7);
                if (!HasMoved && rookSpaceK != null && rookSpaceK.Piece is Rook && rookSpaceK.Piece.Color == Color && !rookSpaceK.Piece.HasMoved)
                {
                    // neither us or the rook have moved, we can castle if nothing is in the way
                    bool wayClear = true;
                    for (int i = 5; i <= 6; i++)
                    {
                        ChessBoard.Space? intermediateSpace = Parent.Parent.GetSpace((Color == PieceColor.White) ? 0 : 7, i);
                        if (intermediateSpace is null || intermediateSpace.Piece is not null) { wayClear = false; break; }
                    }

                    ChessBoard.Space? castleTo = Parent.Parent.GetSpace((Color == PieceColor.White) ? 0 : 7, 6);
                    if (wayClear && castleTo is not null)
                    {
                        yield return new Move(castleTo, Parent, this, !HasMoved, null, rookSpaceK.Piece, castleTo.RelativeSpace(0, -1), rookSpaceK);
                    }
                }
            }
        }
    }
}