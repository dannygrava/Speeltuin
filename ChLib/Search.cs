using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ChLib.Evaluators;

namespace ChLib
{
    public sealed class Search
    {
        // Special search results
        public const int DRAW = 0;
        public const int LOSS = -WIN;
        public const int WIN = 30000;
        private const int UNKNOWN = -1;
        public const int DRAW_BY_REPETITION = 2; // hmm... is draw by repetition to be prefered over an equal position?

        public const int BLACK = MoveGenerator.BLACK;
        public const int WHITE = MoveGenerator.WHITE;
        public const int MAX_SUPPORTED_DEPTH = 100;

        
        private const int ALFA_BETA_FRAME = 300;
        private int _actualDepth;
        private int _color;
        private int _totalSearches;
        private TimeSpan _searchTime;
        private int _peakDepth;
        private int _nominalDepth;
        private int _lastValue;
        private int _hashHits;
        private int _moveSorts;
        private readonly Position[][] _moves;
        private readonly Position[] _currentLine = new Position[MAX_SUPPORTED_DEPTH];
        private readonly Position[] _bestmoves = new Position[MAX_SUPPORTED_DEPTH];
        private readonly List<Position> _blackmoves = new List<Position>();
        private readonly List<Position> _whitemoves = new List<Position>();
        private int _maxSearches;

        public Search()
        {
            Quiescense = QMode.Partial;
            Evaluate = BasicEvaluator.Evaluate;
            //Evaluate = Reve64Evaluator.Evaluate;
            //Evaluate = ICheckersEvaluator.Evaluate;
            //Evaluate = PubliCakeEvaluator.Evaluate;
            //Evaluate = ChkKitEvaluator.Evaluate;
            //Evaluate = GuiCheckersEvaluator.Evaluate;
            _moves = new Position[MAX_SUPPORTED_DEPTH][];
            for (int i = 0; i < MAX_SUPPORTED_DEPTH; i++) 
            {
                _moves[i] = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            }
        }
        public enum QMode { None, Partial, Full };
        public QMode Quiescense { get; set; }
        public Evaluator Evaluate { get; set; }

        public void ClearHistory()
        {
            _blackmoves.Clear();
            _whitemoves.Clear();
        }

        public void StorePosition (Position p, int color)
        {
            if (color == BLACK)
                _blackmoves.Add(p);
            if (color == WHITE)
                _whitemoves.Add(p);
        }

        public int Start2(ref Position p, int maxSearches, int color)
        {
            // todo dg: meer info van vorige iteratie bewaren, om te voorkomen dat naar verouderde info wordt gekeken (BestValue, SearchDepth etc.)
            initStats();
            _nominalDepth = 0;
            _maxSearches = maxSearches;
            DateTime startTime = DateTime.Now;
            _color = color;
            _actualDepth = 0;

            int numMoves = MoveGenerator.GenerateMoves(ref p, _moves[_actualDepth], _color);
            if (numMoves == 0)
                return LOSS;

            if (numMoves == 1)
            {
                _bestmoves[0] = _moves[0][0];
                return UNKNOWN;
            }

            HashTable.Reset();

            // Main flow
            int alfa = LOSS - 1;
            int beta = WIN + 1;

            int searchDepth = 2;
            int value = UNKNOWN;
            Position lastBestMove = _bestmoves[0];

            Debug.WriteLine("Depth;Move;Value;Nodes;Hash hits;Move sorts;Time;Fail;");
            while (_totalSearches < _maxSearches)
            {
                value = negaScout(p, searchDepth, alfa, beta);
                if (_totalSearches > _maxSearches)
                {
                    break;
                }

                if (value <= alfa || value >= beta)
                {
                    log(p, startTime, value, searchDepth, true);

                    // fail high of fail low situation, research with full alfa beta window
                    alfa = LOSS - 1;
                    beta = WIN + 1;
                    value = negaScout(p, searchDepth, alfa, beta);
                    if (_totalSearches > _maxSearches)
                    {
                        break;
                    }
                }
                lastBestMove = _bestmoves[0];
                _lastValue = value;
                log(p, startTime, value, searchDepth, false);
                _nominalDepth = searchDepth;

                if (Math.Abs(value) == WIN)
                {
                    break;
                }

                // set alfabeta window
                alfa = value - ALFA_BETA_FRAME;
                beta = value + ALFA_BETA_FRAME;

                searchDepth +=2;
            }

            _bestmoves[0] = lastBestMove;
            _searchTime = DateTime.Now - startTime;
            return value;
        }


        public int Start(ref Position p, int depth, int color)
        {
            initStats();
            _nominalDepth = depth;
            DateTime startTime = DateTime.Now;
            _color = color;
            _actualDepth = 0;
            _maxSearches = int.MaxValue;

            int numMoves = MoveGenerator.GenerateMoves(ref p, _moves[_actualDepth], _color);
            if (numMoves == 0)
                return LOSS;

            if (numMoves == 1)
            {
                _bestmoves[0] = _moves[0][0];
                return UNKNOWN;
            }

            HashTable.Reset();

            // Main flow
            int alfa = LOSS - 1;
            int beta = WIN + 1;

            int searchDepth = depth==1 ? 1 : 2;
            int value = UNKNOWN;
            Debug.WriteLine("Depth;Move;Value;Nodes;Hash hits;Move sorts;Time;Fail;");
            while (searchDepth <= depth)
            {
                Debug.WriteLine("Start search with depth=" + searchDepth);
                Debug.WriteLine(p);
                value = negaScout(p, searchDepth, alfa, beta);
                if (value <= alfa || value >= beta)
                {
                    log(p, startTime, value, searchDepth, true);
                    
                    // fail high of fail low situation, research with full alfa beta window
                    alfa = LOSS - 1;
                    beta = WIN + 1;
                    value = negaScout(p, searchDepth, alfa, beta);
                }
                log(p, startTime, value, searchDepth, false);

                if (Math.Abs(value) == WIN)
                {
                    break;
                }
                
                // set alfabeta window
                alfa = value - ALFA_BETA_FRAME;
                beta = value + ALFA_BETA_FRAME;
                
                if (depth - searchDepth == 1)
                {
                    searchDepth++;
                }
                else
                {
                    searchDepth += 2;
                }
            }

            _searchTime = DateTime.Now - startTime;
            _lastValue = value;
            return value;
        }

        private void log(Position p, DateTime startTime, int value, int searchDepth, bool fail)
        {
            string failure = fail ? "* fail" : "";
            Debug.WriteLine(string.Format("{0};{1};{2};{4};{5};{7};{3};{6};", searchDepth,
                                          Utils.GetMove(p, _bestmoves[0]), value, Math.Truncate((DateTime.Now - startTime).TotalMilliseconds),
                                          _totalSearches, _hashHits, failure, _moveSorts));
        }

        private void initStats()
        {
            _totalSearches = 0;
            _peakDepth = 0;
            _hashHits = 0;
            _searchTime = new TimeSpan(0);
            _moveSorts = 0;
        }

        // http://en.wikipedia.org/wiki/Negascout
        // http://chessprogramming.wikispaces.com/NegaScout
        private int negaScout(Position p, int depth, int alpha, int beta)
        {
            if (_totalSearches > _maxSearches)
                return alpha;

            _totalSearches++;
            if (_actualDepth > _peakDepth)
            {
                _peakDepth = _actualDepth;
            }
            _currentLine[_actualDepth] = p;

            if (_actualDepth != 0 && (p.Kings & p.WhitePieces) != 0 && (p.Kings & p.BlackPieces) != 0 && checkForRepetition(p))
            {
                //Debug.WriteLine(new string('.', _actualDepth) + "Draw by repetition");
                if (_color == MoveGenerator.BLACK)
                    return DRAW_BY_REPETITION;
                return -DRAW_BY_REPETITION;
            }

            if ((depth <= 0 && isQuiet(p)) || _actualDepth == MAX_SUPPORTED_DEPTH - 1)
            {
                return Evaluate(p, _color, alpha, beta);
            }

            int numMoves = MoveGenerator.GenerateMoves(ref p, _moves[_actualDepth], _color);
            if (numMoves == 0)
            {
                return LOSS;
            }

            int bestValue = alpha;
            int newBeta = beta;
            int value=0;


            if (HashTable.RetrievePosition(ref p, _color, ref value, depth, bestValue, newBeta, ref _bestmoves[_actualDepth]))
            {
                _hashHits++;                
                return value;
            }

            // move ordering
            if (!_bestmoves[_actualDepth].IsEmpty())
            {
                for (int i = 0; i < numMoves; i++)
                {
                    if (_moves[_actualDepth][i] == _bestmoves[_actualDepth])
                    {
                        if (i != 0)
                        {
                            _moves[_actualDepth][i] = _moves[_actualDepth][0];
                            _moves[_actualDepth][0] = _bestmoves[_actualDepth];
                            _moveSorts++;
                        }
                        break;
                    }
                }

                //if (_actualDepth >= 2)
                //{
                //    for (int i = 1; i < numMoves; i++)
                //    {
                //        if (_moves[_actualDepth][i] == _bestmoves[_actualDepth - 2])
                //        {
                //            if (i != 1)
                //            {
                //                _moves[_actualDepth][i] = _moves[_actualDepth][1];
                //                _moves[_actualDepth][1] = _bestmoves[_actualDepth - 2];
                //                _moveSorts++;
                //            }
                //            break;
                //        }
                //    }
                //}

            }

            //if (depth >= 4)
            //{
            //    for (int i = 1; i < numMoves; i++)
            //    {
            //        _moves[_actualDepth][i].Value = -HashTable.RetrieveValue(ref _moves[_actualDepth][i], _color ^ 1);
            //        //_moves[_actualDepth][i].Value = -Evaluate(_moves[_actualDepth][i], _color ^ 1, Search.LOSS, Search.WIN); 
            //    }
            //    _comparer.Color = _color ^ 1;
            //    Array.Sort(_moves[_actualDepth], 1, numMoves - 1, _comparer);
            //}

            for (int i = 0; i < numMoves; i++ )
            {
                if (_totalSearches > _maxSearches)
                    return bestValue;

                Position move = _moves[_actualDepth][i];                
                _actualDepth++;
                _color = _color ^ 1;

                //Debug.WriteLine(currentLineToString(move) + " depth=" + depth);
                int score = -negaScout(move, depth - 1, -newBeta, -bestValue);
                //Debug.WriteLine(currentLineToString(move) + ": " + score);

                if (bestValue < score && score < beta && i > 0)
                {
                    //Debug.WriteLine("Fail: " + currentLineToString(move) + " depth=" + depth);
                    score = -negaScout(move, depth - 1, -beta, -bestValue);
                    //Debug.WriteLine(currentLineToString(move) + ": " + score);
                }

                _actualDepth--;
                _color = _color ^ 1;
                
                if (score > bestValue )
                {
                    bestValue = score;
                    _bestmoves[_actualDepth] = move;
                }

                if (bestValue >= beta)
                {
                    //Debug.WriteLine(new string('.', _actualDepth) + "Beta cutoff");
                    break;
                }
                newBeta = bestValue + 1;
            }
            HashTable.StorePosition(ref p, _color, bestValue, depth, alpha, beta, _bestmoves[_actualDepth]);
            return bestValue;
        }

        private string currentLineToString(Position move)
        {
            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < _actualDepth-1; j++)
            {
                sb.Append(Utils.GetMove(_currentLine[j], _currentLine[j + 1]) + ',');
            }
            sb.Append(Utils.GetMove(_currentLine[_actualDepth-1], move));
            return sb.ToString();
        }

        private bool checkForRepetition(Position position)
        {
            if (position.WhitePieces == 64 && position.BlackPieces == 12 && position.Kings == 76)
            {
                
            }

            for (int i = _actualDepth - 2; i >= 0; i-=2)
            {
                if (_currentLine[i].Kings == 0)
                    return false;
                if (position == _currentLine[i])
                {
                    //Debug.WriteLine(position.ToString());
                    return true;
                }
            }

            List<Position> moves = _color == MoveGenerator.BLACK ? _whitemoves : _blackmoves;
            for (int j = moves.Count - 1; j >= 0; j--)
            {
                if (moves[j].Kings == 0)
                    return false;
                if (position == moves[j])
                    return true;
            }
            return false;
        }

        public Position[] GetBestlineFromTranspositionTable()
        {
            Position [] bestline = new Position[MAX_SUPPORTED_DEPTH];
            HashTable.RetrieveBestline(ref _bestmoves[0], _color, bestline);
            return bestline;
        }

        public int GetVariations(ref Position p, out Position[] moves, out int[] values, out string[] valueTypes)
        {
            moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            values = new int[MoveGenerator.MAX_LEGAL_MOVES];
            valueTypes = new string[MoveGenerator.MAX_LEGAL_MOVES];
            int numMoves = MoveGenerator.GenerateMoves(ref p, moves, _color);
            for (int i = 0; i < numMoves; i++ )
            {
                HashTable.RetrievePosition(ref moves[i], _color ^ 1, out values[i], out valueTypes[i]);
            }
            return numMoves;
        }


        private bool isQuiet(Position p)
        {
            if (Quiescense == QMode.Partial)
            {
                if (_color == WHITE)
                {
                    return MoveGenerator.GetJumpersBlack(p) == 0;
                }
                return MoveGenerator.GetJumpersWhite(p) == 0;
            }
            if (Quiescense == QMode.None)
                return true;
            return MoveGenerator.GetJumpersBlack(p) == 0 && MoveGenerator.GetJumpersWhite(p) == 0;
        }

        #region Search results & stats
        public int LastValue
        {
            get { return _lastValue; }
        }

        public Position BestMove
        {
            get { return _bestmoves[_actualDepth]; }
        }

        public int NominalDepth
        {
            get { return _nominalDepth; }
        }

        public int PeakDepth
        {
            get { return _peakDepth; }
        }

        public TimeSpan SearchTime
        {
            get { return _searchTime; }
        }

        public int TotalSearches
        {
            get { return _totalSearches; }
        }

        public int MoveSorts
        {
            get { return _moveSorts; }
        }
        #endregion
    }

    public delegate int Evaluator (Position position, int color, int alpha, int beta);     

    //public class PositionComparer : IComparer<Position>
    //{
    //    public int Color;
    //    public Evaluator Evaluate;
    //    public int Compare(Position x, Position y)
    //    {
    //        return y.Value-x.Value;
    //    }
    //}

    //public class PositionComparerByEval : IComparer<Position>
    //{
    //    public int Color;
    //    public Evaluator Evaluate;
    //    public int Compare(Position x, Position y)
    //    {
    //        int valueX = Evaluate(x, Color, Search.LOSS, Search.WIN);
    //        x.Value = valueX;
    //        int valueY = Evaluate(x, Color, Search.LOSS, Search.WIN);
    //        y.Value = valueY;
    //        //return valueY - valueX;
    //        if (Color == Search.WHITE)
    //            return valueY - valueX;
    //        return valueX - valueY;
    //    }
    //}

}
