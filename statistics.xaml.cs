using System;
using System.Windows;
using System.Windows.Input;

namespace GoldMine {

    public partial class Statistics {

        public Statistics() {
            InitializeComponent();

            this.UpdateUi();
            this.SetupKeyboardShortcuts();
        }

        private void CloseWindow( Object sender, RoutedEventArgs e ) {
            this.Close();
        }

        private void ResetStatistics( Object sender, RoutedEventArgs e ) {
            var result = MessageBox.Show( "Reset the statistics?", "Reset the statistics?", MessageBoxButton.OKCancel );

            if ( result == MessageBoxResult.OK ) {
                Data.ResetStatistics();
                this.UpdateUi();
            }
        }

        private void SetupKeyboardShortcuts() {

            // esc -- close the window
            var close = new RoutedCommand();
            close.InputGestures.Add( new KeyGesture( Key.Escape ) );
            CommandBindings.Add( new CommandBinding( close, this.CloseWindow ) );
        }

        private void UpdateUi() {
            var totalWins = Data.DATA.TotalWins;
            var bestTime = Data.DATA.BestTime;

            this.TotalWins.Text = Data.DATA.TotalWins.ToString();

            if ( bestTime == 0 ) {
                this.BestTime.Text = "---";
            }
            else {
                this.BestTime.Text = (( Int32 )bestTime).TimeToString();
            }
        }
    }
}