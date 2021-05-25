using System;
using System.Windows.Media;

namespace Thismaker.Goro.Utilities
{
    internal struct HSLColor
    {
        private const int ShadowAdjustment = -333;
        private const int HighlightAdjustment = 500;

        private const int Range = 240;
        public const int HSLMax = Range;
        private const int RGBMax = 255;
        private const int Undefined = HSLMax * 2 / 3;

        private readonly int _hue;
        private readonly int _saturation;

        public HSLColor(Color color)
        {
            int r = color.R;
            int g = color.G;
            int b = color.B;
            int max, min;
            int sum, dif;
            int Rdelta, Gdelta, Bdelta;  // intermediate value: % of spread from max

            // calculate lightness
            max = Math.Max(Math.Max(r, g), b);
            min = Math.Min(Math.Min(r, g), b);
            sum = max + min;

            Luminosity = ((sum * HSLMax) + RGBMax) / (2 * RGBMax);

            dif = max - min;
            if (dif == 0)
            {
                // r=g=b --> achromatic case
                _saturation = 0;
                _hue = Undefined;
            }
            else
            {
                // chromatic case
                _saturation = Luminosity <= (HSLMax / 2)
                    ? ((dif * HSLMax) + (sum / 2)) / sum
                    : ((dif * HSLMax) + (2 * RGBMax - sum) / 2) / (2 * RGBMax - sum);

                Rdelta = (((max - r) * (HSLMax / 6)) + (dif / 2)) / dif;
                Gdelta = (((max - g) * (HSLMax / 6)) + (dif / 2)) / dif;
                Bdelta = (((max - b) * (HSLMax / 6)) + (dif / 2)) / dif;

                if (r == max)
                {
                    _hue = Bdelta - Gdelta;
                }
                else if (g == max)
                {
                    _hue = (HSLMax / 3) + Rdelta - Bdelta;
                }
                else
                {
                    // B == cMax
                    _hue = (2 * HSLMax / 3) + Gdelta - Rdelta;
                }

                if (_hue < 0)
                {
                    _hue += HSLMax;
                }

                if (_hue > HSLMax)
                {
                    _hue -= HSLMax;
                }
            }
        }

        public int Luminosity { get; }

        public int Saturation { get { return _saturation; } }

        public int Hue { get { return _hue; } }

        public Color Darker(float percDarker)
        {
            int zeroLum = NewLuma(ShadowAdjustment, true);
            return ColorFromHSL(_hue, _saturation, zeroLum - (int)(zeroLum * percDarker));
        }

        public override bool Equals(object o)
        {
            if (!(o is HSLColor))
            {
                return false;
            }

            HSLColor c = (HSLColor)o;
            return _hue == c._hue &&
                   _saturation == c._saturation &&
                   Luminosity == c.Luminosity;
        }

        public static bool operator ==(HSLColor a, HSLColor b) => a.Equals(b);

        public static bool operator !=(HSLColor a, HSLColor b) => !a.Equals(b);

        public override int GetHashCode() => HashCode.Combine(Hue, Saturation, Luminosity);

        public Color Lighter(float percentLighter)
        {
            int zeroLum = Luminosity;
            int oneLum = NewLuma(HighlightAdjustment, true);
            return ColorFromHSL(_hue, _saturation, zeroLum + (int)((oneLum - zeroLum) * percentLighter));
        }

        private int NewLuma(int n, bool scale)
        {
            return NewLuma(Luminosity, n, scale);
        }

        private int NewLuma(int luminosity, int n, bool scale)
        {
            if (n == 0)
            {
                return luminosity;
            }

            if (scale)
            {
                return n > 0
                    ? (int)((luminosity * (1000 - n) + (Range + 1L) * n) / 1000)
                    : luminosity * (n + 1000) / 1000;
            }

            luminosity += (int)((long)n * Range / 1000);

            if (luminosity < 0)
            {
                return 0;
            }
            else if (luminosity > HSLMax)
            {
                return HSLMax;
            }

            return luminosity;
        }

        public  static Color ColorFromHSL(int hue,  int saturation, int luminosity)
        {
            byte r, g, b;
            int magic1, magic2;

            if (saturation == 0)
            {
                // achromatic case
                r = g = b = (byte)(luminosity * RGBMax / HSLMax);
                if (hue != Undefined)
                {
                    /* ERROR */
                }
            }
            else
            {
                // chromatic case
                if (luminosity <= (HSLMax / 2))
                {
                    magic2 = ((luminosity * (HSLMax + saturation)) + (HSLMax / 2)) / HSLMax;
                }
                else
                {
                    magic2 = luminosity + saturation - (((luminosity * saturation) + (HSLMax / 2)) / HSLMax);
                }

                magic1 = 2 * luminosity - magic2;

                // get RGB, change units from HLSMax to RGBMax
                r = (byte)((HueToRGB(magic1, magic2, hue + HSLMax / 3) * RGBMax + (HSLMax / 2)) / HSLMax);
                g = (byte)((HueToRGB(magic1, magic2, hue) * RGBMax + (HSLMax / 2)) / HSLMax);
                b = (byte)((HueToRGB(magic1, magic2, hue - HSLMax / 3) * RGBMax + (HSLMax / 2)) / HSLMax);
            }
            return Color.FromRgb(r, g, b);
        }

        public static int HueToRGB(int n1, int n2, int hue)
        {
            // range check: note values passed add/subtract thirds of range

            /* The following is redundant for WORD (unsigned int) */
            if (hue < 0)
            {
                hue += HSLMax;
            }

            if (hue > HSLMax)
            {
                hue -= HSLMax;
            }

            // return r, g, or b value from this tridrant
            if (hue < (HSLMax / 6))
            {
                return n1 + (((n2 - n1) * hue + (HSLMax / 12)) / (HSLMax / 6));
            }

            if (hue < (HSLMax / 2))
            {
                return n2;
            }

            if (hue < (HSLMax * 2 / 3))
            {
                return n1 + (((n2 - n1) * ((HSLMax * 2 / 3) - hue) + (HSLMax / 12)) / (HSLMax / 6));
            }
            else
            {
                return n1;
            }
        }
    }
}

