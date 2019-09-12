using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChLib.Evaluators
{
    public static class SaitekEvaluator
    {
        private const int MAN_VALUE = 1000;
        private const int KING_VALUE = 1700;
        private const int CENTERVALUE = 20;
        private const int SINGLE_CORNER_PENALTY = 30;
        //private const int CENTER = 0x00666600; // square 10,11, 14,15, 17,18, 20,21, 22,23
        //private const int CENTER = 0x06666660; // square 10,11, 14,15, 17,18, 20,21, 22,23 + 6,7, 26,27
        private const int CENTER = 0x66666666; // square 10,11, 14,15, 17,18, 20,21, 22,23 + 6,7, 26,27 + 2,3 + 31,30

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

            //int bp = position.BlackPieces.BitCount();
            //int wp = position.WhitePieces.BitCount();
            //eval += ((bp - wp) * MAN_VALUE) / (bp + wp);
            eval += (position.BlackPieces & CENTER).BitCount() * CENTERVALUE;
            eval -= (position.WhitePieces & CENTER).BitCount() * CENTERVALUE;

            if (((position.BlackPieces & ~position.Kings) & Position.Sq4) != 0)
            {
                eval -= SINGLE_CORNER_PENALTY;
            }

            if (((position.WhitePieces & ~position.Kings) & Position.Sq29) != 0)
            {
                eval += SINGLE_CORNER_PENALTY;
            }

            if (color == MoveGenerator.BLACK)
                return eval;
            return -eval;
        }

    }
}
