using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using ChLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestChLibMs
{
    [TestClass]
    public class CompleteMoveGeneratorTest
    {
        [TestMethod]
        public void TestManMoves()
        {
            foreach (MoveTestData testData in _moveData)
            {
                Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
                Position testPosition = new Position() { BlackPieces = testData.Square };
                int numMoves = MoveGenerator.GenerateMoves(ref testPosition, moves, Search.BLACK);
                Assert.AreEqual(testData.Results.Count(), numMoves, "Number of moves differ for: \n" + Utils.GetSquareNr(testData.Square));
                foreach (int result in testData.Results)
                {
                    Assert.IsTrue(moves.Any(m => m.BlackPieces == result && m.WhitePieces == 0), "Generated move not found: Black at " + Utils.GetSquareNr(testData.Square));
                }

                // Check with white
                testPosition.Flip();
                numMoves = MoveGenerator.GenerateMoves(ref testPosition, moves, Search.WHITE);
                Assert.AreEqual(testData.Results.Count(), numMoves, "Number of moves differ for: \n" + Utils.GetSquareNr(testData.Square));
                foreach (int result in testData.Results)
                {
                    Assert.IsTrue(moves.Any(m => m.WhitePieces == Bits.Reverse(result) && m.BlackPieces == 0), "not Found" + "Generated move not found: White at " + Utils.GetSquareNr(testData.Square));
                }
            }
        }

        [TestMethod]
        public void TestKingMoves()
        {
            foreach (MoveTestData testData in _moveData)
            {
                Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
                Position testPosition = new Position() { BlackPieces = testData.Square, Kings = testData.Square };
                int numMoves = MoveGenerator.GenerateMoves(ref testPosition, moves, Search.BLACK);
                Assert.AreEqual(testData.KingResults.Count(), numMoves, "Number of moves differ for: \n" + Utils.GetSquareNr(testData.Square));
                foreach (int result in testData.KingResults)
                {
                    Assert.IsTrue(moves.Any(m => m.BlackPieces == result && m.Kings == result && m.WhitePieces == 0), "Generated move not found: Black at " + Utils.GetSquareNr(testData.Square));
                }

                // Check with white
                testPosition.Flip();
                numMoves = MoveGenerator.GenerateMoves(ref testPosition, moves, Search.WHITE);
                Assert.AreEqual(testData.KingResults.Count(), numMoves, "Number of moves differ for: \n" + Utils.GetSquareNr(testData.Square));
                foreach (int result in testData.KingResults)
                {
                    Assert.IsTrue(moves.Any(m => m.WhitePieces == Bits.Reverse(result) && m.Kings == m.WhitePieces && m.BlackPieces == 0), "not Found" + "Generated move not found: White at " + Utils.GetSquareNr(testData.Square));
                }
            }
        }

        [TestMethod]
        public void TestJumpData()
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            foreach (JumpTestData testData in _jumpData)
            {
                int numMoves;
                if (!testData.KingOnly)
                {
                    // Check black
                    Position testBlack = testData.Position;
                    Position blackResult = testData.Result;
                    numMoves = MoveGenerator.GenerateMoves(ref testBlack, moves, Search.BLACK);
                    Assert.AreEqual(1, numMoves, "Black: " + Utils.GetMove(testBlack, blackResult));
                    Assert.AreEqual(blackResult, moves[0], "Black: " + Utils.GetMove(testBlack, blackResult));

                    // Check with white
                    Position testWhite = testData.Position;
                    testWhite.Flip();

                    Position whiteResult = testData.Result;
                    whiteResult.Flip();

                    numMoves = MoveGenerator.GenerateMoves(ref testWhite, moves, Search.WHITE);

                    Assert.AreEqual(1, numMoves, "White: " + Utils.GetMove(testWhite, whiteResult));
                    Assert.AreEqual(whiteResult, moves[0], "White: " + Utils.GetMove(testWhite, whiteResult));
                }
                // Check black king
                Position testBlackKings = testData.Position;
                testBlackKings.Kings = testBlackKings.BlackPieces;
                Position testBlackKingResult  = testData.Result;
                testBlackKingResult.Kings = testBlackKingResult.BlackPieces;

                numMoves = MoveGenerator.GenerateMoves(ref testBlackKings, moves, Search.BLACK);
                Assert.AreEqual(1, numMoves, "BlackKings: " + Utils.GetSquareNr(testBlackKings.BlackPieces) + " " + Utils.GetSquareNr(testBlackKingResult.BlackPieces));
                Assert.AreEqual(testBlackKingResult, moves[0], "BlackKings: " + Utils.GetSquareNr(testBlackKings.BlackPieces) + " " + Utils.GetSquareNr(testBlackKingResult.BlackPieces));
                // Check white king
                Position testWhiteKings = testBlackKings;
                testWhiteKings.Flip();
                Position testWhiteKingResult = testBlackKingResult;
                testWhiteKingResult.Flip();

                numMoves = MoveGenerator.GenerateMoves(ref testWhiteKings, moves, Search.WHITE);
                Assert.AreEqual(1, numMoves, "WhiteKings: " + Utils.GetSquareNr(testWhiteKings.WhitePieces) + " " + Utils.GetSquareNr(testWhiteKingResult.WhitePieces));
                Assert.AreEqual(testWhiteKingResult, moves[0], "WhiteKings: " + Utils.GetSquareNr(testWhiteKings.WhitePieces) + " " + Utils.GetSquareNr(testWhiteKingResult.WhitePieces));
            }
        }


        private readonly MoveTestData[] _moveData = new MoveTestData[]
            {
                new MoveTestData(Position.Sq1, new int[]{Position.Sq5, Position.Sq6}, new int[]{}), 
                new MoveTestData(Position.Sq2, new int[]{Position.Sq7, Position.Sq6}, new int[]{}), 
                new MoveTestData(Position.Sq3, new int[]{Position.Sq7, Position.Sq8}, new int[]{}), 
                new MoveTestData(Position.Sq4, new int[]{Position.Sq8}, new int[]{}), 
                new MoveTestData(Position.Sq5, new int[]{Position.Sq9}, new int[]{Position.Sq1}), 
                new MoveTestData(Position.Sq6, new int[]{Position.Sq9, Position.Sq10}, new int[]{Position.Sq1, Position.Sq2}), 
                new MoveTestData(Position.Sq7, new int[]{Position.Sq11, Position.Sq10}, new int[]{Position.Sq2, Position.Sq3}), 
                new MoveTestData(Position.Sq8, new int[]{Position.Sq11, Position.Sq12}, new int[]{Position.Sq3, Position.Sq4}),

                new MoveTestData(Position.Sq9,  new int[]{Position.Sq13, Position.Sq14}, new int[]{Position.Sq5, Position.Sq6}), 
                new MoveTestData(Position.Sq10, new int[]{Position.Sq15, Position.Sq14}, new int[]{Position.Sq6, Position.Sq7}), 
                new MoveTestData(Position.Sq11, new int[]{Position.Sq15, Position.Sq16}, new int[]{Position.Sq7, Position.Sq8}), 
                new MoveTestData(Position.Sq12, new int[]{Position.Sq16}, new int[]{Position.Sq8}), 
                new MoveTestData(Position.Sq13, new int[]{Position.Sq17}, new int[]{Position.Sq9}), 
                new MoveTestData(Position.Sq14, new int[]{Position.Sq18, Position.Sq17}, new int[]{Position.Sq9, Position.Sq10}), 
                new MoveTestData(Position.Sq15, new int[]{Position.Sq18, Position.Sq19}, new int[]{Position.Sq10, Position.Sq11}), 
                new MoveTestData(Position.Sq16, new int[]{Position.Sq20, Position.Sq19}, new int[]{Position.Sq11, Position.Sq12}), 

                new MoveTestData(Position.Sq17, new int[]{Position.Sq21, Position.Sq22}, new int[]{Position.Sq13, Position.Sq14}), 
                new MoveTestData(Position.Sq18, new int[]{Position.Sq22, Position.Sq23}, new int[]{Position.Sq14, Position.Sq15}), 
                new MoveTestData(Position.Sq19, new int[]{Position.Sq23, Position.Sq24}, new int[]{Position.Sq15, Position.Sq16}), 
                new MoveTestData(Position.Sq20, new int[]{Position.Sq24}, new int[]{Position.Sq16}),
                new MoveTestData(Position.Sq21, new int[]{Position.Sq25}, new int[]{Position.Sq17}), 
                new MoveTestData(Position.Sq22, new int[]{Position.Sq25, Position.Sq26}, new int[]{Position.Sq17, Position.Sq18}), 
                new MoveTestData(Position.Sq23, new int[]{Position.Sq26, Position.Sq27}, new int[]{Position.Sq18, Position.Sq19}), 
                new MoveTestData(Position.Sq24, new int[]{Position.Sq27, Position.Sq28}, new int[]{Position.Sq19, Position.Sq20}),                 

                new MoveTestData(Position.Sq25, new int[]{Position.Sq29, Position.Sq30}, new int[]{Position.Sq21, Position.Sq22}), 
                new MoveTestData(Position.Sq26, new int[]{Position.Sq30, Position.Sq31}, new int[]{Position.Sq22, Position.Sq23}), 
                new MoveTestData(Position.Sq27, new int[]{Position.Sq31, Position.Sq32}, new int[]{Position.Sq23, Position.Sq24}), 
                new MoveTestData(Position.Sq28, new int[]{Position.Sq32}, new int[]{Position.Sq24}),
                new MoveTestData(Position.Sq29, new int[]{}, new int[]{Position.Sq25}), 
                new MoveTestData(Position.Sq30, new int[]{}, new int[]{Position.Sq25, Position.Sq26}), 
                new MoveTestData(Position.Sq31, new int[]{}, new int[]{Position.Sq26, Position.Sq27}), 
                new MoveTestData(Position.Sq32, new int[]{}, new int[]{Position.Sq27, Position.Sq28}),                 
            };

            private readonly JumpTestData[] _jumpData = new JumpTestData[]
                {
                    new JumpTestData(Position.Sq1, Position.Sq6, Position.Sq10),                                                                 
                    new JumpTestData(Position.Sq2, Position.Sq6, Position.Sq9),                                                                 
                    new JumpTestData(Position.Sq2, Position.Sq7, Position.Sq11),                                                                 
                    new JumpTestData(Position.Sq3, Position.Sq7, Position.Sq10),                                                                 
                    new JumpTestData(Position.Sq3, Position.Sq8, Position.Sq12),                                                                 
                    new JumpTestData(Position.Sq4, Position.Sq8, Position.Sq11),                                                                 
                    new JumpTestData(Position.Sq5, Position.Sq9,  Position.Sq14),                                                                 
                    new JumpTestData(Position.Sq6, Position.Sq9,  Position.Sq13),                                                                 
                    new JumpTestData(Position.Sq6, Position.Sq10, Position.Sq15),                                                                 
                    new JumpTestData(Position.Sq7, Position.Sq10, Position.Sq14),                                                                 
                    new JumpTestData(Position.Sq7, Position.Sq11, Position.Sq16),                    
                    new JumpTestData(Position.Sq8, Position.Sq11, Position.Sq15),

                    new JumpTestData(Position.Sq9, Position.Sq14,  Position.Sq18),                                                                 
                    new JumpTestData(Position.Sq9, Position.Sq6,  Position.Sq2, true),
                    new JumpTestData(Position.Sq10, Position.Sq14, Position.Sq17),                                                                 
                    new JumpTestData(Position.Sq10, Position.Sq6, Position.Sq1, true),                                                                 
                    new JumpTestData(Position.Sq10, Position.Sq15, Position.Sq19),                                                                 
                    new JumpTestData(Position.Sq10, Position.Sq7, Position.Sq3, true),                                                                 
                    new JumpTestData(Position.Sq11, Position.Sq15, Position.Sq18),                                                                 
                    new JumpTestData(Position.Sq11, Position.Sq7, Position.Sq2, true),                                                                 
                    new JumpTestData(Position.Sq11, Position.Sq16, Position.Sq20),                                                                 
                    new JumpTestData(Position.Sq11, Position.Sq8, Position.Sq4, true),                                                                 
                    new JumpTestData(Position.Sq12, Position.Sq16, Position.Sq19),                                                                 
                    new JumpTestData(Position.Sq12, Position.Sq8, Position.Sq3, true),                                                                 

                    new JumpTestData(Position.Sq13, Position.Sq17, Position.Sq22),                                                                 
                    new JumpTestData(Position.Sq13, Position.Sq9, Position.Sq6, true),                                                                 
                    new JumpTestData(Position.Sq14, Position.Sq17, Position.Sq21),                                                                 
                    new JumpTestData(Position.Sq14, Position.Sq9, Position.Sq5, true),                                                                 
                    new JumpTestData(Position.Sq14, Position.Sq18, Position.Sq23),                                                                 
                    new JumpTestData(Position.Sq14, Position.Sq10, Position.Sq7, true),                                                                 
                    new JumpTestData(Position.Sq15, Position.Sq18, Position.Sq22),                                                                 
                    new JumpTestData(Position.Sq15, Position.Sq10, Position.Sq6, true),                                                                 
                    new JumpTestData(Position.Sq15, Position.Sq19, Position.Sq24),                    
                    new JumpTestData(Position.Sq15, Position.Sq11, Position.Sq8, true),                    
                    new JumpTestData(Position.Sq16, Position.Sq19, Position.Sq23),
                    new JumpTestData(Position.Sq16, Position.Sq11, Position.Sq7, true),
                    
                    new JumpTestData(Position.Sq17, Position.Sq22, Position.Sq26),                                                                 
                    new JumpTestData(Position.Sq17, Position.Sq14, Position.Sq10, true),                                                                 
                    new JumpTestData(Position.Sq18, Position.Sq22, Position.Sq25),                                                                 
                    new JumpTestData(Position.Sq18, Position.Sq14, Position.Sq9, true),                                                                 
                    new JumpTestData(Position.Sq18, Position.Sq23, Position.Sq27),                                                                 
                    new JumpTestData(Position.Sq18, Position.Sq15, Position.Sq11, true),                                                                 
                    new JumpTestData(Position.Sq19, Position.Sq23, Position.Sq26),                                                                 
                    new JumpTestData(Position.Sq19, Position.Sq16, Position.Sq12, true),                                                                 
                    new JumpTestData(Position.Sq19, Position.Sq24, Position.Sq28),
                    new JumpTestData(Position.Sq19, Position.Sq15, Position.Sq10, true),                                                                 
                    new JumpTestData(Position.Sq20, Position.Sq24, Position.Sq27),                                                                 
                    new JumpTestData(Position.Sq20, Position.Sq16, Position.Sq11, true),                                                                 
                    new JumpTestData(Position.Sq21, Position.Sq25, Position.Sq30),                                                                 
                    new JumpTestData(Position.Sq21, Position.Sq17, Position.Sq14, true),                                                                 
                    new JumpTestData(Position.Sq22, Position.Sq25, Position.Sq29),                                                                 
                    new JumpTestData(Position.Sq22, Position.Sq17, Position.Sq13, true),                                                                 
                    new JumpTestData(Position.Sq22, Position.Sq26, Position.Sq31),                                                                 
                    new JumpTestData(Position.Sq22, Position.Sq18, Position.Sq15, true),                                                                 
                    new JumpTestData(Position.Sq23, Position.Sq26, Position.Sq30),                                                                 
                    new JumpTestData(Position.Sq23, Position.Sq18, Position.Sq14, true),                                                                 
                    new JumpTestData(Position.Sq23, Position.Sq27, Position.Sq32),                    
                    new JumpTestData(Position.Sq23, Position.Sq19, Position.Sq16, true),                    
                    new JumpTestData(Position.Sq24, Position.Sq27, Position.Sq31),
                    new JumpTestData(Position.Sq24, Position.Sq19, Position.Sq15, true),
                    
                    new JumpTestData(Position.Sq25, Position.Sq22, Position.Sq18, true),
                    new JumpTestData(Position.Sq26, Position.Sq22, Position.Sq17, true),
                    new JumpTestData(Position.Sq26, Position.Sq23, Position.Sq19, true),
                    new JumpTestData(Position.Sq27, Position.Sq23, Position.Sq18, true),
                    new JumpTestData(Position.Sq27, Position.Sq24, Position.Sq20, true),
                    new JumpTestData(Position.Sq28, Position.Sq24, Position.Sq19, true),
                    new JumpTestData(Position.Sq29, Position.Sq25, Position.Sq22, true),
                    new JumpTestData(Position.Sq30, Position.Sq25, Position.Sq21, true),
                    new JumpTestData(Position.Sq30, Position.Sq26, Position.Sq23, true),
                    new JumpTestData(Position.Sq31, Position.Sq26, Position.Sq22, true),
                    new JumpTestData(Position.Sq31, Position.Sq27, Position.Sq24, true),
                    new JumpTestData(Position.Sq32, Position.Sq27, Position.Sq23, true),
                };

        class MoveTestData
        {
            public MoveTestData(int square, int[] results, int[] kingresults)
            {
                Square = square;
                Results = results;
                KingResults = results.Concat(kingresults).ToArray();
            }

            public int Square { get; private set; }
            public int[] Results { get; private set; }
            public int[] KingResults { get; private set; }
        }

        class JumpTestData
        {
            public JumpTestData(int blackStart, int whiteStart, int blackResult)
            {
                Position = new Position() {BlackPieces = blackStart, WhitePieces = whiteStart};
                Result = new Position() {BlackPieces = blackResult, WhitePieces = 0, Kings = blackResult & MoveGenerator.KINGS_ROW_BLACK};
            }

            public JumpTestData(int blackStart, int whiteStart, int blackResult, bool kingOnly) : this(blackStart, whiteStart, blackResult)
            {
                KingOnly = kingOnly;
            }

            public Position Position { get; private set; }
            public Position Result { get; private set; }
            public bool KingOnly { get; private set; }
        }

    }

}
