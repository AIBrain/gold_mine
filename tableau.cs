using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GoldMine {

    public class Tableau : Container {
        /**
         * Drag all cards starting from the reference card.
         */

        public override Boolean CanDrop( List<Card> cards ) {
            if ( this.Children.Count > 0 ) {
                var lastTableau = ( Card )this.Children[ this.Children.Count - 1 ];
                var firstDrag = cards[ 0 ];

                return lastTableau.value - 1 == firstDrag.value &&
                       lastTableau.color != firstDrag.color;
            }

            // its empty, so any card is valid
            else {
                return true;
            }
        }

        public override void DragCards( Card refCard, List<Card> cardsDragging ) {
            var reached = false;

            foreach ( Card card in this.Children ) {
                if ( ReferenceEquals( card, refCard ) ) {
                    reached = true;
                }

                if ( reached ) {
                    cardsDragging.Add( card );
                }
            }
        }

        /**
         * Get the dimension box of the container (also considers its children).
         */

        public override Utilities.Box GetDimensionBox() {
            var box = new Utilities.Box {
                X = Canvas.GetLeft( this ),
                Y = Canvas.GetTop( this ),
                Width = this.ActualWidth,
                Height = this.ActualHeight
            };

            var lastCard = this.GetLast();

            // the last card may be outside the container dimensions, so need to consider that
            if ( lastCard != null ) {
                var point = lastCard.TranslatePoint( new Point( 0, 0 ), this );
                var combinedHeight = point.Y + lastCard.ActualHeight;

                if ( combinedHeight > box.Height ) {
                    box.Height = combinedHeight;
                }
            }

            return box;
        }

        /**
         * A card is droppable if either the tableau is empty, or if the last card in the tableau is one value above the first card being dropped, and they have alternating colors.
         */

        protected override Size ArrangeOverride( Size finalSize ) {
            var availableHeight = 1.4 * finalSize.Height;
            Double y = 0;
            var step = Math.Round( availableHeight * 0.15 );
            var count = this.InternalChildren.Count;
            var neededHeight = count * step;

            // if there's no space available for the default step, then calculate the possible step
            if ( neededHeight > availableHeight ) {
                step = Math.Floor( availableHeight / count );
            }

            foreach ( UIElement child in this.InternalChildren ) {
                child.Arrange( new Rect( new Point( 0, y ), child.DesiredSize ) );

                y += step;
            }

            return finalSize;
        }
    }
}