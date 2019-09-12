using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChLib;
using ChLib.EndGameEvaluators;
using ChLib.Evaluators;
using ChLibUI.Properties;

namespace ChLibUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Search _searcher = new Search();
        private Position _current;
        private readonly BackgroundWorker _worker = new BackgroundWorker();
        private readonly ObservableCollection<HistoryItem> _historyItems = new ObservableCollection<HistoryItem>();

        public MainWindow()
        {
            InitializeComponent();
            lstHistoryItems.ItemsSource = _historyItems;
            _worker.DoWork += computeMove;
            _worker.RunWorkerCompleted += updateDisplay;

            Left = Settings.Default.MainWindowLeft;
            Top = Settings.Default.MainWindowTop;
            //_searcher.Evaluate = TwoByOneEvaluator.Evaluate;

            // first position
            _current.SetupBlack(12, 32);
            _current.SetupWhite(10, 19);
            _current.SetupKings(32, 10, 19);

            // bug pos
            //unchecked { p.BlackPieces = (int)0x80080; p.WhitePieces = (int)0x80040000; p.Kings = (int)0x80040080; }

            //// 2 vs 1
            //_current = new Position();
            //_current.SetupBlack(4, 29);
            //_current.SetupWhite(1);
            //_current.SetupKings(4,29,1);

            //// 3 vs 2
            //_current = new Position();
            //_current.SetupBlack(4, 29, 32);
            //_current.SetupWhite(1, 5);
            //_current.SetupKings(4, 5, 29, 1, 32);

            //_current = new Position();
            //_current.SetupBlack(14, 15);
            //_current.SetupWhite(30, 1);
            //_current.SetupKings(14, 15, 1);

            //_current.SetupBlack(1,2,4,5,6,8,10,11,16,15);
            //_current.SetupWhite(13,18,20,22,23,26,28,30,31,32);
            //_current.SetupKings();

            addToHistory(new Position(), _current, _enginecolor^1);
        }

        private void updateDisplay(object sender, RunWorkerCompletedEventArgs e)
        {
            txtOutput.Text = Utils.GetSearchStatistics(_searcher, _current);
            addToHistory(_current, _searcher.BestMove, _enginecolor);
            _current = _searcher.BestMove;
            txtDiagram.Text = _current.ToString();
            Mouse.OverrideCursor = null;
        }

        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            txtDiagram.Text = _current.ToString();
        }

        private string _inputBuffer = "";
        private void windowTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Source == txtLevel)
            {
                return;
            }

            if (e.Text == "\r")
            {
                handleMoveInput();
                return;
            }

            if (e.Text.ToCharArray().All(c => Char.IsDigit(c) || c == '-' || c == ' ' || c == 'x'))
            {
                _inputBuffer = _inputBuffer + e.Text;
                txtInput.Text = _inputBuffer;
            }
        }

        private void windowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                if (_inputBuffer.Length >= 1)
                {
                    _inputBuffer = _inputBuffer.Substring(0, _inputBuffer.Length - 1);
                    txtInput.Text = _inputBuffer;
                }
            }

            if (e.Key == Key.S)
            {
                swapColors();
            }
        }

        private void handleMoveInput()
        {
            if (_worker.IsBusy)
            {
                return;
            }

            if (isValidInput(txtInput.Text))
            {
                txtDiagram.Text = _current.ToString();
                _inputBuffer = "";
                txtInput.Text = _inputBuffer;
                Mouse.OverrideCursor = Cursors.Wait;
                _worker.RunWorkerAsync();
            }
            else
            {
                _inputBuffer = "";
                txtInput.Text = "???";
            }
        }

        private void computeMove(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            _searcher.Start2(ref _current, 1 << _level, _enginecolor);
            //_searcher.Start(ref _current, 22, _enginecolor);
            //_searcher.Quiescense = Search.QMode.Full;
        }

        private int _enginecolor = MoveGenerator.BLACK;
        private int _level = 19;

        private bool isValidInput(string input)
        {
            string exp = @"(\d{1,2})[- x](\d{1,2})";
            var m = Regex.Match(input, exp);
            if (!m.Success)
                return false;

            int from;
            if (!int.TryParse(m.Groups[1].Value, out from))
                return false;
            int to;
            if (!int.TryParse(m.Groups[2].Value, out to))
                return false;

            return applyMove(from, to);
        }

        private bool applyMove(int from, int to)
        {
            Position[] moves = new Position[MoveGenerator.MAX_LEGAL_MOVES];
            var numMoves = MoveGenerator.GenerateMoves(ref _current, moves, _enginecolor ^ 1);
            for (int i = 0; i < numMoves; i++)
            {
                int a;
                if (_enginecolor == MoveGenerator.WHITE)
                {
                    a = (moves[i].BlackPieces ^ _current.BlackPieces);
                }
                else
                {
                    a = (moves[i].WhitePieces ^ _current.WhitePieces);
                }

                int b = (1 << (from - 1)) | (1 << (to - 1));
                if (a == b)
                {
                    addToHistory(_current, moves[i], _enginecolor^1);
                    _current = moves[i];
                    return true;
                }
            }
            return false;
        }

        private void addToHistory(Position old, Position current, int color)
        {
            string move = old.IsEmpty()? current.ShowFen(color) : Utils.GetMove(old, current);
            var historyItem = new HistoryItem(move, current, color);
            _historyItems.Add(historyItem);
            lstHistoryItems.SelectedItem = historyItem;
            lstHistoryItems.ScrollIntoView(historyItem);
            _searcher.StorePosition(current, color);
        }

        private void windowClosed(object sender, EventArgs e)
        {
            Settings.Default.MainWindowLeft = Left;
            Settings.Default.MainWindowTop = Top;
            Settings.Default.Save();
        }

        private void newGameClick(object sender, RoutedEventArgs e)
        {
            _current = new Position
                           {
                               BlackPieces = 0xFFF,
                               WhitePieces = 0xFFF00000.ToInt()
                           };
            _enginecolor = MoveGenerator.WHITE;
            txtDiagram.Text = _current.ToString();
            addToHistory(new Position(), _current, _enginecolor ^ 1);
            _searcher.ClearHistory();
        }

        private void swapColorsClick(object sender, RoutedEventArgs e)
        {
            swapColors();
        }

        private void swapColors()
        {
            if (_worker.IsBusy)
                return;

            _enginecolor ^= 1;
            Mouse.OverrideCursor = Cursors.Wait;
            _worker.RunWorkerAsync();
        }


        private void executeHistoryItemCopy(object sender, ExecutedRoutedEventArgs e)
        {
            if (lstHistoryItems.SelectedItem is HistoryItem )
                Clipboard.SetText(((HistoryItem) lstHistoryItems.SelectedItem).DebugPosition);
        }

        private void evaluatorClick(object sender, RoutedEventArgs e)
        {
            if (_worker.IsBusy)
                return;

            string evaluator = ((MenuItem) e.OriginalSource).Tag.ToString();
            switch(evaluator)
            {
                case "Basic":
                    _searcher.Evaluate = BasicEvaluator.Evaluate;
                    break;
                case "Reve64":
                    _searcher.Evaluate = Reve64Evaluator.Evaluate;
                    break;
                case "ICheckers":
                    _searcher.Evaluate = ICheckersEvaluator.Evaluate;
                    break;
                case "PubliCake":
                    _searcher.Evaluate = PubliCakeEvaluator.Evaluate;
                    break;
                case "ChkKit":
                    _searcher.Evaluate = ChkKitEvaluator.Evaluate;
                    break;
                case "SimpleCheckers":
                    _searcher.Evaluate = SimpleCheckersEvaluator.Evaluate;
                    break;
                case "Saitek":
                    _searcher.Evaluate = SaitekEvaluator.Evaluate;
                    break;
                case "GuiCheckers":
                    _searcher.Evaluate = GuiCheckersEvaluator.Evaluate;
                    break;
            }
            MenuItem parent = (MenuItem) sender;
            foreach (MenuItem menuItem in parent.Items)
            {
                menuItem.IsChecked = false;
            }
            ((MenuItem) e.OriginalSource).IsChecked = true;
        }

        private void txtLevelTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!int.TryParse(txtLevel.Text, out _level))
                _level = 19;
        }
    }

    public class HistoryItem
    {
        public HistoryItem(string move, Position position, int color)
        {
            Move = move;
            Position = position;
            ColorToMove = color;
        }

        public string Move { get; private set; }
        public Position Position { get; private set; }
        public int ColorToMove { get; private set; }
        public string DebugPosition { get { return string.Format("p.BlackPieces=0x{0:X}; p.WhitePieces=0x{1:X}; p.Kings=0x{2:X};", Position.BlackPieces, Position.WhitePieces, Position.Kings); } }
    }
}
