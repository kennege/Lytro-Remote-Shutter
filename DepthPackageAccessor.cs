using System;
using System.Collections.Generic;
using System.Linq;
using UAM.InformatiX.Text.Json;
using UAM.Optics.LightField.Lytro.Metadata;

namespace UAM.Optics.LightField.Lytro.IO
{
    /// <summary>
    /// Provides access to the depth and confidence maps in a <see cref="LightFieldPackage"/>.
    /// </summary>
    public class DepthPackageAccessor : PackageAccessor
    {
        private Json.LytroDepthMap _depthMap;

        /// <summary>
        /// Initializes new instance of the <see cref="DepthPackageAccessor"/> class.
        /// </summary>
        /// <param name="package">The package with raw images.</param>
        public DepthPackageAccessor(LightFieldPackage package) : base(package)
        {

        }

        /// <summary>
        /// When overriden in derived class, initializes the accessor and returns whether any content is available.
        /// </summary>
        /// <returns>true if the package contains any frames; false otherwise.</returns>
        protected override bool Initialize()
        {
            if (Metadata == null || Metadata.Picture == null || Metadata.Picture.AccelerationArray == null)
                return false;

            foreach (Json.AccelerationItem accelerationItem in Metadata.Picture.AccelerationArray)
                if (accelerationItem.Type == Json.LytroDepthMap.Key)
                {
                    _depthMap = accelerationItem.VendorContent as Json.LytroDepthMap;

                    if (_depthMap != null)
                        return true;
                }

            return false;
        }

        /// <summary>
        /// Gets whether the package has a depth map.
        /// </summary>
        public bool HasDepthMap
        {
            get
            {
                return GetDepthMapComponent() != null;
            }
        }

        /// <summary>
        /// Gets the <see cref="LightFieldComponent"/> for depth map.
        /// </summary>
        /// <returns>the <see cref="LightFieldComponent"/> for depth map if found in the package; null otherwise.</returns>
        public LightFieldComponent GetDepthMapComponent()
        {
            if (_depthMap == null || _depthMap.DepthMap == null)
                return null;

            return Package.GetComponent(_depthMap.DepthMap.ImageRef).FirstOrDefault();
        }

        /// <summary>
        /// Gets whether the package has a confidence map.
        /// </summary>
        public bool HasConfidenceMap
        {
            get
            {
                return GetConfidenceMapComponent() != null;
            }
        }

        /// <summary>
        /// Gets the <see cref="LightFieldComponent"/> for confidence map.
        /// </summary>
        /// <returns>the <see cref="LightFieldComponent"/> for confidence map if found in the package; null otherwise.</returns>
        public LightFieldComponent GetConfidenceMapComponent()
        {
            if (_depthMap == null || _depthMap.ConfidenceMap == null)
                return null;

            return Package.GetComponent(_depthMap.ConfidenceMap.ImageRef).FirstOrDefault();
        }
    }
}
