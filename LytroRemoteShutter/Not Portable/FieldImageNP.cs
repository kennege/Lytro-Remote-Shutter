namespace UAM.Optics.LightField.Lytro
{
    using System;
    using System.IO;
    using UAM.InformatiX.Text.Json;
    using UAM.Optics.LightField.Lytro.IO;
    using UAM.Optics.LightField.Lytro.Metadata;

    partial class FieldImage
    {
        /// <summary>
        /// Loads a <see cref="FieldImage"/> from a <see cref="LightFieldPackage"/> on a disk.
        /// </summary>
        /// <param name="path">Path to the package to load the field image from.</param>
        /// <returns>a new instance of the <see cref="FieldImage"/> class.</returns>
        public static FieldImage From(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            LightFieldPackage package;

            using (Stream packageStream = File.OpenRead(path))
                package = new LightFieldPackage(packageStream);

            return From(package);
        }

        /// <summary>
        /// Loads a <see cref="FieldImage"/> from a raw and metadata components data.
        /// </summary>
        /// <param name="framePath">Path to the raw frame data.</param>
        /// <param name="metadataPath">Path to the metadata.</param>
        /// <returns>a new instance of the <see cref="FieldImage"/> class.</returns>
        /// <remarks>This overload allows to load an image from components exported from a backup package.</remarks>
        public static FieldImage From(string framePath, string metadataPath)
        {
            if (framePath == null)
                throw new ArgumentNullException("framePath");

            if (metadataPath == null)
                throw new ArgumentNullException("metadataPath");

            byte[] frameData = File.ReadAllBytes(framePath);

            Json.Root root = new Json.Root();
            try { root.LoadFromJson(File.ReadAllText(metadataPath)); }
            catch (FormatException e) { throw new ArgumentException("Invalid metadata.", e); }

            if (root.Master == null || root.Master.Picture == null || root.Master.Picture.FrameArray == null || 
                root.Master.Picture.FrameArray.Length < 1 || root.Master.Picture.FrameArray[0].Frame == null)
                throw new ArgumentException("Critical metadata missing.");

            Json.FrameMetadata frameMetadata = root.Master.Picture.FrameArray[0].Frame.Metadata;

            return From(frameData, frameMetadata);
        }
    }
}
