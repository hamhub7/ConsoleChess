using ConsoleChess.Pieces;

namespace ConsoleChess
{
    public class Move
    {
        public ChessBoard.Space MoveTo { get; }
        public ChessBoard.Space MoveFrom { get; }
        public Piece MovingPiece { get; }
        public bool FirstMove { get; }
        public Piece? CapturedPiece { get; }
        public Piece? CastledPiece { get; }
        public ChessBoard.Space? CastleTo { get; }
        public ChessBoard.Space? CastleFrom { get; }
        public Piece? PromotedPiece { get; }

        public Move(ChessBoard.Space moveTo, ChessBoard.Space moveFrom, Piece movingPiece, bool firstMove, Piece? capturedPiece = null, Piece? castledPiece = null, ChessBoard.Space? castleTo = null, ChessBoard.Space? castleFrom = null, Piece? promotedPiece = null)
        {
            MoveTo = moveTo;
            MoveFrom = moveFrom;
            MovingPiece = movingPiece;
            FirstMove = firstMove;
            CapturedPiece = capturedPiece;
            CastledPiece = castledPiece;
            CastleTo = castleTo;
            CastleFrom = castleFrom;
            PromotedPiece = promotedPiece;
        }

        public override string ToString()
        {
            if (CastledPiece is not null) return "0-0";
            return $"{(MovingPiece is Pawn ? "" : MovingPiece.Char)}{(CapturedPiece is null ? "" : "x")}{MoveFrom}->{MoveTo}{(PromotedPiece is null ? "" : $"={PromotedPiece.Char}")}";
        }

        public override bool Equals(object? obj)
        {
            return obj is Move move &&
                   EqualityComparer<ChessBoard.Space>.Default.Equals(MoveTo, move.MoveTo) &&
                   EqualityComparer<ChessBoard.Space>.Default.Equals(MoveFrom, move.MoveFrom) &&
                   EqualityComparer<Piece>.Default.Equals(MovingPiece, move.MovingPiece) &&
                   FirstMove == move.FirstMove &&
                   EqualityComparer<Piece?>.Default.Equals(CapturedPiece, move.CapturedPiece) &&
                   EqualityComparer<Piece?>.Default.Equals(CastledPiece, move.CastledPiece) &&
                   EqualityComparer<ChessBoard.Space?>.Default.Equals(CastleTo, move.CastleTo) &&
                   EqualityComparer<ChessBoard.Space?>.Default.Equals(CastleFrom, move.CastleFrom) &&
                   EqualityComparer<Piece?>.Default.Equals(PromotedPiece, move.PromotedPiece);
        }

        public override int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(MoveTo);
            hash.Add(MoveFrom);
            hash.Add(MovingPiece);
            hash.Add(FirstMove);
            hash.Add(CapturedPiece);
            hash.Add(CastledPiece);
            hash.Add(CastleTo);
            hash.Add(CastleFrom);
            hash.Add(PromotedPiece);
            return hash.ToHashCode();
        }
    }
}