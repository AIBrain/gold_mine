using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GoldMine {

    public class Card : Image {
        public const Double OriginalHeight = 976;
        public const Double OriginalWidth = 672;
        public const Double Ratio = OriginalWidth / OriginalHeight;

        public Color color;

        public Suit suit;

        public Value value;

        public enum Color { black, red };

        public enum Suit { clubs, diamonds, hearts, spades };

        public enum Value { ace, two, three, four, five, six, seven, eight, nine, ten, jack, queen, king };

        public Card( Suit suit, Value value ) {
            this.suit = suit;
            this.value = value;

            if ( suit == Suit.clubs || suit == Suit.spades ) {
                this.color = Color.black;
            }
            else {
                this.color = Color.red;
            }

            this.ShowBack();
        }

        public void ShowBack() {
            var mainResources = Application.Current.Resources;

            this.Source = mainResources[ "card_back" ] as ImageSource;
        }

        public void showFront() {
            var mainResources = Application.Current.Resources;

            var imageName = this.value + "_of_" + this.suit;
            this.Source = mainResources[ imageName ] as ImageSource;
        }
    }
}