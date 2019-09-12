using ChLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System;

namespace TestChLibMs
{
    /// <summary>
    ///This is a test class for SearchTest and is intended
    ///to contain all SearchTest Unit Tests
    ///</summary>
    [TestClass]
    public class SearchTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        [TestMethod]
        public void TestSimplePosition()
        {
            Position p = new Position();
            p.SetupBlack(2, 3, 4, 7);
            p.SetupWhite(14, 16, 19, 22);

            Search s = new Search();
            s.Start(ref p, 11, Search.BLACK);
            showSearchResults(s, p);
            Assert.AreEqual("7-11", Utils.GetMove(p, s.BestMove));
        }

        [TestMethod()]
        public void TestSimplePositionWhite()
        {
            Position p = new Position();
            p.SetupBlack(11, 14, 17, 19);
            p.SetupWhite(31, 30, 29, 26);
            Search s = new Search();
            s.Start(ref p, 8, Search.WHITE);

            showSearchResults(s, p);
            Assert.AreEqual("26-22", Utils.GetMove(p, s.BestMove));
        }

        [TestMethod()]
        public void TestForcingPosition()
        {
            Position p = new Position();
            p.SetupBlack(20, 16, 11, 10, 9, 6, 7, 5, 2, 1);
            p.SetupWhite(21, 22, 23, 25, 26, 27, 30, 31, 32, 28);

            Search s = new Search();
            s.Start(ref p, 12, Search.WHITE);

            showSearchResults(s, p);
            Assert.AreEqual("28-24", Utils.GetMove(p, s.BestMove));
        }

        [TestMethod()]
        public void TestSimpleForcingPosition()
        {
            Position p = new Position();
            p.SetupBlack(20, 16, 7);
            p.SetupWhite(23, 24, 27);

            Search s = new Search();
            s.Start(ref p, 3, Search.WHITE);
            showSearchResults(s, p);
            Assert.AreEqual("24-19", Utils.GetMove(p, s.BestMove));
        }

        [TestMethod()]
        public void TestProblemLoy01()
        {
            Position p = new Position();
            p.SetupBlack(31, 30, 17, 14);
            p.SetupWhite(6, 11, 23, 21);
            p.SetupKings(31, 30, 23, 14, 6);

            Search s = new Search();
            int value = s.Start(ref p, 17, Search.BLACK);

            showSearchResults(s, p);
            Assert.AreEqual("30-26", Utils.GetMove(p, s.BestMove));
            Assert.AreEqual(Search.WIN, value);
        }

        [TestMethod()]
        public void TestProblemLoy01b()
        {
            Position p = new Position();
            p.SetupBlack(31, 2);
            p.SetupWhite(30, 11);
            p.SetupKings(31, 30, 2);

            Search s = new Search();
            int value = s.Start(ref p, 20, Search.WHITE);
            showSearchResults(s, p);
            Assert.AreEqual(Search.LOSS, value);
        }

        private static void showSearchResults(Search s, Position p)
        {
            //Console.WriteLine(p);
            Console.WriteLine("{0:N0}", s.TotalSearches);
            Console.WriteLine(Utils.GetSearchStatistics(s, p));
        }

        [TestMethod()]
        public void TestProblemLoy02()
        {
            Position p = new Position();
            p.SetupBlack(2, 18, 7, 27);
            p.SetupWhite(13, 15, 5, 19);
            p.SetupKings(5, 18);

            Search s = new Search();
            int value = s.Start(ref p, 17, Search.WHITE);

            showSearchResults(s, p);
            Assert.AreEqual("13-9", Utils.GetMove(p, s.BestMove));
            Assert.AreEqual(Search.WIN, value);
        }

        [TestMethod()]
        public void TestProblemLoy03()
        {
            Position p = new Position();
            p.SetupBlack(32, 27, 20, 19);
            p.SetupWhite(30, 28, 26);
            p.SetupKings(32, 26);

            Search s = new Search();
            int value = s.Start(ref p, 7, Search.WHITE);

            showSearchResults(s, p);
            Assert.AreEqual("28-24", Utils.GetMove(p, s.BestMove));
            Assert.AreEqual(Search.WIN, value);
        }

        [TestMethod()]
        public void TestProblemLoy03m()
        {
            Position p = new Position();
            p.SetupBlack(3, 5, 7);
            p.SetupWhite(1, 6, 13, 14);
            p.SetupKings(1, 7);

            Search s = new Search();
            int value = s.Start(ref p, 7, Search.BLACK);

            showSearchResults(s, p);
            Assert.AreEqual("5-9", Utils.GetMove(p, s.BestMove));
            Assert.AreEqual(Search.WIN, value);
        }

        [TestMethod()]
        public void TestProblemLoy13()
        {
            Position p = new Position();
            p.SetupBlack(8, 10, 13, 19, 21);
            p.SetupWhite(16, 17, 22, 30, 32);

            Search s = new Search();
            s.Start(ref p, 7, Search.WHITE);
            showSearchResults(s, p);
            Assert.AreEqual("16-12", Utils.GetMove(p, s.BestMove));
        }

        [TestMethod()]
        public void TestProblemLoy18()
        {
            Position p = new Position();
            p.SetupBlack(1, 3, 5, 7, 8, 11, 13, 17);
            p.SetupWhite(14, 19, 20, 25, 26, 27, 29, 30);

            Search s = new Search();
            s.Start(ref p, 11, Search.BLACK);
            showSearchResults(s, p);
            Assert.AreEqual("11-15", Utils.GetMove(p, s.BestMove));
        }

        [TestMethod()]
        public void TestProblemLoy24()
        {
            Position p = new Position();
            p.SetupBlack(2, 5, 10, 17);
            p.SetupWhite(31, 27, 25, 11);
            p.SetupKings(11, 10);

            Search s = new Search();
            int value = s.Start(ref p, 17, Search.BLACK);

            showSearchResults(s, p);
            Assert.AreEqual(Search.WIN, value);
            Assert.IsTrue(new[] { "17-22", "2-7" }.Contains(Utils.GetMove(p, s.BestMove)));
            //Assert.AreEqual("2-7", Utils.GetMove(p, s.BestMove));
        }

        [TestMethod]
        public void TestOneVsTwoShallow()
        {
            Position p = new Position();
            p.SetupBlack(9, 14);
            p.SetupWhite(1);
            p.SetupKings(9, 1, 14);

            Search s = new Search();
            int value = s.Start(ref p, 12, Search.BLACK);

            showSearchResults(s, p);
            Assert.AreEqual(Search.WIN, value);
        }

        [TestMethod]
        public void TestOneVsTwoDeep()
        {
            Position p = new Position();
            p.BlackPieces = Position.Sq32 | Position.Sq31;
            p.WhitePieces = Position.Sq1;
            p.Kings = Position.Sq1 | Position.Sq31 | Position.Sq32;

            Search s = new Search();
            int value = s.Start(ref p, 34, Search.BLACK);

            showSearchResults(s, p);
            Assert.AreEqual(Search.WIN, value);
        }

        [TestMethod]
        public void TestDeepFailHighLowImpactOnSearchTime()
        {
            Position p = new Position();
            p.SetupBlack(2, 3, 5, 6, 7, 9, 11, 12);
            p.SetupWhite(14, 17, 18, 25, 28, 29, 30, 32);
            p.SetupKings(14, 17, 18, 25, 28, 29, 30, 32);

            Search s = new Search();
            s.Start(ref p, 14, Search.WHITE);
            showSearchResults(s, p);
        }

        [TestMethod()]
        public void TestManvsManBMovesFirst()
        {
            Position p = new Position();
            p.BlackPieces = Position.Sq2;
            p.WhitePieces = Position.Sq31;
            p.Kings = Position.Sq2;
            Search s = new Search();
            int value = s.Start(ref p, 8, Search.BLACK);

            showSearchResults(s, p);
            Assert.AreEqual(Search.WIN, value);
        }

        [TestMethod()]
        public void TestManvsManPlyMin1BMoves()
        {
            Position p = new Position();
            p.BlackPieces = Position.Sq6;
            p.WhitePieces = Position.Sq31;
            p.Kings = Position.Sq6;
            Search s = new Search();
            int value = s.Start(ref p, 7, Search.WHITE);

            showSearchResults(s, p);
            Assert.AreEqual(Search.LOSS, value);
        }


        [TestMethod()]
        public void TestManvsManWMovesFirst()
        {
            Position p = new Position();
            p.BlackPieces = Position.Sq2;
            p.WhitePieces = Position.Sq31;
            p.Kings = Position.Sq31;
            Search s = new Search();
            int value = s.Start(ref p, 8, Search.WHITE);

            showSearchResults(s, p);
            Assert.AreEqual(Search.WIN, value);
        }


        [TestMethod()]
        public void TestManvsManPlyMin1WMoves()
        {
            Position p = new Position();
            p.BlackPieces = Position.Sq2;
            p.WhitePieces = Position.Sq27;
            p.Kings = Position.Sq27;
            Search s = new Search();
            int value = s.Start(ref p, 7, Search.BLACK);

            showSearchResults(s, p);
            Assert.AreEqual(Search.LOSS, value);
        }

        [TestMethod()]
        public void TestQuickWin()
        {
            Position p = new Position();
            p.SetupBlack(3, 7, 2, 14);
            p.SetupWhite(4, 12, 22, 13);
            p.SetupKings(4);
            Search s = new Search();
            int value = s.Start(ref p, 12, Search.BLACK);

            showSearchResults(s, p);
            Assert.AreEqual(Search.WIN, value);
        }

        [TestMethod()]
        public void TestOldburrySageProblem()
        {
            // http://homepages.tcp.co.uk/~pcsol/sagehlp1.htm#Tutorial
            //W 6,10,14,15,18,19,22,23,27 : B 1,3,7,12,K13,20,K25,26
            Position p = new Position();
            p.SetupBlack(1, 3, 7, 12, 13, 20, 25, 26);
            p.SetupWhite(6, 10, 14, 15, 18, 19, 22, 23, 27);
            p.SetupKings(13, 25);

            Search s = new Search();
            int value = s.Start(ref p, 17, Search.WHITE);

            showSearchResults(s, p);
            Assert.AreEqual(Search.WIN, value);
        }

        [TestMethod()]
        public void TestSimple1By2Problem()
        {
            Position p = new Position();
            p.BlackPieces = Position.Sq3 | Position.Sq7 | Position.Sq2;
            p.WhitePieces = Position.Sq16 | Position.Sq19 | Position.Sq14;

            Search s = new Search();
            s.Quiescense = Search.QMode.Full;
            s.Start(ref p, 1, Search.BLACK);

            showSearchResults(s, p);
            Assert.AreEqual("7-11", Utils.GetMove(p, s.BestMove));
        }

        [TestMethod()]
        public void TestFindsDeepWin()
        {
            Position p = new Position();
            p.BlackPieces = 0x2FCC;
            p.WhitePieces =0xFD840000.ToInt();
            p.Kings = 0x0;

            Search s = new Search();
            int value = s.Start(ref p, 16, Search.WHITE);

            showSearchResults(s, p);
            Assert.AreEqual("24-20", Utils.GetMove(p, s.BestMove));
            Assert.IsTrue(value >= 1000);
        }

        [TestMethod()]
        public void TestIsBestToSacrifice()
        {
            Position p = new Position();
            p.BlackPieces = 0x4006222;
            p.WhitePieces = (0x1171000 | Position.Sq22) & ~Position.Sq18;
            p.Kings = 0x0;

            Search s = new Search();
            int value = s.Start(ref p, 16, Search.WHITE);

            showSearchResults(s, p);
            Assert.AreEqual("22-18", Utils.GetMove(p, s.BestMove));
            Assert.IsTrue(value <= 1000);
        }

        [TestMethod()]
        public void TestFifthPosition()
        {
            Position p = new Position();
            p.SetupBlack(10, 11, 12, 13, 14);
            p.SetupWhite(19, 20, 21, 22, 27);

            Search s = new Search();
            int value = s.Start(ref p, 20, Search.WHITE);

            showSearchResults(s, p);
            Assert.AreEqual("20-16", Utils.GetMove(p, s.BestMove));
        }

        //::: . ::: . ::: . ::: . 
        // b ::: . ::: . ::: . :::
        //::: . ::: w ::: . ::: w 
        // . ::: w ::: . ::: . :::
        //::: . ::: . ::: . ::: b 
        // b ::: . ::: b ::: w :::
        //::: . ::: . ::: . ::: . 
        // . ::: . ::: . ::: . :::
        [TestMethod()]
        public void TestFifthPositionA()
        {
            Position p = new Position();
            p.SetupBlack(10, 12, 13, 28);
            p.SetupWhite(9, 19, 21, 23);

            Search s = new Search();
            int value = s.Start(ref p, 14, Search.BLACK);

            showSearchResults(s, p);
            Assert.AreEqual("10-14", Utils.GetMove(p, s.BestMove));
        }


        [TestMethod()]
        public void TestGlassonsGem()
        {
            Position p = new Position();
            p.SetupBlack(12, 18, 24, 22);
            p.SetupWhite(20, 31, 2, 15);
            p.SetupKings(22, 2, 15);

            Search s = new Search();
            int value = s.Start(ref p, 16, Search.WHITE);

            showSearchResults(s, p);
            Assert.AreEqual("15-19", Utils.GetMove(p, s.BestMove));
            //Assert.GreaterOrEqual(value, 1000);
        }

        [TestMethod()]
        public void TestGlassonsGemPv4()
        {
            Position p = new Position();
            p.BlackPieces = 0x80220800.ToInt();
            p.WhitePieces = 0x4048002;
            p.Kings = 0x80240002.ToInt();
            Search s = new Search();
            int value = s.Start(ref p, 12, Search.WHITE);

            showSearchResults(s, p);
            
            Assert.IsTrue(new []{"2-6", "2-7"}.Contains(Utils.GetMove(p, s.BestMove)));
            Assert.IsTrue(value >= 1000);
        }


        [TestMethod()]
        public void TestBarkervsFreemanKelsoTrap()
        {
            Position p = new Position();
            p.SetupBlack(2, 6, 8, 9, 10, 12, 17, 20);
            p.SetupWhite(32, 30, 27, 24, 19, 18, 15, 13);

            Search s = new Search();
            s.Start(ref p, 22, Search.WHITE);

            showSearchResults(s, p);
            Assert.AreEqual("27-23", Utils.GetMove(p, s.BestMove));
        }

        [TestMethod()]
        public void TestWiswellProblem()
        {
            Position p = new Position();
            p.SetupBlack(2, 4, 9, 10, 12, 13, 14);
            p.SetupWhite(32, 30, 26, 23, 21, 20, 18);

            Search s = new Search();
            s.Start(ref p, 17, Search.WHITE);

            showSearchResults(s, p);
            Assert.AreEqual("32-28", Utils.GetMove(p, s.BestMove));
        }

        [TestMethod]
        public void Test2vs1PiecesDeepWin()
        {
            Position p = new Position();
            p.SetupBlack(4, 8);
            p.SetupWhite(29);

            Search s = new Search();
            int value = s.Start(ref p, 60, Search.BLACK);
            Assert.AreEqual(Search.WIN, value);            
        }

        [TestMethod(), Ignore]
        public void TestEasyButDeepProblem()
        {
            Position p = new Position();
            p.SetupBlack(3,8,12,17,20, 21, 25,28, 29,30, 31, 32);
            p.SetupWhite(15,18,19);
            p.SetupKings(15,18,19, 29,30,31,32);
            Search s = new Search();
            var value = s.Start(ref p, 35, Search.WHITE);

            showSearchResults(s, p);
            Assert.AreEqual(Search.WIN, value);
        }

        [TestMethod, Ignore]
        public void TestFirstPosition()
        {
            Position p = new Position();
            p.SetupBlack(14, 15);
            p.SetupWhite(30, 1);
            p.SetupKings(14, 15, 1);

            Search s = new Search();
            s.Start(ref p, 26, Search.BLACK);

            showSearchResults(s, p);
            Assert.AreEqual("32-28", Utils.GetMove(p, s.BestMove));
        }

    }
}

