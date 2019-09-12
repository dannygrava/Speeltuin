using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChLib.Evaluators
{
    public static class GuiCheckersEvaluator
    {
        const int LAZY_EVAL_MARGIN = 62;
        private const int KV = 33;
        private const int KBoardMinus3 = Position.Sq4 | Position.Sq29;
        private const int KBoard0 = Position.Sq1 | Position.Sq2 | Position.Sq3 | Position.Sq5 | Position.Sq28 |
            Position.Sq30 | Position.Sq31 | Position.Sq32;
        private const int KBoardPlus1 = Position.Sq12 | Position.Sq13 | Position.Sq21 | Position.Sq20;
        private const int KBoardPlus2 = Position.Sq9 | Position.Sq24;
        private const int KBoardPlus3 = Position.Sq6 | Position.Sq7 | Position.Sq27 | Position.Sq26;
        private const int KBoardPlus4 = Position.Sq11 | Position.Sq10 | Position.Sq22 | Position.Sq23;
        private const int KBoardPlus6 = Position.Sq14 | Position.Sq15 | Position.Sq18 | Position.Sq19;
        private const int EBoardMin2 = Position.Sq8 | Position.Sq16 | Position.Sq24 | Position.Sq9 | Position.Sq17 | Position.Sq25;
        private const int EBoardMin3 = Position.Sq4 | Position.Sq12 | Position.Sq20 | Position.Sq28 | Position.Sq5 | Position.Sq13 | Position.Sq21 | Position.Sq29;
        // Bonus for leaving pieces guarding the back row.
        private static int[] BackRowGuard = new int[] { 0, 4, 5, 13, 4, 20, 11, 25 };


        public static int Evaluate(Position p, int color, int alpha, int beta)
        {
            if (p.BlackPieces == 0)
                return color == Search.BLACK ? Search.LOSS : Search.WIN;
            if (p.WhitePieces == 0)
                return color == Search.WHITE ? Search.LOSS : Search.WIN;

            int eval = 0;
            
            int blackMen = p.BlackPieces & ~p.Kings;
            int blackKings = p.BlackPieces & p.Kings;
            int whiteMen = p.WhitePieces & ~p.Kings;
            int whiteKings = p.WhitePieces & p.Kings;
            int free = ~(p.BlackPieces | p.WhitePieces);

            //int bm = (int)blackMen.BitCount(), wm = (int)whiteMen.BitCount(), bk = (int)blackKings.BitCount(), wk = (int)whiteKings.BitCount();

            int nWhite = p.WhitePieces.BitCount();
            int nBlack = p.BlackPieces.BitCount();
            // Number of pieces present. Scaled higher for less material
            if ((nWhite + nBlack) > 12) 
                eval = (nWhite - nBlack) * 100;
            else 
                eval = (nWhite - nBlack) * (160 - (nWhite + nBlack) * 5);
            
            int nPSq = 0;
            if (blackKings != 0)
            {
                nPSq -= (blackKings & KBoardMinus3).BitCount() * -3;
                nPSq -= (blackKings & KBoardPlus1).BitCount();
                nPSq -= (blackKings & KBoardPlus2).BitCount() * 2;
                nPSq -= (blackKings & KBoardPlus3).BitCount() * 3;
                nPSq -= (blackKings & KBoardPlus4).BitCount() * 4;
                nPSq -= (blackKings & KBoardPlus6).BitCount() * 6;
            }
            if (whiteKings != 0) 
            {
                nPSq += (whiteKings & KBoardMinus3).BitCount() * -3;
                nPSq += (whiteKings & KBoardPlus1).BitCount();
                nPSq += (whiteKings & KBoardPlus2).BitCount() * 2;
                nPSq += (whiteKings & KBoardPlus3).BitCount() * 3;
                nPSq += (whiteKings & KBoardPlus4).BitCount() * 4;
                nPSq += (whiteKings & KBoardPlus6).BitCount() * 6;
            }
            if (blackMen != 0) 
            {
                nPSq -= (blackMen & EBoardMin3).BitCount() * -3;
                nPSq -= (blackMen & EBoardMin2).BitCount() * -2;
            }
            if (whiteMen != 0) 
            {
                nPSq += (whiteMen & EBoardMin3).BitCount() * -3;
                nPSq += (whiteMen & EBoardMin2).BitCount() * -2;
            }
            // Add in the Piece Sq table, also includes bonuses for kings
            eval += nPSq;

            // surely winning advantage
            if (nWhite == 1 && nBlack >= 3 && nBlack < 8) 
                eval = eval + (eval >> 1);
            if (nBlack == 1 && nWhite >= 3 && nWhite < 8) 
                eval = eval + (eval >> 1);

            // Too far from the alpha beta window? Forget about the rest of the eval, it probably won't get value back within the window
            // done dg multiply by 10
            // done dg revert depending on color
            int tempEval = color==MoveGenerator.WHITE ? eval * 10: -eval *10;
            if (tempEval + LAZY_EVAL_MARGIN * 10 < alpha)
                return tempEval;
            if (tempEval - LAZY_EVAL_MARGIN * 10> beta)
                return tempEval;

            // The losing side can make it tough to win the game by moving a King back and forth on the double corner squares.
            if (eval > 18)
            {
                if ((blackKings & (Position.Sq1 | Position.Sq5)) != 0)
                {
                    eval -= 8;
                    if ((free & (Position.Sq1 | Position.Sq5)) != 0)
                        eval -= 10;
                }

                if ((blackKings & (Position.Sq32 | Position.Sq28)) != 0)
                {
                    eval -= 8;
                    if ((free & (Position.Sq32 | Position.Sq28)) != 0)
                        eval -= 10;
                }
            }

            if (eval < -18)
            {
                if ((whiteKings & (Position.Sq1 | Position.Sq5)) != 0)
                {
                    eval += 8;
                    if ((free & (Position.Sq1 | Position.Sq5)) != 0)
                        eval += 10;
                }

                if ((whiteKings & (Position.Sq32 | Position.Sq28)) != 0)
                {
                    eval += 8;
                    if ((free & (Position.Sq32 | Position.Sq28)) != 0)
                        eval += 10;
                }
            }
	
            
            // Bonus for leaving pieces guarding the back row.
	        int nBackB = 0, nBackW = 0;
            int nWK = (p.WhitePieces & p.Kings).BitCount();
            int nBK = (p.WhitePieces & p.Kings).BitCount();
            if ((nWK * 2) < nWhite)
            {
                if ((blackMen & Position.Sq3) != 0)
                    nBackB += 1;
                if ((blackMen & Position.Sq2) != 0)
                    nBackB += 2;
                if ((blackMen & Position.Sq1) != 0)
                    nBackB += 4;
                eval -= BackRowGuard[nBackB];
            }
            if ((nBK * 2) < nBlack)
            {
                if ((whiteMen & Position.Sq32) != 0)
                    nBackW += 1;
                if ((whiteMen & Position.Sq31) != 0)
                    nBackW += 2;
                if ((whiteMen & Position.Sq30) != 0)
                    nBackW += 4;
                eval += BackRowGuard[nBackW];
            }
            // Number of Active Kings
            int nAWK = nWK, nABK = nBK;
            // Kings trapped on back row
            if ((whiteKings & Position.Sq4) != 0 && ((p.BlackPieces & Position.Sq3) != 0))
            {
                eval -= 22; nAWK--;
                if ((free & Position.Sq12) == 0) 
                    eval += 7; 
            }
            // if (Sqs [5] == WKING && Sqs[6]  == BPIECE) {eval-=22; nAWK--; if (Sqs[14] != EMPTY) eval+=7;}
            if ((whiteKings & Position.Sq8) != 0 && (free & Position.Sq12) == 0 && (p.BlackPieces & Position.Sq3) != 0)
            {
                eval -= 10; nAWK--; 
            }            
            //if (Sqs[10] == WKING && Sqs[14] != EMPTY && Sqs[6] == BPIECE) { eval -= 10; nAWK--; }
            if ((whiteKings & Position.Sq3) != 0 && (free & Position.Sq11) != 0 && (p.BlackPieces & Position.Sq4) != 0 && (p.BlackPieces & Position.Sq2) != 0)
            {
                eval -= 14; nAWK--; 
            }
            //if (Sqs[6] == WKING && Sqs[15] == EMPTY && Sqs[5] == BPIECE && Sqs[7] == BPIECE) { eval -= 14; nAWK--; }
            if ((whiteKings & Position.Sq2) != 0 && (free & Position.Sq10) != 0 && (p.BlackPieces & Position.Sq3) != 0 && (p.BlackPieces & Position.Sq1) != 0)
            {
                eval -= 14; nAWK--; 
            }
            //if (Sqs[7] == WKING && Sqs[16] == EMPTY && Sqs[6] == BPIECE && Sqs[8] == BPIECE) { eval -= 14; nAWK--; }
            if ((whiteKings & Position.Sq1) != 0 && (free & Position.Sq5) == 0 && (p.BlackPieces & Position.Sq2) != 0 && (free & Position.Sq9) != 0)
            {
                eval -= 16; nAWK--;
            }
            //if (Sqs[8] == WKING && Sqs[13] != EMPTY && Sqs[7] == BPIECE && Sqs[17] == EMPTY) { eval -= 16; nAWK--; }
            if ((blackKings & Position.Sq32) != 0 && (free & Position.Sq28) == 0 && (p.WhitePieces & Position.Sq31) != 0 && (free & Position.Sq24) != 0)
            {
                eval += 16; nABK--; 
            }
            //if (Sqs[37] == BKING && Sqs[32] != EMPTY && Sqs[38] == WPIECE && Sqs[28] == EMPTY) { eval += 16; nABK--; }
            if ((blackKings & Position.Sq31) != 0 && (free & Position.Sq23) != 0 && (p.WhitePieces & Position.Sq30) != 0 && (p.WhitePieces & Position.Sq32) != 0)
            {
                eval += 14; nABK--; 
            }
            //if (Sqs[38] == BKING && Sqs[29] == EMPTY && Sqs[37] == WPIECE && Sqs[39] == WPIECE) { eval += 14; nABK--; }
            if ((blackKings & Position.Sq30) != 0 && (free & Position.Sq22) != 0 && (p.WhitePieces & Position.Sq29) != 0 && (p.WhitePieces & Position.Sq31) != 0)
            {
                eval += 14; nABK--; 
            }
            //if (Sqs[39] == BKING && Sqs[30] == EMPTY && Sqs[40] == WPIECE && Sqs[38] == WPIECE) { eval += 14; nABK--; }
            if ((blackKings & Position.Sq29) != 0 && ((p.WhitePieces & Position.Sq30) != 0))
            {
                eval += 22; nABK--; 
                if ((free & Position.Sq21) == 0)
                    eval -= 7; 
            }
            //if (Sqs[40] == BKING && Sqs[39] == WPIECE) { eval += 22; nABK--; if (Sqs[31] != EMPTY) eval -= 7; }
            if ((blackKings & Position.Sq25) != 0 && (free & Position.Sq21) == 0 && (p.WhitePieces & Position.Sq30) != 0)
            {
                eval += 10; nABK--; 
            }
            //if (Sqs[35] == BKING && Sqs[39] == WPIECE && Sqs[31] != EMPTY) { eval += 10; nABK--; }

            // Reward checkers that will king on the next move
            // note dg: rewritten for bitboards
            const int rank7 = Position.Sq25 | Position.Sq26 | Position.Sq27 | Position.Sq28;
            const int maskShl5 = Position.Sq25 | Position.Sq26 | Position.Sq27;
            const int rank2 = Position.Sq5 | Position.Sq6 | Position.Sq7 | Position.Sq8;
            const int maskShr5 = Position.Sq6 | Position.Sq7 | Position.Sq8;
            int possibleBlackPromotions = (((blackMen & rank7) << 4) | ((blackMen & rank7 & maskShl5) << 5) & free);

            if (possibleBlackPromotions != 0)
            {
                // note dg: this looks a bit odd, one would expect that if two pieces are able to promote, then the would both get a reward
                // perhaps it was the intention, but it was not how it was implemented
                if (possibleBlackPromotions == Position.Sq29)
                    eval -= KV - 5 - 3; // note dg 29 has less value
                else
                    eval -= KV - 5;                
            }
            
            // Opponent has no Active Kings and backrow is still protected, so give a bonus
            if (nAWK > 0 && (possibleBlackPromotions & ~Position.Sq29) == 0 && nABK == 0)
            {
                eval += 20;
                if (nAWK > 1) 
                    eval += 10;
                if (BackRowGuard[nBackW] > 10)
                    eval += 15;
                if (BackRowGuard[nBackW] > 20)
                    eval += 15;
            }
            //if (nAWK > 0 && (square == 36 || KSQ == 40) && nABK == 0)
            //{
            //    eval += 20;
            //    if (nAWK > 1) eval += 10;
            //    if (BackRowGuard[nBackW] > 10) eval += 15;
            //    if (BackRowGuard[nBackW] > 20) eval += 20;
            //}

            // Now the same for the other color
            int possibleWhitePromotions = (int) (((uint)(whiteMen & rank2) >> 4) | ((uint) (whiteMen & rank2 & maskShr5) >> 5) & free);
            if (possibleWhitePromotions != 0)
            {
                if (possibleWhitePromotions == Position.Sq4)
                    eval += KV - 5 - 3; // note dg 4 has less value
                else
                    eval += KV - 5;
            }

            if (nABK > 0 && (possibleWhitePromotions & ~Position.Sq4) == 0 && nAWK == 0)
            {
                eval -= 20;
                if (nABK > 1)
                    eval -= 10; // note dg bug? shouldn't this be -= 10? Yup see version 105 
                if (BackRowGuard[nBackW] > 10)
                    eval -= 15;
                if (BackRowGuard[nBackW] > 20)
                    eval -= 15;
            }
            //if (nABK > 0 && (square == 14 || KSQ == 5) && nAWK == 0)
            //{
            //    eval -= 20;
            //    if (nABK > 1) eval += 10;
            //    if (BackRowGuard[nBackB] > 10) eval -= 15;
            //    if (BackRowGuard[nBackB] > 20) eval -= 20;
            //}

            // lower score, if equal number of pieces, but one side has all kinged versus all but one kinged (or all kings)
            if (nWhite == nBlack)
            {
                if (eval > 0 && eval < 200 && nWK >= nBK && nBK >= nWhite - 1) eval = (eval >> 1) + (eval >> 3);
                if (eval < 0 && eval > -200 && nBK >= nWK && nWK >= nBlack - 1) eval = (eval >> 1) + (eval >> 3);
            }

            //if (nWhite == nBlack)
            //{
            //    if (eval > 0 && eval < 200 && nWK >= nBK && nBK >= nWhite - 1) eval = (eval >> 1) + (eval >> 3);
            //    if (eval < 0 && eval > -200 && nBK >= nWK && nWK >= nBlack - 1) eval = (eval >> 1) + (eval >> 3);
            //}


            eval *= 10;
            if (color == MoveGenerator.WHITE)
                return eval;
            return -eval;
        }

        /*      37  38  39  40
              32  33  34  35
                28  29  30  31
              23  24  25  26
                19  20  21  22
              14  15  16  17
                10  11  12  13
               5   6   7   8         */

//        #define PD 0
//const int KV = 33;
//const int KBoard[64] = { PD, PD, PD, PD, PD, //0..4
//                         KV-3,  KV+0,  KV+0,  KV+0, PD,//5..9
//                            KV+2,  KV+3,  KV+3,  KV+0,//10..13
//                          KV+1,  KV+4,  KV+4,  KV+3, PD,//14..18
//                            KV+4,  KV+6,  KV+6,  KV+1,//19..22
//                          KV+1,  KV+6,  KV+6,  KV+4, PD,//23..27
//                            KV+3,  KV+4,  KV+4,  KV+1,//28..31
//                          KV+0,  KV+3,  KV+3,  KV+2, PD,//32..36
//                            KV+0,  KV+0,  KV+0, KV-3};//37..40
//const int EBoard[64] = { PD, PD, PD, PD, PD,//0..4
//                         -3,  0,  0, 0, PD,//5..9
//                           -2,  0,  0,  -3,//10..13
//                         -3,  0,  0, -2, PD,//14..18
//                           -2,  0,  0,  -3,//19..22
//                         -3,  0,  0, -2, PD,//23..27
//                           -2,  0,  0,  -3,//28..31
//                         -3,  0,  0, -2, PD,//32..36
//                            0,  0,  0, -3};//37..40

    }
}
