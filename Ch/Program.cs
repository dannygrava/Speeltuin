using System;
using ChLib;

namespace Ch
{
    class Program
    {
        static void Main(string[] args)
        {
            Position p = new Position();
            p.BlackPieces = Position.Sq31 | Position.Sq30 | Position.Sq17 | Position.Sq14;
            p.WhitePieces = Position.Sq6 | Position.Sq11 | Position.Sq23 | Position.Sq21;
            p.Kings = Position.Sq31 | Position.Sq30 | Position.Sq23 | Position.Sq14 | Position.Sq6;

            Search s = new Search();
            Console.WriteLine(p);
            Console.WriteLine(Utils.GetSearchStatistics(s, p));
        }

    }
}
