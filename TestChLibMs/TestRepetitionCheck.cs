using System;
using System.Linq;
using ChLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestChLibMs
{
    [TestClass]
    public class TestRepetitionCheck
    {        
        [TestMethod]
        public void TestMoveRepetition()
        {
            Position p = new Position();
            p.SetupBlack(18, 11, 16);
            p.SetupWhite(20, 3);
            p.SetupKings(3, 18);
            Search s = new Search();
            var value = s.Start(ref p, 8, Search.WHITE);

            showSearchResults(s, p, Search.WHITE);
            Assert.AreEqual(-Search.DRAW_BY_REPETITION, value);
            p.Flip();
            value = s.Start(ref p, 8, Search.BLACK);
            showSearchResults(s, p, Search.BLACK);
            Assert.AreEqual(Search.DRAW, value);
        }

        private void showSearchResults(Search search, Position position, int color)
        {
            Position temp = search.BestMove;
            var entries = HashTable.RetrieveEntriesMainLine(ref temp, color);
            foreach (var entry in entries)
            {
                Console.WriteLine(entry);
            }

            Position q = new Position();
            q.SetupBlack(3, 4);
            q.SetupWhite(7);
            q.Kings = q.WhitePieces | q.BlackPieces;
            
            entries = HashTable.RetrieveEntriesMainLine(ref q, color);
            if (entries.Any())
            {
                Console.WriteLine("***Target pos***");
                foreach (var entry in entries)
                {
                    Console.WriteLine(entry);
                }
            }
            else
            {
                Console.WriteLine("***TargetPos not found***");
            }
        }

        [TestMethod]
        public void TestMoveRepetitionStoredInHistory()
        {
            Position p = new Position();
            p.SetupBlack(29,25);
            p.SetupWhite(1);
            p.SetupKings(1,29,25);
            Search s = new Search();
            var value = s.Start(ref p, 12, Search.WHITE);

            showSearchResults(s, p, Search.WHITE);
            Assert.AreNotEqual(-Search.DRAW_BY_REPETITION, value);
            Position q = new Position();
            q.SetupBlack(29, 25);
            q.SetupWhite(5);
            q.SetupKings(5, 29, 25);
            s.StorePosition(q, Search.WHITE);
            value = s.Start(ref p, 12, Search.WHITE);
            Assert.AreEqual(-Search.DRAW_BY_REPETITION, value);
            Assert.AreEqual(q, s.BestMove);
        }

        [TestMethod]
        public void TestMoveRepetitionStoredInHistoryOtherColor()
        {
            Position p = new Position();
            p.SetupBlack(29, 25);
            p.SetupWhite(1);
            p.SetupKings(1, 29, 25);
            Search s = new Search();
            var value = s.Start(ref p, 12, Search.WHITE);

            showSearchResults(s, p, Search.WHITE);
            Assert.AreNotEqual(Search.DRAW_BY_REPETITION, value);
            Position q = new Position();
            q.SetupBlack(29, 25);
            q.SetupWhite(5);
            q.SetupKings(5, 29, 25);
            s.StorePosition(q, Search.BLACK);
            value = s.Start(ref p, 12, Search.WHITE);
            Assert.AreNotEqual(Search.DRAW_BY_REPETITION, value);
        }

        [TestMethod]
        public void TestMoveRepetitionNotCurrent()
        {
            Position p = new Position();
            p.SetupBlack(29, 25);
            p.SetupWhite(1);
            p.SetupKings(1, 29, 25);
            Search s = new Search();
            s.StorePosition(p, Search.BLACK);
            var value = s.Start(ref p, 12, Search.WHITE);

            showSearchResults(s, p, Search.WHITE);
            Assert.AreNotEqual(Search.DRAW_BY_REPETITION, value);
        }

        [TestMethod]
        public void TestTwoVsOneDrawingByRepetition()
        {
            Position p = new Position();
            p.SetupBlack(29, 25);
            p.SetupWhite(26);
            p.SetupKings(26, 29, 25);
            Search s = new Search();
            var value = s.Start(ref p, 7, Search.BLACK);

            showSearchResults(s, p, Search.BLACK);
            Assert.AreEqual(Search.DRAW_BY_REPETITION, value);

            p.Flip();
            value = s.Start(ref p, 7, Search.WHITE);

            showSearchResults(s, p, Search.WHITE);
            Assert.AreEqual(Search.DRAW, value);
        }

        [TestMethod]
        public void TestTwoVsOneDrawingByRepetitionWhiteAt16()
        {
            Position p = new Position();
            p.SetupBlack(4, 12);
            p.SetupWhite(16);
            p.Kings = p.WhitePieces | p.BlackPieces;
            Search s = new Search();
            var value = s.Start(ref p, 8, Search.WHITE);

            showSearchResults(s, p, Search.WHITE);
            Assert.AreEqual(-Search.DRAW_BY_REPETITION, value);
            Assert.AreEqual(Position.Sq11, s.BestMove.WhitePieces);
        }

        [TestMethod]
        public void TestTwoVsOneDrawingByRepetitionWhite()
        {
            Position p = new Position();
            p.SetupBlack(3,4);
            p.SetupWhite(7);
            p.Kings = p.WhitePieces | p.BlackPieces;
            Search s = new Search();
            var value = s.Start(ref p, 16, Search.WHITE);

            showSearchResults(s, p, Search.WHITE);
            Assert.AreEqual(Search.DRAW_BY_REPETITION, value);
            Assert.AreEqual(Position.Sq11, s.BestMove.WhitePieces);
        }

        [TestMethod]
        public void TestTwoVsOneDrawingByRepetitionWhite2()
        {
            Position p = new Position();
            p.SetupWhite(4, 12);
            p.SetupBlack(11);
            p.SetupKings(4,11);
            Console.WriteLine(p.ToString());
            Search s = new Search();
            var value = s.Start(ref p, 10, Search.WHITE);
            showSearchResults(s, p, Search.WHITE);
            Assert.AreEqual(Search.DRAW_BY_REPETITION, value);
        }

        [TestMethod]
        public void TestTwoVsOneDrawingByRepetitionWhite21()
        {
            Position p = new Position();
            p.SetupWhite(4, 12);
            p.SetupBlack(16);
            p.SetupKings(4, 16);
            Console.WriteLine(p.ToString());
            Search s = new Search();
            //s.Quiescense = Search.QMode.None;
            var value = s.Start(ref p, 12, Search.BLACK);
            //p.Flip();
            //var value = s.Start(ref p, 12, Search.BLACK);

            showSearchResults(s, p, Search.WHITE);
            Assert.AreEqual(Search.DRAW, value);
        }


        [TestMethod]
        public void TestTwoVsOneDrawingByRepetitionWhite3()
        {
            Position p = new Position();
            p.SetupBlack(4, 8);
            p.SetupWhite(11);
            p.Kings = p.WhitePieces | p.BlackPieces;
            Search s = new Search();
            var value = s.Start(ref p, 8, Search.WHITE);

            showSearchResults(s, p, Search.WHITE);
            Assert.AreEqual(-Search.DRAW_BY_REPETITION, value);
        }

        [TestMethod]
        public void TestTwoVsOneDrawingByRepetitionWhite4()
        {
            Position p = new Position();
            p.SetupBlack(4, 3);
            p.SetupWhite(11);
            p.Kings = p.WhitePieces | p.BlackPieces;
            Search s = new Search();
            var value = s.Start(ref p, 8, Search.BLACK);

            showSearchResults(s, p, Search.BLACK);
            Assert.AreEqual(Math.Abs(Search.DRAW_BY_REPETITION), value);
        }


        [TestMethod]
        public void TestThreeVsTwoDrawingByRepetition()
        {
            Position p = new Position();
            p.SetupWhite(11, 15);
            p.SetupBlack(4, 8, 12);
            p.Kings = p.WhitePieces | p.BlackPieces;
            Search s = new Search();
            //s.Quiescense = Search.QMode.None;
            var value = s.Start(ref p, 12, Search.BLACK);
            showSearchResults(s, p, Search.BLACK);
            Assert.AreEqual(Math.Abs(Search.DRAW_BY_REPETITION), value);
        }

        [TestMethod]
        public void TestMoveGenerator()
        {
            Position p = new Position();
            p.SetupBlack(3, 4);
            p.SetupWhite(7);
            p.Kings = p.WhitePieces | p.BlackPieces;
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            int numMoves = MoveGenerator.GenerateMoves(ref p, moves, Search.WHITE);
            Assert.AreEqual(3, numMoves);
            Assert.IsTrue(moves.Any(pos => pos.WhitePieces == Position.Sq11));
        }
    }
}
