namespace UAM.Optics.LightField.Lytro
{
    using System;
    using UAM.Optics.LightField.Lytro.IO;
    using UAM.Optics.LightField.Lytro.Metadata;

    /// <summary>
    /// Represents a raw, unpacked sensor image (grayscale).
    /// </summary>
    public partial class RawImage : ISampled2D<ushort>
    {
        private readonly int _width;
        private readonly int _height;
        private readonly ushort[] _data;

        /// <summary>
        /// Gets the image width.
        /// </summary>
        public int Width
        {
            get { return _width; }
        }

        /// <summary>
        /// Gets the image height.
        /// </summary>
        public int Height
        {
            get { return _height; }
        }

        /// <summary>
        /// Gets the raw, unpacked image data (grayscale).
        /// </summary>
        public ushort[] Data
        {
            get { return _data; }
        }

        /// <summary>
        /// Creates a new blank raw image.
        /// </summary>
        /// <param name="width">The image width.</param>
        /// <param name="height">The image height.</param>
        public RawImage(int width, int height)
        {
            _width = width;
            _height = height;

            _data = new ushort[width * height];
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RawImage"/> class from existing data.
        /// </summary>
        /// <param name="imageData">The raw, unpacked image data (grayscale).</param>
        /// <param name="width">The image width.</param>
        /// <param name="height">The image height.</param>
        public RawImage(ushort[] imageData, int width, int height)
        {
            if (imageData == null)
                throw new ArgumentNullException("imageData");

            if (imageData.Length != width * height)
                throw new ArgumentException("Data array length mismatch.", "imageData");

            _width = width;
            _height = height;
            _data = imageData;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RawImage"/> class from packed sensor data.
        /// </summary>
        /// <param name="frameData">The raw, packed sensor data.</param>
        /// <param name="imageMetadata">The metadata with packing parameters.</param>
        public RawImage(byte[] frameData, Json.FrameImage imageMetadata)
        {
            if (frameData == null)
                throw new ArgumentNullException("frameData");

            VerifyMetadata(imageMetadata, out _width, out _height);
            _data = Unpack(frameData, _width, _height);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RawImage"/> class from a <see cref="LightFieldComponent"/>.
        /// </summary>
        /// <param name="frame">The raw frame component.</param>
        /// <param name="imageMetadata">The metadata with packing parameters.</param>
        public RawImage(LightFieldComponent frame, Json.FrameImage imageMetadata)
        {
            if (frame == null)
                throw new ArgumentNullException("frame");
            if (frame.Length < 1)
                throw new ArgumentException("Component does not contain any data.", "frame");

            VerifyMetadata(imageMetadata, out _width, out _height);
            _data = Unpack(frame.Data, _width, _height);
        }

        private void VerifyMetadata(Json.FrameImage imageMetadata, out int width, out int height)
        {
            if (imageMetadata == null)
                throw new ArgumentNullException("imageMetadata");
            if (imageMetadata.RawDetails == null ||
                imageMetadata.RawDetails.PixelPacking == null)
                throw new ArgumentException("Metadata does not contain required information.", "imageMetadata");

            if (imageMetadata.Representation != "rawPacked")
                throw new NotSupportedException("Unsupported representation.");
            if (imageMetadata.RawDetails.PixelPacking.Endianness != "big")
                throw new NotSupportedException("Unsupported endianness.");
            if (imageMetadata.RawDetails.PixelPacking.BitsPerPixel != 12)
                throw new NotSupportedException("Unsupported number of bits per pixel.");

            width = (int)imageMetadata.Width;
            height = (int)imageMetadata.Height;

            if (width < 0 || height < 0)
                throw new ArgumentException("Invalid frame dimensions.");
        }

        /// <summary>
        /// Gets a grayscale value at given pixel.
        /// </summary>
        /// <param name="x">The pixel column.</param>
        /// <param name="y">The pixel row.</param>
        /// <returns>a grayscale value at given pixel.</returns>
        public ushort this[int x, int y]
        {
            get { return _data[y * _width + x]; }
        }

        private static ushort[] Unpack(byte[] packed, int width, int height)
        {
#if UNSAFE
            return UnpackUnsafe(packed, width, height);
#else
            return UnpackSafe(packed, width, height);
#endif
        }

#if UNSAFE
        private static unsafe ushort[] UnpackUnsafe(byte[] packed, int width, int height)
        {
            ushort[] unpacked = new ushort[width * height];

            fixed (byte* pPacked = packed)
            fixed (ushort* pUnpacked = unpacked)
            {
                byte* pUnpackedInt8 = (byte*)pUnpacked;

                byte* pPackedInt8 = pPacked;
                byte* pPackedInt8Last = pPacked + packed.Length - 2;

                while (pPackedInt8 <= pPackedInt8Last)
                {
                    pUnpackedInt8[0] = (byte)((pPackedInt8[0] << 4) | (pPackedInt8[1] >> 4));
                    pUnpackedInt8[1] = (byte)(pPackedInt8[0] >> 4);

                    pUnpackedInt8[2] = (byte)pPackedInt8[2];
                    pUnpackedInt8[3] = (byte)(pPackedInt8[1] & 0x0F);

                    pUnpackedInt8 += 4;
                    pPackedInt8 += 3;
                }
            }

            return unpacked;
        }
#endif

        private static ushort[] UnpackSafe(byte[] packed, int width, int height)
        {
            ushort[] unpacked = new ushort[width * height];

            for (int b = 0, s = 0; b < packed.Length; b += 3, s += 2)
            {
                unpacked[s] = (ushort)((packed[b] << 4) | (packed[b + 1] >> 4));
                unpacked[s + 1] = (ushort)(((packed[b + 1] << 8) | packed[b + 2]) & 0x0FFF);
            }

            return unpacked;
        }

        internal static Json.FrameImage DefaultMetadata(int width, int height)
        {
            return new Json.FrameImage
            {
                Width = width,
                Height = height,
                Representation = "rawPacked",
                RawDetails = new Json.RawDetails
                {
                    PixelPacking = new Json.PixelPacking
                    {
                        BitsPerPixel = 12,
                        Endianness = "big"
                    }
                }
            };
        }

        /// <summary>
        /// Gets unpacked 16-bit grayscale data.
        /// </summary>
        /// <returns>an array of length <see cref="Width"/> * <see cref="Height"/> with unpacked 16-bit grayscale data.</returns>
        public ushort[] ToGray16Data()
        {
            ushort[] data = new ushort[_data.Length];

            for (int i = 0; i < _data.Length; i++)
             // data[i] = (ushort)(_data[i] << 4);         //  0x0000 - 0xFFF0
                data[i] = (ushort)(_data[i] * 4369 / 273); //  0x0000 - 0xFFFF  

            return data;
        }

        /// <summary>
        /// Gets unpacked 8-bit grayscale data.
        /// </summary>
        /// <returns>an array of length <see cref="Width"/> * <see cref="Height"/> with unpacked 8-bit grayscale data.</returns>
        public byte[] ToGray8Data()
        {
            byte[] data = new byte[_data.Length];

            for (int i = 0; i < _data.Length; i++)
                data[i] = (byte)(_data[i] >> 2);            

            return data;
        }

        /// <summary>
        /// Normalizes the image using the specified image.
        /// </summary>
        /// <param name="image">The image to normalize with.</param>
        /// <remarks>
        /// The supplied <paramref name="image"/> is searched for maximums in each filter color separately (discarding hot pixels);
        /// Pixels of current image are multiplied by the factor that would be needed to bring the corresponding pixels in supplied <paramref name="image"/> to the respective maximum.
        /// </remarks>
        public void NormalizeBy(RawImage image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            if (image.Width != _width || image.Height != _height)
                throw new ArgumentException("Images must be of equal size.", "image");

            ushort[] data = image._data;
            ushort[,] maxs = new ushort[2, 2];

            for (int y = 0; y < _height; y++)
                for (int x = 0; x < _width; x++)
                {
                    ushort value = data[y * _width + x];
                    ushort max = maxs[x % 2, y % 2];

                    if (value > max && value < 4095)
                        maxs[x % 2, y % 2] = value;
                }

            for (int y = 0; y < _height; y++)
                for (int x = 0; x < _width; x++)
                {
                    float value = data[y * _width + x];
                    float max = maxs[x % 2, y % 2];

                    float newValue = Math.Min(4095, max / value * _data[y * _width + x]);
                    _data[y * _width + x] = (ushort)newValue;
                }
        }

        /// <summary>
        /// Divides the image with the specified image.
        /// </summary>
        /// <param name="modulationImage">The image to divide with.</param>
        public void Demodulate(RawImage modulationImage)
        {
            if (modulationImage == null)
                throw new ArgumentNullException("modulationImage");

            if (modulationImage.Width != _width || modulationImage.Height != _height)
                throw new ArgumentException("Images must be of equal size.", "image");

            ushort[] data = modulationImage._data;

            for (int y = 0; y < _height; y++)
                for (int x = 0; x < _width; x++)
                {
                    float m = data[y * _width + x];

                    float newValue = Math.Min(4095, _data[y * _width + x] / (m / 4095));
                    _data[y * _width + x] = (ushort)newValue;
                }
        }
    }
}

/* scaled (<< 4)

//pUnpackedInt8[0] = (byte)(pPackedInt8[1] & 0xF0);
//pUnpackedInt8[1] = (byte)(pPackedInt8[0]);

//pUnpackedInt8[2] = (byte)(pPackedInt8[2] << 4);
//pUnpackedInt8[3] = (byte)((pPackedInt8[2] >> 4) | (pPackedInt8[1] << 4));

*/