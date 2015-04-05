using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DesktopSistersCSharpForm.Utils
{
    public class DesktopSistersRandom : RandomNumberGenerator
    {
        private RandomNumberGenerator _randNumberGenerator;

        public DesktopSistersRandom()
        {
            _randNumberGenerator = RandomNumberGenerator.Create();

            
        }

        public override void GetBytes(byte[] buffer)
        {
            _randNumberGenerator.GetBytes(buffer);
        }

        public double NextDouble()
        {
            byte[] b = new byte[4];
            _randNumberGenerator.GetBytes(b);
            return (double)BitConverter.ToUInt32(b, 0) / UInt32.MaxValue;
        }

        public int Next(int minValue, int maxValue)
        {
            return (int)Math.Round(NextDouble() * (maxValue - minValue - 1)) + minValue;
        }

        public int Next(int maxValue)
        {
            return Next(0, maxValue);
        }

    }
}
