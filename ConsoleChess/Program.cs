using ConsoleChess;

ChessGame game = new();
bool gameOver = false;
while (!gameOver)
{
    Console.WriteLine(game.BoardString());

    bool gotMove = false;
    Move? nextMove = null;
    while (!gotMove)
    {
        Console.WriteLine($"{game.ActivePlayer} to move");
        for (int i = 0; i < game.GetMoves().Count(); i++)
        {
            Move move = game.GetMoves().ElementAt(i);
            Console.WriteLine($"{i}: {move}");
        }
        Console.WriteLine("Input number of the move you want to make");
        //Console.WriteLine(game.BoardString());
        string? line = Console.ReadLine();
        if (line is not null && int.TryParse(line, out int moveNum))
        {
            gotMove = true;
            nextMove = game.GetMoves().ElementAt(moveNum);
        }
        else Console.WriteLine("Please input a valid number");
    }
    if (nextMove is null) throw new NullReferenceException("Move is null, despite selecting one");

    gameOver = game.PlayMove(nextMove);
}

PieceColor? winner = game.DetermineWinner();

if (winner is null)
{
    Console.WriteLine("The game ended in a draw");
}
else
{
    Console.WriteLine($"{winner} has won");
}