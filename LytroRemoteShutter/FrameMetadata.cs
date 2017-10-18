using System;

namespace UAM.Optics.LightField.Lytro.Metadata
{
    /// <summary>
    /// Represents a frame part of the package metadata.
    /// </summary>
    public class FrameMetadata
    {
        internal Json.FrameItem JsonFrameItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameMetadata"/> class.
        /// </summary>
        public FrameMetadata()
        {
            JsonFrameItem = new Json.FrameItem();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameMetadata"/> class with an existing <see cref="Json.FrameItem"/> storage.
        /// </summary>
        /// <param name="frame">A <see cref="Json.FrameItem"/> to use as a storage for the entry.</param>
        /// <exception cref="ArgumentNullException"><paramref name="frame"/> is null.</exception>
        public FrameMetadata(Json.FrameItem frame)
        {
            if (frame == null)
                throw new ArgumentNullException("frame");

            JsonFrameItem = frame;
        }

        /// <summary>
        /// Gets or sets the picture metadata reference identifier.
        /// </summary>
        public string MetadataReference
        {
            get { return IsFrameAvailable ? JsonFrameItem.Frame.MetadataRef : null; }
            set { EnsureFrame(); JsonFrameItem.Frame.MetadataRef = value; }
        }
        /// <summary>
        /// Gets or sets the hardware metadata reference identifier.
        /// </summary>
        public string PrivateMetadataReference
        {
            get { return IsFrameAvailable ? JsonFrameItem.Frame.PrivateMetadataRef : null; }
            set { EnsureFrame(); JsonFrameItem.Frame.PrivateMetadataRef = value; }
        }
        /// <summary>
        /// Gets or sets the raw sensor data reference identifier. 
        /// </summary>
        public string ImageReference
        {
            get { return IsFrameAvailable ? JsonFrameItem.Frame.ImageRef : null; }
            set { EnsureFrame(); JsonFrameItem.Frame.ImageRef = value; }
        }

        /// <summary>
        /// Gets or sets whether the frame is a dark calibration frame.
        /// </summary>
        public bool IsDarkFrame
        {
            get { return IsLytroTagsAvailable ? LytroTags.DarkFrame : false; }
            set { EnsureLytroTags(); LytroTags.DarkFrame = value; }
        }
        /// <summary>
        /// Gets or sets whether the frame is a modulation calibration frame.
        /// </summary>
        public bool IsModulationFrame
        {
            get { return IsLytroTagsAvailable ? LytroTags.ModulationFrame : false; }
            set { EnsureLytroTags(); LytroTags.ModulationFrame = value; }
        }

        /// <summary>
        /// Get the com.lytro.tags object in the Json.FrameItem.Parameters.VendorContent if exists, null otherwise. 
        /// </summary>
        protected Json.LytroTags LytroTags
        {
            get
            {
                if (JsonFrameItem.Parameters == null) return null;
                if (JsonFrameItem.Parameters.VendorContent == null) return null;
                return JsonFrameItem.Parameters.VendorContent[Json.LytroTags.Key] as Json.LytroTags;
            }
        }

        /// <summary>
        /// Ensures the Json.FrameItem.Framce instance.
        /// </summary>
        protected void EnsureFrame()
        {
            if (JsonFrameItem.Frame == null)
                JsonFrameItem.Frame = new Json.FrameReferences();
        }
        /// <summary>
        /// Ensures the Json.FrameItem.Parameters.VendorContent instance.
        /// </summary>
        protected void EnsureParameters()
        {
            if (JsonFrameItem.Parameters == null)
                JsonFrameItem.Parameters = new Json.FrameParameters();

            if (JsonFrameItem.Parameters.VendorContent == null)
                JsonFrameItem.Parameters.VendorContent = new System.Collections.Generic.Dictionary<string, object>();
        }
        /// <summary>
        /// Ensures the com.lytro.tags object in the Json.FrameItem.Parameters.VendorContent.
        /// </summary>
        protected void EnsureLytroTags()
        {
            EnsureParameters();

            if (JsonFrameItem.Parameters.VendorContent[Json.LytroTags.Key] == null)
                JsonFrameItem.Parameters.VendorContent[Json.LytroTags.Key] = new Json.LytroTags();
        }

        /// <summary>
        /// Returns whether the Json.FrameItem.Frame instance is available.
        /// </summary>
        protected bool IsFrameAvailable
        {
            get { return JsonFrameItem.Frame != null; }
        }
        /// <summary>
        /// Returns whether the com.lytro.tags object is in the Json.FrameItem.Parameters.VendorContent.
        /// </summary>
        protected bool IsLytroTagsAvailable
        {
            get { return LytroTags != null; }
        }
    }
}
