using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChLib.Evaluators
{
    public static class ChkKitEvaluator
    {
        private const int VAL_BRIDGE = 9;
        private const int VAL_OREO = 5;
        private const int VAL_BACK = 3;
        private const int VAL_TRIANGLE = 5;
        private const int VAL_DOG1 = 10;		// Dog hole dcorn..
        private const int VAL_DOG2 = 10;		// Dog hole single..
        private const int valman = 100;               // Material weight..
        private const int valking = 130;

        /// <summary>
        /// Evaluation function from 
        /// CHKKIT.C   (or Adrian Millett's Simple Checkers Program!)
        /// (C) A.Millett 1980-2002. A very simple checkers 
        /// 
        /// It assigns a 2 point bonus to king moves moving away from the back rank: this is not implemented
        /// note this evaluation evaluates the position from the viewpoint of white
        /// </summary>
        /// <param name="p"></param>
        /// <param name="color"></param>
        /// <returns></returns>
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

            int menValue = (whiteMen.BitCount() - blackMen.BitCount());
            int kingValue =(whiteKings.BitCount() - blackKings.BitCount() );
            int thescore = valman * menValue + valking * kingValue;
            // Remember to INCREASE this for (WHITE gain, DECREASE for (BLACK gain.
	        // ie. Add 9 points if (WHITE has the strong back-bridge.

            if ((blackMen & Position.Sq1) != 0)
            {
                if ((blackMen & Position.Sq3) != 0)
                    thescore -= VAL_BRIDGE;
                if ((whiteMen & Position.Sq5) != 0)
                    thescore += VAL_DOG1;
            }

            if ((whiteMen & Position.Sq32)!= 0)
            {
                if ((whiteMen & Position.Sq30) != 0)
                    thescore += VAL_BRIDGE;
                if ((blackMen & Position.Sq28) != 0)
                    thescore -= VAL_BRIDGE;
            }
            //  if (board[1] == BLK_MAN) {
            //    if (board[5] == BLK_MAN) thescore -= VAL_BRIDGE; // BLK BRIDGE squares (1,3)
            //if (board [8] == WHT_MAN) thescore -= VAL_DOG1;		
            //  }
            //  if (board[62] == WHT_MAN) {
            //    if (board[58] == WHT_MAN) thescore += VAL_BRIDGE; // WHT BRIDGE squares (30,32)
            //if (board [55] == BLK_MAN) thescore += VAL_DOG1;		
            //  }

            // Keep Back row
            if ((blackMen & Position.Sq2)!= 0)
            {
                thescore -= VAL_BACK;
                if ((blackMen & (Position.Sq1 | Position.Sq6)) == (Position.Sq1 | Position.Sq6))
                    thescore -= VAL_TRIANGLE;
                if ((blackMen & (Position.Sq3 | Position.Sq7)) == (Position.Sq3 | Position.Sq7))
                    thescore -= VAL_OREO;
            }

            if ((whiteMen & Position.Sq31) != 0)
            {
                thescore += VAL_BACK;
                if ((whiteMen & (Position.Sq32 | Position.Sq27)) == (Position.Sq32 | Position.Sq27))
                    thescore += VAL_TRIANGLE;
                if ((blackMen & (Position.Sq30 | Position.Sq26)) == (Position.Sq30 | Position.Sq26))
                    thescore += VAL_OREO;
            }
    //  if (board[3] == BLK_MAN) {
    //    thescore -= VAL_BACK;
    //if (board[1] == BLK_MAN && board[10] == BLK_MAN) thescore -= VAL_TRIANGLE; // BLK TRIANGLE squares (1,2,6)
    //if (board[5] == BLK_MAN && board[12] == BLK_MAN) thescore -= VAL_OREO; // BLK OREO squares (2,3,7)
    //  }
    //  if (board[60] == WHT_MAN) {
    //    thescore += VAL_BACK;
    //    if (board[62] == WHT_MAN && board[53] == WHT_MAN) thescore += VAL_TRIANGLE; // WHT TRIANGLE squares (31,32,27)
    //    if (board[58] == WHT_MAN && board[51] == WHT_MAN) thescore += VAL_OREO; // WHT OREO squares (30,31,26)
    //  }

            if ((blackKings & Position.Sq29) != 0)
                thescore += 30;// Nullify king in dcorn until escape..
            if ((whiteKings & Position.Sq4) != 0)
                thescore -= 30;// Nullify king in dcorn until escape..

    //  if (board [56] == BLK_KING) thescore += 30;   // Nullify king in dcorn until escape..
    //  if (board [7] == WHT_KING) thescore -= 30;   // Nullify king in dcorn until escape..
            if ((whiteMen & Position.Sq29) != 0)
                thescore -= 4; // Evac sing corn..
            if ((blackMen & Position.Sq4) != 0)
                thescore += 4;

    //  if (board [56] == WHT_MAN) thescore -= 4;   // Evac sing corn..
    //  if (board [7] == BLK_MAN) thescore += 4; 

            // Kings in center
            if ((blackKings & Position.Sq15) != 0)
                thescore -= 4;
            if ((blackKings & Position.Sq18) != 0)
                thescore -= 4;
            if ((whiteKings & Position.Sq15) != 0)
                thescore += 4;
            if ((whiteKings & Position.Sq18) != 0)
                thescore += 4;

    //  if (board[28] == BLK_KING) thescore = thescore - 4;
    //  if (board[35] == BLK_KING) thescore = thescore - 4;
    //  if (board[28] == WHT_KING) thescore = thescore + 4;
    //  if (board[35] == WHT_KING) thescore = thescore + 4;

            // if exch has taken place, exagerate dif to encourage xchs when ahead..
            thescore += (thescore / 16);
    //  if (havetaken) {
    //    thescore += (thescore / 16);
            thescore *= 10;
            if (color == MoveGenerator.WHITE)
                return thescore;
            return -thescore;

        }
    }
}
