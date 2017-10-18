namespace UAM.Optics.LightField.Lytro
{
    using System;

    /// <summary>
    /// Provides a linear interpolation of an image.
    /// </summary>
    public class InterpolatedImage : IContinuous2D<ColorRgb128Float>
    {
        private ISampled2D<ColorRgb128Float> _sampledImage;
        private double _width;
        private double _height;

        /// <summary>
        /// Gets the image width.
        /// </summary>
        public double Width
        {
            get { return _width; }
        }

        /// <summary>
        /// Gets the image height.
        /// </summary>
        public double Height
        {
            get { return _height; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterpolatedImage"/> class.
        /// </summary>
        /// <param name="sampledImage">The image to be interpolated.</param>
        public InterpolatedImage(ISampled2D<ColorRgb128Float> sampledImage)
        {
            if (sampledImage == null)
                throw new ArgumentNullException("sampledImage");

            _sampledImage = sampledImage;
            _width = sampledImage.Width;
            _height = sampledImage.Height;
        }

        /// <summary>
        /// Gets an interpolated color at given pixel.
        /// </summary>
        /// <param name="x">The pixel column.</param>
        /// <param name="y">The pixel row.</param>
        /// <returns>an interpolated color at given pixel</returns>
        public ColorRgb128Float this[double x, double y]
        {
            get
            {
                int xTopLeft = (int)x;
                int yTopLeft = (int)y;

                ColorRgb128Float top = Interpolate(Get(xTopLeft, yTopLeft), Get(xTopLeft + 1, yTopLeft), x - xTopLeft);
                ColorRgb128Float bottom = Interpolate(Get(xTopLeft, yTopLeft + 1), Get(xTopLeft + 1, yTopLeft + 1), x - xTopLeft);

                return Interpolate(top, bottom, y - yTopLeft);
            }
        }

        private ColorRgb128Float Get(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _width || y >= _height)
                return default(ColorRgb128Float);

            return _sampledImage[x, y];
        }

        internal static ColorRgb128Float Interpolate(ColorRgb128Float from, ColorRgb128Float to, double progress)
        {
            return from + (to - from) * (float)progress;
        }
    }
}
