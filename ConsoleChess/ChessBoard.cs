using ConsoleChess.Pieces;
using System.Text;

namespace ConsoleChess
{
    public class ChessBoard
    {
        public class Space
        {
            public ChessBoard Parent { get; private set; }

            public int Rank { get; private set; }
            public int File { get; private set; }

            public Piece? Piece { get; set; }

            public Space(ChessBoard parent, int rank, int file)
            {
                Parent = parent;
                Rank = rank;
                File = file;
            }

            public Space? RelativeSpace(int dr, int df)
            {
                return Parent.GetSpace(Rank + dr, File + df);
            }

            public override string ToString()
            {
                return $"{"abcdefgh"[File]}{"12345678"[Rank]}";
            }

            public override bool Equals(object? obj)
            {
                return obj is Space space &&
                       Rank == space.Rank &&
                       File == space.File;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Rank, File);
            }

            public enum Ranks
            { one, two, three, four, five, six, seven, eight }

            public enum Files
            { a, b, c, d, e, f, g, h }
        }

        private readonly Space[,] spaces;

        public Stack<Move> MovesSoFar { get; private set; }

        public IEnumerable<Piece> Pieces
        {
            get
            {
                for (int i = 0; i < spaces.GetLength(0); i++)
                {
                    for (int j = 0; j < spaces.GetLength(1); j++)
                    {
                        Space space = spaces[i, j];
                        if (space.Piece is not null) yield return space.Piece;
                    }
                }
            }
        }

        public ChessBoard()
        {
            spaces = new Space[8, 8];
            for (int r = 0; r < spaces.GetLength(0); r++)
            {
                for (int f = 0; f < spaces.GetLength(1); f++)
                {
                    spaces[r, f] = new Space(this, r, f);
                }
            }

            MovesSoFar = new Stack<Move>();
        }

        public Space? GetSpace(int rank, int file)
        {
            if (rank < 0 || rank >= spaces.GetLength(0) || file < 0 || file >= spaces.GetLength(1)) return null;
            return spaces[rank, file];
        }

        private static void PlacePiece(Space space, Piece piece)
        {
            space.Piece = piece;
            piece.OnPlace(space);
        }

        private static void MovePiece(Space space, Piece piece)
        {
            Space? from = piece.Parent;
            if (from == null) throw new NullReferenceException("Piece is trying to be moved without being on the board");
            space.Piece = piece;
            from.Piece = null;
            piece.OnMove(space);
        }

        public IEnumerable<Move> LegalMoves(PieceColor color)
        {
            foreach (Piece piece in Pieces.Where(p => p.Color == color))
            {
                foreach (Move move in piece.PossibleMoves)
                {
                    if (move.CastledPiece is not null)
                    {
                        if (move.CastleFrom is null || move.CastleTo is null) throw new NullReferenceException("Castled rook's destination or current space is null");

                        // move king to ensure that hes not in check
                        // this is a bit hacky, but basically create a move for the intermediate position and check there for check
                        Space? intermediateSpace;
                        if (move.MoveTo.File < move.MoveFrom.File) intermediateSpace = move.MoveTo.RelativeSpace(0, 1);
                        else intermediateSpace = move.MoveTo.RelativeSpace(0, -1);
                        if (intermediateSpace is null) throw new NullReferenceException("Castle check verification gave us a null intermediate space");

                        Move intermediateMove = new(intermediateSpace, move.MoveFrom, move.MovingPiece, move.FirstMove);
                        if (!MoveChecks(intermediateMove, color) && !MoveChecks(move, color)) yield return move;
                    }
                    else if (!MoveChecks(move, color)) yield return move;
                }
            }
        }

        public void ExecuteMove(Move move)
        {
            MovePiece(move.MoveTo, move.MovingPiece);
            if (move.CastledPiece is not null)
            {
                if (move.CastleFrom is null || move.CastleTo is null) throw new NullReferenceException("Castled rook's destination or current space is null");
                MovePiece(move.CastleTo, move.CastledPiece);
            }
            MovesSoFar.Push(move);
        }

        private void ReverseLast()
        {
            Move move = MovesSoFar.Pop();
            MovePiece(move.MoveFrom, move.MovingPiece);
            if (move.CastledPiece is not null)
            {
                if (move.CastleFrom is null || move.CastleTo is null) throw new NullReferenceException("Castled rook's destination or current space is null");
                MovePiece(move.CastleFrom, move.CastledPiece);
            }
            if (move.CapturedPiece is not null) PlacePiece(move.MoveTo, move.CapturedPiece);
            if (move.FirstMove) move.MovingPiece.HasMoved = false;
        }

        public bool KingChecked(PieceColor color)
        {
            Piece? king = Pieces.Where(p => p is King && p.Color == color).FirstOrDefault();

            if (king is null || king.Parent is null) throw new NullReferenceException($"{color} King is null, or its space is null");

            IEnumerable<Piece> opposingPieces = Pieces.Where(p => p.Color != color);

            foreach (Piece piece in opposingPieces)
            {
                if (piece.PossibleMoves.Select(m => m.MoveTo).Contains(king.Parent)) return true;
            }

            return false;
        }

        public bool MoveChecks(Move move, PieceColor color)
        {
            ExecuteMove(move);
            bool check = KingChecked(color);
            ReverseLast();
            return check;
        }

        public void SetupPiece(Piece piece, int rank, int file)
        {
            PlacePiece(spaces[rank, file], piece);
        }

        public void SetupBoard()
        {
            for (int r = 0; r < spaces.GetLength(0); r++)
            {
                for (int f = 0; f < spaces.GetLength(1); f++)
                {
                    spaces[r, f] = new Space(this, r, f);
                }
            }

            PlacePiece(spaces[0, 0], new Rook(PieceColor.White));
            PlacePiece(spaces[0, 1], new Knight(PieceColor.White));
            PlacePiece(spaces[0, 2], new Bishop(PieceColor.White));
            PlacePiece(spaces[0, 3], new Queen(PieceColor.White));
            PlacePiece(spaces[0, 4], new King(PieceColor.White));
            PlacePiece(spaces[0, 5], new Bishop(PieceColor.White));
            PlacePiece(spaces[0, 6], new Knight(PieceColor.White));
            PlacePiece(spaces[0, 7], new Rook(PieceColor.White));

            PlacePiece(spaces[1, 0], new Pawn(PieceColor.White));
            PlacePiece(spaces[1, 1], new Pawn(PieceColor.White));
            PlacePiece(spaces[1, 2], new Pawn(PieceColor.White));
            PlacePiece(spaces[1, 3], new Pawn(PieceColor.White));
            PlacePiece(spaces[1, 4], new Pawn(PieceColor.White));
            PlacePiece(spaces[1, 5], new Pawn(PieceColor.White));
            PlacePiece(spaces[1, 6], new Pawn(PieceColor.White));
            PlacePiece(spaces[1, 7], new Pawn(PieceColor.White));

            PlacePiece(spaces[6, 0], new Pawn(PieceColor.Black));
            PlacePiece(spaces[6, 1], new Pawn(PieceColor.Black));
            PlacePiece(spaces[6, 2], new Pawn(PieceColor.Black));
            PlacePiece(spaces[6, 3], new Pawn(PieceColor.Black));
            PlacePiece(spaces[6, 4], new Pawn(PieceColor.Black));
            PlacePiece(spaces[6, 5], new Pawn(PieceColor.Black));
            PlacePiece(spaces[6, 6], new Pawn(PieceColor.Black));
            PlacePiece(spaces[6, 7], new Pawn(PieceColor.Black));

            PlacePiece(spaces[7, 0], new Rook(PieceColor.Black));
            PlacePiece(spaces[7, 1], new Knight(PieceColor.Black));
            PlacePiece(spaces[7, 2], new Bishop(PieceColor.Black));
            PlacePiece(spaces[7, 3], new Queen(PieceColor.Black));
            PlacePiece(spaces[7, 4], new King(PieceColor.Black));
            PlacePiece(spaces[7, 5], new Bishop(PieceColor.Black));
            PlacePiece(spaces[7, 6], new Knight(PieceColor.Black));
            PlacePiece(spaces[7, 7], new Rook(PieceColor.Black));
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            for (int r = 7; r >= 0; r--)
            {
                for (int f = 0; f <= 7; f++)
                {
                    Space? space = GetSpace(r, f);
                    if (space is not null)
                    {
                        char rep = space.Piece?.Char ?? ' ';
                        sb.Append(space.Piece?.Color == PieceColor.Black ? char.ToLower(rep) : rep);
                    }
                }
                sb.AppendLine();
            }
            /*foreach(Piece piece in Pieces)
            {
                char rep = piece?.Char ?? ' ';
                sb.Append(piece?.Color == PieceColor.Black ? char.ToLower(rep) : rep);
                sb.AppendLine(piece?.Parent?.ToString());
            }*/
            return sb.ToString();
        }
    }
}