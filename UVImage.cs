using System;

namespace UAM.Optics.LightField.Lytro
{
    /// <summary>
    /// Represents the microlens image.
    /// </summary>
    public partial class UVImage : ISampled2D<ColorRgba128Float>
    {
        private IContinuous2D<ColorRgb128Float> _rawImage;
        private MicroLens _lens;
        private int _width;
        private int _height;
        private double _xMin;
        private double _yMin;

        /// <summary>
        /// Initializes a new instance of the <see cref="UVImage"/> class.
        /// </summary>
        /// <param name="rawImage">The raw sensor image.</param>
        /// <param name="lens">The microlens which image to represent.</param>
        public UVImage(ISampled2D<ColorRgb128Float> rawImage, MicroLens lens)
        {
            if (rawImage == null)
                throw new ArgumentNullException("rawImage");

            if (lens == null)
                throw new ArgumentNullException("lens");

            _rawImage = new InterpolatedImage(rawImage);

            _lens = lens;
            _width = rawImage.Width;
            _height = rawImage.Height;

            _xMin = -lens.Diameter / 2;
            _yMin = -lens.Diameter / 2;
        }

        /// <summary>
        /// Gets a pixel of the microlens image.
        /// </summary>
        /// <param name="x">The x coordinate of the pixel.</param>
        /// <param name="y">The y coordinate of the pixel.</param>
        /// <returns>a pixel of the microlens image at specified location.</returns>
        public ColorRgba128Float this[int x, int y]
        {
            get
            {
                double rawX = _lens.CenterX + _xMin + x;
                double rawY = _lens.CenterY + _yMin + y;
                if (rawX < 0 || rawY < 0 || rawX >= _width || rawY >= _height)
                    return default(ColorRgba128Float);

                return _rawImage[rawX, rawY];
            }
        }

        /// <summary>
        /// Gets the width of the microlens image.
        /// </summary>
        public int Width
        {
            get { return (int)_lens.Diameter; }
        }

        /// <summary>
        /// Gets the height of the microlens image.
        /// </summary>
        public int Height
        {
            get { return (int)_lens.Diameter; }
        }
    }
}
