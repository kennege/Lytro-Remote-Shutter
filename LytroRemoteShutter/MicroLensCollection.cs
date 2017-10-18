namespace UAM.Optics.LightField.Lytro
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UAM.Optics.LightField.Lytro.Metadata;

    /// <summary>
    /// Represents a microlens array configuration with access to individual microlenses.
    /// </summary>
    public partial class MicroLensCollection : IEnumerable<MicroLens>
    {
        private const double HexagonalMinorFactor = 1.7320508075688772935274463415059;
        private const double HexagonalSkew = 0.57735026918962576450914878050196;

        private Json.FrameMetadata _metadata;

        private double _rotation;
        private double _startX;
        private double _startY;
        private double _radius;
        private double _deltaX;
        private double _deltaY;
        private double _skewX;

        private int _width;
        private int _height;

        private int _xMinStep;
        private int _xMaxStep;
        private int _yMinStep;
        private int _yMaxStep;

        private bool _orthogonal;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroLensCollection"/> from metadata.
        /// </summary>
        /// <param name="metadata">The metadata with microlens array parameters.</param>
        /// <exception cref="ArgumentNullException"><paramref name="metadata"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="metadata"/> contains invalid or no information about the microlens array.</exception>
        /// <exception cref="NotSupportedException">The microlens tiling specified by <paramref name="metadata"/> is not supported.</exception>
        public MicroLensCollection(Json.FrameMetadata metadata)
        {
            if (metadata == null)
                throw new ArgumentNullException("metadata");

            if (metadata.Image == null || metadata.Devices == null)
                throw new ArgumentException("Critical metadata missing.");

            _metadata = metadata;
            Json.Mla mla = metadata.Devices.Mla;
            Json.Sensor sensor = metadata.Devices.Sensor;

            if (mla == null || sensor == null)
                throw new ArgumentException("Critical metadata missing.");

            switch (mla.Tiling)
            {
                case "squareUniform":
                    Initialize(metadata, 0.0, 1.0, 1.0);
                    _orthogonal = true;
                    break;

                case "hexUniformRowMajor":
                    Initialize(metadata, HexagonalSkew, 1.0, HexagonalMinorFactor / 2.0);
                    break;

                default:
                    throw new NotSupportedException("Unsupported MLA tiling.");
            }
        }
        private MicroLensCollection()
        {

        }

        private void Initialize(Json.FrameMetadata metadata, double skewX, double deltaXfactor, double deltaYfactor)
        {
            global::System.Diagnostics.Debug.Assert(deltaXfactor != 0, "X factor cannot be zero.");
            global::System.Diagnostics.Debug.Assert(deltaYfactor != 0, "Y factor cannot be zero.");

            Json.Mla mla = metadata.Devices.Mla;
            Json.Sensor sensor = metadata.Devices.Sensor;
            Json.Lens lens = metadata.Devices.Lens;

            if (sensor.PixelPitch == 0) throw new ArgumentException("Pixel pitch cannot be zero.", "metadata");
            else if (mla.LensPitch == 0) throw new ArgumentException("Lens pitch cannot be zero.", "metadata");
            else if (mla.ScaleFactor.X == 0 || mla.ScaleFactor.Y == 0) throw new ArgumentException("Lens scale factor cannot be zero.", "metadata");

            _width = (int)metadata.Image.Width;
            _height = (int)metadata.Image.Height;

            decimal projectedPitch = mla.LensPitch;
            if (lens != null && lens.ExitPupilOffset.Z != 0)
                projectedPitch *= (lens.ExitPupilOffset.Z + mla.SensorOffset.Z) / lens.ExitPupilOffset.Z;

            _rotation = (double)mla.Rotation;
            _startX = _width / 2 + (double)(mla.SensorOffset.X / sensor.PixelPitch);
            _startY = _height / 2 + (double)(mla.SensorOffset.Y / sensor.PixelPitch);
            _radius = (double)(projectedPitch / sensor.PixelPitch);
            _deltaX = (double)(projectedPitch / sensor.PixelPitch * mla.ScaleFactor.X) * deltaXfactor;
            _deltaY = (double)(projectedPitch / sensor.PixelPitch * mla.ScaleFactor.Y) * deltaYfactor;
            _skewX = skewX;

            double pX = _startY * Math.Cos(_rotation) - _startX * Math.Sin(_rotation);
            double pY = _startY * Math.Sin(_rotation) + _startX * Math.Cos(_rotation);

            double xMin, xMax, yMin, yMax;

            if (_rotation >= 0)
            {
                xMin = -pY;
                xMax = _height * Math.Sin(_rotation) + _width * Math.Cos(_rotation) - pY;

                yMin = -_width * Math.Sin(_rotation) - pX;
                yMax = _height * Math.Cos(_rotation) - pX;

            }
            else
            {
                yMin = -pX;
                yMax = -_width * Math.Sin(_rotation) + _height * Math.Cos(_rotation) - pX;

                xMin = _width * Math.Sin(_rotation) - pY;
                xMax = _height * Math.Cos(_rotation) - pY;
            }

            if (_skewX > 0)
            {
                xMin -= (_height - _startY) * _skewX;
                xMax += _startY * _skewX;
            }
            else if (_skewX < 0)
            {
                xMin += _startY * _skewX;
                xMax -= (_height - _startY) * _skewX;
            }

            _xMinStep = (int)Math.Floor(xMin / _deltaX);
            _xMaxStep = (int)Math.Ceiling(xMax / _deltaX);

            _yMinStep = (int)Math.Floor(yMin / _deltaY);
            _yMaxStep = (int)Math.Ceiling(yMax / _deltaY);
        }

        /// <summary>
        /// Gets the <see cref="MicroLens"/> of specified coordinates.
        /// </summary>
        /// <param name="x">Horizontal coordinate of the microlens.</param>
        /// <param name="y">Vertical coordinate of the microlens.</param>
        /// <returns>the <see cref="MicroLens"/> of specified coordinates.</returns>
        /// <remarks>The microlens array is treated as virtual. Microlens coordinates are not necessary orthogonal and have origin at the center of an image. Each microlens represents one unit.</remarks>
        public MicroLens this[int x, int y]
        {
            get { return this[(double)x, (double)y]; }
        }
        /// <summary>
        /// Experimental.
        /// </summary>
        /// <param name="x">Horizontal coordinate of the microlens.</param>
        /// <param name="y">Vertical coordinate of the microlens.</param>
        /// <param name="uPixels">Horizontal displacement from the microlens center in image pixels.</param>
        /// <param name="vPixels">Vertical displacement from the microlens center in image pixels.</param>
        /// <returns>a displaced <see cref="MicroLens"/> of specified coordinates..</returns>
        /// <remarks>The microlens array is treated as virtual. Microlens coordinates are not necessary orthogonal and have origin at the center of an image. Each microlens represents one unit.</remarks>
        public MicroLens this[int x, int y, int uPixels, int vPixels]
        {
            get
            {
                MicroLens lens = this[x, y];
                lens.CenterX += uPixels;
                lens.CenterY += vPixels;
                return lens;
            }
        }
        /// <summary>
        /// Gets the <see cref="MicroLens"/> of specified coordinates.
        /// </summary>
        /// <param name="x">Horizontal coordinate of the microlens.</param>
        /// <param name="y">Vertical coordinate of the microlens.</param>
        /// <returns>the <see cref="MicroLens"/> of specified coordinates.</returns>
        /// <remarks>The microlens array is treated as virtual. Microlens coordinates are not necessary orthogonal and have origin at the center of an image. Each microlens represents one unit.</remarks>
        private MicroLens this[double x, double y]
        {
            get
            {
                double xLens = x * _deltaX + y * _deltaY * _skewX;
                double yLens = y * _deltaY; // + x * _deltaX * _skewY;

                double xReal = xLens * Math.Cos(_rotation) - yLens * Math.Sin(_rotation);
                double yReal = xLens * Math.Sin(_rotation) + yLens * Math.Cos(_rotation);

                return new MicroLens(_startX + xReal, _startY + yReal, _radius);
            }
        }

        /// <summary>
        /// Returns the width in microlens coordinates (the number of microlenses) that cover the entire image.
        /// </summary>
        public int BoundingWidth
        {
            get { return _xMaxStep - _xMinStep; }
        }
        /// <summary>
        /// Returns the height in microlens coordinates (the number of microlenses) that cover the entire image.
        /// </summary>
        public int BoundingHeight
        {
            get { return _yMaxStep - _yMinStep; }
        }

        /// <summary>
        /// Gets the bounding microlens coordinates that cover the entire image.
        /// </summary>
        /// <param name="xMin">The left-most microlens.</param>
        /// <param name="xMax">The right-most microlens.</param>
        /// <param name="yMin">The top-most microlens.</param>
        /// <param name="yMax">The bottom-most microlens.</param>
        public void GetBounds(out int xMin, out int xMax, out int yMin, out int yMax)
        {
            xMin = _xMinStep;
            xMax = _xMaxStep;
            yMin = _yMinStep;
            yMax = _yMaxStep;
        }

        /// <summary>
        /// Enumerates all microlenses in the image.
        /// </summary>
        /// <returns>The enumerator for this collection.</returns>
        public IEnumerator<MicroLens> GetEnumerator()
        {
            for (int y = _yMinStep; y < _yMaxStep; y++)
                for (int x = _xMinStep; x < _xMaxStep; x++)
                {
                    MicroLens microlens = this[x, y];
                    if (microlens.CenterX >= 0 && microlens.CenterY >= 0 && microlens.CenterX < _width && microlens.CenterY < _height)
                        yield return this[x, y];
                }
        }

        /// <summary>
        /// Enumerates all microlenses in the image.
        /// </summary>
        /// <returns>The enumerator for this collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets a <see cref="MicroLensCollection"/> that contains only orthogonally arranged microlenses from the current collection.
        /// </summary>
        /// <returns>a <see cref="MicroLensCollection"/> that contains only orthogonally arranged microlenses from the current collection.</returns>
        /// <remarks>In case the current <see cref="MicroLensCollection"/> is already orthogonal, a reference to the current collection is returned.</remarks>
        public MicroLensCollection GetOrthogonalSubset()
        {
            if (_orthogonal)
                return this;

            MicroLensCollection orthogonal = new MicroLensCollection();
            orthogonal.Initialize(_metadata, 0.0, 1.0, HexagonalMinorFactor);

            return orthogonal;
        }
    }
}
