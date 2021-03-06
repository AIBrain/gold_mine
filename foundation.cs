﻿using System;
using System.Collections.Generic;
using System.Windows;

namespace GoldMine {

    public class Foundation : Container {
        /**
         * A card is droppable if:
         *     - Its an ace and the foundation is empty.
         *     - Its 1 value higher than the last card, and of the same suit.
         * Can only drop 1 card at a time.
         */

        public override Boolean CanDrop( List<Card> cards ) {
            if ( cards.Count == 1 ) {
                var card = cards[ 0 ];

                if ( card == null ) {
                    return false;
                }

                if ( this.Children.Count > 0 ) {
                    var last = ( Card )this.Children[ this.Children.Count - 1 ];

                    if ( card.value == last.value + 1 &&
                         card.suit == last.suit ) {
                        return true;
                    }
                }
                else if ( card.value == Card.Value.ace ) {
                    return true;
                }
            }

            return false;
        }

        protected override Size ArrangeOverride( Size finalSize ) {
            foreach ( UIElement child in this.InternalChildren ) {
                child.Arrange( new Rect( new Point( 0, 0 ), child.DesiredSize ) );
            }

            return finalSize;
        }
    }
}