using System;
using ChLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace TestChLibMs
{
    [TestClass]
    public class TestThreeVsTwo
    {
        private const int CONVERSION_FOUND = 1633;

        private static void showSearchResults(Search s, Position p)
        {
            Console.WriteLine(p);
            Console.WriteLine(Utils.GetSearchStatistics(s, p));
        }

        private Search _search;

        [TestInitialize]
        public void SetUp()
        {
            _search = new Search();
            //_search.Evaluate = TwoByOneEvaluator.Evaluate;
        }

        [TestMethod]
        public void TestOppositeCornerFindConversion()
        {
            Position p = new Position();
            p.BlackPieces = Position.Sq14 | Position.Sq15 | Position.Sq18;
            p.WhitePieces = Position.Sq1 | Position.Sq32;
            p.Kings = Position.Sq1 | Position.Sq32 | Position.Sq14 | Position.Sq15 | Position.Sq18;
            
            int value = _search.Start(ref p, 16, Search.BLACK);

            showSearchResults(_search, p);
            Assert.AreEqual(CONVERSION_FOUND, value);
        }

        [TestMethod]
        public void TestOppositeCornerFindWin()
        {
            Position p = new Position();
            p.BlackPieces = Position.Sq14 | Position.Sq23 | Position.Sq18;
            p.WhitePieces = Position.Sq1 | Position.Sq32;
            p.Kings = Position.Sq1 | Position.Sq32 | Position.Sq14 | Position.Sq23 | Position.Sq18;

            int value = _search.Start(ref p, 30, Search.BLACK);

            showSearchResults(_search, p);
            Assert.AreEqual(Search.WIN, value);
        }

        [TestMethod]
        public void TestSameCornerFindConversion()
        {
            Position p = new Position();
            p.BlackPieces = Position.Sq19 | Position.Sq23 | Position.Sq18;
            p.WhitePieces = Position.Sq1 | Position.Sq2;
            p.Kings = Position.Sq1 | Position.Sq2 | Position.Sq19 | Position.Sq23 | Position.Sq18;

            int value = _search.Start(ref p, 24, Search.BLACK);

            showSearchResults(_search, p);
            Assert.AreEqual(CONVERSION_FOUND, value);
        }

        [TestMethod]
        public void TestSameCornerFindBlackWin()
        {
            Position p = new Position();
            p.BlackPieces = Position.Sq6 | Position.Sq10 | Position.Sq18;
            p.WhitePieces = Position.Sq1 | Position.Sq17;
            p.Kings = p.BlackPieces | p.WhitePieces;
            int value = _search.Start(ref p, 23, Search.BLACK);

            showSearchResults(_search, p);
            Assert.AreEqual(Search.WIN, value);
            //Assert.AreEqual("6-9", Utils.GetMove(p, _search.BestMove));
        }

        [TestMethod]
        public void TestSameCornerFindWhiteWin()
        {
            Position p = new Position();
            p.BlackPieces = Position.Sq1 | Position.Sq17;
            p.WhitePieces = Position.Sq6 | Position.Sq10 | Position.Sq18;
            p.Kings = p.BlackPieces | p.WhitePieces;
            int value = _search.Start(ref p, 23, Search.WHITE);

            showSearchResults(_search, p);
            Assert.AreEqual(Search.WIN, value);
            //Assert.AreEqual("6-9", Utils.GetMove(p, _search.BestMove));
        }

        [TestMethod]
        public void TestSameCornerFindBlackWinMirrored()
        {
            Position p = new Position();
            p.BlackPieces = Position.Sq27 | Position.Sq23 | Position.Sq15;
            p.WhitePieces = Position.Sq32 | Position.Sq16;
            p.Kings = p.BlackPieces | p.WhitePieces;
            int value = _search.Start(ref p, 23, Search.BLACK);

            showSearchResults(_search, p);
            Assert.AreEqual(Search.WIN, value);
        }

        [TestMethod]
        public void TestSameCornerFindBlackConversionWithWhite()
        {
            Position p = new Position();
            p.BlackPieces = Position.Sq9 | Position.Sq10 | Position.Sq18;
            p.WhitePieces = Position.Sq1 | Position.Sq17;
            p.Kings = p.BlackPieces | p.WhitePieces;
            int value = _search.Start(ref p, 12, Search.WHITE);

            showSearchResults(_search, p);
            Assert.AreEqual(-CONVERSION_FOUND, value);
        }

        [TestMethod]
        public void TestSameCornerFindBlackConversionWithBlack()
        {
            Position p = new Position();
            p.BlackPieces = Position.Sq9 | Position.Sq10 | Position.Sq18;
            p.WhitePieces = Position.Sq1 | Position.Sq13;
            p.Kings = p.BlackPieces | p.WhitePieces;
            int value = _search.Start(ref p, 6, Search.BLACK);

            showSearchResults(_search, p);
            Assert.AreEqual(CONVERSION_FOUND, value);
        }


    }
}
