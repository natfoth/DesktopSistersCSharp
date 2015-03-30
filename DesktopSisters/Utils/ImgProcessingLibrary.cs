using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSistersCSharpForm.Utils
{
    public static class ImgProcLib
    {
        public static byte[] BlendColor(byte[] firstColor, byte[] secondColor, double ratio)
        {
            double revRatio = 1.0f - ratio;

            byte blue = (byte)(firstColor[0] * revRatio + secondColor[0] * ratio);
            var green = (byte)(firstColor[1] * revRatio + secondColor[1] * ratio);
            var red = (byte)(firstColor[2] * revRatio + secondColor[2] * ratio);
            var alpha = (byte)(firstColor[3] * revRatio + secondColor[3] * ratio);

            return new[] { blue, green, red, (byte)255 };
        }

        public static byte[] Merge(byte[] dest, byte[] src, double opacity = 1.0)
        {
            int srcAlpha = (int)(src[3] * opacity);

            if (srcAlpha <= 0)
                return dest;
            else if (srcAlpha == 255)
            {
                return src;
            }

            double newalpha = srcAlpha + (((255 - srcAlpha) * dest[3]) / 255.0);// / 255.0;
            int alpha255 = (int)((dest[3] * (255 - srcAlpha)) / 255.0);//  /255.0;

            newalpha = 1.0 / newalpha;

            var blue = (byte)((src[0] * srcAlpha + (dest[0] * alpha255)) * newalpha);
            var green = (byte)((src[1] * srcAlpha + (dest[1] * alpha255)) * newalpha);
            var red = (byte)((src[2] * srcAlpha + (dest[2] * alpha255)) * newalpha);

            return new[] { blue, green, red, (byte)255 };
        }
    }
}
