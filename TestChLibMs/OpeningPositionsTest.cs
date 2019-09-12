using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestChLibMs
{
    [TestClass]
    public class OpeningPositionsTest
    {
        [TestMethod()]
        public void TestBeginPosition()
        {
            Position p = new Position();
            p.SetupBlack(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
            p.SetupWhite(21, 22, 23, 24, 25,26, 27,28, 29, 30, 31, 32);

            Search s = new Search();
            s.Start(ref p, 13, Search.BLACK);
            Console.WriteLine(Utils.GetMove(p, s.BestMove));
        }
        

        [TestMethod()]
        public void TestDundee()
        {
            Position p = new Position();
            p.SetupBlack(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 16);
            p.SetupWhite(21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32);

            Search s = new Search();
            s.Start(ref p, 13, Search.WHITE);
            Console.WriteLine(Utils.GetMove(p, s.BestMove));
        }

        [TestMethod()]
        public void TestBristol()
        {
            Position p = new Position();
            p.SetupBlack(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 16, 12);
            p.SetupWhite(21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32);

            Search s = new Search();
            s.Start(ref p, 13, Search.WHITE);
            Console.WriteLine(Utils.GetMove(p, s.BestMove));
        }

        [TestMethod()]
        public void TestOldFaithfull()
        {
            Position p = new Position();
            p.SetupBlack(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 12);
            p.SetupWhite(21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32);

            Search s = new Search();
            s.Start(ref p, 13, Search.WHITE);
            Console.WriteLine(Utils.GetMove(p, s.BestMove));
        }

        [TestMethod()]
        public void TestKelso()
        {
            Position p = new Position();
            p.SetupBlack(1, 2, 3, 4, 5, 6, 7, 8, 9, 15, 11, 12);
            p.SetupWhite(21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32);

            Search s = new Search();
            s.Start(ref p, 13, Search.WHITE);
            Console.WriteLine(Utils.GetMove(p, s.BestMove));
        }

        [TestMethod()]
        public void TestDenny()
        {
            Position p = new Position();
            p.SetupBlack(1, 2, 3, 4, 5, 6, 7, 8, 9, 14, 11, 12);
            p.SetupWhite(21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32);

            Search s = new Search();
            s.Start(ref p, 13, Search.WHITE);
            Console.WriteLine(Utils.GetMove(p, s.BestMove));
        }

        [TestMethod()]
        public void TestPioneer()
        {
            Position p = new Position();
            p.SetupBlack(1, 2, 3, 4, 5, 6, 7, 8, 14, 10, 11, 12);
            p.SetupWhite(21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32);

            Search s = new Search();
            s.Start(ref p, 13, Search.WHITE);
            Console.WriteLine(Utils.GetMove(p, s.BestMove));
        }

        [TestMethod()]
        public void TestEdinburgh()
        {
            Position p = new Position();
            p.SetupBlack(1, 2, 3, 4, 5, 6, 7, 8, 13, 10, 11, 12);
            p.SetupWhite(21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32);

            Search s = new Search();
            s.Start(ref p, 13, Search.WHITE);
            Console.WriteLine(Utils.GetMove(p, s.BestMove));
        }

    }
}
