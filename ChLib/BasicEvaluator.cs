namespace ChLib
{
    public static class BasicEvaluator
    {
        private const int MAN_VALUE = 1000;
        private const int KING_VALUE = 1300;

        public static int Evaluate(Position position, int color, int alpha, int beta)
        {
            if (position.BlackPieces == 0)
                return color == Search.BLACK ? Search.LOSS : Search.WIN;
            if (position.WhitePieces == 0)
                return color == Search.WHITE ? Search.LOSS : Search.WIN;

            int menValue = ((position.BlackPieces & ~position.Kings).BitCount() -
                            (position.WhitePieces & ~position.Kings).BitCount());
            int kingValue = ((position.BlackPieces & position.Kings).BitCount() -
                 (position.WhitePieces & position.Kings).BitCount());
            int eval = MAN_VALUE * menValue + KING_VALUE * kingValue;

            int bp = position.BlackPieces.BitCount();
            int wp = position.WhitePieces.BitCount();
            eval += ((bp - wp) * MAN_VALUE) / (bp + wp);

            if (color == MoveGenerator.BLACK)
                return eval;
            return -eval;
        }

    }

}
