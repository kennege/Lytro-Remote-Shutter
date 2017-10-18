using System;

namespace UAM.Optics
{
    /// <summary>
    /// Represents a color in terms of red, green and blue channels, using 32 bits per channel.
    /// </summary>
    public struct ColorRgb128Float
    {
        /// <summary>
        /// R channel.
        /// </summary>
        public float R;
        /// <summary>
        /// G channel.
        /// </summary>
        public float G;
        /// <summary>
        /// B channel.
        /// </summary>
        public float B;
        
        #pragma warning disable 414 // assigned but unused field
        private float A;
        #pragma warning restore

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorRgb128Float"/> class using the specified color channel values.
        /// </summary>
        /// <param name="r">The red channel, <see cref="R"/> of the new color.</param>
        /// <param name="g">The blue channel <see cref="G"/> of the new color.</param>
        /// <param name="b">The blue channel, <see cref="B"/> of the new color.</param>
        public ColorRgb128Float(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;

            A = 0;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorRgb128Float"/> class using the same value for all channels.
        /// </summary>
        /// <param name="rgb">The value for all channels.</param>
        public ColorRgb128Float(float rgb)
        {
            R = G = B = rgb;
            A = 0;
        }

        private static float sRgbToScRgb(float val)
        {
            if (!(val > 0.0f))       // Handles NaN case too.
                return 0.0f;

            else if (val <= 0.04045f)
                return val / 12.92f;

            else if (val < 1.0f)
                return (float)Math.Pow(((double)val + 0.055) / 1.055, 2.4);

            else
                return 1.0f;
        }

        /// <summary>Adds two <see cref="ColorRgb128Float"/> structures.</summary>
        /// <param name="color1">The first <see cref="ColorRgb128Float" /> structure to add.</param>
        /// <param name="color2">The second <see cref="ColorRgb128Float" /> structure to add.</param>
        /// <returns>A new <see cref="ColorRgb128Float" /> structure whose color values are the results of the addition operation.</returns>
        public static ColorRgb128Float operator +(ColorRgb128Float color1, ColorRgb128Float color2)
        {
            return new ColorRgb128Float(color1.R + color2.R, color1.G + color2.G, color1.B + color2.B);
        }
        /// <summary>Subtracts a <see cref="ColorRgb128Float" /> structure from a <see cref="ColorRgb128Float" /> structure. </summary>
        /// <param name="color1">The <see cref="ColorRgb128Float" /> structure to be subtracted from.</param>
        /// <param name="color2">The <see cref="ColorRgb128Float" /> structure to subtract from <paramref name="color1" />.</param>
        /// <returns>A new <see cref="ColorRgb128Float" /> structure whose color values are the results of the subtraction operation.</returns>
        public static ColorRgb128Float operator -(ColorRgb128Float color1, ColorRgb128Float color2)
        {
            return new ColorRgb128Float(color1.R - color2.R, color1.G - color2.G, color1.B - color2.B);
        }
        /// <summary>Multiplies the red, blue, and green channels of the specified <see cref="ColorRgb128Float" /> structure by the specified value.</summary>
        /// <param name="color">The <see cref="ColorRgb128Float" /> to be multiplied.</param>
        /// <param name="coefficient">The value to multiply by.</param>
        /// <returns>A new <see cref="ColorRgb128Float" /> structure whose color values are the results of the multiplication operation.</returns>
        public static ColorRgb128Float operator *(ColorRgb128Float color, float coefficient)
        {
            return new ColorRgb128Float(color.R * coefficient, color.G * coefficient, color.B * coefficient);
        }
        /// <summary>Multiplies two <see cref="ColorRgb128Float" /> structures channel by channel.</summary>
        /// <param name="color">The first <see cref="ColorRgb128Float" /> structure to add.</param>
        /// <param name="coefficients">The second <see cref="ColorRgb128Float" /> structure to add.</param>
        /// <returns>A new <see cref="ColorRgb128Float" /> structure whose color values are the results of the multiplication operation channel by channel.</returns>
        public static ColorRgb128Float operator *(ColorRgb128Float color, ColorRgb128Float coefficients)
        {
            return new ColorRgb128Float(color.R * coefficients.R, color.G * coefficients.G, color.B * coefficients.B);
        }
        /// <summary>Divides the red, blue, and green channels of the specified <see cref="ColorRgb128Float" /> structure by the specified value.</summary>
        /// <param name="color">The <see cref="ColorRgb128Float" /> to be divided.</param>
        /// <param name="divider">The value to divide by.</param>
        /// <returns>A new <see cref="ColorRgb128Float" /> structure whose color values are the results of the division operation.</returns>
        public static ColorRgb128Float operator /(ColorRgb128Float color, float divider)
        {
            return new ColorRgb128Float(color.R / divider, color.G / divider, color.B / divider);
        }
        /// <summary>Divides a <see cref="ColorRgb128Float" /> structure by a <see cref="ColorRgb128Float" /> structure channel by channel.</summary>
        /// <param name="color">The <see cref="ColorRgb128Float" /> structure to be divided.</param>
        /// <param name="divider">The <see cref="ColorRgb128Float" /> structure to divide the <paramref name="color" />.</param>
        /// <returns>A new <see cref="ColorRgb128Float" /> structure whose color values are the results of the division operation channel by channel.</returns>
        public static ColorRgb128Float operator /(ColorRgb128Float color, ColorRgb128Float divider)
        {
            return new ColorRgb128Float(color.R / divider.R, color.G / divider.G, color.B / divider.B);
        }

        /// <summary>Adds two <see cref="ColorRgb128Float"/> structures.</summary>
        /// <param name="color1">The first <see cref="ColorRgb128Float" /> structure to add.</param>
        /// <param name="color2">The second <see cref="ColorRgb128Float" /> structure to add.</param>
        /// <returns>A new <see cref="ColorRgb128Float" /> structure whose color values are the results of the addition operation.</returns>
        public static ColorRgb128Float Add(ColorRgb128Float color1, ColorRgb128Float color2)
        {
            return color1 + color2;
        }
        /// <summary>Subtracts a <see cref="ColorRgb128Float" /> structure from a <see cref="ColorRgb128Float" /> structure. </summary>
        /// <param name="color1">The <see cref="ColorRgb128Float" /> structure to be subtracted from.</param>
        /// <param name="color2">The <see cref="ColorRgb128Float" /> structure to subtract from <paramref name="color1" />.</param>
        /// <returns>A new <see cref="ColorRgb128Float" /> structure whose color values are the results of the subtraction operation.</returns>
        public static ColorRgb128Float Substract(ColorRgb128Float color1, ColorRgb128Float color2)
        {
            return color1 - color2;
        }
        /// <summary>Multiplies the red, blue, and green channels of the specified <see cref="ColorRgb128Float" /> structure by the specified value.</summary>
        /// <param name="color">The <see cref="ColorRgb128Float" /> to be multiplied.</param>
        /// <param name="coefficient">The value to multiply by.</param>
        /// <returns>A new <see cref="ColorRgb128Float" /> structure whose color values are the results of the multiplication operation.</returns>
        public static ColorRgb128Float Multiply(ColorRgb128Float color, float coefficient)
        {
            return color * coefficient;
        }
        /// <summary>Multiplies two <see cref="ColorRgb128Float" /> structures channel by channel.</summary>
        /// <param name="color">The first <see cref="ColorRgb128Float" /> structure to add.</param>
        /// <param name="coefficients">The second <see cref="ColorRgb128Float" /> structure to add.</param>
        /// <returns>A new <see cref="ColorRgb128Float" /> structure whose color values are the results of the multiplication operation channel by channel.</returns>
        public static ColorRgb128Float Multiply(ColorRgb128Float color, ColorRgb128Float coefficients)
        {
            return color * coefficients;
        }
        /// <summary>Divides the red, blue, and green channels of the specified <see cref="ColorRgb128Float" /> structure by the specified value.</summary>
        /// <param name="color">The <see cref="ColorRgb128Float" /> to be divided.</param>
        /// <param name="divider">The value to divide by.</param>
        /// <returns>A new <see cref="ColorRgb128Float" /> structure whose color values are the results of the division operation.</returns>
        public static ColorRgb128Float Divide(ColorRgb128Float color, float divider)
        {
            return color / divider;
        }
        /// <summary>Divides a <see cref="ColorRgb128Float" /> structure by a <see cref="ColorRgb128Float" /> structure channel by channel.</summary>
        /// <param name="color1">The <see cref="ColorRgb128Float" /> structure to be divided.</param>
        /// <param name="color2">The <see cref="ColorRgb128Float" /> structure to divide the <paramref name="color1" />.</param>
        /// <returns>A new <see cref="ColorRgb128Float" /> structure whose color values are the results of the division operation channel by channel.</returns>
        public static ColorRgb128Float Divide(ColorRgb128Float color1, ColorRgb128Float color2)
        {
            return color1 / color2;
        }
        /// <summary>Raises the red, blue, and green channels of the specified <see cref="ColorRgb128Float" /> structure to the specified value.</summary>
        /// <param name="color">The <see cref="ColorRgb128Float" /> to be raised.</param>
        /// <param name="exponent">The exponent value.</param>
        /// <returns>A new <see cref="ColorRgb128Float" /> structure whose color values are the raised to the specified value.</returns>
        public static ColorRgb128Float Power(ColorRgb128Float color, double exponent)
        {
            return new ColorRgb128Float((float)Math.Pow(color.R, exponent), (float)Math.Pow(color.G, exponent), (float)Math.Pow(color.B, exponent)); 
        }

        /// <summary>Creates a new <see cref="ColorRgb128Float" /> structure in ScRGB space by using the specified sRGB color channel values. </summary>
        /// <param name="r">The sRGB red channel of the new color.</param>
        /// <param name="g">The sRGB green channel of the new color.</param>
        /// <param name="b">The sRGB blue channel of the new color.</param>
        /// <returns>A <see cref="ColorRgb128Float" /> structure with the values in ScRGB space.</returns>
        public static ColorRgb128Float ScFromRgb(float r, float g, float b)
        {
            return new ColorRgb128Float(sRgbToScRgb(r), sRgbToScRgb(g), sRgbToScRgb(b));
        }
        /// <summary>Creates a new <see cref="ColorRgb128Float" /> structure in ScRGB space by using the specified sRGB color.</summary>
        /// <param name="color">The <see cref="ColorRgb128Float"/> in sRGB space.</param>
        /// <returns>A <see cref="ColorRgb128Float" /> structure with the values in ScRGB space.</returns>
        public static ColorRgb128Float ScFromRgb(ColorRgb128Float color)
        {
            return new ColorRgb128Float(sRgbToScRgb(color.R), sRgbToScRgb(color.G), sRgbToScRgb(color.B));
        }

        /// <summary>
        /// Creates a string representation of the color.
        /// </summary>
        /// <returns>the string representation of the color.</returns>
        public override string ToString()
        {
            return string.Format("R={0}, G={1}, B={2}", R, G, B);
        }
    }
}
