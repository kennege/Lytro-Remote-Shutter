using System;

namespace UAM.Optics.LightField.Lytro
{
    /// <summary>
    /// Represents the sub-aperture image.
    /// </summary>
    public partial class XYImage : ISampled2D<ColorRgba128Float>
    {
        private IContinuous2D<ColorRgb128Float> _rawImage;
        private MicroLensCollection _mla;
        private int _u;
        private int _v;
        private int _width;
        private int _height;
        private int _xMin;
        private int _yMin;

        /// <summary>
        /// Initializes a new instance of the <see cref="XYImage"/> class.
        /// </summary>
        /// <param name="rawImage">The raw sensor image.</param>
        /// <param name="mla">The microlens collection to use.</param>
        /// <param name="u">Horizontal offset from the microlens center.</param>
        /// <param name="v">Vertical offset from the microlens center.</param>
        public XYImage(ISampled2D<ColorRgb128Float> rawImage, MicroLensCollection mla, int u, int v)
        {
            if (rawImage == null)
                throw new ArgumentNullException("rawImage");

            if (mla == null)
                throw new ArgumentNullException("mla");

            _rawImage = new InterpolatedImage(rawImage);
            _mla = mla;
            _u = u;
            _v = v;

            _width = rawImage.Width;
            _height = rawImage.Height;

            int dummy;
            mla.GetBounds(out _xMin, out dummy, out _yMin, out dummy);
        }

        /// <summary>
        /// Gets a pixel of the sub-aperture image.
        /// </summary>
        /// <param name="x">The x coordinate of the pixel.</param>
        /// <param name="y">The y coordinate of the pixel.</param>
        /// <returns>a pixel of the sub-aperture image at specified location.</returns>
        public ColorRgba128Float this[int x, int y]
        {
            get
            {
                MicroLens lens = _mla[_xMin + x, _yMin + y, _u, _v];

                double rawX = lens.CenterX;
                double rawY = lens.CenterY;
                if (rawX < 0 || rawY < 0 || rawX >= _width || rawY >= _height)
                    return default(ColorRgba128Float);

                return _rawImage[rawX, rawY];
            }
        }

        /// <summary>
        /// Gets the width of the sub-aperture image.
        /// </summary>
        public int Width
        {
            get { return _mla.BoundingWidth; }
        }

        /// <summary>
        /// Gets the height of the sub-aperture image.
        /// </summary>
        public int Height
        {
            get { return _mla.BoundingHeight; }
        }
    }
}
