namespace UAM.Optics.LightField.Lytro
{
    using System;

    /// <summary>
    /// Provides a linear interpolation of a depth map.
    /// </summary>
    public class InterpolatedDepth : IContinuous2D<float>
    {
        private ISampled2D<float> _sampledDepth;
        private float _width;
        private float _height;

        /// <summary>
        /// Gets the depth map width.
        /// </summary>
        public float Width
        {
            get { return _width; }
        }

        /// <summary>
        /// Gets the depth map height.
        /// </summary>
        public float Height
        {
            get { return _height; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterpolatedDepth"/> class.
        /// </summary>
        /// <param name="sampledDepth">The depth map to be interpolated.</param>
        public InterpolatedDepth(ISampled2D<float> sampledDepth)
        {
            if (sampledDepth == null)
                throw new ArgumentNullException("sampledDepth");

            _sampledDepth = sampledDepth;
            _width = sampledDepth.Width;
            _height = sampledDepth.Height;
        }

        /// <summary>
        /// Gets an interpolated depth at given coordinate.
        /// </summary>
        /// <param name="x">The depth map column.</param>
        /// <param name="y">The depth map row.</param>
        /// <returns>an interpolated depth at given coordinate.</returns>
        public float this[double x, double y]
        {
            get
            {
                int xTopLeft = (int)x;
                int yTopLeft = (int)y;

                float top = Interpolate(Get(xTopLeft, yTopLeft), Get(xTopLeft + 1, yTopLeft), x - xTopLeft);
                float bottom = Interpolate(Get(xTopLeft, yTopLeft + 1), Get(xTopLeft + 1, yTopLeft + 1), x - xTopLeft);

                return Interpolate(top, bottom, y - yTopLeft);
            }
        }

        private float Get(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _width || y >= _height)
                return default(ushort);

            return _sampledDepth[x, y];
        }

        internal static float Interpolate(float from, float to, double progress)
        {
            return from + (to - from) * (float)progress;
        }
    }
}
