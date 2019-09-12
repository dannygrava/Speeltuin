using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChLib.Evaluators
{
    public static class SimpleCheckersEvaluator
    {
        const int CHECKER  =    100; // a checkers' worth
        const int KING     =    130; // a kings' worth
        const int BACKRANK =     10;

        public static int Evaluate(Position p, int color, int alpha, int beta)
        {
            if (p.BlackPieces == 0)
                return color == Search.BLACK ? Search.LOSS : Search.WIN;
            if (p.WhitePieces == 0)
                return color == Search.WHITE ? Search.LOSS : Search.WIN;

            int Score = 0;
            int blackMen = p.BlackPieces & ~p.Kings;
            int blackKings = p.BlackPieces & p.Kings;
            int whiteMen = p.WhitePieces & ~p.Kings;
            int whiteKings = p.WhitePieces & p.Kings;

            int NBMen = blackMen.BitCount(), NWMen = whiteMen.BitCount(), NBKings = blackKings.BitCount(), NWKings = whiteKings.BitCount();
            if ((whiteMen & Position.Sq32) != 0 && (whiteMen & Position.Sq30) != 0 && NBMen > 1)
                Score -= BACKRANK;

            if ((blackMen & Position.Sq1) != 0 && (blackMen & Position.Sq3) != 0 && NWMen > 1)
                Score += BACKRANK;            

            int MaterialBlack = NBKings * KING + NBMen * CHECKER;
            int MaterialWhite = NWKings * KING + NWMen * CHECKER;

            //hints to exchange pieces if ahead in material
            Score += ((MaterialBlack - MaterialWhite) * 200)
                      / (MaterialBlack + MaterialWhite);

            Score += MaterialBlack - MaterialWhite;
            Score *= 10;

            if (color == MoveGenerator.BLACK)
                return Score;
            return -Score;
        }
    }
}
