namespace UAM.Optics.LightField.Lytro.Net
{
    /// <summary>
    /// Specifies the format of picture data.
    /// </summary>
    public enum LoadPictureFormat
    {
        /// <summary>
        /// A single JPEG file.
        /// </summary>
        Jpeg = '0',

        /// <summary>
        /// A single RAW file.
        /// </summary>
        Raw = '1',

        /// <summary>
        /// A single TXT file.
        /// </summary>
        Metadata = '2',

        /// <summary>
        /// A single thumbnail image with dimensions of 128×128 pixels, raw data, 16 bits per pixel, 4:2:2 YUY2 format. 
        /// </summary>
        Thumbnail128 = '3',

        /// <summary>
        /// Four prerendered JPEG files with dimensions of 320×1280 pixels, each containing 4 frames of 320×320 pixels at different lambda. 
        /// </summary>
        Stack = '4'
    }
}
