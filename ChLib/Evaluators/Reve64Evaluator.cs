using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChLib.Evaluators
{
    /// <summary>
    /// Evaluation function from Reve64.pas ported.
    /// </summary>
    public static class Reve64Evaluator
    {
        private const int MAN_VALUE = 1000;
        private const int KING_VALUE = 1300;
        private const int OREO_BONUS = 100;
        private const int TRIANGLE_BONUS = 60;
        private const int BACKRANK_VALUE = 50;
        private const int BRIDGE_BONUS = 50;
        private const int GOALKEEPER_BONUS = 10;
        private const int DEV_SINGLE_CORNER = 60;
        private const int DOGHOLE = 5;
        private const int CENTER_CONTROL_MEN = 20;
        private const int CENTER_CONTROL_KINGS = 50;
        private const int SINGLE_CORNER_CRAMP = 150;
        private const int DANGLING_PIECE_BONUS = 200;
        private const int DOUBLE_CORNER_CRAMP = 90;
        private const int ENDGAME_TEMPO_MULTIPLIER = 10;

        private const int CENTER_SQUARES = Position.Sq14 | Position.Sq15 | Position.Sq16 | Position.Sq19 | Position.Sq22 | Position.Sq23;

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
            int eval = MAN_VALUE * menValue + KING_VALUE * kingValue;

            int bp = p.BlackPieces.BitCount();
            int wp = p.WhitePieces.BitCount();

            #region Reve64 specific
            // Evaluate Backrank
            eval += (blackMen & (Position.Sq1 | Position.Sq2 | Position.Sq3)).BitCount() * BACKRANK_VALUE;
            eval -= (whiteMen & (Position.Sq32 | Position.Sq31 | Position.Sq30)).BitCount()*BACKRANK_VALUE;

            if ((blackMen & (Position.Sq1 | Position.Sq3)) == (Position.Sq1 | Position.Sq3))
                eval += BRIDGE_BONUS;
            if ((whiteMen & (Position.Sq32 | Position.Sq30)) == (Position.Sq32 | Position.Sq30))
                eval -= BRIDGE_BONUS;

            if ((blackMen & (Position.Sq2)) != 0)
                eval += GOALKEEPER_BONUS;
            if ((whiteMen & (Position.Sq31)) != 0)
                eval -= GOALKEEPER_BONUS;
            // Check developed single corner
            if ((blackMen & (Position.Sq4)) == 0)
                eval += DEV_SINGLE_CORNER;
            if ((whiteMen & (Position.Sq29)) == 0)
                eval -= DEV_SINGLE_CORNER;

            // Check the dog holes
            if ((blackMen & Position.Sq5) != 0 && (p.WhitePieces & Position.Sq1) == 0)
                eval -= DOGHOLE;
            if ((whiteMen & Position.Sq28) != 0 && (p.BlackPieces & Position.Sq32) == 0)
                eval += DOGHOLE;
            if ((blackMen & Position.Sq21) != 0 && (p.WhitePieces & Position.Sq30) != 0)
                eval -= DOGHOLE;
            if ((whiteMen & Position.Sq12) != 0 && (p.BlackPieces & Position.Sq3) != 0)
                eval += DOGHOLE;
            // Evaluate Oreo's triangle
            if ((blackMen & (Position.Sq3 | Position.Sq7)) == (Position.Sq3 | Position.Sq7))
                eval += OREO_BONUS;
            if ((whiteMen & (Position.Sq30 | Position.Sq26)) == (Position.Sq30 | Position.Sq26))
                eval -= OREO_BONUS;
            // Evaluate other triangle
            if ((blackMen & (Position.Sq1 | Position.Sq6)) == (Position.Sq1 | Position.Sq6))
                eval += TRIANGLE_BONUS;
            if ((whiteMen & (Position.Sq32 | Position.Sq27)) == (Position.Sq32 | Position.Sq27))
                eval -= TRIANGLE_BONUS;
            // Evaluate center control with men
            if ((blackMen & Position.Sq14) != 0)
                eval += CENTER_CONTROL_MEN;
            if ((blackMen & Position.Sq15) != 0)
                eval += CENTER_CONTROL_MEN;
            if ((whiteMen & Position.Sq18) != 0)
                eval -= CENTER_CONTROL_MEN;
            if ((whiteMen & Position.Sq19) != 0)
                eval -= CENTER_CONTROL_MEN;
            // Evaluate center control with kings
            eval += ((blackKings & CENTER_SQUARES).BitCount() - (whiteKings & CENTER_SQUARES).BitCount())*
                    CENTER_CONTROL_KINGS;
            // Check single corner cramp
            if ((blackMen & Position.Sq13) != 0 && (whiteMen & (Position.Sq17 | Position.Sq22)) == (Position.Sq17 | Position.Sq22))
            {
                eval += SINGLE_CORNER_CRAMP;
                if ((whiteMen & (Position.Sq21 | Position.Sq25)) == (Position.Sq21 | Position.Sq25) && (whiteMen & Position.Sq29) == 0)
                    eval += DANGLING_PIECE_BONUS;
            }

            if ((whiteMen & Position.Sq20) != 0 && (blackMen & (Position.Sq16 | Position.Sq11)) == (Position.Sq16 | Position.Sq11))
            {
                eval -= SINGLE_CORNER_CRAMP;
                if ((blackMen & (Position.Sq12 | Position.Sq8)) == (Position.Sq12 | Position.Sq8) && (blackMen & Position.Sq5) == 0)
                    eval -= DANGLING_PIECE_BONUS;
            }
            // Double corner cramp
            if ((blackMen & Position.Sq20) != 0 && (whiteMen & (Position.Sq24 | Position.Sq27)) == (Position.Sq24 | Position.Sq27))
            {
                eval += DOUBLE_CORNER_CRAMP;
            }

            if ((whiteMen & Position.Sq13) != 0 && (blackMen & (Position.Sq9 | Position.Sq6)) == (Position.Sq9 | Position.Sq6))
            {
                eval -= DOUBLE_CORNER_CRAMP;
            }
            // Give bonus for development near the end of the game

            if (bp + wp <= 16 && whiteMen != 0 && blackMen != 0)
            {
                int tempo =
                     (blackMen & Position.Rank1).BitCount() - (whiteMen & Position.Rank7).BitCount() +
                2 * ((blackMen & Position.Rank2).BitCount() - (whiteMen & Position.Rank6).BitCount()) +
                3 * ((blackMen & Position.Rank3).BitCount() - (whiteMen & Position.Rank5).BitCount()) +
                4 * ((blackMen & Position.Rank4).BitCount() - (whiteMen & Position.Rank4).BitCount()) +
                5 * ((blackMen & Position.Rank5).BitCount() - (whiteMen & Position.Rank3).BitCount()) +
                6 * ((blackMen & Position.Rank6).BitCount() - (whiteMen & Position.Rank2).BitCount()) +
                7 * ((blackMen & Position.Rank7).BitCount() - (whiteMen & Position.Rank1).BitCount());

                eval += tempo*ENDGAME_TEMPO_MULTIPLIER;
            }
            #endregion

            if (color == MoveGenerator.BLACK)
                return eval;
            return -eval;
        }
    }
}
