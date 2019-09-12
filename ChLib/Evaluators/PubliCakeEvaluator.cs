using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChLib.Evaluators
{
    /// <summary>
    /// Evaluation function from  Martin Fierz's PubliCake v0.2 from Sourceforge
    /// </summary>
    public static class PubliCakeEvaluator
    {
        private const int DEVSINGLECORNER = 4;
        private const int OREO = 10;
        private const int IDEALBACKRANK = 20;
        private const int BRIDGEBACKRANK = 15;
        private const int GOODBACKRANK = 10;
        private const int GOALKEEPERBACKRANK = 5;
        private const int KINGCENTERVALUE = 5;
        private const int OPENINGTEMPO = -2;
        private const int ENDGAMETEMPO = 2;
        private const int DOGHOLE = 5;
        private const int SINGLEROAMINGKING = 90;
        private const int TRAPPEDKINGSC = 30;
        private const int CENTER = 0x00666600; // square 10,11, 14,15, 17,18, 20,21, 22,23

        static PubliCakeEvaluator()
        {
            initeval();
        }

        public static int Evaluate(Position p, int color, int alpha, int beta)
        {
            if (p.BlackPieces == 0)
                return color == Search.BLACK ? Search.LOSS : Search.WIN;
            if (p.WhitePieces == 0)
                return color == Search.WHITE ? Search.LOSS : Search.WIN;

            int value = 0;
            int tempo = 0;

            int blackMen = p.BlackPieces & ~p.Kings;
            int blackKings = p.BlackPieces & p.Kings;
            int whiteMen = p.WhitePieces & ~p.Kings;
            int whiteKings = p.WhitePieces & p.Kings;

            int bm = blackMen.BitCount(), wm = whiteMen.BitCount(), bk = blackKings.BitCount(), wk = whiteKings.BitCount();
            int free = ~(p.BlackPieces | p.WhitePieces);

            // material ------------------------------------------

            value += 100 * (bm - wm);
            value += 130 * (bk - wk);

            // exchange when you have more:
            value += ((bm + bk - wm - wk) * 100) / (bm + bk + wm + wk);


            // positional stuff ----------------------------------

            // back rank
            value += _backrankblack[blackMen & 0xFF];
            value -= _backrankwhite[((uint) whiteMen >> 24) & 0xFF];

            // tempo
            if ((bk + wk)== 0  && bm == wm)
            {
                tempo += (blackMen & 0x0FFFFFF0).BitCount();
                tempo += (blackMen & 0x0FFFFF00).BitCount();
                tempo += (blackMen & 0x0FFFF000).BitCount(); 
                tempo += (blackMen & 0x0FFF0000).BitCount(); 
                tempo += (blackMen & 0x0FF00000).BitCount(); 
                tempo += (blackMen & 0x0F000000).BitCount(); 

                tempo -= (whiteMen & 0x0FFFFFF0).BitCount(); 
                tempo -= (whiteMen & 0x00FFFFF0).BitCount(); 
                tempo -= (whiteMen & 0x000FFFF0).BitCount(); 
                tempo -= (whiteMen & 0x0000FFF0).BitCount(); 
                tempo -= (whiteMen & 0x00000FF0).BitCount(); 
                tempo -= (whiteMen & 0x000000F0).BitCount(); 

                if (bm + bk >= 18)
                    value += OPENINGTEMPO * tempo;

                if (bm + bk <= 10)
                    value += ENDGAMETEMPO * tempo;

            }

            // cramps

            // center control

            // doghole
            if ((blackMen & Position.Sq1)!= 0 && (whiteMen & Position.Sq5) != 0)
                value += DOGHOLE;
            if ((blackMen & Position.Sq3) != 0 && (whiteMen & Position.Sq12) != 0)
                value += DOGHOLE;
            if ((whiteMen & Position.Sq32) != 0 && (blackMen & Position.Sq28) != 0)
                value -= DOGHOLE;
            if ((whiteMen & Position.Sq30) != 0 && (blackMen & Position.Sq21) != 0)
                value -= DOGHOLE;

            // runaway men

            // king centralization
            value += (blackKings & CENTER).BitCount() * KINGCENTERVALUE;
            value -= (whiteKings & CENTER).BitCount() * KINGCENTERVALUE;

            // trapped kings
            // in single corner:
            if ((blackKings & Position.Sq29) != 0 && (whiteMen & Position.Sq30) != 0 && (free & Position.Sq21) != 0)
                value -= TRAPPEDKINGSC;
            if ((whiteKings & Position.Sq4) != 0 && (blackMen & Position.Sq3) != 0 && (free & Position.Sq12) != 0)
                value += TRAPPEDKINGSC;


            // single roaming king
            if (bk != 0 && wk==0)
            {
                if ((blackKings & CENTER) != 0 && ((blackMen & (Position.Sq1 | Position.Sq2 | Position.Sq3)) == (Position.Sq1 | Position.Sq2 | Position.Sq3)))
                    value += SINGLEROAMINGKING;
            }
            if (wk != 0 && bk==0)
            {
                if ((whiteKings & CENTER) != 0 && ((whiteMen & (Position.Sq30 | Position.Sq31 | Position.Sq32)) == (Position.Sq30 | Position.Sq31 | Position.Sq32)))
                    value -= SINGLEROAMINGKING;
            }

            value *= 10;
            if (color == MoveGenerator.BLACK)
                return value;
            return -value;
        }

        private static readonly int[] _backrankblack = new int[256];
        private static readonly int[] _backrankwhite = new int[256];

        private static void initeval()
        {
            // evaluation uses some tables to look up values which are initialized here
            int i, j;
            int eval;


            // init backrankblack
            for (i = 0; i < 256; i++)
            {
                eval = 0;
                // imagine black pieces set up as i and evaluate them

                // developed single corner
                if ((~i & Position.Sq4) != 0 && (~i & Position.Sq8) != 0)
                    eval += DEVSINGLECORNER;

                // oreo
                if ((i & Position.Sq2) != 0  && (i & Position.Sq3) != 0 && (i & Position.Sq7) != 0)
                    eval += OREO;

                // ideal back rank
                if ((i & Position.Sq1) != 0 && (i & Position.Sq2) != 0 && (i & Position.Sq3) != 0)
                    eval += IDEALBACKRANK;

                // bridge back rank
                if ((i & Position.Sq1) != 0 && (~i & Position.Sq2) != 0 && (i & Position.Sq3) != 0)
                    eval += BRIDGEBACKRANK;

                // good back rank
                if ((i & Position.Sq2) != 0 && (i & Position.Sq3) != 0 && (~i & Position.Sq1) != 0)
                    eval += GOODBACKRANK;

                // goalkeeper-piece - if only one piece is left on the backrank, this is the best
                if ((i & Position.Sq2) != 0)
                    eval += GOALKEEPERBACKRANK;

                _backrankblack[i] = eval;
            }

            // init backrankwhite
            for (j = 0; j < 256; j++)
            {
                eval = 0;
                // imagine white pieces set up as j<<24 and evaluate them
                i = j << 24;

                // developed single corner
                if ((~i & Position.Sq25) != 0 && (~i & Position.Sq29) != 0)
                    eval += DEVSINGLECORNER;

                // oreo
                if ((i & Position.Sq31) != 0 && (i & Position.Sq26) != 0 && (i & Position.Sq30) != 0)
                    eval += OREO;

                // ideal back rank
                if ((i & Position.Sq30) != 0 && (i & Position.Sq31) != 0 && (i & Position.Sq32) != 0)
                    eval += IDEALBACKRANK;

                // bridge back rank
                if ((i & Position.Sq32) != 0 && (~i & Position.Sq31) != 0 && (i & Position.Sq30) != 0)
                    eval += BRIDGEBACKRANK;

                // good back rank
                if ((i & Position.Sq30) != 0 && (i & Position.Sq31) != 0 && (~i & Position.Sq32) != 0)
                    eval += GOODBACKRANK;

                // goalkeeper-piece - if only one piece is left on the backrank, this is the best
                if ((i & Position.Sq31) != 0)
                    eval += GOALKEEPERBACKRANK;

                _backrankwhite[j] = eval;
            }
        }
    }
}


