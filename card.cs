using System.Media;

namespace GoldMine {
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class Card : Image {
        public const Double OriginalHeight = 976;
        public const Double OriginalWidth = 672;
        public const Double Ratio = OriginalWidth / OriginalHeight;

        public readonly Color color;

        public readonly Suit suit;

        public readonly Value value;

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
            //Utilities.SoundPlayer = new SoundPlayer( Properties.Resources.cardflip );Utilities.SoundPlayer.PlaySync();

            this.Source = Application.Current.Resources[ "card_back" ] as ImageSource;
        }

        public void ShowFront()
        {
            Utilities.SoundPlayers.Value.Stream = Properties.Resources.cardflip;
            Utilities.SoundPlayers.Value.Play();

            var imageName = this.value + "_of_" + this.suit;
            this.Source = Application.Current.Resources[ imageName ] as ImageSource;
        }
    }
}