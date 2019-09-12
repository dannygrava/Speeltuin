using System.Linq;
using ChLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestChLibMs
{
    
    
    /// <summary>
    ///This is a test class for MoveGeneratorTest and is intended
    ///to contain all MoveGeneratorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MoveGeneratorTest
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

        [TestMethod()]
        public void TestGenerateWhiteMoves()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.WhitePieces = Position.Sq22 | Position.Sq23;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.WHITE);
            Assert.AreEqual(4, generatedMoveCount);
            checkPositionPresent(moves, new Position(){WhitePieces = Position.Sq18 | Position.Sq23} );
            checkPositionPresent(moves, new Position() { WhitePieces = Position.Sq17 | Position.Sq23 });
            checkPositionPresent(moves, new Position() { WhitePieces = Position.Sq19 | Position.Sq22 });
            checkPositionPresent(moves, new Position() { WhitePieces = Position.Sq18 | Position.Sq22 });
            
            //Assert.AreEqual(Position.Empty, moves[0].Kings);
            //Assert.AreEqual((Position.Sq17 | Position.Sq23), moves[1].WhitePieces);
            //Assert.AreEqual((Position.Sq19 | Position.Sq22), moves[2].WhitePieces);
            //Assert.AreEqual((Position.Sq18 | Position.Sq22), moves[3].WhitePieces);
        }

        [TestMethod()]
        public void TestGenerateWhiteKingMoves()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.WhitePieces = Position.Sq22;
            position.Kings = Position.Sq22;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.WHITE);
            Assert.AreEqual(4, generatedMoveCount);
            Console.WriteLine(moves[0]);
            Assert.AreEqual(Position.Sq18, moves[0].WhitePieces, "Move#1 (white pc)");
            Assert.AreEqual(Position.Sq18, moves[0].Kings, "Move#1 (K)");
            Assert.AreEqual(Position.Sq17, moves[1].WhitePieces, "Move#2 (white pc)");
            Assert.AreEqual(Position.Sq17, moves[1].Kings, "Move#2 (K)");
            Assert.AreEqual(Position.Sq26, moves[2].WhitePieces, "Move#3 (white pc)");
            Assert.AreEqual(Position.Sq26, moves[2].Kings, "Move#3 (K)");
            Assert.AreEqual(Position.Sq25, moves[3].WhitePieces, "Move#4 (white pc)");
            Assert.AreEqual(Position.Sq25, moves[3].Kings, "Move#4 (K)");
        }

        [TestMethod()]
        public void TestWhiteCrowning()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.WhitePieces = Position.Sq6 | Position.Sq5;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.WHITE);
            Assert.AreEqual(3, generatedMoveCount);
            checkPositionPresent(moves, new Position() { WhitePieces = Position.Sq6 | Position.Sq1, Kings=Position.Sq1 });
            checkPositionPresent(moves, new Position() { WhitePieces = Position.Sq2 | Position.Sq5, Kings = Position.Sq2 });
            checkPositionPresent(moves, new Position() { WhitePieces = Position.Sq1 | Position.Sq5, Kings = Position.Sq1 });
        }

        [TestMethod()]
        public void TestGenerateBlackMoves()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq2 | Position.Sq11;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.BLACK);
            Assert.AreEqual(4, generatedMoveCount);
            Assert.AreEqual((Position.Sq6 | Position.Sq11), moves[0].BlackPieces, moves[0].ToString());
            Assert.AreEqual(Position.Empty, moves[0].Kings, moves[0].ToString());
            Assert.AreEqual((Position.Sq7 | Position.Sq11), moves[1].BlackPieces, moves[1].ToString());
            Assert.AreEqual((Position.Sq2 | Position.Sq15), moves[2].BlackPieces, moves[2].ToString());
            Assert.AreEqual((Position.Sq2 | Position.Sq16), moves[3].BlackPieces, moves[3].ToString());
        }

        [TestMethod()]
        public void TestGenerateBlackKingMoves()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq22;
            position.Kings = Position.Sq22;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.BLACK);
            Assert.AreEqual(4, generatedMoveCount);
            Console.WriteLine(moves[0]);
            Assert.AreEqual(Position.Sq26, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq26, moves[0].Kings, "Move#1 (K)" + moves[0]);
            Assert.AreEqual(Position.Sq25, moves[1].BlackPieces, "Move#2" + moves[1]);
            Assert.AreEqual(Position.Sq25, moves[1].Kings, "Move#2 (K)" + moves[1]);
            Assert.AreEqual(Position.Sq18, moves[2].BlackPieces, "Move#3 (pc)" + moves[2]);
            Assert.AreEqual(Position.Sq18, moves[2].Kings, "Move#3 (K)" + moves[2]);
            Assert.AreEqual(Position.Sq17, moves[3].BlackPieces, "Move#4 (pc)" + moves[3]);
            Assert.AreEqual(Position.Sq17, moves[3].Kings, "Move#4 (K)" + moves[3]);
        }

        [TestMethod()]
        public void TestBlackCrowning()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq28 | Position.Sq27;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.BLACK);
            Assert.AreEqual(3, generatedMoveCount);
            Assert.AreEqual(Position.Sq31 | Position.Sq28, moves[0].BlackPieces, "Move#3 (white pc)" + moves[0]);
            Assert.AreEqual(Position.Sq31, moves[0].Kings, "Move#3 (K)" + moves[2]);
            Assert.AreEqual(Position.Sq32 | Position.Sq28, moves[1].BlackPieces, "Move#1 (white pc)" + moves[1]);
            Assert.AreEqual(Position.Sq32, moves[1].Kings, "Move#1 (K)" + moves[1]);
            Assert.AreEqual(Position.Sq32 | Position.Sq27, moves[2].BlackPieces, "Move#2 (white pc)" + moves[2]);
            Assert.AreEqual(Position.Sq32, moves[2].Kings, "Move#2 (K)" + moves[2]);
        }

        #region Black jumps
        [TestMethod()]
        public void TestBlackSingleJumpDir4()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq11;
            position.WhitePieces = Position.Sq15;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.BLACK);
            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Sq18, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Empty, moves[0].WhitePieces, "Move#1" + moves[0]);
        }

        [TestMethod()]
        public void TestBlackSingleJumpDir3()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq22;
            position.WhitePieces = Position.Sq25;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.BLACK);
            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Sq29, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Empty, moves[0].WhitePieces, "Move#1" + moves[0]);
        }

        [TestMethod()]
        public void TestBlackSingleJumpDir5()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq1;
            position.WhitePieces = Position.Sq6;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.BLACK);
            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Sq10, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Empty, moves[0].WhitePieces, "Move#1" + moves[0]);
        }

        [TestMethod()]
        public void TestBlackKingJumpDir4Backwards()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq22;
            position.Kings = Position.Sq22;
            position.WhitePieces = Position.Sq18;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.BLACK);
            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Sq15, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq15, moves[0].Kings, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Empty, moves[0].WhitePieces, "Move#1" + moves[0]);
        }

        [TestMethod()]
        public void TestBlackKingJumpDir3Backwards()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq27;
            position.Kings = Position.Sq27;
            position.WhitePieces = Position.Sq24;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.BLACK);
            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Sq20, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq20, moves[0].Kings, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Empty, moves[0].WhitePieces, "Move#1" + moves[0]);
        }

        [TestMethod()]
        public void TestBlackKingJumpDir5Backwards()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq30;
            position.Kings = Position.Sq30;
            position.WhitePieces = Position.Sq25;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.BLACK);
            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Sq21, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq21, moves[0].Kings, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Empty, moves[0].WhitePieces, "Move#1" + moves[0]);
        }

        [TestMethod()]
        public void TestBlackDoubleJump()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq2;
            position.WhitePieces = Position.Sq6 | Position.Sq14;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.BLACK);
            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Sq18, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Empty, moves[0].WhitePieces, "Move#1" + moves[0]);
        }

        [TestMethod()]
        public void TestBlackTripleJump()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq2;
            position.WhitePieces = Position.Sq6 | Position.Sq14 | Position.Sq23;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.BLACK);
            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Sq27, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Empty, moves[0].WhitePieces, "Move#1" + moves[0]);
        }

        [TestMethod()]
        public void TestBlackMultipleJumps()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq3;
            position.WhitePieces = Position.Sq8 | Position.Sq7 | Position.Sq14 | Position.Sq15 | Position.Sq24;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.BLACK);
            Console.WriteLine(moves[0]);
            Console.WriteLine(moves[1]);
            Console.WriteLine(moves[2]);
            Assert.AreEqual(3, generatedMoveCount);

            Assert.AreEqual(Position.Sq17, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq8 | Position.Sq15 | Position.Sq24, moves[0].WhitePieces, "Move#1" + moves[0]);

            Assert.AreEqual(Position.Sq28, moves[1].BlackPieces, "Move#2" + moves[1]);
            Assert.AreEqual(Position.Sq8 | Position.Sq14, moves[1].WhitePieces, "Move#2" + moves[1]);

            Assert.AreEqual(Position.Sq12, moves[2].BlackPieces, "Move#3" + moves[2]);
            Assert.AreEqual(Position.Sq7 | Position.Sq14 | Position.Sq15 | Position.Sq24, moves[2].WhitePieces, "Move#3" + moves[2]);

            //Assert.AreEqual(Board.Empty, moves[0].WhitePieces, "Move#1" + moves[0]);
        }

        [TestMethod()]
        public void TestBlackKingBackwardsJumps()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq21;
            position.Kings = Position.Sq21 | Position.Sq25;
            position.WhitePieces = Position.Sq25 | Position.Sq26;

            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.BLACK);

            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Sq23, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq23, moves[0].Kings, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Empty, moves[0].WhitePieces, "Move#1" + moves[0]);
        }

        [TestMethod()]
        public void TestBlackCrowningJump()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq21;
            position.WhitePieces = Position.Sq25 | Position.Sq26;

            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.BLACK);

            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Sq30, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq30, moves[0].Kings, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq26, moves[0].WhitePieces, "Move#1" + moves[0]);
        }

        [TestMethod()]
        public void TestBlackNoJump()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq21;
            position.WhitePieces = Position.Sq26;

            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.BLACK);
            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Sq25, moves[0].BlackPieces);
        }

        [TestMethod()]
        public void TestBlackJumpBoardNotChanged()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq21;
            position.WhitePieces = Position.Sq25;

            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.BLACK);

            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Sq21, position.BlackPieces);
            Assert.AreEqual(Position.Sq25, position.WhitePieces);
            Assert.AreEqual(Position.Empty, position.Kings);
        }
        #endregion

        #region White Jumps
        [TestMethod()]
        public void TestWhiteSingleJumpDir4()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq18;
            position.WhitePieces = Position.Sq22;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.WHITE);
            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Empty, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq15, moves[0].WhitePieces, "Move#1" + moves[0]);
        }

        [TestMethod()]
        public void TestWhiteSingleJumpDir3()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq8;
            position.WhitePieces = Position.Sq11;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.WHITE);
            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Empty, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq4, moves[0].WhitePieces, "Move#1" + moves[0]);
        }

        [TestMethod()]
        public void TestWhiteSingleJumpDir5()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq27;
            position.WhitePieces = Position.Sq32;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.WHITE);
            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Empty, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq23, moves[0].WhitePieces, "Move#1" + moves[0]);
        }

        [TestMethod()]
        public void TestWhiteKingJumpDir4Backwards()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq18;
            position.Kings = Position.Sq15;
            position.WhitePieces = Position.Sq15;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.WHITE);
            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Empty, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq22, moves[0].Kings, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq22, moves[0].WhitePieces, "Move#1" + moves[0]);
        }

        [TestMethod()]
        public void TestWhiteKingJumpDir3Backwards()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq9;
            position.Kings = Position.Sq6;
            position.WhitePieces = Position.Sq6;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.WHITE);
            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Empty, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq13, moves[0].Kings, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq13, moves[0].WhitePieces, "Move#1" + moves[0]);
        }

        [TestMethod()]
        public void TestWhiteKingJumpDir5Backwards()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq8;
            position.Kings = Position.Sq3;
            position.WhitePieces = Position.Sq3;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.WHITE);
            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Empty, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq12, moves[0].Kings, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq12, moves[0].WhitePieces, "Move#1" + moves[0]);
        }

        [TestMethod()]
        public void TestWhiteDoubleJump()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq22 | Position.Sq14;
            position.WhitePieces = Position.Sq26;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.WHITE);
            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Empty, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq10, moves[0].WhitePieces, "Move#1" + moves[0]);
        }

        [TestMethod()]
        public void TestWhiteTripleJump()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq25 | Position.Sq18 | Position.Sq10;
            position.WhitePieces = Position.Sq29;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.WHITE);
            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Empty, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq6, moves[0].WhitePieces, "Move#1" + moves[0]);
        }

        [TestMethod()]
        public void TestWhiteMultipleJumps()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq27 | Position.Sq26 | Position.Sq17 | Position.Sq18 | Position.Sq11;
            position.WhitePieces = Position.Sq31;
            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.WHITE);

            Assert.AreEqual(3, generatedMoveCount);
            checkPositionPresent(moves, new Position { WhitePieces = Position.Sq24, BlackPieces = Position.Sq26 | Position.Sq17 | Position.Sq18 | Position.Sq11 });
            checkPositionPresent(moves, new Position { WhitePieces = Position.Sq13, BlackPieces = Position.Sq27 | Position.Sq18 | Position.Sq11 });
            checkPositionPresent(moves, new Position { WhitePieces = Position.Sq8, BlackPieces = Position.Sq27 | Position.Sq17 });
        }

        [TestMethod()]
        public void TestWhiteKingBackwardsJumps()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq25 | Position.Sq26;
            position.Kings = Position.Sq21 | Position.Sq25;
            position.WhitePieces = Position.Sq21;

            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.WHITE);

            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Empty, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq23, moves[0].Kings, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq23, moves[0].WhitePieces, "Move#1" + moves[0]);
        }

        [TestMethod()]
        public void TestWhiteCrowningJump()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq8 | Position.Sq7;
            position.WhitePieces = Position.Sq12;

            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.WHITE);

            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Sq7, moves[0].BlackPieces, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq3, moves[0].Kings, "Move#1" + moves[0]);
            Assert.AreEqual(Position.Sq3, moves[0].WhitePieces, "Move#1" + moves[0]);
        }

        [TestMethod()]
        public void TestWhiteNoJump()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq18;
            position.WhitePieces = Position.Sq12;

            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.WHITE);
            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Sq8, moves[0].WhitePieces);
        }

        [TestMethod()]
        public void TestWhiteJumpBoardNotChanged()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            Position position = new Position();
            position.BlackPieces = Position.Sq17;
            position.WhitePieces = Position.Sq21;

            var generatedMoveCount = MoveGenerator.GenerateMoves(ref position, moves, MoveGenerator.WHITE);

            Assert.AreEqual(1, generatedMoveCount);
            Assert.AreEqual(Position.Sq17, position.BlackPieces);
            Assert.AreEqual(Position.Sq21, position.WhitePieces);
            Assert.AreEqual(Position.Empty, position.Kings);
        }
        #endregion

        [TestMethod()]
        public void TestProblemPosition1()
        {
            Position p = new Position();
            p.BlackPieces = Position.Sq16;
            p.WhitePieces = Position.Sq23 | Position.Sq19 | Position.Sq27;
            var moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            int numMoves = MoveGenerator.GenerateMoves(ref p, moves, MoveGenerator.BLACK);
            Assert.AreEqual(1, numMoves, "Move found!?!" + moves[0]);
            Assert.AreEqual(Position.Sq20, moves[0].BlackPieces);
        }

        [TestMethod()]
        public void TestProblemPosition2()
        {
            Position p = new Position();
            p.BlackPieces = Position.Sq2 | Position.Sq18 | Position.Sq7 | Position.Sq27;
            p.WhitePieces = Position.Sq13 | Position.Sq15 | Position.Sq5 | Position.Sq19;
            p.Kings = Position.Sq5 | Position.Sq18;
            var moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];

            int numMoves = MoveGenerator.GenerateMoves(ref p, moves, MoveGenerator.WHITE);
            Assert.AreEqual(6, numMoves);
            Position expectedPosition = p;
            expectedPosition.WhitePieces = p.WhitePieces & ~Position.Sq13 | Position.Sq9;
            checkPositionPresent(moves, expectedPosition);
        }

        [TestMethod()]
        public void TestProblemPosition2B()
        {
            Position p = new Position();
            p.WhitePieces = Position.Sq13 | Position.Sq5;
            p.Kings = Position.Sq5;
            var moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];

            int numMoves = MoveGenerator.GenerateMoves(ref p, moves, MoveGenerator.WHITE);
            Assert.AreEqual(3, numMoves);
            Position expectedPosition = p;
            expectedPosition.WhitePieces = p.WhitePieces & ~Position.Sq13 | Position.Sq9;
            checkPositionPresent(moves, expectedPosition);
        }

        private void checkPositionPresent(Position[] moves, Position position)
        {
            if (!moves.Contains(position))
            {
                var positions = moves.Where(p => !p.IsEmpty()).ToArray();
                Console.WriteLine(string.Format("Following {0} moves found: ", positions.Count()));
                foreach (var p in positions)
                {
                    Console.WriteLine(p.ToString());
                }
                Assert.Fail("Position not found: " + position.ToString());
            }
        }

        
    }
}
