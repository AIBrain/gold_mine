﻿using System.Windows;


namespace GoldMine
    {
    class Waste : Container
        {
        protected override Size ArrangeOverride( Size finalSize )
            {
            int x = 0;
            int step = 25;
            int count = this.InternalChildren.Count;

            for (int a = 0 ; a < count ; a++)
                {
                var child = this.InternalChildren[ a ];

                child.Arrange( new Rect( new Point( x, 0 ), child.DesiredSize ) );

                    // the last 3 cards are place with a x-offset, the others are stacked in each other
                if ( a >= count - 3 )
                    {
                    x += step;
                    }
                }

            return finalSize;
            }
        }
    }
