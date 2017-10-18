namespace UAM.Optics.LightField.Lytro
{
    /// <summary>
    /// Represents a single microlens in an array.
    /// </summary>
    public partial class MicroLens
    {
        /// <summary>
        /// Position of the microlens horizontal center.
        /// </summary>
        public double CenterX;
        /// <summary>
        /// Position of the microlens vertical center.
        /// </summary>
        public double CenterY;
        /// <summary>
        /// Diameter of the microlens.
        /// </summary>
        public double Diameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroLens"/> class.
        /// </summary>
        public MicroLens()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroLens"/> class at specified location.
        /// </summary>
        /// <param name="centerX">Position of the microlens horizontal center..</param>
        /// <param name="centerY">Position of the microlens vertical center.</param>
        /// <param name="diameter">Diameter of the microlens.</param>
        public MicroLens(double centerX, double centerY, double diameter)
        {
            CenterX = centerX;
            CenterY = centerY;
            Diameter = diameter;
        }

        /// <summary>
        /// Gets a sample under the microlens.
        /// </summary>
        /// <typeparam name="TSample">Type of samples the image uses.</typeparam>
        /// <param name="image">The image to get the sample from.</param>
        /// <param name="u">Horizontal offset from the microlens center.</param>
        /// <param name="v">Vertical offset from the microlens center.</param>
        /// <returns>a sample from supplied <paramref name="image"/> under the microlens with given coordinates.</returns>
        public TSample GetUV<TSample>(IContinuous2D<TSample> image, double u, double v)
        {
            return image[CenterX + u, CenterY + v];            
        }
    }
}
