using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSistersCSharpForm
{
    static class Extensions
    {
        public static Double ToDouble(this DateTime datetime)
        {
            double d = datetime.Hour;
            var prct = (double)datetime.Minute/ 60.0;
            d = d + prct;
            d = Math.Round(d, 2, MidpointRounding.AwayFromZero);
            var secPrct = ((double) datetime.Second / 60) / 100.0;
            secPrct = Math.Round(secPrct, 4, MidpointRounding.AwayFromZero);
            d += secPrct;

            return (double) d;
        }

        public static byte[] ToByteArray(this System.Drawing.Color color)
        {
            return new[] {color.B, color.G, color.R, color.A};
        }
    }
}
