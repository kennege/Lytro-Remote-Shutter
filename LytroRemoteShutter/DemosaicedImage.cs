#define USEFILTER

namespace UAM.Optics.LightField.Lytro
{
    using System;
    using UAM.Optics.LightField.Lytro.Metadata;

    /// <summary>
    /// Represents a raw, demosaiced sensor image (color).
    /// </summary>
    public partial class DemosaicedImage : ISampled2D<ColorRgb128Float>
    {
        private int _width;
        private int _height;

        private float[] _input;
        private ColorRgb128Float[] _output;
        private bool[] _valid;

        private double _gamma = 1.0;
        private double[,] _ccmRgbToSrgbArray = new double[,] { {1,0,0} , {0,1,0} , {0,0,1} };
        private int _bRemainder;

        private enum ColorFilter
        {
            Red,
            Green,
            Blue
        }

        /// <summary>
        /// Gets the width of the image.
        /// </summary>
        public int Width
        {
            get { return _width; }
        }

        /// <summary>
        /// Gets the height of the image.
        /// </summary>
        public int Height
        {
            get { return _height; }
        }

        /// <summary>
        /// Gets a demosaiced color at given pixel.
        /// </summary>
        /// <param name="x">The pixel column.</param>
        /// <param name="y">The pixel row.</param>
        /// <returns>an instance of <see cref="ColorRgb128Float"/> representing the color at given pixel.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="x"/> or <paramref name="y"/> are less then zero or greater than <see cref="Width"/> and <see cref="Height"/>, respectively.</exception>
        public ColorRgb128Float this[int x, int y]
        {
            get
            {
                if (_valid[y * _width + x])
                    return _output[y * _width + x];

                return this[x, y] = Demosaic(x, y);
            }
            private set
            {
                _output[y * _width + x] = value;
                _valid[y * _width + x] = true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DemosaicedImage"/> class from raw image and metadata.
        /// </summary>
        /// <param name="rawFrame">The raw image to be demosaiced.</param>
        /// <param name="frameMetadata">The metadata specifying the demosaic parameters.</param>
        public DemosaicedImage(RawImage rawFrame, Json.FrameImage frameMetadata)
        {
            if (rawFrame == null)
                throw new ArgumentNullException("rawFrame");
            if (frameMetadata == null)
                throw new ArgumentNullException("frameMetadata");

            if (frameMetadata.RawDetails == null ||
                frameMetadata.RawDetails.PixelFormat == null ||
                frameMetadata.RawDetails.Mosaic == null)
                throw new ArgumentException("Metadata does not contain required information.", "frameMetadata");

            if (frameMetadata.RawDetails.Mosaic.Tile != "r,gr:gb,b")
                throw new NotSupportedException("Unsupported mosaic tile.");

            _width = rawFrame.Width;
            _height = rawFrame.Height;

            if (_width < 1 || _height < 1)
                throw new ArgumentException("Invalid image dimensions.", "rawFrame");

            if (rawFrame.Data == null || rawFrame.Data.Length != _width * _height)
                throw new ArgumentException("Raw frame dimensions do not match metadata dimensions.");

            if (frameMetadata.Color != null)
            {
                if (frameMetadata.Color.Gamma != decimal.Zero)
                    _gamma = ((double)frameMetadata.Color.Gamma);

                if (frameMetadata.Color.CcmRgbToSrgbArray != null && frameMetadata.Color.CcmRgbToSrgbArray.Length == 9)
                {
                    _ccmRgbToSrgbArray[0, 0] = (double)frameMetadata.Color.CcmRgbToSrgbArray[0];
                    _ccmRgbToSrgbArray[0, 1] = (double)frameMetadata.Color.CcmRgbToSrgbArray[1];
                    _ccmRgbToSrgbArray[0, 2] = (double)frameMetadata.Color.CcmRgbToSrgbArray[2];

                    _ccmRgbToSrgbArray[1, 0] = (double)frameMetadata.Color.CcmRgbToSrgbArray[3];
                    _ccmRgbToSrgbArray[1, 1] = (double)frameMetadata.Color.CcmRgbToSrgbArray[4];
                    _ccmRgbToSrgbArray[1, 2] = (double)frameMetadata.Color.CcmRgbToSrgbArray[5];

                    _ccmRgbToSrgbArray[2, 0] = (double)frameMetadata.Color.CcmRgbToSrgbArray[6];
                    _ccmRgbToSrgbArray[2, 1] = (double)frameMetadata.Color.CcmRgbToSrgbArray[7];
                    _ccmRgbToSrgbArray[2, 2] = (double)frameMetadata.Color.CcmRgbToSrgbArray[8];
                }
            }

            InitializeInput(rawFrame.Data, frameMetadata);

            _output = new ColorRgb128Float[_input.Length];
            _valid = new bool[_output.Length];
        }
        private void InitializeInput(ushort[] data, Json.FrameImage frameMetadata)
        {
            ushort[,] minimums = new ushort[2, 2];
            ushort[,] maximums = new ushort[2, 2];

            float[,] groundedMaximums = new float[2, 2];
            float[,] whiteBalanceGains = new float[2, 2];
            InitializeMosaic(minimums, maximums, groundedMaximums, whiteBalanceGains, frameMetadata);

            _input = new float[data.Length];
            int i = 0;

            for (int y = 0; y < _height; ++y)
                for (int x = 0; x < _width; ++x, ++i)
                    _input[i] = ((float)data[i] /* * whiteBalanceGains[x % 2, y % 2]*/ - minimums[x % 2, y % 2]) / groundedMaximums[x % 2, y % 2];
                                                                      // 
            //  _input[i] = (float)Math.Pow((data[i] * whiteBalanceGains[x % 2, y % 2] - minimums[x % 2, y % 2]) / groundedMaximums[x % 2, y % 2], _gamma);
        }
        private void InitializeMosaic(ushort[,] minimums, ushort[,] maximums, float[,] groundedMaximums, float[,] whiteBalanceGains, Json.FrameImage frameMetadata)
        {
            string upperLeftPixel = frameMetadata.RawDetails.Mosaic.UpperLeftPixel;
            Json.BayerValue black = frameMetadata.RawDetails.PixelFormat.Black;
            Json.BayerValue white = frameMetadata.RawDetails.PixelFormat.White;
            Json.BayerValue balance = frameMetadata.Color.WhiteBalanceGain;

            switch (upperLeftPixel)
            {
                case "b":
                    _bRemainder = 0;
                    FillMatrix(minimums, (ushort)black.B, (ushort)black.Gb, (ushort)black.Gr, (ushort)black.R);
                    FillMatrix(maximums, (ushort)white.B, (ushort)white.Gb, (ushort)white.Gr, (ushort)white.R);
                    FillMatrix(whiteBalanceGains, (float)balance.B, (float)balance.Gb, (float)balance.Gr, (float)balance.R);
                    break;

                case "r":
                    _bRemainder = 1;
                    FillMatrix(minimums, (ushort)black.R, (ushort)black.Gr, (ushort)black.Gb, (ushort)black.B);
                    FillMatrix(maximums, (ushort)white.R, (ushort)white.Gr, (ushort)white.Gb, (ushort)white.B);
                    FillMatrix(whiteBalanceGains, (float)balance.R, (float)balance.Gr, (float)balance.Gb, (float)balance.B);
                    break;

                default:
                    throw new NotSupportedException("Unsupported upper left mosaic pixel.");
            }

            for (int x = 0; x <= 1; x++)
                for (int y = 0; y <= 1; y++)
                    groundedMaximums[x, y] = maximums[x, y] - minimums[x, y];
        }
        private static void FillMatrix<T>(T[,] matrix, T m00, T m01, T m10, T m11)
        {
            matrix[0, 0] = m00;
            matrix[0, 1] = m01;
            matrix[1, 0] = m10;
            matrix[1, 1] = m11;
        }

        private ColorFilter GetInFilter(int x, int y)
        {
            if (y % 2 == _bRemainder)
            {
                if (x % 2 == _bRemainder)
                    return ColorFilter.Blue;
            }
            else if (x % 2 != _bRemainder)
                return ColorFilter.Red;

            return ColorFilter.Green;
        }

        private float GetIn(int x, int y)
        {
            return _input[y * _width + x];
        }
        private float GetInG(int x, int y, ColorFilter f)
        {
            if (f == ColorFilter.Green)
                return GetIn(x, y);
            else
                return Filter4(x, y);
        }
        private float GetInR(int x, int y, ColorFilter f)
        {
            if (f == ColorFilter.Red)
                return GetIn(x, y);
            else if (f == ColorFilter.Blue)
                return Filter4X(x, y);
            else
                return (y % 2 == _bRemainder) ? Filter2V(x, y) : Filter2H(x, y);
        }
        private float GetInB(int x, int y, ColorFilter f)
        {
            if (f == ColorFilter.Blue)
                return GetIn(x, y);
            else if (f == ColorFilter.Red)
                return Filter4X(x, y);
            else
                return (y % 2 == _bRemainder) ? Filter2H(x, y) : Filter2V(x, y);
        }

        private float Average4(int x, int y)
        {
            int count = 0;
            float value = 0.0f;

            if (y > 0)
            {
                ++count;
                value += GetIn(x, y - 1);
            }
            if (y < _height - 1)
            {
                ++count;
                value += GetIn(x, y + 1);
            }
            if (x > 0)
            {
                ++count;
                value += GetIn(x - 1, y);
            }
            if (x < _width - 1)
            {
                ++count;
                value += GetIn(x + 1, y);
            }

            global::System.Diagnostics.Debug.Assert(count > 0);
            return value / count;
        }
        private float Average2H(int x, int y)
        {
            int count = 0;
            float value = 0.0f;

            if (x > 0)
            {
                ++count;
                value += GetIn(x - 1, y);
            }
            if (x < _width - 1)
            {
                ++count;
                value += GetIn(x + 1, y);
            }

            global::System.Diagnostics.Debug.Assert(count > 0);
            return value / count;
        }
        private float Average2V(int x, int y)
        {
            int count = 0;
            float value = 0.0f;

            if (y > 0)
            {
                ++count;
                value += GetIn(x, y - 1);
            }
            if (y < _height - 1)
            {
                ++count;
                value += GetIn(x, y + 1);
            }

            global::System.Diagnostics.Debug.Assert(count > 0);
            return value / count;
        }
        private float Average4X(int x, int y)
        {
            int count = 0;
            float value = 0.0f;

            if (y > 0)
            {
                if (x > 0)
                {
                    ++count;
                    value += GetIn(x - 1, y - 1);
                }
                if (x < _width - 1)
                {
                    ++count;
                    value += GetIn(x + 1, y - 1);
                }
            }
            if (y < _height - 1)
            {
                if (x > 0)
                {
                    ++count;
                    value += GetIn(x - 1, y + 1);
                }
                if (x < _width - 1)
                {
                    ++count;
                    value += GetIn(x + 1, y + 1);
                }
            }

            global::System.Diagnostics.Debug.Assert(count > 0);
            return value / count;
        }

        private float Filter4(int x, int y)
        {
#if USEFILTER
            if (x > 1 && y > 1 && x < _width - 2 && y < _height - 2)
                return (
                     4 * (GetIn(x, y)) +
                     2 * (GetIn(x + 1, y) + GetIn(x, y + 1) + GetIn(x - 1, y) + GetIn(x, y - 1)) +
                    -1 * (GetIn(x + 2, y) + GetIn(x, y + 2) + GetIn(x - 2, y) + GetIn(x, y - 2))
                    ) / 8;
            else
#endif
            return Average4(x, y);
        }
        private float Filter2H(int x, int y)
        {
#if USEFILTER
            if (x > 1 && y > 1 && x < _width - 2 && y < _height - 2)
                return (
                     5 * (GetIn(x, y)) +
                     4 * (GetIn(x + 1, y) + GetIn(x - 1, y)) +
                    -1 * (GetIn(x + 2, y) + GetIn(x + 1, y + 1) + GetIn(x - 1, y + 1) + GetIn(x - 2, y) + GetIn(x - 1, y - 1) + GetIn(x + 1, y - 1)) +
                         (GetIn(x, y - 2) + GetIn(x, y + 2)) / 2
                    ) / 8;
            else
#endif
            return Average2H(x, y);
        }
        private float Filter2V(int x, int y)
        {
#if USEFILTER
            if (x > 1 && y > 1 && x < _width - 2 && y < _height - 2)
                return (
                     5 * (GetIn(x, y)) +
                     4 * (GetIn(x, y - 1) + GetIn(x, y + 1)) +
                    -1 * (GetIn(x + 2, y) + GetIn(x + 1, y + 1) + GetIn(x - 1, y + 1) + GetIn(x - 2, y) + GetIn(x - 1, y - 1) + GetIn(x + 1, y - 1)) +
                         (GetIn(x, y - 2) + GetIn(x, y + 2)) / 2
                    ) / 8;
            else
#endif
            return Average2V(x, y);
        }
        private float Filter4X(int x, int y)
        {
#if USEFILTER
            if (x > 1 && y > 1 && x < _width - 2 && y < _height - 2)
                return (
                     6 * (GetIn(x, y)) +
                     2 * (GetIn(x + 1, y + 1) + GetIn(x - 1, y + 1) + GetIn(x - 1, y - 1) + GetIn(x + 1, y - 1)) +
                    -3 * (GetIn(x + 2, y) + GetIn(x, y + 2) + GetIn(x - 2, y) + GetIn(x, y - 2)) / 2
                    ) / 8;
            else
#endif
            return Average4X(x, y);
        }

        internal ColorRgb128Float Demosaic(int x, int y)
        {
            ColorFilter f = GetInFilter(x, y);
            float r = GetInR(x, y, f);
            float g = GetInG(x, y, f);
            float b = GetInB(x, y, f);

            double ccmR = r;
            double ccmG = g;
            double ccmB = b;

            //ccmG = Math.Pow(ccmG, _gamma);
            //ccmB = Math.Pow(ccmB, _gamma);
            //ccmR = Math.Pow(ccmR, _gamma);

            //ccmR = r * _ccmRgbToSrgbArray[0, 0] + g * _ccmRgbToSrgbArray[0, 1] + b * _ccmRgbToSrgbArray[0, 2];
            //ccmG = r * _ccmRgbToSrgbArray[1, 0] + g * _ccmRgbToSrgbArray[1, 1] + b * _ccmRgbToSrgbArray[1, 2];
            //ccmB = r * _ccmRgbToSrgbArray[2, 0] + g * _ccmRgbToSrgbArray[2, 1] + b * _ccmRgbToSrgbArray[2, 2];

            //return new ColorRgb128Float((float)ccmR, (float)ccmG, (float)ccmB);
            return ColorRgb128Float.ScFromRgb((float)ccmR, (float)ccmG, (float)ccmB);
        }

        /// <summary>
        /// Demosaics the whole image.
        /// </summary>
        /// <returns>an array of length <see cref="Width"/> * <see cref="Height"/> containing the demosaiced image.</returns>
        public ColorRgb128Float[] Demosaic()
        {
            int i = 0;
            for (int y = 0; y < _height; ++y)
                for (int x = 0; x < _width; ++x, ++i)
                    if (!_valid[i])
                        this[x, y] = Demosaic(x, y);

            return _output;
        }
    }
}
