using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UAM.InformatiX.Text.Json;
using UAM.Optics.LightField.Lytro.Metadata;

namespace UAM.Optics.LightField.Lytro.IO
{
    partial class LightFieldPackage
    {
        /// <summary>
        /// Creates a new, single frame <see cref="LightFieldPackage"/> from raw camera files.
        /// </summary>
        /// <param name="rootMetadataPath">The path to the file with picture metadata.</param>
        /// <param name="imageDataPath">The path to the file with the frame raw data.</param>
        /// <returns>A <see cref="LightFieldPackage"/> with components containing the specified files.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rootMetadataPath"/> or <paramref name="imageDataPath"/> is null.</exception>
        /// <exception cref="ArgumentException">Metadata do not contain information required to create the package.</exception>
        /// <remarks>This method is intended to build <see cref="LightFieldPackage"/> from files coming directly from the camera storage. For Lytro camera, the <paramref name="rootMetadataPath"/> and <paramref name="imageDataPath"/> correspond to the TXT and RAW files, respectively.</remarks>
        public static LightFieldPackage FromCameraFiles(string rootMetadataPath, string imageDataPath)
        {
            return FromCameraFiles(rootMetadataPath, new string[] { imageDataPath });
        }

        /// <summary>
        /// Creates a new <see cref="LightFieldPackage"/> from raw camera files.
        /// </summary>
        /// <param name="rootMetadataPath">The path to the file with picture metadata.</param>
        /// <param name="imageDataPaths">Paths to the files with frames raw data in order specified by the metadata.</param>
        /// <returns>A <see cref="LightFieldPackage"/> with components containing the specified files.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rootMetadataPath"/>, <paramref name="imageDataPaths"/> itself or any path in the array is null.</exception>
        /// <exception cref="ArgumentException">Metadata do not contain information required to create the package, or the <paramref name="imageDataPaths"/> contains less paths than there is frames in the metadata.</exception>
        public static LightFieldPackage FromCameraFiles(string rootMetadataPath, params string[] imageDataPaths)
        {
            if (rootMetadataPath == null)
                throw new ArgumentNullException("rootMetadataPath");

            if (imageDataPaths == null)
                throw new ArgumentNullException("imageDataPaths");

            string rootJson = File.ReadAllText(rootMetadataPath);
            Json.Root rootMetadata = new Json.Root();
            rootMetadata.LoadFromJson(rootJson);

            return FromCameraFiles(rootMetadata, from rawDataPath in imageDataPaths
                                                 select File.ReadAllBytes(rawDataPath));
        }

        /// <summary>
        /// Creates a new, single frame <see cref="LightFieldPackage"/> from raw camera data.
        /// </summary>
        /// <param name="rootMetadata">The picture metadata.</param>
        /// <param name="imageData">The frame raw data.</param>
        /// <returns>A <see cref="LightFieldPackage"/> with components containing the supplied data.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rootMetadata"/> or <paramref name="imageData"/> is null.</exception>
        /// <exception cref="ArgumentException">Metadata do not contain information required to create the package.</exception>
        /// <remarks>This method is intended to build <see cref="LightFieldPackage"/> from files coming directly from the camera storage. For Lytro camera, the <paramref name="rootMetadata"/> and <paramref name="imageData"/> correspond to the contents of TXT and RAW files, respectively.</remarks>
        public static LightFieldPackage FromCameraFiles(Json.Root rootMetadata, byte[] imageData)
        {
            return FromCameraFiles(rootMetadata, new byte[][] { imageData });
        }
        /// <summary>
        /// Creates a new <see cref="LightFieldPackage"/> from raw camera files.
        /// </summary>
        /// <param name="rootMetadata">The picture metadata.</param>
        /// <param name="imageData">Frames raw data in order specified by the metadata.</param>
        /// <returns>A <see cref="LightFieldPackage"/> with components containing the specified files.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rootMetadata"/>, <paramref name="imageData"/> itself or any data in the array is null.</exception>
        /// <exception cref="ArgumentException">Metadata do not contain information required to create the package, or the <paramref name="imageData"/> contains less items than there is frames in the metadata.</exception>
        public static LightFieldPackage FromCameraFiles(Json.Root rootMetadata, IEnumerable<byte[]> imageData)
        {
            if (imageData == null)
                throw new ArgumentNullException("imageData");

            if (rootMetadata == null)
                throw new ArgumentNullException("rootMetadata");

            if (rootMetadata.Master == null || rootMetadata.Master.Picture == null || rootMetadata.Master.Picture.FrameArray == null)
                throw new ArgumentException("Critical metadata missing.", "rootMetadata");

            LightFieldPackage package = new LightFieldPackage();
            Json.Master metadata = rootMetadata.Master;

            IEnumerator<byte[]> rawDataEnumerator = imageData.GetEnumerator();
            for (int i = 0; i < metadata.Picture.FrameArray.Length; i++)
            {
                Json.FrameItem frameItem = metadata.Picture.FrameArray[i];
                if (frameItem == null || frameItem.Frame == null || frameItem.Frame.Metadata == null)
                    throw new ArgumentException("Missing metadata for frame " + i + ".", "rootMetadata");

                if (!rawDataEnumerator.MoveNext())
                    throw new ArgumentException("Missing image data for frame " + i + ".", "imageData");

                byte[] data = rawDataEnumerator.Current;
                if (data == null)
                    throw new ArgumentNullException("Image data cannot be null.", "imageData");

                Json.FrameMetadata frameMetadata = frameItem.Frame.Metadata;
                frameItem.Frame.Metadata = null;

                Json.FrameMetadata privateMetadata = frameItem.Frame.PrivateMetadata;
                frameItem.Frame.PrivateMetadata = null;

                AddComponents(package, frameItem.Frame, data, frameMetadata, privateMetadata);
            }

            LightFieldComponent metadataComponent = new LightFieldComponent();
            metadataComponent.ComponentType = 'M';
            metadataComponent.Data = Encoding.UTF8.GetBytes(metadata.ToStringJson());
            metadataComponent.Reference = GenerateRef(metadataComponent.Data);

            package.Components.Insert(1, metadataComponent);

            return package;
        }

        private static void AddComponents(LightFieldPackage package, Json.FrameReferences references, byte[] imageData, Json.FrameMetadata frameMetadata, Json.FrameMetadata privateMetadata)
        {
            global::System.Diagnostics.Debug.Assert(package != null, "Package cannot be null.");
            global::System.Diagnostics.Debug.Assert(references != null, "References cannot be null.");
            global::System.Diagnostics.Debug.Assert(imageData != null, "Image data cannot be null.");
            global::System.Diagnostics.Debug.Assert(frameMetadata != null, "Frame metadata cannot be null.");
            global::System.Diagnostics.Debug.Assert(privateMetadata != null, "Private metadata cannot be null.");

            LightFieldComponent privateComponent = new LightFieldComponent();
            string privateJson = privateMetadata.ToStringJson();
            privateComponent.Data = Encoding.UTF8.GetBytes(privateJson);
            privateComponent.Reference = GenerateRef(privateComponent.Data);
            references.PrivateMetadataRef = privateComponent.Reference;

            LightFieldComponent frameComponent = new LightFieldComponent();
            string frameJson = frameMetadata.ToStringJson();
            frameComponent.Data = Encoding.UTF8.GetBytes(frameJson);
            frameComponent.Reference = GenerateRef(frameComponent.Data);
            references.MetadataRef = frameComponent.Reference;

            LightFieldComponent imageComponent = new LightFieldComponent();
            imageComponent.Data = imageData;
            imageComponent.Reference = GenerateRef(imageComponent.Data);
            references.ImageRef = imageComponent.Reference;

            package.Components.Add(imageComponent);
            package.Components.Add(frameComponent);
            package.Components.Add(privateComponent);
        }

        private static string GenerateRef(byte[] data)
        {
            StringBuilder reference = new StringBuilder(5 + 40);
            reference.Append("sha1-");

            SHA1Managed sha = new SHA1Managed();
            byte[] hash = sha.ComputeHash(data);

            for (int i = 0; i < hash.Length; i++)
                reference.Append(hash[i].ToString("x2"));

            return reference.ToString();
        }
    }
}
