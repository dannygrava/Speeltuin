using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using PdnLib;

namespace PdnParserUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PdnGame _game;
        private ObservableCollection<PdnGame> _games;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnPdnLoadClick(object sender, RoutedEventArgs e)
        {
            _games = new ObservableCollection<PdnGame>();
            PdnParser parser = new PdnParser();
            try
            {
                var startTime = DateTime.Now;
                parser.OnStartNewGame += newGameStarted;
                parser.OnTagFound += tagFound;
                parser.OnBodyFound += bodyFound;

                parser.Parse(File.ReadAllText(txtFilename.Text));
                var parsingTime = (DateTime.Now - startTime).TotalSeconds;
                lstGames.ItemsSource = _games;
                StatusBarItem item = (StatusBarItem) statusBar.Items[0];
                item.Content = string.Format("{0} {1}", parsingTime, _games.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }

        private void bodyFound(object sender, BodyEventArgs e)
        {
            _game.Body = e.Value;
        }

        private void tagFound(object sender, TagEventArgs e)
        {
            switch(e.Name)
            {
                case "Black":
                    _game.Black = e.Value;
                    break;
                case "White":
                    _game.White = e.Value;
                    break;
                case "Date":
                    _game.Date = e.Value;
                    break;
                case "Event":
                    _game.Event = e.Value;
                    break;
                case "Result":
                    _game.Result = e.Value;
                    break;
            }
        }

        private void newGameStarted(object sender, EventArgs e)
        {
            _game = new PdnGame();
            _games.Add(_game);
        }
    }
}
