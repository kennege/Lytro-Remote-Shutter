namespace UAM.Optics.LightField.Lytro
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UAM.InformatiX.Text.Json;
    using UAM.Optics.LightField.Lytro.IO;
    using UAM.Optics.LightField.Lytro.Metadata;

    /// <summary>
    /// Represents a light field image with access to the individual microlenses.
    /// </summary>
    public partial class FieldImage
    {
        private int _width;
        private int _height;

        private Json.FrameMetadata _metadata;
        private Json.FrameMetadata _privateMetadata;
        private byte[] _frameData;
        private RawImage _rawImage;
        private DemosaicedImage _demosaicedImage;
        private MicroLensCollection _microLenses;

        /// <summary>
        /// Gets the raw frame metadata.
        /// </summary>
        public Json.FrameMetadata Metadata
        {
            get { return _metadata; }
        }

        /// <summary>
        /// Gets the raw frame private metadata.
        /// </summary>
        public Json.FrameMetadata PrivateMetadata
        {
            get { return _privateMetadata; }
        }
        
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
        /// Gets the raw image.
        /// </summary>
        public RawImage RawImage
        {
            get
            {
                if (_rawImage == null)
                    _rawImage = new RawImage(_frameData, _metadata.Image);

                return _rawImage;
            }
        }

        /// <summary>
        /// Gets the demosaiced image.
        /// </summary>
        public DemosaicedImage ColorImage
        {
            get
            {
                if (_demosaicedImage == null)
                    _demosaicedImage = new DemosaicedImage(RawImage, _metadata.Image);

                return _demosaicedImage;
            }
        }

        /// <summary>
        /// Gets the subaperture image.
        /// </summary>
        /// <param name="u">Horizontal offset from the microlens center.</param>
        /// <param name="v">Vertical offset from the microlens center.</param>
        /// <returns>a subaperture image.</returns>
        public XYImage GetSubapertureImage(int u, int v)
        {
            return new XYImage(ColorImage, MicroLenses, u, v);
        }

        /// <summary>
        /// Gets the microlens imag.e
        /// </summary>
        /// <param name="x">Horizontal microlens index from the image center.</param>
        /// <param name="y">Vertical microlens index from the image center.</param>
        /// <returns>a microlens image.</returns>
        public UVImage GetMicrolensImage(int x, int y)
        {
            return new UVImage(ColorImage, MicroLenses[x, y]);
        }

        private FieldImage(byte[] frameData, Json.FrameMetadata frameMetadata, Json.FrameMetadata privateMetadata)
        {
            _metadata = frameMetadata;
            _privateMetadata = privateMetadata;
            _frameData = frameData;
        }
        private FieldImage(LightFieldComponent frame, Json.FrameMetadata frameMetadata, Json.FrameMetadata privateMetadata)
        {
            _metadata = frameMetadata;
            _privateMetadata = privateMetadata;
            _frameData = frame.Data;

            _width = (int)_metadata.Image.Width;
            _height = (int)_metadata.Image.Height;
        }

        /// <summary>
        /// Loads a <see cref="FieldImage"/> from a <see cref="LightFieldPackage"/>.
        /// </summary>
        /// <param name="package">The package to load the field image from.</param>
        /// <returns>a new instance of the <see cref="FieldImage"/> class.</returns>
        public static FieldImage From(LightFieldPackage package)
        {
            if (package == null)
                throw new ArgumentNullException("package");

            LightFieldComponent metadataComponent = package.GetMetadata().FirstOrDefault();
            if (metadataComponent == null)
                throw new ArgumentException("The package does not contain any metadata.", "package");

            Json.Master master = new Json.Master();
            try { master.LoadFromJson(metadataComponent.GetDataAsString()); }
            catch (FormatException e) { throw new ArgumentException("The package contains invalid metadata.", e); }
            if (master.Picture == null)
                throw new ArgumentException("The package does not contain required metadata.", "package");

            PictureMetadata pictureMetadata = new PictureMetadata(master.Picture);

            LightFieldComponent frameMetadataComponent = package.GetComponent(pictureMetadata.Frame.MetadataReference).FirstOrDefault();
            if (frameMetadataComponent == null)
                throw new ArgumentException("The package does not contain any frame metadata.", "package");

            Json.FrameMetadata frameMetadata = new Json.FrameMetadata();
            try { frameMetadata.LoadFromJson(frameMetadataComponent.GetDataAsString()); }
            catch (FormatException e) { throw new ArgumentException("The package contains invalid metadata.", e); }


            Json.FrameMetadata privateMetadata = new Json.FrameMetadata();
            LightFieldComponent privateMetadataComponent = package.GetComponent(pictureMetadata.Frame.PrivateMetadataReference).FirstOrDefault();
            if (privateMetadataComponent != null)
                try { privateMetadata.LoadFromJson(frameMetadataComponent.GetDataAsString()); }
                catch (FormatException) { }

            LightFieldComponent frameComponent = package.GetComponent(pictureMetadata.Frame.ImageReference).FirstOrDefault();
            if (frameComponent == null)
                throw new ArgumentException("The package does not contain the frame data.", "package");

            return new FieldImage(frameComponent, frameMetadata, privateMetadata);
        }
        /// <summary>
        /// Loads a <see cref="FieldImage"/> from a <see cref="LightFieldComponent"/> and metadata.
        /// </summary>
        /// <param name="frame">The raw frame component.</param>
        /// <param name="frameMetadata">The raw frame metadata.</param>
        /// <param name="privateMetadata">The private metadata.</param>
        /// <returns>a new instance of the <see cref="FieldImage"/> class.</returns>
        public static FieldImage From(LightFieldComponent frame, Json.FrameMetadata frameMetadata, Json.FrameMetadata privateMetadata = null)
        {
            if (frame == null)
                throw new ArgumentNullException("frame");

            return new FieldImage(frame, frameMetadata, privateMetadata);
        }
        /// <summary>
        /// Loads a <see cref="FieldImage"/> from a byte array and metadata.
        /// </summary>
        /// <param name="frameData">The raw frame data.</param>
        /// <param name="frameMetadata">The raw frame metadata.</param>
        /// <param name="privateMetadata">The private metadata.</param>
        /// <returns>a new instance of the <see cref="FieldImage"/> class.</returns>
        public static FieldImage From(byte[] frameData, Json.FrameMetadata frameMetadata, Json.FrameMetadata privateMetadata = null)
        {
            if (frameData == null)
                throw new ArgumentNullException("frameData");

            return new FieldImage(frameData, frameMetadata, privateMetadata);
        }

        /// <summary>
        /// Gets the collection of microlenses in the light field image.
        /// </summary>
        public MicroLensCollection MicroLenses
        {
            get
            {
                if (_microLenses == null)
                    _microLenses = new MicroLensCollection(_metadata);

                return _microLenses;
            }
        }
    }
}
