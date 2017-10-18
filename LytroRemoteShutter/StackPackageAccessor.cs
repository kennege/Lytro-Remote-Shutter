using System;
using System.Collections.Generic;
using System.Linq;
using UAM.InformatiX.Text.Json;
using UAM.Optics.LightField.Lytro.Metadata;

namespace UAM.Optics.LightField.Lytro.IO
{
    /// <summary>
    /// Provides access to the prerendered stack images in a <see cref="LightFieldPackage"/>.
    /// </summary>
    public class StackPackageAccessor : PackageAccessor
    {
        private Json.LytroRefocusStack _refocusStack;
        private Json.LytroEdofParallax _parallaxStack;

        /// <summary>
        /// Initializes new instance of the <see cref="StackPackageAccessor"/> class.
        /// </summary>
        /// <param name="package">The package with raw images.</param>
        public StackPackageAccessor(LightFieldPackage package) : base(package)
        {
             
        }

        /// <summary>
        /// When overriden in derived class, initializes the accessor and returns whether any content is available.
        /// </summary>
        /// <returns>true if the package contains any stacks; false otherwise.</returns>
        protected override bool Initialize()
        {
            if (Metadata == null || Metadata.Picture == null || Metadata.Picture.AccelerationArray == null)
                return false;

            _refocusStack = null;
            _parallaxStack = null;

            Json.AccelerationItem[] accelerationArray = Metadata.Picture.AccelerationArray;

            for (int i = 0; i < accelerationArray.Length; i++)
                LoadAcceleration(accelerationArray[i]);

            return _refocusStack != null || _parallaxStack != null;
        }

        private void LoadAcceleration(Json.AccelerationItem accelerationItem)
        {
            if (accelerationItem == null)
                return;

            switch (accelerationItem.Type)
            {
                case Json.LytroRefocusStack.Key:
                    _refocusStack = accelerationItem.VendorContent as Json.LytroRefocusStack;
                    break;

                case Json.LytroEdofParallax.Key:
                    _parallaxStack = accelerationItem.VendorContent as Json.LytroEdofParallax;
                    break;
            }
        }

        /// <summary>
        /// Gets the <see cref="LightFieldComponent"/> for depth lookup table.
        /// </summary>
        /// <returns>the <see cref="LightFieldComponent"/> for depth lookup table if found in the package; null otherwise.</returns>
        public LightFieldComponent GetDepthLutComponent()
        {
            if (_refocusStack == null || _refocusStack.DepthLut == null)
                return null;

            string imageReference = _refocusStack.DepthLut.ImageRef;

            if (imageReference == null)
                return null;

            return Package.GetComponent(imageReference).FirstOrDefault();
        }

        /// <summary>
        /// Gets whether the package contains a refocus stack.
        /// </summary>
        public bool HasRefocusStack
        {
            get { return _refocusStack != null; }
        }

        /// <summary>
        /// Gets the refocus stack metadata.
        /// </summary>
        /// <returns>the <see cref="RefocusStackAcceleration"/> for the package if available; null otherwise.</returns>
        public RefocusStackAcceleration GetRefocusMetadata()
        {
            if (_refocusStack == null)
                return null;

            return new RefocusStackAcceleration(_refocusStack);
        }

        /// <summary>
        /// Gets whether the package contains a parallax stack.
        /// </summary>
        public bool HasParallaxStack
        {
            get { return _parallaxStack != null; }
        }

        /// <summary>
        /// Gets the parallax stack metadata.
        /// </summary>
        /// <returns>the <see cref="RefocusStackAcceleration"/> for the package if available; null otherwise.</returns>
        public ParallaxStackAcceleration GetParallaxMetadata()
        {
            if (_parallaxStack == null)
                return null;

            return new ParallaxStackAcceleration(_parallaxStack);
        }
    }
}
