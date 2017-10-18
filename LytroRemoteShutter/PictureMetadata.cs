using System;
using UAM.InformatiX;

namespace UAM.Optics.LightField.Lytro.Metadata
{
    /// <summary>
    /// Represents the picture part of the package metadata.
    /// </summary>
    public class PictureMetadata
    {
        internal Json.Picture JsonPicture;

        /// <summary>
        /// Initializes a new instance of the <see cref="PictureMetadata"/> class.
        /// </summary>
        public PictureMetadata()
        {
            JsonPicture = new Json.Picture();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PictureMetadata"/> class with an existing <see cref="Json.Picture"/> storage.
        /// </summary>
        /// <param name="picture">A <see cref="Json.Picture"/> to use as a storage for the metadata.</param>
        public PictureMetadata(Json.Picture picture)
        {
            if (picture == null)
                throw new ArgumentNullException("picture");

            JsonPicture = picture;
        }

        /// <summary>
        /// Gets or sets whether the picture was starred in Lytro camera or software.
        /// </summary>
        public bool IsStarred
        {
            get { return IsLytroStarsAvailable ? LytroStars.Starred : false; }
            set { EnsureLytroStars(); LytroStars.Starred = value; }
        }

        private FrameMetadata _frame;
        /// <summary>
        /// Gets or sets the metadata of the first frame in this picture.
        /// </summary>
        public FrameMetadata Frame
        {
            get
            {
                if (_frame != null)
                    return _frame;

                Json.FrameItem frameItem = FindFrameItem();
                if (frameItem != null)
                    return _frame = new FrameMetadata(frameItem);

                return null;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                EnsureFrameArray(value.JsonFrameItem);
                _frame = value;
            }
        }

        private RefocusStackAcceleration _refocusStack;
        /// <summary>
        /// Gets or sets the Lytro's refocus acceleration data.
        /// </summary>
        public RefocusStackAcceleration RefocusStack
        {
            get
            {
                if (_refocusStack != null)
                    return _refocusStack;

                Json.AccelerationItem item = FindAcceleration(Json.LytroRefocusStack.Key);
                if (item != null && item.VendorContent is Json.LytroRefocusStack)
                    return _refocusStack = new RefocusStackAcceleration((Json.LytroRefocusStack)item.VendorContent) { Generator = item.Generator };

                return null;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                EnsureAcceleration(Json.LytroRefocusStack.Key, value.JsonRefocusStack, value.Generator);
                _refocusStack = value;
            }
        }

        private ParallaxStackAcceleration _parallaxStack;
        /// <summary>
        /// Gets or sets the Lytro's parallax acceleration data.
        /// </summary>
        public ParallaxStackAcceleration ParallaxStack
        {
            get
            {
                if (_parallaxStack != null)
                    return _parallaxStack;

                Json.AccelerationItem item = FindAcceleration(Json.LytroEdofParallax.Key);
                if (item != null && item.VendorContent is Json.LytroEdofParallax)
                    return _parallaxStack = new ParallaxStackAcceleration((Json.LytroEdofParallax)item.VendorContent) { Generator = item.Generator };

                return null;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                EnsureAcceleration(Json.LytroEdofParallax.Key, value.JsonParallaxStack, value.Generator);
                _parallaxStack = value;
            }
        }

        private DepthMapAcceleration _depthMap;
        /// <summary>
        /// Gets or sets the Lytro's depth map acceleration data.
        /// </summary>
        public DepthMapAcceleration DepthMap
        {
            get
            {
                if (_depthMap != null)
                    return _depthMap;

                Json.AccelerationItem item = FindAcceleration(Json.LytroDepthMap.Key);
                if (item != null && item.VendorContent is Json.LytroDepthMap)
                    return _depthMap = new DepthMapAcceleration((Json.LytroDepthMap)item.VendorContent) { Generator = item.Generator };

                return null;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                EnsureAcceleration(Json.LytroDepthMap.Key, value.JsonDepthMap, value.Generator);
                _depthMap = value;
            }
        }

        /// <summary>
        /// Gets derivation reference identifiers.
        /// </summary>
        public string[] DerivationReferences
        {
            get
            {
                if (JsonPicture.DerivationArray == null)
                    return null;

                return (string[])JsonPicture.DerivationArray.Clone();
            }
        }

        /// <summary>
        /// Gets the first instance of a com.lytro.stars object in the Json.Picture.ViewArray.
        /// </summary>
        protected Json.LytroStars LytroStars
        {
            get
            {
                if (JsonPicture.ViewArray == null) return null;
                for (int i = 0; i < JsonPicture.ViewArray.Length; i++)
                    if (JsonPicture.ViewArray[i] != null)
                        if (JsonPicture.ViewArray[i].Type == Json.LytroStars.Key)
                            return JsonPicture.ViewArray[i].VendorContent as Json.LytroStars;

                return null;
            }
        }

        private Json.FrameItem FindFrameItem()
        {
            if (JsonPicture.FrameArray == null)
                return null;

            if (JsonPicture.FrameArray.Length < 1)
                return null;

            return JsonPicture.FrameArray[0];
        }
        private Json.AccelerationItem FindAcceleration(string key)
        {
            if (JsonPicture.AccelerationArray == null)
                return null;

            for (int i = 0; i < JsonPicture.AccelerationArray.Length; i++)
                if (JsonPicture.AccelerationArray[i] != null)
                    if (JsonPicture.AccelerationArray[i].Type == key)
                        return JsonPicture.AccelerationArray[i];

            return null;
        }

        private void EnsureFrameArray(Json.FrameItem newItem)
        {
            if (JsonPicture.FrameArray == null || JsonPicture.FrameArray.Length < 1)
                JsonPicture.FrameArray = new Json.FrameItem[1] { newItem };
            else
                JsonPicture.FrameArray[0] = newItem;
        }
        private void EnsureAcceleration(string key, object accelerationValue, string generator)
        {
            if (JsonPicture.AccelerationArray == null || JsonPicture.AccelerationArray.Length < 1)
                JsonPicture.AccelerationArray = new Json.AccelerationItem[1];

            int emptyIndex = -1;
            for (int i = 0; i < JsonPicture.AccelerationArray.Length; i++)
            {
                if (JsonPicture.AccelerationArray[i] == null)
                    emptyIndex = i;
                else if (JsonPicture.AccelerationArray[i].Type == key)
                {
                    JsonPicture.AccelerationArray[i].Generator = generator;
                    JsonPicture.AccelerationArray[i].VendorContent = accelerationValue;
                    return;
                }
            }

            Json.AccelerationItem newItem = new Json.AccelerationItem
            {
                Type = Json.LytroRefocusStack.Key,
                Generator = generator,
                VendorContent = accelerationValue
            };

            if (emptyIndex >= 0)
                JsonPicture.AccelerationArray[emptyIndex] = newItem;
            else
                JsonPicture.AccelerationArray = JsonPicture.AccelerationArray.Append(newItem);
        }
        /// <summary>
        /// Ensures a com.lytro.stars instance in the Json.Picture.ViewArray.
        /// </summary>
        protected void EnsureLytroStars()
        {
            if (JsonPicture.ViewArray == null || JsonPicture.ViewArray.Length < 1)
                JsonPicture.ViewArray = new Json.ViewItem[1];

            int emptyIndex = -1;
            for (int i = 0; i < JsonPicture.ViewArray.Length; i++)
            {
                if (JsonPicture.ViewArray[i] == null)
                    emptyIndex = i;
                else if (JsonPicture.ViewArray[i].Type == Json.LytroStars.Key)
                {
                    EnsureLytroStars(JsonPicture.ViewArray[i]);
                    return;
                }
            }

            Json.ViewItem newItem = new Json.ViewItem
            {
                Type = Json.LytroStars.Key,
                VendorContent = new Json.LytroStars()
            };

            if (emptyIndex >= 0)
                JsonPicture.ViewArray[emptyIndex] = newItem;
            else
                JsonPicture.ViewArray = JsonPicture.ViewArray.Append(newItem);
        }
        private void EnsureLytroStars(Json.ViewItem viewItem)
        {
            global::System.Diagnostics.Debug.Assert(viewItem != null);

            if (viewItem.VendorContent is Json.LytroStars)
                return;

            viewItem.VendorContent = new Json.LytroStars();
        }

        /// <summary>
        /// Returns whether the com.lytro.stars object is in the Json.Picture.ViewArray.
        /// </summary>
        protected bool IsLytroStarsAvailable
        {
            get { return LytroStars != null; }
        }
    }
}
