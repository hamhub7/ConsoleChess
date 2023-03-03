using ConsoleChess.Pieces;

namespace ConsoleChess
{
    public class ChessGame
    {
        private readonly ChessBoard board;

        public PieceColor ActivePlayer { get; private set; }

        public ChessGame()
        {
            board = new ChessBoard();
            ActivePlayer = PieceColor.White;
            board.SetupBoard();
        }

        public IEnumerable<Move> GetMoves()
        {
            foreach (Move move in board.LegalMoves(ActivePlayer))
            {
                yield return move;
            }
        }

        // return true if the game is over
        public bool PlayMove(Move move)
        {
            if (!GetMoves().Contains(move)) throw new MoveException($"{move} was not a legal move for {ActivePlayer}");
            board.ExecuteMove(move);

            ActivePlayer = ActivePlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;

            // Check endgame conditions
            if (!GetMoves().Any()) return true; // No legal moves, means checkmate or stalemate
            if (board.MovesSoFar.Skip(Math.Max(0, board.MovesSoFar.Count - 50)).Where(m => m.MovingPiece is not Pawn && m.CapturedPiece is null).Any()) return true; //None of the last 50 moves have captured a piece or moved a pawn. 50-move rule
            //implement repetition?
            if(!board.KingChecked(ActivePlayer)) // Implement material. make sure the king isnt in check before calculating material
            {
                if(!board.Pieces.Any(p => p is Pawn || p is Rook || p is Queen)) // Any of these pieces means we have sufficient material
                {
                    int whiteKnights = board.Pieces.Where(p => p.Color == PieceColor.White && p is Knight).Count();
                    int blackKnights = board.Pieces.Where(p => p.Color == PieceColor.Black && p is Knight).Count();
                    int whiteMinors = board.Pieces.Where(p => p.Color == PieceColor.White && (p is Bishop || p is Knight)).Count();
                    int blackMinors = board.Pieces.Where(p => p.Color == PieceColor.Black && (p is Bishop || p is Knight)).Count();

                    if (whiteMinors <= 1 && blackMinors <= 1) return true; // King v King, Minor + King v King, or Minor + King v Minor + King
                    if (whiteMinors == whiteKnights && whiteKnights == 2 && blackMinors == 0) return true; // 2 Knights + King v King
                    if (blackMinors == blackKnights && blackKnights == 2 && whiteMinors == 0) return true; // King v 2 Knights + King
                }
            }
            return false;
        }

        public PieceColor? DetermineWinner()
        {
            if (GetMoves().Any()) throw new Exception($"The game isn't over yet! {ActivePlayer} can still make moves");

            if (board.KingChecked(ActivePlayer)) return ActivePlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;
            return null; //Stalemate
        }

        public string BoardString()
        {
            return board.ToString();
        }

        public class MoveException : Exception
        {
            public MoveException()
            { }

            public MoveException(string? message) : base(message)
            {
            }
        }
    }
}