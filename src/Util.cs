using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schluesselzahlen
{
    public class Util
    {
        public static int toInt(String s)
        {
            char[] zeichen = s.ToCharArray();
            int r = 0;

            for (int i = 0; i < zeichen.Length; i++)
            {
                r *= 10;
                switch (zeichen[i])
                {
                    case '0': r += 0; break;
                    case '1': r += 1; break;
                    case '2': r += 2; break;
                    case '3': r += 3; break;
                    case '4': r += 4; break;
                    case '5': r += 5; break;
                    case '6': r += 6; break;
                    case '7': r += 7; break;
                    case '8': r += 8; break;
                    case '9': r += 9; break;
                    default: r /= 10; break;
                }
            }
            return r;
        }
    }
}
