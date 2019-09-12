using System;
using System.Text;

namespace ChLib
{
    public static class Utils
    {
        //B:W18,24,27,28,K10,K15:B12,16,20,K22,K25,K29
        public static string ShowFen(this Position p, int color)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("{0}:W", color == MoveGenerator.BLACK ? "B":"W"));
            bool firstAdd = true;
            for (int i = 0; i < 32; i++)
            {
                int squareMask = (1 << i);
                if ((p.WhitePieces & p.Kings & squareMask) != 0)
                {
                    if (!firstAdd)
                    {
                        sb.Append(",");
                    }
                    sb.Append(string.Format("K{0}", i + 1));
                    firstAdd = false;
                }
                else if ((p.WhitePieces & squareMask) != 0)
                {
                    if (!firstAdd)
                    {
                        sb.Append(",");
                    }
                    sb.Append(string.Format("{0}", i + 1));
                    firstAdd = false;
                }
            }

            sb.Append(":B");
            firstAdd = true;
            for (int i = 0; i < 32; i++)
            {
                int squareMask = (1 << i);
                if ((p.BlackPieces & p.Kings & squareMask) != 0)
                {
                    if (!firstAdd)
                    {
                        sb.Append(",");
                    }
                    sb.Append(string.Format("K{0}", i + 1));
                    firstAdd = false;
                }
                else if ((p.BlackPieces & squareMask) != 0)
                {
                    if (!firstAdd)
                    {
                        sb.Append(",");
                    }
                    sb.Append(string.Format("{0}", i + 1));
                    firstAdd = false;
                }
            }
            return sb.ToString();
        }

        public static string GetMove(Position from, Position to)
        {
            if (from.BlackPieces != to.BlackPieces)
            {
                int diff = from.BlackPieces ^ to.BlackPieces;
                int f = diff & from.BlackPieces;
                int t = diff & to.BlackPieces;
                if (t != 0)
                {
                    string separator = from.WhitePieces != to.WhitePieces ? "x" : "-";
                    return string.Format("{0}{2}{1}", GetSquareNr(f), GetSquareNr(t), separator);
                }
            }

            if (from.WhitePieces != to.WhitePieces)
            {
                int diff = from.WhitePieces ^ to.WhitePieces;
                int f = diff & from.WhitePieces;
                int t = diff & to.WhitePieces;
                if (t != 0)
                {
                    string separator = from.BlackPieces != to.BlackPieces ? "x" : "-";
                    return string.Format("{0}{2}{1}", GetSquareNr(f), GetSquareNr(t), separator);
                }
            }
            return "";
        }

        public static int GetSquareNr(int value)
        {
            int i = 0;
            do
            {
                value = (int)((uint) value >> 1);
                i++;
            } while (value != 0);
            return i;
        }

        static readonly Random _random = new Random();
        public static Position GenerateRandomPosition(uint maxPiecesPerSide = 12)
        {
            Position b = new Position();
            var r = _random;
            b.BlackPieces = r.Next();
            while (Bits.BitCount(b.BlackPieces) > maxPiecesPerSide)
            {
                b.BlackPieces = b.BlackPieces << 1;
            }
            b.WhitePieces = r.Next();

            while (Bits.BitCount(b.WhitePieces) > maxPiecesPerSide)
            {
                b.WhitePieces = b.WhitePieces << 1;
            }
            b.Kings = r.Next();
            b.Kings = b.Kings & (b.BlackPieces | b.WhitePieces) | (b.BlackPieces & MoveGenerator.KINGS_ROW_BLACK) |
                      (b.WhitePieces & MoveGenerator.KINGS_ROW_WHITE);
            return b;
        }

        public static string GetSearchStatistics(Search s, Position p)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Value: " + s.LastValue);
            Position m1 = p;
            foreach (var m in s.GetBestlineFromTranspositionTable())
            {
                string move = GetMove(m1, m);
                if (move == "")
                    break;
                sb.Append(string.Format("{0} ", move));
                m1 = m;
            }

            sb.AppendLine();
            //Position[] moves;
            //int[] values;
            //string[] valueTypes;
            //uint numVariations = s.GetVariations(ref p, out moves, out values, out valueTypes);
            //for (int i = 0; i < numVariations; i++)
            //{
            //    sb.AppendLine(string.Format("{0} value: {1} {2}", GetMove(p, moves[i]), valueTypes[i], values[i]));
            //}
            //sb.AppendLine();

            sb.AppendLine(Math.Truncate(s.SearchTime.TotalMilliseconds) + "ms");
            sb.AppendLine(s.TotalSearches + " nodes");
            sb.AppendLine(Math.Truncate(s.TotalSearches / s.SearchTime.TotalSeconds / 1000) + " KNodes/s searched");
            //sb.AppendLine(Math.Truncate(s.TotalEvaluations / s.SearchTime.TotalSeconds / 1000) + " KNodes/s evaluated");
            sb.AppendLine(string.Format("Depth: {0} / {1}", s.NominalDepth, s.PeakDepth));

            foreach (var pos in s.GetBestlineFromTranspositionTable())
            {
                if (pos.IsEmpty())
                    break;
                sb.AppendLine(pos.ToString());
            }

            return sb.ToString();
        }

        public static int ToInt(this uint value)
        {
            return (int)value;
        }
    }
}
