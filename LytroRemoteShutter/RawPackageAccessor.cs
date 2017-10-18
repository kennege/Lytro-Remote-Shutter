using System;
using System.Collections.Generic;
using System.Linq;
using UAM.InformatiX.Text.Json;
using UAM.Optics.LightField.Lytro.Metadata;

namespace UAM.Optics.LightField.Lytro.IO
{
    /// <summary>
    /// Provides access to the raw images in a <see cref="LightFieldPackage"/>.
    /// </summary>
    public class RawPackageAccessor : PackageAccessor
    {
        private List<Json.FrameReferences> _frames;

        /// <summary>
        /// Initializes new instance of the <see cref="RawPackageAccessor"/> class.
        /// </summary>
        /// <param name="package">The package with raw images.</param>
        public RawPackageAccessor(LightFieldPackage package) : base(package)
        {

        }

        /// <summary>
        /// When overriden in derived class, initializes the accessor and returns whether any content is available.
        /// </summary>
        /// <returns>true if the package contains any frames; false otherwise.</returns>
        protected override bool Initialize()
        {
            if (Metadata == null || Metadata.Picture == null || Metadata.Picture.FrameArray == null)
                return false;

            _frames = new List<Json.FrameReferences>();

            Json.FrameItem[] frameArray = Metadata.Picture.FrameArray;

            for (int i = 0; i < frameArray.Length; i++)
                LoadFrame(frameArray[i]);

            return _frames.Count > 0;
        }

        private void LoadFrame(Json.FrameItem frameItem)
        {
            if (frameItem == null || frameItem.Frame == null)
                return;

            Json.FrameReferences frameReferences = frameItem.Frame;

            if (frameReferences.Metadata == null && frameReferences.MetadataRef != null)
            {
                LightFieldComponent frameMetadataComponent = Package.GetComponent(frameReferences.MetadataRef).FirstOrDefault();
                if (frameMetadataComponent != null)
                    try
                    {
                        Json.FrameMetadata frameMetadata = new Json.FrameMetadata();
                        frameMetadata.LoadFromJson(frameMetadataComponent.GetDataAsString());
                        frameReferences.Metadata = frameMetadata;
                    }
                    catch (FormatException e) { OnException(e); }
            }

            if (frameReferences.PrivateMetadata == null && frameReferences.PrivateMetadataRef != null)
            {
                LightFieldComponent privateMetadataComponent = Package.GetComponent(frameReferences.PrivateMetadataRef).FirstOrDefault();
                if (privateMetadataComponent != null)
                    try
                    {
                        Json.FrameMetadata privateMetadata = new Json.FrameMetadata();
                        privateMetadata.LoadFromJson(privateMetadataComponent.GetDataAsString());
                        frameReferences.PrivateMetadata = privateMetadata;
                    }
                    catch (FormatException e) { OnException(e); }
            }

            _frames.Add(frameReferences);
        }

        /// <summary>
        /// Gets the number of frames available in the package.
        /// </summary>
        public int FrameCount
        {
            get
            {
                if (!HasContent)
                    return 0;

                return _frames.Count;
            }
        }

        /// <summary>
        /// Gets the <see cref="LightFieldComponent"/> for raw image data.
        /// </summary>
        /// <param name="frameIndex">The frame index.</param>
        /// <returns>the <see cref="LightFieldComponent"/> for raw image data if found in the package; null otherwise.</returns>
        public LightFieldComponent GetImageComponent(int frameIndex = 0)
        {
            if (frameIndex < 0 || frameIndex >= FrameCount)
                return null;

            Json.FrameReferences frame = _frames[frameIndex];
            string imageReference = frame.ImageRef;

            if (imageReference == null)
                return null;

            return Package.GetComponent(imageReference).FirstOrDefault();
        }

        /// <summary>
        /// Gets the <see cref="FieldImage"/> for image data.
        /// </summary>
        /// <param name="frameIndex">The frame index.</param>
        /// <returns>the <see cref="FieldImage"/> for image data if available in the package; null otherwise.</returns>
        public FieldImage GetFieldImage(int frameIndex = 0)
        {
            LightFieldComponent imageComponent = GetImageComponent(frameIndex);
            if (imageComponent == null)
                return null;

            Json.FrameReferences frame = _frames[frameIndex];
            if (frame.Metadata != null)
                return FieldImage.From(imageComponent, frame.Metadata, frame.PrivateMetadata);

            return null;
        }

        /// <summary>
        /// Gets a frame references.
        /// </summary>
        /// <param name="frameIndex">The frame index.</param>
        /// <returns>a frame references if available in the package; null otherwise.</returns>
        public Json.FrameReferences GetFrame(int frameIndex = 0)
        {
            if (frameIndex < 0 || frameIndex >= FrameCount)
                return null;

            return _frames[frameIndex];
        }

        /// <summary>
        /// Gets the picture metadata.
        /// </summary>
        /// <returns>the <see cref="PictureMetadata"/> for the package if available; null otherwise.</returns>
        public PictureMetadata GetPictureMetadata()
        {
            if (Metadata == null || Metadata.Picture == null)
                return null;

            return new PictureMetadata(Metadata.Picture);
        }

        /// <summary>
        /// Gets frame references.
        /// </summary>
        /// <returns>all frame references found in the package; empty array otherwise.</returns>
        public Json.FrameReferences[] GetFrames()
        {
            if (!HasContent)
                return new Json.FrameReferences[0];

            Json.FrameReferences[] frames = new Json.FrameReferences[_frames.Count];
            _frames.CopyTo(frames);
            return frames;
        }
    }
}
