using System;

namespace UAM.Optics
{
    /// <summary>
    /// Represents a color in terms of red, green, blue and alpha channels, using 32 bits per channel.
    /// </summary>
    public struct ColorRgba128Float
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
        /// <summary>
        /// Alpha channel.
        /// </summary>
        public float A;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorRgba128Float"/> class using the specified channel values.
        /// </summary>
        /// <param name="r">The red channel, <see cref="R"/> of the new color.</param>
        /// <param name="g">The blue channel <see cref="G"/> of the new color.</param>
        /// <param name="b">The blue channel, <see cref="B"/> of the new color.</param>
        /// <param name="a">The alpha channel, <see cref="A"/>  of the new color. Default is 1 (opaque).</param>
        public ColorRgba128Float(float r, float g, float b, float a = 1f)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorRgb128Float"/> class using the same value for all color channels.
        /// </summary>
        /// <param name="rgb">The value for all color channels.</param>
        /// <param name="a">The alpha channel, <see cref="A"/> of the new color. Default is 1 (opaque).</param>
        public ColorRgba128Float(float rgb, float a = 1f)
        {
            R = G = B = rgb;
            A = a;
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

        /// <summary>
        /// Conver a <see cref="ColorRgb128Float"/> color to <see cref="ColorRgba128Float"/> color.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>the corresponding <see cref="ColorRgba128Float"/> color.</returns>
        public static implicit operator ColorRgba128Float(ColorRgb128Float color)
        {
            return new ColorRgba128Float(color.R, color.G, color.B, 1f);
        }

        /// <summary>Creates a new <see cref="ColorRgba128Float" /> structure in ScRGB space by using the specified sRGB color channel values. </summary>
        /// <param name="r">The sRGB red channel of the new color.</param>
        /// <param name="g">The sRGB green channel of the new color.</param>
        /// <param name="b">The sRGB blue channel of the new color.</param>
        /// <param name="a">The alpha channel of the new collor.</param>
        /// <returns>A <see cref="ColorRgba128Float" /> structure with the values in ScRGB space.</returns>
        public static ColorRgba128Float ScFromRgb(float r, float g, float b, float a = 1f)
        {
            return new ColorRgba128Float(sRgbToScRgb(r), sRgbToScRgb(g), sRgbToScRgb(b), a);
        }
        /// <summary>Creates a new <see cref="ColorRgba128Float" /> structure in ScRGB space by using the specified sRGB color.</summary>
        /// <param name="color">The <see cref="ColorRgba128Float"/> in sRGB space.</param>
        /// <returns>A <see cref="ColorRgb128Float" /> structure with the values in ScRGB space.</returns>
        public static ColorRgba128Float ScFromRgb(ColorRgba128Float color)
        {
            return new ColorRgba128Float(sRgbToScRgb(color.R), sRgbToScRgb(color.G), sRgbToScRgb(color.B), color.A);
        }

        private static float Coerce(float value)
        {
            if (value < 0f)
                return 0f;
            else if (value > 1f)
                return 1f;

            return value;
        }

        /// <summary>
        /// Creates a string representation of the color.
        /// </summary>
        /// <returns>the string representation of the color.</returns>
        public override string ToString()
        {
            return string.Format("R={0}, G={1}, B={2}, A={3}", R, G, B, A);
        }
    }
}
