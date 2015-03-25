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

            return (double) d;
        }
    }
}
