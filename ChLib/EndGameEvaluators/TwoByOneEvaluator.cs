namespace ChLib.EndGameEvaluators
{
    public class TwoByOneEvaluator
    {
        private const int MAN_VALUE = 1000;
        private const int KING_VALUE = 1300;
        private const int DCORNER_LOSER_BONUS = 20;
        private const int DCORNER_WINNER_BONUS = 5;
        private const int CENTER_WINNER_BONUS = 2;

        private const int DCORNER_BLACK = Position.Sq1 | Position.Sq5;
        private const int DCORNER_WHITE = Position.Sq28 | Position.Sq32;
        private const int DCORNER_EXT_BLACK = Position.Sq1 | Position.Sq5 | Position.Sq2 | Position.Sq6 | Position.Sq9 | Position.Sq10 | Position.Sq14;
        private const int DCORNER_EXT_WHITE = Position.Sq32 | Position.Sq28 | Position.Sq24 | Position.Sq27 | Position.Sq31 | Position.Sq19 | Position.Sq23;
        private const int CENTER = Position.Sq15 | Position.Sq18;


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

            if (bp > wp)
            {
                if ((position.WhitePieces & DCORNER_BLACK)  != 0)
                {
                    eval -= DCORNER_LOSER_BONUS * (position.WhitePieces & DCORNER_BLACK).BitCount();
                    eval +=  DCORNER_WINNER_BONUS * (position.BlackPieces & DCORNER_EXT_BLACK).BitCount();
                }

                if ((position.WhitePieces & DCORNER_WHITE) != 0)
                {
                    eval -= DCORNER_LOSER_BONUS * (position.WhitePieces & DCORNER_WHITE).BitCount(); 
                    eval += DCORNER_WINNER_BONUS * (position.BlackPieces & DCORNER_EXT_WHITE).BitCount();
                }

                eval += CENTER_WINNER_BONUS * (position.BlackPieces & CENTER).BitCount();
            }

            if (bp < wp)
            {
                if ((position.BlackPieces & DCORNER_BLACK) != 0)
                {
                    eval += DCORNER_LOSER_BONUS * (position.BlackPieces & DCORNER_BLACK).BitCount();
                    eval -= DCORNER_WINNER_BONUS * (position.WhitePieces & DCORNER_EXT_BLACK).BitCount();
                }

                if ((position.BlackPieces & DCORNER_WHITE) != 0)
                {
                    eval += DCORNER_LOSER_BONUS * (position.BlackPieces & DCORNER_WHITE).BitCount();
                    eval -= DCORNER_WINNER_BONUS * (position.WhitePieces & DCORNER_EXT_WHITE).BitCount();
                }

                eval -= CENTER_WINNER_BONUS * (position.WhitePieces & CENTER).BitCount();
            }

            eval += ((bp - wp) * MAN_VALUE) / (bp + wp);

            if (color == MoveGenerator.BLACK)
                return eval;
            return -eval;
        }

    }
}
