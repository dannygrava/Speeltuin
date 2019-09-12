using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChLib
{
    // note dg: right shift op een int met bit 32 set (sign bit) geeft een ander resultaat dan right shift op een uint met bit 32 set
    // If the first operand is an int or long, the right-shift is an arithmetic shift (high-order empty bits are set to the sign bit). If the first operand is of type uint or ulong, the right-shift is a logical shift (high-order bits are zero-filled).
    // Zie ook: http://msdn.microsoft.com/en-US/library/xt18et0d%28v=VS.80%29.aspx
    public static class MoveGenerator
    {
        public const int WHITE = 1;
        public const int BLACK = 0;
        public const int MAX_LEGAL_MOVES = 28;
        public const int KINGS_ROW_WHITE = Position.Sq1 | Position.Sq2 | Position.Sq3 | Position.Sq4;
        public const int KINGS_ROW_BLACK = Position.Sq29 | Position.Sq30 | Position.Sq31 | Position.Sq32;

        private const int _maskShl3 = Position.Sq6 | Position.Sq7 | Position.Sq8 | Position.Sq14 | Position.Sq15 | Position.Sq16 | Position.Sq22 | Position.Sq23 | Position.Sq24;
        private const int _maskShl5 = Position.Sq1 | Position.Sq2 | Position.Sq3 | Position.Sq9 | Position.Sq10 | Position.Sq11 | Position.Sq17 | Position.Sq18 | Position.Sq19 | Position.Sq25 | Position.Sq26 | Position.Sq27;
        private const int _maskShr3 = Position.Sq27 | Position.Sq26 | Position.Sq25 | Position.Sq19 | Position.Sq18 | Position.Sq17 | Position.Sq11 | Position.Sq10 | Position.Sq9;
        private const int _maskShr5 = Position.Sq32 | Position.Sq31 | Position.Sq30 | Position.Sq24 | Position.Sq23 | Position.Sq22 | Position.Sq16 | Position.Sq15 | Position.Sq14 | Position.Sq8 | Position.Sq7 | Position.Sq6;

        // to avoid the use of ref parametes ivm java-port
        private static int _moveIndex;
        private static Position[] _moves;

        public static int GenerateMoves(ref Position p, Position[] moves, int color)
        {
            _moves = moves;
            int numMoves = 0;
            if (color == WHITE)
            {
                numMoves = generateWhiteJumps(ref p);
                return numMoves == 0 ? generateWhiteMoves(ref p) : numMoves;
            }

            if (color == BLACK)
            {
                numMoves = generateBlackJumps(ref p);
                return numMoves == 0 ? generateBlackMoves(ref p) : numMoves;
            }
            return numMoves;
        }

        private static int generateWhiteMoves(ref Position position)
        {
            _moveIndex = 0;
            int whitepieces = position.WhitePieces;
            int notOccupied = ~(position.WhitePieces | position.BlackPieces);

            while (whitepieces != 0)
            {
                //uint moveFrom = Bits.FindHighBit(whitepieces);
                int moveFrom = whitepieces & -whitepieces;
                int moveTo = (int)((uint)moveFrom >> 4) & notOccupied;
                if (moveTo != 0)
                {
                    _moves[_moveIndex].WhitePieces = (position.WhitePieces & ~moveFrom) | moveTo;
                    // note dg: adjust hashkey here
                    if (((position.Kings & moveFrom) != 0) || (moveTo & KINGS_ROW_WHITE) != 0)
                    {
                        _moves[_moveIndex].Kings = (position.Kings & ~moveFrom) | moveTo;
                        // note dg: adjust hashkey here
                    }
                    else
                    {
                        _moves[_moveIndex].Kings = position.Kings;
                    }
                    _moves[_moveIndex].BlackPieces = position.BlackPieces;
                    _moveIndex++;
                }

                moveTo = (int)(((uint)(moveFrom & _maskShr3) >> 3 & notOccupied) | (uint)(moveFrom & _maskShr5) >> 5 & notOccupied);
                if (moveTo != 0)
                {
                    _moves[_moveIndex].WhitePieces = (position.WhitePieces & ~moveFrom) | moveTo;
                    // note dg: adjust hashkey here
                    if (((position.Kings & moveFrom) != 0) || (moveTo & KINGS_ROW_WHITE) != 0)
                    {
                        _moves[_moveIndex].Kings = (position.Kings & ~moveFrom) | moveTo;
                        // note dg: adjust hashkey here
                    }
                    else
                    {
                        _moves[_moveIndex].Kings = position.Kings;
                    }
                    _moves[_moveIndex].BlackPieces = position.BlackPieces;
                    _moveIndex++;
                }

                if ((moveFrom & position.Kings) != 0)
                {
                    moveTo = moveFrom << 4 & notOccupied;
                    if (moveTo != 0)
                    {
                        _moves[_moveIndex].WhitePieces = (position.WhitePieces & ~moveFrom) | moveTo;
                        _moves[_moveIndex].Kings = (position.Kings & ~moveFrom) | moveTo;
                        _moves[_moveIndex].BlackPieces = position.BlackPieces;
                        _moveIndex++;
                    }

                    moveTo = ((moveFrom & _maskShl3) << 3 & notOccupied) | ((moveFrom & _maskShl5) << 5 & notOccupied);
                    if (moveTo != 0)
                    {
                        _moves[_moveIndex].WhitePieces = (position.WhitePieces & ~moveFrom) | moveTo;
                        _moves[_moveIndex].Kings = (position.Kings & ~moveFrom) | moveTo;
                        _moves[_moveIndex].BlackPieces = position.BlackPieces;
                        _moveIndex++;
                    }
                }
                //whitepieces = Bits.ClearHighBit(whitepieces);
                whitepieces = whitepieces & (whitepieces - 1);
            }
            return _moveIndex;
        }

        private static int generateBlackMoves(ref Position position)
        {
            _moveIndex = 0;


            int blackPieces = position.BlackPieces;
            int notOccupied = ~(position.WhitePieces | position.BlackPieces);
            while (blackPieces != 0)
            {
                //FindHighBit
                int moveFrom = blackPieces & -blackPieces;
                int moveTo = moveFrom << 4 & notOccupied;
                if (moveTo != 0)
                {
                    _moves[_moveIndex].BlackPieces = (position.BlackPieces & ~moveFrom) | moveTo;
                    if (((position.Kings & moveFrom) != 0) || (moveTo & KINGS_ROW_BLACK) != 0)
                    {
                        _moves[_moveIndex].Kings = (position.Kings & ~moveFrom) | moveTo;
                    }
                    else
                    {
                        _moves[_moveIndex].Kings = position.Kings;
                    }

                    _moves[_moveIndex].WhitePieces = position.WhitePieces;
                    _moveIndex++;
                }

                moveTo = ((moveFrom & _maskShl3) << 3 & notOccupied) | ((moveFrom & _maskShl5) << 5 & notOccupied);
                if (moveTo != 0)
                {
                    _moves[_moveIndex].BlackPieces = (position.BlackPieces & ~moveFrom) | moveTo;
                    if (((position.Kings & moveFrom) != 0) || (moveTo & KINGS_ROW_BLACK) != 0)
                    {
                        _moves[_moveIndex].Kings = (position.Kings & ~moveFrom) | moveTo;
                    }
                    else
                    {
                        _moves[_moveIndex].Kings = position.Kings;
                    }
                    _moves[_moveIndex].WhitePieces = position.WhitePieces;
                    _moveIndex++;
                }

                if ((moveFrom & position.Kings) != 0)
                {
                    moveTo = (int)((uint)moveFrom >> 4 & notOccupied);
                    if (moveTo != 0)
                    {
                        _moves[_moveIndex].BlackPieces = (position.BlackPieces & ~moveFrom) | moveTo;
                        _moves[_moveIndex].Kings = (position.Kings & ~moveFrom) | moveTo;
                        _moves[_moveIndex].WhitePieces = position.WhitePieces;
                        _moveIndex++;
                    }

                    moveTo =
                        (int)
                        (((uint)(moveFrom & _maskShr3) >> 3 & notOccupied) |
                         ((uint)(moveFrom & _maskShr5) >> 5 & notOccupied));
                    if (moveTo != 0)
                    {
                        _moves[_moveIndex].BlackPieces = (position.BlackPieces & ~moveFrom) | moveTo;
                        _moves[_moveIndex].Kings = (position.Kings & ~moveFrom) | moveTo;
                        _moves[_moveIndex].WhitePieces = position.WhitePieces;
                        _moveIndex++;
                    }

                }
                //blackPieces = Bits.ClearHighBit(blackPieces);
                blackPieces = blackPieces & (blackPieces - 1);
            }
            return _moveIndex;
        }

        private static int generateBlackJumps(ref Position position)
        {
            _moveIndex = 0;

            int blackPieces = position.BlackPieces;
            while (blackPieces != 0)
            {
                //uint moveFrom = Bits.FindHighBit(blackPieces);
                int moveFrom = blackPieces & -blackPieces;
                doGenerateBlackJumps(ref position, moveFrom);
                //blackPieces = Bits.ClearHighBit(blackPieces);
                blackPieces = blackPieces & (blackPieces - 1);
            }
            return _moveIndex;
        }

        private static bool doGenerateBlackJumps(ref Position position, int moveFrom)
        {
            bool jumpsFound = false;
            int whitePieces = position.WhitePieces;
            int free = ~(position.BlackPieces | whitePieces);
            bool crowns;

            int next = moveFrom << 4 & whitePieces;
            if (next != 0)
            {
                int moveTo = ((next & _maskShl3) << 3 | (next & _maskShl5) << 5) & free;
                if (moveTo != 0)
                {
                    Position tempPosition = position;
                    executeBlackJump(ref tempPosition, moveFrom, next, moveTo, out crowns);
                    if (crowns || !doGenerateBlackJumps(ref tempPosition, moveTo))
                        backupMove(ref tempPosition);
                    jumpsFound = true;
                }
            }

            next = ((moveFrom & _maskShl3) << 3 | (moveFrom & _maskShl5) << 5) & whitePieces;
            if (next != 0)
            {
                int moveTo = next << 4 & free;
                if (moveTo != 0)
                {
                    Position tempPosition = position;
                    executeBlackJump(ref tempPosition, moveFrom, next, moveTo, out crowns);
                    if (crowns || !doGenerateBlackJumps(ref tempPosition, moveTo))
                        backupMove(ref tempPosition);
                    jumpsFound = true;
                }
            }

            if ((moveFrom & position.Kings) != 0)
            {
                next = (int)((uint)moveFrom >> 4 & whitePieces);
                if (next != 0)
                {
                    int moveTo = (int)((uint)(next & _maskShr3) >> 3 | (uint)(next & _maskShr5) >> 5) & free;
                    if (moveTo != 0)
                    {
                        Position tempPosition = position;
                        executeBlackJump(ref tempPosition, moveFrom, next, moveTo, out crowns);
                        if (crowns || !doGenerateBlackJumps(ref tempPosition, moveTo))
                            backupMove(ref tempPosition);
                        jumpsFound = true;
                    }
                }

                next = (int)((uint)(moveFrom & _maskShr3) >> 3 | (uint)(moveFrom & _maskShr5) >> 5) & whitePieces;
                if (next != 0)
                {
                    int moveTo = (int)((uint)next >> 4 & free);
                    if (moveTo != 0)
                    {
                        Position tempPosition = position;
                        executeBlackJump(ref tempPosition, moveFrom, next, moveTo, out crowns);
                        if (crowns || !doGenerateBlackJumps(ref tempPosition, moveTo))
                            backupMove(ref tempPosition);
                        jumpsFound = true;
                    }
                }
            }
            return jumpsFound;
        }

        private static void backupMove(ref Position position)
        {
            _moves[_moveIndex].BlackPieces = position.BlackPieces;
            _moves[_moveIndex].WhitePieces = position.WhitePieces;
            _moves[_moveIndex].Kings = position.Kings;
            _moveIndex++;
        }

        private static void executeBlackJump(ref Position position, int moveFrom, int next, int moveTo, out bool crowns)
        {
            crowns = (position.Kings & moveFrom) == 0 && (moveTo & KINGS_ROW_BLACK) != 0;
            position.BlackPieces = (position.BlackPieces & ~moveFrom) | moveTo;
            position.WhitePieces = (position.WhitePieces & ~next);
            position.Kings = (position.Kings & ~next);
            if (((position.Kings & moveFrom) != 0) || (moveTo & KINGS_ROW_BLACK) != 0)
            {
                position.Kings = (position.Kings & ~moveFrom) | moveTo;
            }
        }

        private static int generateWhiteJumps(ref Position position)
        {
            _moveIndex = 0;
            int whitePieces = position.WhitePieces;
            while (whitePieces != 0)
            {
                //int moveFrom = Bits.FindHighBit(whitePieces);
                int moveFrom = whitePieces & -whitePieces;
                doGenerateWhiteJumps(ref position, moveFrom);
                //whitePieces = Bits.ClearHighBit(whitePieces);
                whitePieces = whitePieces & (whitePieces - 1);
            }
            return _moveIndex;
        }

        private static bool doGenerateWhiteJumps(ref Position position, int moveFrom)
        {
            bool jumpsFound = false;
            int blackPieces = position.BlackPieces;
            int free = ~(position.WhitePieces | blackPieces);
            bool crowns;

            int next = (int)((uint)moveFrom >> 4 & blackPieces);
            if (next != 0)
            {
                int moveTo = (int)((uint)(next & _maskShr3) >> 3 | (uint)(next & _maskShr5) >> 5) & free;
                if (moveTo != 0)
                {
                    Position tempPosition = position;
                    executeWhiteJump(ref tempPosition, moveFrom, next, moveTo, out crowns);
                    if (crowns || !doGenerateWhiteJumps(ref tempPosition, moveTo))
                        backupMove(ref tempPosition);
                    jumpsFound = true;
                }
            }

            next = (int)((uint)(moveFrom & _maskShr3) >> 3 | (uint)(moveFrom & _maskShr5) >> 5) & blackPieces;
            if (next != 0)
            {
                int moveTo = (int)((uint)next >> 4 & free);
                if (moveTo != 0)
                {
                    Position tempPosition = position;
                    executeWhiteJump(ref tempPosition, moveFrom, next, moveTo, out crowns);
                    if (crowns || !doGenerateWhiteJumps(ref tempPosition, moveTo))
                        backupMove(ref tempPosition);
                    jumpsFound = true;
                }
            }

            if ((moveFrom & position.Kings) != 0)
            {
                next = moveFrom << 4 & blackPieces;
                if (next != 0)
                {
                    int moveTo = ((next & _maskShl3) << 3 | (next & _maskShl5) << 5) & free;
                    if (moveTo != 0)
                    {
                        Position tempPosition = position;
                        executeWhiteJump(ref tempPosition, moveFrom, next, moveTo, out crowns);
                        if (crowns || !doGenerateWhiteJumps(ref tempPosition, moveTo))
                            backupMove(ref tempPosition);
                        jumpsFound = true;
                    }
                }

                next = ((moveFrom & _maskShl3) << 3 | (moveFrom & _maskShl5) << 5) & blackPieces;
                if (next != 0)
                {
                    int moveTo = next << 4 & free;
                    if (moveTo != 0)
                    {
                        Position tempPosition = position;
                        executeWhiteJump(ref tempPosition, moveFrom, next, moveTo, out crowns);
                        if (crowns || !doGenerateWhiteJumps(ref tempPosition, moveTo))
                            backupMove(ref tempPosition);
                        jumpsFound = true;
                    }
                }
            }
            return jumpsFound;
        }

        private static void executeWhiteJump(ref Position position, int moveFrom, int next, int moveTo, out bool crowns)
        {
            crowns = (position.Kings & moveFrom) == 0 && (moveTo & KINGS_ROW_WHITE) != 0;
            position.WhitePieces = (position.WhitePieces & ~moveFrom) | moveTo;
            position.BlackPieces = (position.BlackPieces & ~next);
            position.Kings = (position.Kings & ~next);
            if (((position.Kings & moveFrom) != 0) || (moveTo & KINGS_ROW_WHITE) != 0)
            {
                position.Kings = (position.Kings & ~moveFrom) | moveTo;
            }
        }

        public static int GetJumpersWhite(Position p)
        {
            int free = ~(p.WhitePieces | p.BlackPieces);
            int whiteKings = p.WhitePieces & p.Kings;
            int jumpers = 0;
            int next = (free << 4) & p.BlackPieces;
            if (next != 0)
                jumpers |= (((next & _maskShl3) << 3) | ((next & _maskShl5) << 5)) & p.WhitePieces;

            next = (((free & _maskShl3) << 3) | ((free & _maskShl5) << 5)) & p.BlackPieces;
            jumpers |= (next << 4) & p.WhitePieces;

            if (whiteKings != 0)
            {
                next = (int)((uint)free >> 4) & p.BlackPieces;
                if (next != 0)
                    jumpers |= (int)(((uint)(next & _maskShr3) >> 3) | ((uint)(next & _maskShr5) >> 5)) & whiteKings;
                next = (int)(((uint)(free & _maskShr3) >> 3) | ((uint)(free & _maskShr5) >> 5)) & p.BlackPieces;
                if (next != 0)
                    jumpers |= (int)((uint)next >> 4) & whiteKings;
            }
            return jumpers;
        }

        public static int GetJumpersBlack(Position p)
        {
            int free = ~(p.WhitePieces | p.BlackPieces);
            int blackKings = p.BlackPieces & p.Kings;
            int jumpers = 0;
            int next = (int)((uint)free >> 4) & p.WhitePieces;
            if (next != 0)
                jumpers |= (int)(((uint)(next & _maskShr3) >> 3) | ((uint)(next & _maskShr5) >> 5)) & p.BlackPieces;

            next = (int)(((uint)(free & _maskShr3) >> 3) | ((uint)(free & _maskShr5) >> 5)) & p.WhitePieces;
            jumpers |= (int)((uint)next >> 4) & p.BlackPieces;

            if (blackKings != 0)
            {
                next = (free << 4) & p.WhitePieces;
                if (next != 0)
                    jumpers |= (((next & _maskShl3) << 3) | ((next & _maskShl5) << 5)) & blackKings;
                next = (((free & _maskShl3) << 3) | ((free & _maskShl5) << 5)) & p.WhitePieces;
                if (next != 0)
                    jumpers |= (next << 4) & blackKings;
            }
            return jumpers;
        }


    }
}
