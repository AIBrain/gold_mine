using System;
using System.Windows;

namespace GoldMine {

    internal class Waste : Container {

        protected override Size ArrangeOverride( Size finalSize ) {
            Double x = 0;
            var step = Math.Round( finalSize.Width * 0.3 );
            var count = this.InternalChildren.Count;

            for ( var a = 0; a < count; a++ ) {
                var child = this.InternalChildren[ a ];

                child.Arrange( new Rect( new Point( x, 0 ), child.DesiredSize ) );

                // the last 3 cards are place with a x-offset, the others are stacked in each other
                if ( a >= count - 3 ) {
                    x += step;
                }
            }

            return finalSize;
        }
    }
}