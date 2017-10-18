using System;
using System.Collections.Generic;
using System.Linq;
using UAM.InformatiX.Text.Json;
using UAM.Optics.LightField.Lytro.Metadata;

namespace UAM.Optics.LightField.Lytro.IO
{
    /// <summary>
    /// Represents a set of flat field images.
    /// </summary>
    public partial class FlatFieldSet
    {
        private class FlatFieldItem : IComparable<FlatFieldItem>
        {
            public int ZoomStep;
            public int FocusStep;
            public string DataReference;
            public string PackageReference;
            public Json.FrameImage FrameImage;

            public int CompareTo(FlatFieldItem other)
            {
                int result = this.ZoomStep.CompareTo(other.ZoomStep);
                if (result != 0)
                    return result;

                return this.FocusStep.CompareTo(other.FocusStep);
            }
        }

        private List<FlatFieldItem> _items = new List<FlatFieldItem>();
        private string _serialNumber;

        /// <summary>
        /// Gets the first camera serial number available in the set.
        /// </summary>
        public string CameraSerialNumber
        {
            get { return _serialNumber; }
        }

        /// <summary>
        /// Loads all available flat field images from a calibration package.
        /// </summary>
        /// <param name="package">The package to load the flat field images from.</param>
        /// <param name="packageReference">Reference to the package (usually its file path).</param>
        /// <returns>a number of flat field images that were added to the current set.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="packageReference"/> is null</exception>
        public int LoadFrom(LightFieldPackage package, string packageReference)
        {
            Dictionary<string, FlatFieldItem> items = new Dictionary<string, FlatFieldItem>(StringComparer.OrdinalIgnoreCase);
            LoadFromPhase1(package, packageReference, items);
            return LoadFromPhase2(items);
        }

        private string LoadFromPhase1(LightFieldPackage package, string packageReference, Dictionary<string, FlatFieldItem> items)
        {
            LightFieldComponent metadataComponent = package.GetMetadata().FirstOrDefault();
            if (metadataComponent == null)
                return null;

            Json.Master master = new Json.Master();
            master.LoadFromJson(metadataComponent.GetDataAsString());

            if (master.Files != null)
                foreach (Json.File file in master.Files)
                    if (file.Name != null && file.Name.StartsWith(@"C:\T1CALIB\MOD_", StringComparison.OrdinalIgnoreCase))
                        if (file.Name.EndsWith(".TXT", StringComparison.OrdinalIgnoreCase))
                        {
                            LightFieldComponent imageMetadataComponent = package.GetComponent(file.DataRef).FirstOrDefault();
                            if (imageMetadataComponent != null)
                            {
                                Json.Root imageRoot = new Json.Root();
                                imageRoot.LoadFromJson(imageMetadataComponent.GetDataAsString());

                                if (imageRoot.Master != null && imageRoot.Master.Picture != null && imageRoot.Master.Picture.FrameArray != null && imageRoot.Master.Picture.FrameArray.Length > 0)
                                {
                                    Json.FrameItem frameItem = imageRoot.Master.Picture.FrameArray[0];
                                    if (frameItem != null && frameItem.Frame != null && frameItem.Frame.Metadata != null && frameItem.Frame.Metadata.Devices != null && frameItem.Frame.Metadata.Devices.Lens != null)
                                    {
                                        string imageFileName = file.Name.Substring(0, file.Name.Length - 3) + "RAW";

                                        FlatFieldItem item;
                                        if (!items.TryGetValue(imageFileName, out item))
                                            items[imageFileName] = item = new FlatFieldItem();

                                        item.FrameImage = frameItem.Frame.Metadata.Image;
                                        item.ZoomStep = (int)frameItem.Frame.Metadata.Devices.Lens.ZoomStep;
                                        item.FocusStep = (int)frameItem.Frame.Metadata.Devices.Lens.FocusStep;

                                        if (_serialNumber == null && frameItem.Frame.PrivateMetadata != null && frameItem.Frame.PrivateMetadata.Camera != null)
                                            _serialNumber = frameItem.Frame.PrivateMetadata.Camera.SerialNumber;
                                    }
                                }
                            }
                        }
                        else if (file.Name.EndsWith(".RAW", StringComparison.OrdinalIgnoreCase))
                        {
                            FlatFieldItem item;
                            if (!items.TryGetValue(file.Name, out item))
                                items[file.Name] = item = new FlatFieldItem();

                            item.DataReference = file.DataRef;
                            item.PackageReference = packageReference;
                        }

            RawPackageAccessor raw = package.AccessRaw();
            if (raw.HasContent)
                foreach (Json.FrameReferences frame in raw.GetFrames())
                    if (frame.Metadata != null && frame.ImageRef != null)
                    {
                        FlatFieldItem item = new FlatFieldItem();
                        item.PackageReference = packageReference;
                        item.DataReference = frame.ImageRef;
                        item.FrameImage = frame.Metadata.Image;

                        if (frame.Metadata.Devices != null && frame.Metadata.Devices.Lens != null)
                        {
                            item.ZoomStep = (int)frame.Metadata.Devices.Lens.ZoomStep;
                            item.FocusStep = (int)frame.Metadata.Devices.Lens.FocusStep;

                            items[frame.ImageRef] = item;
                        }
                    }

            return master.NextFile;
        }
        private int LoadFromPhase2(Dictionary<string, FlatFieldItem> items)
        {
            int count = 0;
            foreach (FlatFieldItem item in items.Values)
                if (item.DataReference != null)
                {
                    int index = _items.BinarySearch(item);
                    if (index < 0) index = ~index;
                    _items.Insert(index, item);
                    ++count;
                }

            return count;
        }

        /// <summary>
        /// Finds the nearest matching flat field image in the set.
        /// </summary>
        /// <param name="metadata">The metadata of the reference image.</param>
        /// <param name="packageReference">The package reference passed in to the <see cref="LoadFrom(LightFieldPackage,string)"/> reference that contains the nearest matching flat field image.</param>
        /// <param name="componentReference">The component reference of the nearest flat field image in the package.</param>
        /// <param name="frameImage">Cached image metadata of the flat field image.</param>
        /// <returns>true if a nearest matching flat field could be found; false otherwise.</returns>
        /// <remarks>Currently, this method returns false if and only if there are no images in the set.</remarks>
        public bool FindNearestFlatFieldImage(Json.FrameMetadata metadata, out string packageReference, out string componentReference, out Json.FrameImage frameImage)
        {
            if (metadata == null)
                throw new ArgumentNullException("metadata");

            if (metadata.Devices == null || metadata.Devices.Lens == null)
                throw new ArgumentException("Critical metadata missing.", "metadata");

            return FindNearestFlatFieldImage((int)metadata.Devices.Lens.ZoomStep, (int)metadata.Devices.Lens.FocusStep, out packageReference, out componentReference, out frameImage);
        }

        /// <summary>
        /// Finds the nearest matching flat field image in the set.
        /// </summary>
        /// <param name="zoomStep">The frame:Devices/Lens/ZoomStep of the reference image.</param>
        /// <param name="focusStep">The frame:Devices/Lens/FocusStep of the reference image.</param>
        /// <param name="packageReference">The package reference passed in to the <see cref="LoadFrom(LightFieldPackage,string)"/> reference that contains the nearest matching flat field image.</param>
        /// <param name="componentReference">The component reference of the nearest flat field image in the package.</param>
        /// <param name="frameImage">Cached image metadata of the flat field image.</param>
        /// <returns>true if a nearest matching flat field could be found; false otherwise.</returns>
        /// <remarks>Currently, this method returns false if and only if there are no images in the set.</remarks>
        public bool FindNearestFlatFieldImage(int zoomStep, int focusStep, out string packageReference, out string componentReference, out Json.FrameImage frameImage)
        {
            packageReference = null;
            componentReference = null;
            frameImage = null;

            FlatFieldItem searchItem = new FlatFieldItem();
            searchItem.FocusStep = focusStep;
            searchItem.ZoomStep = zoomStep;

            int index = _items.BinarySearch(searchItem);
            if (index < 0) index = ~index;
            if (index >= _items.Count) index = _items.Count - 1;

            if (index < 0)
                return false;

            packageReference = _items[index].PackageReference;
            componentReference = _items[index].DataReference;
            frameImage = _items[index].FrameImage;
            return true;
        }
    }
}
