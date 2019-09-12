using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChLib.Evaluators
{
// ReSharper disable InconsistentNaming
    public static class ICheckersEvaluator
// ReSharper restore InconsistentNaming
    {
        public const int PIECE_WEIGHT = 100;
        public const int KING_WEIGHT = 150;
        public const int TURN_WEIGHT = 5;
        public const int BACK_RANK = 10;
        public const int FRONT_RANK_ROW4 = 2;
        public const int FRONT_RANK_ROW5 = 5;
        public const int FRONT_RANK_ROW6 = 7;
        public const int FRONT_RANK_ROW7 = 10;
        public static int Evaluate(Position p, int color, int alpha, int beta)
        {
            if (p.BlackPieces == 0)
                return color == Search.BLACK ? Search.LOSS : Search.WIN;
            if (p.WhitePieces == 0)
                return color == Search.WHITE ? Search.LOSS : Search.WIN;

            int blackMen = p.BlackPieces & ~p.Kings;
            int blackKings = p.BlackPieces & p.Kings;
            int whiteMen = p.WhitePieces & ~p.Kings;
            int whiteKings = p.WhitePieces & p.Kings;

            int menValue = (blackMen.BitCount() - whiteMen.BitCount());
            int kingValue = (blackKings.BitCount() - whiteKings.BitCount());
            int eval = PIECE_WEIGHT * menValue + KING_WEIGHT * kingValue;

            int bp = p.BlackPieces.BitCount();
            int wp = p.WhitePieces.BitCount();
            eval += ((bp - wp) * PIECE_WEIGHT) / (bp + wp);

            #region ICheckers specific
            eval += (blackMen & Position.Rank1).BitCount() * BACK_RANK;
            eval -= (whiteMen & Position.Rank2).BitCount() * FRONT_RANK_ROW7;
            eval -= (whiteMen & Position.Rank3).BitCount() * FRONT_RANK_ROW6;
            eval += (blackMen & Position.Rank4).BitCount() * FRONT_RANK_ROW4;
            eval -= (whiteMen & Position.Rank4).BitCount() * FRONT_RANK_ROW5;
            eval += (blackMen & Position.Rank5).BitCount() * FRONT_RANK_ROW5;
            eval -= (whiteMen & Position.Rank5).BitCount() * FRONT_RANK_ROW4;
            eval += (blackMen & Position.Rank6).BitCount() * FRONT_RANK_ROW6;
            eval += (blackMen & Position.Rank7).BitCount() * FRONT_RANK_ROW7;
            eval -= (whiteMen & Position.Rank8).BitCount() * BACK_RANK;

            if (color == MoveGenerator.BLACK)
                eval += TURN_WEIGHT;
            else
                eval -= TURN_WEIGHT;

            eval *= 10;
            #endregion

            if (color == MoveGenerator.BLACK)
                return eval;
            return -eval;
        }


    }
}
