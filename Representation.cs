
namespace UAM.Optics.LightField.Lytro.Metadata
{
    /// <summary>
    /// Specifies the data representation.
    /// </summary>
    public enum Representation
    {
        /// <summary>
        /// Raw values.
        /// </summary>
        Raw,

        /// <summary>
        /// JFIF image.
        /// </summary>
        Jpeg,

        /// <summary>
        /// H.264 Annex B stream.
        /// </summary>
        H264,

        /// <summary>
        /// JFIF image with packed raw data.
        /// </summary>
        RawPackedJpegCompressed
    }
}
