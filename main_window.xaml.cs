using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GoldMine {

    public partial class MainWindow {
        private readonly List<Card> _cards = new List<Card>();

        private readonly List<Container> _droppableElements = new List<Container>();
        private readonly List<Foundation> _foundations = new List<Foundation>();
        private readonly Stock _stock;
        private readonly List<Tableau> _tableaus = new List<Tableau>();
        private readonly Timer _timer;
        private readonly Waste _waste;
        private Drag _drag;
        private UInt32 _secondsPassed;

        public MainWindow() {
            InitializeComponent();

            this._drag.CardsDragging = new List<Card>();
            this._timer = new Timer( 1000 );
            this._timer.Elapsed += this.OnTimeElapsed;

            this.SetupKeyboardShortcuts();
            Data.Load();

            // initialize all the game elements
            this._stock = new Stock();
            this._stock.MouseUp += this.OnStockMouseUp;
            this.MainCanvas.Children.Add( this._stock );

            this._waste = new Waste();
            this.MainCanvas.Children.Add( this._waste );

            for ( var a = 0; a < 4; a++ ) {
                var foundation = new Foundation();

                this.MainCanvas.Children.Add( foundation );
                this._droppableElements.Add( foundation );
                this._foundations.Add( foundation );
            }

            for ( var a = 0; a < 7; a++ ) {
                var tableau = new Tableau();

                this.MainCanvas.Children.Add( tableau );
                this._droppableElements.Add( tableau );
                this._tableaus.Add( tableau );
            }

            foreach ( Card.Suit suit in Enum.GetValues( typeof( Card.Suit ) ) ) {
                foreach ( Card.Value value in Enum.GetValues( typeof( Card.Value ) ) ) {
                    var card = new Card( suit, value );

                    card.MouseDown += this.onMouseDown;
                    card.MouseMove += this.onMouseMove;
                    card.MouseUp += this.onMouseUp;

                    this._cards.Add( card );
                }
            }

            this.StartGame();
        }

        private Utilities.Box CardsDimension( List<Card> cards ) {
            var firstCard = cards[ 0 ];
            return new Utilities.Box {
                X = Canvas.GetLeft( firstCard ),
                Y = Canvas.GetTop( firstCard ),
                Width = firstCard.ActualWidth,
                Height = firstCard.ActualHeight + Drag.Diff * ( cards.Count - 1 )
            };
        }

        private void CheckGameEnd() {
            var cardCount = this._cards.Count;
            var foundationCount = 0;

            foreach ( var foundation in this._foundations ) {
                foundationCount += foundation.Children.Count;
            }

            // game has ended
            if ( cardCount == foundationCount ) {
                this._timer.Stop();

                var best = Data.OneMoreWin( this._secondsPassed );
                var message = String.Format( "You Win!\nTime: {0}", Utilities.TimeToString( ( Int32 )this._secondsPassed ) );

                if ( this._secondsPassed == best ) {
                    message += "\nYou beat your best time!";
                }

                MessageBox.Show( message, "Game Over!", MessageBoxButton.OK );
                this.StartGame();
            }
        }

        private Container CollisionDetection( List<Card> cards ) {
            var cardsBox = this.CardsDimension( cards );
            Container colliding = null;
            Double collidingArea = 0;

            for ( var a = 0; a < this._droppableElements.Count; a++ ) {
                var container = this._droppableElements[ a ];

                if ( container != this._drag.OriginalContainer && container.CanDrop( cards ) ) {
                    var containerBox = container.GetDimensionBox();

                    var area = Utilities.CalculateIntersectionArea( cardsBox, containerBox );

                    if ( area > collidingArea ) {
                        collidingArea = area;
                        colliding = container;
                    }
                }
            }

            return colliding;
        }

        private Boolean IsCardDraggable( Card card ) {
            var parent = card.Parent;

            if ( parent is Stock ) {
                return false;
            }

            // the last card is draggable, the others aren't
            if ( parent is Waste ) {
                if ( this._waste.Children.Count != 0 ) {
                    var last = this._waste.Children[ this._waste.Children.Count - 1 ];

                    if ( last != card ) {
                        return false;
                    }
                }
            }

            return true;
        }

        private void MoveCards( List<Card> cards, Container container ) {
            if ( this._drag.HighlightedContainer != null ) {
                this._drag.HighlightedContainer.RemoveDropEffect();
                this._drag.HighlightedContainer = null;
            }

            foreach ( var card in cards ) {
                var parent = card.Parent as Panel;
                parent.Children.Remove( card );
                container.Children.Add( card );
            }

            this._drag.OriginalContainer = null;
            this._drag.CardsDragging.Clear();
            this._drag.IsDragging = false;
        }

        private void NewGameClick( Object sender, RoutedEventArgs e ) {
            this.StartGame();
        }

        private void onMouseDown( Object sender, MouseButtonEventArgs e ) {
            var card = ( Card )sender;
            var parent = card.Parent as Container;

            if ( !this.IsCardDraggable( card ) ) {
                return;
            }

            if ( e.ClickCount == 2 ) {
                this.SendToFoundation( parent );
                return;
            }

            if ( this._drag.IsDragging == true ) {
                this.MoveCards( this._drag.CardsDragging, this._drag.OriginalContainer );
                return;
            }

            this._drag.IsDragging = true;
            this._drag.ClickPosition = Mouse.GetPosition( card );
            this._drag.OriginalContainer = parent;
            parent.DragCards( card, this._drag.CardsDragging );

            foreach ( var dragCard in this._drag.CardsDragging ) {
                parent.Children.Remove( dragCard );
                this.MainCanvas.Children.Add( dragCard );
            }

            this.PositionCards( this._drag.CardsDragging, e );
        }

        private void onMouseMove( Object sender, MouseEventArgs e ) {
            if ( this._drag.IsDragging ) {
                if ( e.LeftButton == MouseButtonState.Released ) {
                    this.MoveCards( this._drag.CardsDragging, this._drag.OriginalContainer );
                }
                else {
                    this.PositionCards( this._drag.CardsDragging, e );
                }
            }
        }

        private void onMouseUp( Object sender, MouseButtonEventArgs e ) {
            if ( !this._drag.IsDragging ) {
                return;
            }

            var container = this.CollisionDetection( this._drag.CardsDragging );

            if ( container != null ) {
                this.MoveCards( this._drag.CardsDragging, container );
                this.CheckGameEnd();
            }

            // wasn't dropped on any container, so its not a valid drag operation. return to the original container
            else {
                this.MoveCards( this._drag.CardsDragging, this._drag.OriginalContainer );
            }
        }

        private void OnSizeChange( Object sender, SizeChangedEventArgs e ) {
            this.PositionResizeElements();
        }

        private void OnStateChange( Object sender, EventArgs e ) {
            this.PositionResizeElements();
        }

        private void OnStockMouseUp( Object sender, MouseButtonEventArgs e ) {
            var count = this._stock.Children.Count;

            for ( var a = 0; a < 3 && count > 0; a++ ) {
                var lastPosition = count - 1;

                var card = ( Card )this._stock.Children[ lastPosition ];
                this._stock.Children.RemoveAt( lastPosition );
                this._waste.Children.Add( card );

                card.showFront();

                count = this._stock.Children.Count;
            }

            this.UpdateStockLeft();
        }

        private void OnTimeElapsed( Object source, ElapsedEventArgs e ) {
            this.Dispatcher.Invoke( () => {
                this._secondsPassed++;
                this.UpdateTimePassed();
            } );
        }

        private void OnWindowClosing( Object sender, System.ComponentModel.CancelEventArgs e ) {
            this._timer.Stop();
        }

        private void OpenAboutPage( Object sender, RoutedEventArgs e ) {
            System.Diagnostics.Process.Start( "https://bitbucket.org/drk4/gold_mine" );
        }

        private void OpenStatisticsWindow( Object sender, RoutedEventArgs e ) {
            var statistics = new Statistics();
            statistics.ShowDialog();
        }

        private void PositionCards( List<Card> cards, MouseEventArgs e ) {
            var position = e.GetPosition( this.MainCanvas );

            for ( var a = 0; a < cards.Count; a++ ) {
                Canvas.SetLeft( cards[ a ], position.X - this._drag.ClickPosition.X );
                Canvas.SetTop( cards[ a ], position.Y - this._drag.ClickPosition.Y + Drag.Diff * a );
            }

            var container = this.CollisionDetection( cards );

            if ( this._drag.HighlightedContainer != null ) {
                this._drag.HighlightedContainer.RemoveDropEffect();
                this._drag.HighlightedContainer = null;
            }

            if ( container != null ) {
                container.ApplyDropEffect();

                this._drag.HighlightedContainer = container;
            }
        }

        private void PositionResizeElements() {

            // the layout is a grid with 7 columns and 3 lines
            // each position has space for a card + margin
            // we calculate these values from the available window dimensions
            var canvasWidth = this.MainCanvas.ActualWidth;
            var canvasHeight = this.MainCanvas.ActualHeight;
            var positionWidth = canvasWidth / 7;
            var positionHeight = canvasHeight / 3;
            var availableCardWidth = positionWidth * 0.9;

            var cardHeight = positionHeight * 0.9;
            var cardWidth = cardHeight * Card.Ratio;

            if ( cardWidth > availableCardWidth ) {
                cardWidth = availableCardWidth;
                cardHeight = cardWidth / Card.Ratio;
            }

            var horizontalMargin = ( positionWidth - cardWidth ) / 2;   // divide by 2 since there's margin in both sides
            var verticalMargin = ( positionHeight - cardHeight ) / 2;

            // resize all the elements
            foreach ( var card in this._cards ) {
                card.Height = cardHeight;   // the image will maintain the aspect ratio, so only need to set one
            }

            foreach ( var tableau in this._tableaus ) {
                tableau.Width = cardWidth;
                tableau.Height = cardHeight;
            }

            foreach ( var foundation in this._foundations ) {
                foundation.Width = cardWidth;
                foundation.Height = cardHeight;
            }

            this._waste.Width = cardWidth;
            this._waste.Height = cardHeight;

            this._stock.Width = cardWidth;
            this._stock.Height = cardHeight;

            // position all the elements
            // add the stock element in the top left
            var left = horizontalMargin;
            var top = verticalMargin;

            Canvas.SetLeft( this._stock, left );
            Canvas.SetTop( this._stock, top );

            // add the waste element next to the stock
            left += cardWidth + horizontalMargin * 2;

            Canvas.SetLeft( this._waste, left );
            Canvas.SetTop( this._waste, top );

            // add the foundations in the top right corner (the foundation is to where the cards need to be stacked (starting on an ace until the king)
            left = canvasWidth - cardWidth - horizontalMargin;

            foreach ( var foundation in this._foundations ) {
                Canvas.SetLeft( foundation, left );
                Canvas.SetTop( foundation, top );

                left -= cardWidth + 2 * horizontalMargin;
            }

            // add the tableau piles (where you can move any card to)
            left = horizontalMargin;
            top += cardHeight + verticalMargin;

            foreach ( var tableau in this._tableaus ) {
                Canvas.SetLeft( tableau, left );
                Canvas.SetTop( tableau, top );

                left += cardWidth + 2 * horizontalMargin;
            }
        }

        private void RestartGame( Object sender, RoutedEventArgs e ) {
            this.StartGame( false );
        }

        private Boolean SendToFoundation( Container container ) {
            var last = container.GetLast();

            // need to have a list to work with the 'canDrop' function
            var cards = new List<Card> { last };

            foreach ( var foundation in this._foundations.Where( foundation => foundation.CanDrop( cards ) ) ) {
                this.MoveCards( cards, foundation );
                this.CheckGameEnd();
                return true;
            }

            return false;
        }

        private void SetupKeyboardShortcuts() {

            // ctrl + n -- start a new game
            var newGame = new RoutedCommand();
            newGame.InputGestures.Add( new KeyGesture( Key.N, ModifierKeys.Control ) );
            CommandBindings.Add( new CommandBinding( newGame, this.NewGameClick ) );

            // ctrl + r -- restart the game
            var restart = new RoutedCommand();
            restart.InputGestures.Add( new KeyGesture( Key.R, ModifierKeys.Control ) );
            CommandBindings.Add( new CommandBinding( restart, this.RestartGame ) );

            // ctrl + s -- open the statistics window
            var openStatistics = new RoutedCommand();
            openStatistics.InputGestures.Add( new KeyGesture( Key.S, ModifierKeys.Control ) );
            CommandBindings.Add( new CommandBinding( openStatistics, this.OpenStatisticsWindow ) );

            // ctrl + f -- try to move all the possible cards to the foundation
            var moveToFoundation = new RoutedCommand();
            moveToFoundation.InputGestures.Add( new KeyGesture( Key.F, ModifierKeys.Control ) );
            CommandBindings.Add( new CommandBinding( moveToFoundation, this.ToFoundationClick ) );

            // ctrl + a -- open the about webpage
            var openAbout = new RoutedCommand();
            openAbout.InputGestures.Add( new KeyGesture( Key.A, ModifierKeys.Control ) );
            CommandBindings.Add( new CommandBinding( openAbout, this.OpenAboutPage ) );
        }

        private void StartGame( Boolean shuffle = true ) {

            // disconnect the cards from their previous container
            foreach ( var card in this._cards ) {
                var parent = card.Parent as Panel;

                parent?.Children.Remove( card );
            }

            if ( shuffle ) {
                this._cards.Shuffle();
            }

            // add all the shuffled cards to the stock
            foreach ( var card in this._cards ) {
                card.ShowBack();
                this._stock.Children.Add( card );
            }

            this.UpdateStockLeft();
            this._timer.Stop();
            this._secondsPassed = 0;
            this.UpdateTimePassed();
            this._timer.Start();
        }

        private void ToFoundationClick( Object sender, RoutedEventArgs e ) {

            // if a card was moved to the foundation
            // we keep checking until there's no more possible moves
            Boolean moved;

            do
            {
                moved = this.SendToFoundation( this._waste );

                foreach (var tableau in this._tableaus.Where(this.SendToFoundation))
                {
                    moved = true;
                }
            } while ( moved == true );
        }

        private void UpdateStockLeft() {
            this.StockLeft.Text = "In stock: " + this._stock.Children.Count;
        }

        private void UpdateTimePassed() {
            this.TimePassed.Text = "Time: " + Utilities.TimeToString( ( Int32 )this._secondsPassed );
        }

        // data use for the drag and drop operation of cards
        public struct Drag {
            public const Int32 Diff = 25;

            // space between each card during the drag
            public List<Card> CardsDragging;

            public Point ClickPosition;
            public Container HighlightedContainer;
            public Boolean IsDragging;
            public Container OriginalContainer;    // original container before the drag occurred. if the drag isn't valid, we need to return the cards to the original place

            // when dragging a card on top of a container, highlight that container, and keep a reference to it (to know when to remove the highlight)
        }

        /**
         * Checks if the game has ended, and if so then show a message.
         * The game is over when all the cards are in the foundations.
         */
        /**
         * When we click on the stock, we move 3 cards to the waste.
         */
        /**
         * Try to send the last card of a container to a foundation.
         */
        /**
         * Calculates the intersection area between the reference element and the droppable elements, and returns the one where the area was higher.
         */
        /**
         * When the window is resized, we need to reposition the game elements.
         */
        /**
         * Position/resize all the elements in the right place (given the current width/height of the canvas).
         */
        /**
         * Finishes the drag operation, moving a list of cards to a container.
         */
        /**
         * Determine the dimensions of the cards stack.
         * Use the first card to determine the x/y/width.
         * The height is calculated from the number of cards.
         * The 'diff' is the space between each card.
         */
        /**
         * Depending on where the card is located, it may be draggable or not.
         */
        /**
         * Tries to move all the possible cards from the waste/tableau to the foundation.
         * Useful for the ending of a game, so that you don't have to manually move all the last cards.
         */
    }
}