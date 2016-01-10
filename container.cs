using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Expression.Media.Effects;

namespace GoldMine {

    public class Container : Panel {
        private ColorToneEffect _dropEffect;

        public Container() {
            this.Background = new SolidColorBrush( Color.FromRgb( 87, 129, 50 ) );

            this._dropEffect = new ColorToneEffect {
                DarkColor = Colors.Black,
                LightColor = Colors.CornflowerBlue
            };
        }

        /**
         * Get the dimension box of the container.
         */

        public void ApplyDropEffect() {
            this.Effect = this._dropEffect;
        }

        public virtual Boolean CanDrop( List<Card> cards ) {
            return false;
        }

        public virtual void DragCards( Card refCard, List<Card> cardsDragging ) {
            cardsDragging.Add( refCard );
        }

        public virtual Utilities.Box GetDimensionBox() {
            var box = new Utilities.Box {
                X = Canvas.GetLeft( this ),
                Y = Canvas.GetTop( this ),
                Width = this.ActualWidth,
                Height = this.ActualHeight
            };

            return box;
        }

        public Card GetLast() {
            if ( this.Children.Count > 0 ) {
                return ( Card )this.Children[ this.Children.Count - 1 ];
            }

            return null;
        }

        public void RemoveDropEffect() {
            this.Effect = null;
        }

        /**
         * Says if the given cards can be dropped unto this container or not.
         */
        /**
         * Populate the list with the cards to be dragged. The reference card is one where the drag started.
         */

        protected override Size MeasureOverride( Size availableSize ) {
            var panelDesiredSize = new Size();

            foreach ( UIElement child in this.InternalChildren ) {
                child.Measure( availableSize );
                panelDesiredSize = child.DesiredSize;
            }

            return panelDesiredSize;
        }
    }
}