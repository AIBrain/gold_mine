using System;
using System.Collections.Generic;
using System.Media;
using System.Threading;
using GoldMine.Properties;

namespace GoldMine {

    public static class Utilities {

        public static ThreadLocal< SoundPlayer> SoundPlayers { get; } = new ThreadLocal<SoundPlayer>(() => new SoundPlayer(), true);

        public static Random Random { get; } = new Random();

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

        public static void Shuffle<TType>( this List<TType> list ) {
            var currentIndex = list.Count;

            SoundPlayers.Value.Stream = Resources.cardshuffle;
            SoundPlayers.Value.Play();

            // while there's still elements to shuffle
            while ( currentIndex != 0 ) {

                // pick a remaining element
                var randomIndex = Random.Next( currentIndex );
                currentIndex--;

                // swap it with the current element
                var temporaryValue = list[ currentIndex ];
                list[ currentIndex ] = list[ randomIndex ];
                list[ randomIndex ] = temporaryValue;
            }
        }

        public static String TimeToString( this Int32 seconds ) {
            const Int32 minute = 60;
            var minutesCount = 0;

            while ( seconds >= minute ) {
                minutesCount++;
                seconds -= minute;
            }

            if ( minutesCount != 0 ) {
                return $"{minutesCount}m {seconds}s";
            }
            else {
                return $"{seconds}s";
            }
        }

        public static String TimeToString( this UInt32 seconds ) {
            const UInt32 minute = 60;
            var minutesCount = 0;

            while ( seconds >= minute ) {
                minutesCount++;
                seconds -= minute;
            }

            if ( minutesCount != 0 ) {
                return $"{minutesCount}m {seconds}s";
            }
            else {
                return $"{seconds}s";
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