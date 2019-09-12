using System.Text;

namespace ChLib
{
    public struct Position
    {
        public const int Empty = 0;
        public int WhitePieces;
        public int BlackPieces;
        public int Kings;
        
        public override string ToString()
        {
            return showAsDiagram();
        }

        public void SetupBlack(params int [] squares)
        {
            BlackPieces = toUint(squares);
        }

        public void SetupWhite(params int[] squares)
        {
            WhitePieces = toUint(squares);
        }

        public void SetupKings(params int[] squares)
        {
            Kings = toUint(squares);
        }

        private int toUint(params int [] squares)
        {
            int result = 0;
            foreach (int sq in squares)
            {
                result |= (1 << (sq-1));
            }
            return result;
        }

        private string showAsDiagram()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 31; i >= 0; i--)
            {
                int squareMask = (1 << i);
                //// 0, 7, 15, 23, 31
                if ((i + 1)%8 == 0)
                {
                    sb.Append(":::");
                }

                if ((WhitePieces & Kings & squareMask) != 0) // check white king at square
                {
                    sb.Append(" W ");
                }
                else if ((WhitePieces & squareMask) != 0) // check white piece at square
                {
                    sb.Append(" w ");
                }
                else if ((BlackPieces & Kings & squareMask) != 0)
                {
                    sb.Append(" B ");
                }
                else if ((BlackPieces & squareMask) != 0)
                {
                    sb.Append(" b ");
                }
                else
                {
                    sb.Append(" . ");
                }
                //// 3, 11, 19, 27
                if ((i + 4)%8 != 0)
                {
                    sb.Append(":::");
                }
                if ((i)%4 == 0)
                {
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }

        #region Square Consts
        public const int Sq1 = 0x00000001;
        public const int Sq2 = 0x00000002;
        public const int Sq3 = 0x00000004;
        public const int Sq4 = 0x00000008;
        public const int Sq5 = 0x00000010;
        public const int Sq6 = 0x00000020;
        public const int Sq7 = 0x00000040;
        public const int Sq8 = 0x00000080;
        public const int Sq9 = 0x00000100;
        public const int Sq10 = 0x00000200;
        public const int Sq11 = 0x00000400;
        public const int Sq12 = 0x00000800;
        public const int Sq13 = 0x00001000;
        public const int Sq14 = 0x00002000;
        public const int Sq15 = 0x00004000;
        public const int Sq16 = 0x00008000;
        public const int Sq17 = 0x00010000;
        public const int Sq18 = 0x00020000;
        public const int Sq19 = 0x00040000;
        public const int Sq20 = 0x00080000;
        public const int Sq21 = 0x00100000;
        public const int Sq22 = 0x00200000;
        public const int Sq23 = 0x00400000;
        public const int Sq24 = 0x00800000;
        public const int Sq25 = 0x01000000;
        public const int Sq26 = 0x02000000;
        public const int Sq27 = 0x04000000;
        public const int Sq28 = 0x08000000;
        public const int Sq29 = 0x10000000;
        public const int Sq30 = 0x20000000;
        public const int Sq31 = 0x40000000;
        public const int Sq32 = -2147483648; // = 0x80000000;

        public const int Rank1 = Sq1 | Sq2 | Sq3 | Sq4 ;
        public const int Rank2 = Sq5 | Sq6 | Sq7 | Sq8 ;
        public const int Rank3 = Sq9 | Sq10 | Sq11 | Sq12 ;
        public const int Rank4 = Sq13 | Sq14 | Sq15 | Sq16 ;
        public const int Rank5 = Sq17 | Sq18 | Sq19 | Sq20 ;
        public const int Rank6 = Sq21 | Sq22 | Sq23 | Sq24 ;
        public const int Rank7 = Sq25 | Sq26 | Sq27 | Sq28 ;
        public const int Rank8 = Sq29 | Sq30 | Sq31 | Sq32 ;
        #endregion

        public bool IsEmpty()
        {
            return WhitePieces == Empty && BlackPieces == Empty;
        }

        public static bool operator == (Position one, Position other)
        {
            return one.BlackPieces == other.BlackPieces && one.WhitePieces == other.WhitePieces &&
                   one.Kings == other.Kings;
        }

        public static bool operator !=(Position one, Position other)
        {
            return one.BlackPieces != other.BlackPieces || one.WhitePieces != other.WhitePieces ||
                   one.Kings != other.Kings;
        }

        // Resharper generated code
        //public bool Equals(Position other)
        //{
        //    return other.WhitePieces == WhitePieces && other.BlackPieces == BlackPieces && other.Kings == Kings;
        //}

        //public override bool Equals(object obj)
        //{
        //    if (ReferenceEquals(null, obj)) return false;
        //    if (obj.GetType() != typeof (Position)) return false;
        //    return Equals((Position) obj);
        //}

        //public override int GetHashCode()
        //{
        //    unchecked
        //    {
        //        int result = WhitePieces.GetHashCode();
        //        result = (result*397) ^ BlackPieces.GetHashCode();
        //        result = (result*397) ^ Kings.GetHashCode();
        //        return result;
        //    }
        //}
        public void Flip()
        {
            int tmp = WhitePieces;
            WhitePieces = BlackPieces.Reverse();
            BlackPieces = tmp.Reverse(); 
            Kings = Kings.Reverse();
        }
    }
}
