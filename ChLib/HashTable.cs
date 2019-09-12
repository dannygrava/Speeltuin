using System;
using System.Collections.Generic;

namespace ChLib
{
    public static class HashTable
    {
        private const int UPPER = 1;
        private const int LOWER = 2;
        private const int EXACT = 3;
        private const int HASH_TABLE_SIZE = 0x7FFFF;//iets meer dan een miljoen
        private static HashEntry[] _entries = new HashEntry[HASH_TABLE_SIZE+1];
        readonly static uint[,] _seeds = new uint[12, 256];
        private static uint _colorSeed;
        private static void initSeeds()
        {
            Random r = new Random();
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    _seeds[i, j] = (uint)r.Next();
                }
            }
            _colorSeed = (uint)r.Next();
        }

        static HashTable()
        {
            initSeeds();
        }

        // Zobrist hashing scheme applied
        // See http://burtleburtle.net/bob/hash/doobs.html
        // Zobrist has a better hash distribution at small numbers of pieces and this may have a dramatic effect on the performance
        // in end games such as 2vs3 kings.
        private static int calculateHash(ref Position p, int color)
        {
            uint hash = 0;
            hash ^= _seeds[0, p.BlackPieces & 0xFF];
            hash ^= _seeds[1, (uint)p.BlackPieces >> 8 & 0xFF];
            hash ^= _seeds[2, (uint)p.BlackPieces >> 16 & 0xFF];
            hash ^= _seeds[3, (uint)p.BlackPieces >> 24];

            hash ^= _seeds[4, p.WhitePieces & 0xFF];
            hash ^= _seeds[5, (uint)p.WhitePieces >> 8 & 0xFF];
            hash ^= _seeds[6, (uint)p.WhitePieces >> 16 & 0xFF];
            hash ^= _seeds[7, (uint)p.WhitePieces >> 24];

            hash ^= _seeds[8, p.Kings & 0xFF];
            hash ^= _seeds[9, (uint)p.Kings >> 8 & 0xFF];
            hash ^= _seeds[10, (uint)p.Kings >> 16 & 0xFF];
            hash ^= _seeds[11, (uint)p.Kings >> 24];
            if (color == MoveGenerator.BLACK)
            {
                hash ^= _colorSeed;
            }
            return (int)hash;
        }


        //private uint calculateHash(ref Position p, uint color)
        //{
        //    uint hash;
        //    hash = p.BlackPieces ^ p.BlackPieces >> 8 ^ p.BlackPieces >> 16 ^ p.BlackPieces >> 24;
        //    uint blackKings = p.Kings & p.BlackPieces;
        //    hash ^= blackKings ^ blackKings >> 1 ^ blackKings >> 9 ^ blackKings >> 17;
        //    hash ^= p.WhitePieces ^ p.WhitePieces >> 2 ^ p.WhitePieces >> 10 ^ p.WhitePieces >> 24;
        //    uint whiteKings = p.Kings & p.WhitePieces;
        //    hash ^= whiteKings ^ whiteKings >> 3 ^ whiteKings >> 11 ^ whiteKings >> 19;
        //    hash ^= (color << 10);
        //    //h.key := h.key xor (p.color shl 10);
        //    return hash;
        //}

        public static void StorePosition(ref Position p, int color, int value, int depth, int alpha, int beta, Position bestmove)
        {
            int key = calculateHash(ref p, color);
            int index = key & HASH_TABLE_SIZE;
            if (_entries[index].Depth > depth)
                return;

            _entries[index].Position = p;
            _entries[index].BestMove = bestmove;
            _entries[index].Depth = depth;
            _entries[index].Value = value;
            _entries[index].Color = color;

            if (value <= alpha)
            {
                _entries[index].ValueType = UPPER;
                return;
            }

            if (value >= beta)
            {
                _entries[index].ValueType = LOWER;
                return;
            }
            _entries[index].ValueType = EXACT;
        }

        public static bool RetrievePosition(ref Position p, int color, ref int value, int depth, int alpha, int beta, ref Position bestmove)
        {
            int key = calculateHash(ref p, color);
            int index = key & HASH_TABLE_SIZE;

            if (_entries[index].Position != p || _entries[index].Color != color)
            {
                return false;
            }

            if (depth > _entries[index].Depth)
            {
                bestmove = _entries[index].BestMove;
                return false;
            }

            // we have sufficient depth in the hashtable to possibly cause a cutoff.

            // if we have an exact value, we don't need to search for a new value.
            if (_entries[index].ValueType == EXACT)
            {
                value = _entries[index].Value;
                return true;
            }

            // if we have a lower bound, we might either get a cutoff or raise alpha.
            if (_entries[index].ValueType == LOWER)
            {
                // the value stored in the hashtable is a lower bound, so it's useful
                if (_entries[index].Value >= beta)
                {
                    value = _entries[index].Value;
                    return true;
                }

                //if (_entries[index].Value > alpha)
                //{
                //    // value > alpha: we can adjust bounds
                //    alpha = _entries[index].Value;
                //}
                bestmove = _entries[index].BestMove;
                return false;
            }

            // if we have an upper bound, we can either get a cutoff or lower beta.
            if (_entries[index].Value <= alpha)
            {
                value = _entries[index].Value;
                return true;
            }

            //if (_entries[index].Value < beta)
            //{
            //    beta = _entries[index].Value;
            //}
            bestmove = _entries[index].BestMove;
            return false;
        }

        public static void Reset()
        {
            //_entries = new HashEntry[HASH_TABLE_SIZE+1];
            for (int i = 0; i < HASH_TABLE_SIZE; i++)
            {
                _entries[i].Depth = -10000;
                _entries[i].Position.WhitePieces = 0;
                _entries[i].Position.BlackPieces = 0;
                _entries[i].Position.Kings = 0;
            }
        }

        public static IEnumerable<HashEntry> RetrieveEntriesMainLine(ref Position p, int color)
        {
            List<HashEntry> mainEntries = new List<HashEntry>();
            Position bestmove = p;
            int i = 0;
            while (true)
            {
                if (i >= Search.MAX_SUPPORTED_DEPTH)
                    break;

                color ^= 1;
                int key = calculateHash(ref bestmove, color);
                int index = key & HASH_TABLE_SIZE;

                if (_entries[index].Position != bestmove || _entries[index].BestMove.IsEmpty())
                {
                    break;
                }
                mainEntries.Add(_entries[index]);
                bestmove = _entries[index].BestMove;
                i++;
            }
            return mainEntries;
        }

        public static int RetrieveBestline(ref Position p, int color, Position[] moves)
        {
            Position bestmove = p;
            int i = 0;
            while (true)
            {
                if (i >= Search.MAX_SUPPORTED_DEPTH)
                    break;

                moves[i] = bestmove;
                color ^= 1;
                int key = calculateHash(ref bestmove, color);
                int index = key & HASH_TABLE_SIZE;

                if (_entries[index].Position != bestmove || _entries[index].BestMove.IsEmpty())
                {
                    break;
                }
                bestmove = _entries[index].BestMove;
                i++;
            }
            return i;
        }

        public static bool RetrievePosition(ref Position position, int color, out int value, out string valueType)
        {
            int key = calculateHash(ref position, color);
            int index = key & HASH_TABLE_SIZE;

            if (_entries[index].Position != position && _entries[index].Color != color)
            {
                value = int.MaxValue;
                valueType = "Not found";
                return false;
            }
            value = -_entries[index].Value;
            switch (_entries[index].ValueType)
            {
                case EXACT: valueType = "Exact";
                    break;
                case LOWER:
                    valueType = "Less than";
                    break;
                case UPPER:
                    valueType = "More than";
                    break;
                default:
                    valueType = "Unknown";
                    break;
            }
            return true;
        }
    }

}
