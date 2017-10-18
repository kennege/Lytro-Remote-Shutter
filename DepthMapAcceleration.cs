using System;
using System.Collections.Generic;
using System.Linq;
using UAM.InformatiX;

namespace UAM.Optics.LightField.Lytro.Metadata
{
    /// <summary>
    /// Represents Lytro's depth map acceleration data.
    /// </summary>
    public class DepthMapAcceleration
    {
        internal Json.LytroDepthMap JsonDepthMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepthMapAcceleration"/> class.
        /// </summary>
        public DepthMapAcceleration()
        {
            JsonDepthMap = new Json.LytroDepthMap();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DepthMapAcceleration"/> class with an existing <see cref="Json.LytroDepthMap"/> storage.
        /// </summary>
        /// <param name="depthMap">A <see cref="Json.LytroDepthMap"/> to use as a storage for the parallax.</param>
        public DepthMapAcceleration(Json.LytroDepthMap depthMap)
        {
            if (depthMap == null)
                throw new ArgumentNullException("depthMap");

            JsonDepthMap = depthMap;
        }

        /// <summary>
        /// Gets or sets the depth map generator.
        /// </summary>
        public string Generator { get; set; }


        /// <summary>
        /// Gets or sets the width of the map.
        /// </summary>
        public int Width
        {
            get { return (int)JsonDepthMap.Width; }
            set { JsonDepthMap.Width = value; }
        }
        /// <summary>
        /// Gets or sets the height of the map.
        /// </summary>
        public int Height
        {
            get { return (int)JsonDepthMap.Height; }
            set { JsonDepthMap.Height = value; }
        }
        /// <summary>
        /// Gets or sets the minimum lambda of the map.
        /// </summary>
        public double MinimumLambda
        {
            get { return (double)JsonDepthMap.MinLambda; }
            set { JsonDepthMap.MinLambda = (decimal)value; }
        }
        /// <summary>
        /// Gets or sets the maximum lambda of the map.
        /// </summary>
        public double MaximumLambda
        {
            get { return (double)JsonDepthMap.MaxLambda; }
            set { JsonDepthMap.MaxLambda = (decimal)value; }
        }

        /// <summary>
        /// Gets or sets the format of the depth map data.
        /// </summary>
        public Representation DepthMapRepresentation
        {
            get
            {
                if (JsonDepthMap.DepthMap == null)
                    return Representation.Raw;

                return EnumEx.Parse<Representation>(JsonDepthMap.DepthMap.Representation, true); }
            set
            {
                if (JsonDepthMap.DepthMap == null)
                    JsonDepthMap.DepthMap = new Json.DepthMap();

                JsonDepthMap.DepthMap.Representation = value.ToString().ToLowerInvariant();
            }
        }
        /// <summary>
        /// Gets or sets the depth map data reference identifier.
        /// </summary>
        public string DepthMapReference
        {
            get
            {
                if (JsonDepthMap.DepthMap == null)
                    return null;

                return JsonDepthMap.DepthMap.ImageRef;
            }
            set
            {
                if (JsonDepthMap.DepthMap == null)
                    JsonDepthMap.DepthMap = new Json.DepthMap();

                JsonDepthMap.DepthMap.ImageRef = value;
            }
        }

        /// <summary>
        /// Gets or sets the format of the confidence map data.
        /// </summary>
        public Representation ConfidenceMapRepresentation
        {
            get
            {
                if (JsonDepthMap.ConfidenceMap == null)
                    return Representation.Raw;

                return EnumEx.Parse<Representation>(JsonDepthMap.ConfidenceMap.Representation, true);
            }
            set
            {
                if (JsonDepthMap.ConfidenceMap == null)
                    JsonDepthMap.ConfidenceMap = new Json.ConfidenceMap();

                JsonDepthMap.ConfidenceMap.Representation = value.ToString().ToLowerInvariant();
            }
        }
        /// <summary>
        /// Gets or sets the confidence map data reference identifier.
        /// </summary>
        public string ConfidenceMapReference
        {
            get
            {
                if (JsonDepthMap.ConfidenceMap == null)
                    return null;

                return JsonDepthMap.ConfidenceMap.ImageRef;
            }
            set
            {
                if (JsonDepthMap.ConfidenceMap == null)
                    JsonDepthMap.ConfidenceMap = new Json.ConfidenceMap();

                JsonDepthMap.ConfidenceMap.ImageRef = value;
            }
        }
    }
}