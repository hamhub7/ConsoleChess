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

            if (!GetMoves().Any()) return true;
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