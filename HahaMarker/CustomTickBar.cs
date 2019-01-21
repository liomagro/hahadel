using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace HahaMarker
{
    class CustomTickBar : TickBar
    {
        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            double num = this.Maximum - this.Minimum;
            double y = this.ReservedSpace * 0.5;
            FormattedText formattedText = null;
            double x = 0;
            for (double i = 0; i <= num; i += this.TickFrequency)
            {
                formattedText = new FormattedText(i.ToString(), new CultureInfo("ru-RU"), FlowDirection.LeftToRight,
                    new Typeface("Verdana"), 8, Brushes.Black);
                if (this.Minimum == i)
                    x = 0;
                else
                    x += this.ActualWidth / (num / this.TickFrequency);
                dc.DrawText(formattedText, new Point(x, 10));
            }
        }
    }
}
