using System;
using System.Collections.Generic;

namespace GoldMine {

    public static class Utilities {

        public static Boolean BoxBoxCollision( Box one, Box two ) {
            return !(
                    one.X > two.X + two.Width ||
                    one.X + one.Width < two.X ||
                    one.Y > two.Y + two.Height ||
                    one.Y + one.Height < two.Y
                );
        }

        public static Double CalculateIntersectionArea( Box one, Box two ) {
            var left = Math.Max( one.X, two.X );
            var right = Math.Min( one.X + one.Width, two.X + two.Width );
            var bottom = Math.Min( one.Y + one.Height, two.Y + two.Height );
            var top = Math.Max( one.Y, two.Y );

            // if there's an intersection
            if ( left < right && bottom > top ) {
                return ( right - left ) * ( bottom - top );
            }

            return 0;
        }

        public static void Shuffle<TType>(this List<TType> list ) {
            var currentIndex = list.Count;
            var random = new Random();

            // while there's still elements to shuffle
            while ( currentIndex != 0 ) {

                // pick a remaining element
                var randomIndex = random.Next( currentIndex );
                currentIndex--;

                // swap it with the current element
                var temporaryValue = list[ currentIndex ];
                list[ currentIndex ] = list[ randomIndex ];
                list[ randomIndex ] = temporaryValue;
            }
        }

        public static String TimeToString( Int32 seconds ) {
            var minute = 60;
            var minutesCount = 0;

            while ( seconds >= minute ) {
                minutesCount++;
                seconds -= minute;
            }

            if ( minutesCount != 0 ) {
                return String.Format( "{0}m {1}s", minutesCount, seconds );
            }
            else {
                return String.Format( "{0}s", seconds );
            }
        }

        public struct Box {
            public Double Height;
            public Double Width;
            public Double X;
            public Double Y;
        }

        /**
         * Returns the string representation of the seconds value.
         * For example: "1m 2s" or "30s"
         */
    }
}