namespace Chess
{
	public class ChessProblem
	{
		private readonly Board board;

	    public ChessProblem(Board board)
	    {
	        this.board = board;
	    }

		// Определяет мат, шах или пат белым.
		public ChessStatus GetChessStatusFor(PieceColor color)
		{
			var isCheck = IsCheckFor(color);
			var hasMoves = HasMoves(color);
			
			if (isCheck)
				return hasMoves ? ChessStatus.Check : ChessStatus.Mate;
			return hasMoves ? ChessStatus.Ok : ChessStatus.Stalemate;
		}

	    private bool HasMoves(PieceColor color)
	    {
	        foreach (var locFrom in board.GetPieces(color))
	        {
	            foreach (var locTo in board.GetPiece(locFrom).GetMoves(locFrom, board))
	            {
	                using (board.PerformTemporaryMove(locFrom, locTo))
	                    return !IsCheckFor(color);
	            }
	        }
	        return false;
	    }

		// check — это шах
		private bool IsCheckFor(PieceColor color)
		{
			foreach (var locFrom in board.GetPieces(GetEnemyColor(color)))
			{
				foreach (var locTo in board.GetPiece(locFrom).GetMoves(locFrom, board))
				{
					if (board.GetPiece(locTo).Is(color, PieceType.King))
						return true;
				}
			}
			return false;
		}

	    private static PieceColor GetEnemyColor(PieceColor color) => 
            color == PieceColor.Black ? PieceColor.White : PieceColor.Black;
	}
}