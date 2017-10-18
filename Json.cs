using System.Collections.Generic;
using UAM.InformatiX.Text.Json;
using UAM.Optics.LightField.Lytro.Metadata;

[assembly: JsonTypeMapping(Json.LytroTags.Key, typeof(Json.LytroTags))]
[assembly: JsonTypeMapping(Json.LytroStars.Key, typeof(Json.LytroStars))]
[assembly: JsonTypeMapping(Json.LytroRefocusStack.Key, typeof(Json.LytroRefocusStack))]
[assembly: JsonTypeMapping(Json.LytroDepthMap.Key, typeof(Json.LytroDepthMap))]
[assembly: JsonTypeMapping(Json.LytroParameters.Key, typeof(Json.LytroParameters))]
[assembly: JsonTypeMapping(Json.LytroEdofParallax.Key, typeof(Json.LytroEdofParallax))]

namespace UAM.Optics.LightField.Lytro.Metadata
{
    /// <summary>
    /// Encapsulates classes for light field metadata parsing.
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// Represents the root metadata structure. Used in calibration files only.
        /// </summary>
        public class Root
        {
            /// <summary />
            public Dictionary<string, object> Debug { get; set; }
            /// <summary />
            public Master Master { get; set; }
            /// <summary />
            public CompressedFrame CompressedFrame { get; set; }
        }

        #region Common

        /// <summary>
        /// Represents 2D value.
        /// </summary>
        public abstract class XYValue
        {
            /// <summary />
            public virtual decimal X { get; set; }
            /// <summary />
            public virtual decimal Y { get; set; }
        }

        /// <summary>
        /// Represents 1D value.
        /// </summary>
        public abstract class ZValue
        {
            /// <summary />
            public virtual decimal Z { get; set; }
        }

        /// <summary>
        /// Represents 3D value.
        /// </summary>
        public abstract class XYZValue
        {
            /// <summary />
            public virtual decimal X { get; set; }
            /// <summary />
            public virtual decimal Y { get; set; }
            /// <summary />
            public virtual decimal Z { get; set; }
        }

        #endregion

        #region File Metadata

        /// <summary>
        /// Represents //compressedFrame structure.
        /// </summary>
        public class CompressedFrame
        {
            /// <summary />
            public CompressionMetadata Metadata { get; set; }
        }

        /// <summary>
        /// Represents //compressedFrame/metadata structure.
        /// </summary>
        public class CompressionMetadata
        {
            /// <summary />
            public string Representation { get; set; }
            /// <summary />
            public decimal[] CcmRgbToYuvArray { get; set; }
            /// <summary />
            public decimal[] CcmYuvToRgbArray { get; set; }
            /// <summary />
            public bool Demodulated { get; set; }
        }

        /// <summary>
        /// Represents //master structure. This is the root structure for standard picture files.
        /// </summary>
        public class Master
        {
            /// <summary>
            /// Optional.
            /// </summary>
            public Picture Picture { get; set; }
            /// <summary>
            /// May be empty.
            /// </summary>
            public ThumbnailItem[] ThumbnailArray { get; set; }
            /// <summary />
            public Version Version { get; set; }

            /// <summary />
            public File[] Files { get; set; }
            /// <summary />
            public string NextFile { get; set; }
        }

        /// <summary>
        /// Represents /files structure.
        /// </summary>
        public class File
        {
            /// <summary />
            public string Name { get; set; }
            /// <summary />
            public string DataRef { get; set; }
        }

        /// <summary>
        /// Represents /thumbnailArray item structure.
        /// </summary>
        public class ThumbnailItem
        {
            /// <summary />
            public string ImageRef { get; set; }
            /// <summary />
            public string ImageUrl { get; set; }
            /// <summary />
            public object Image { get; set; }
            /// <summary />
            /// <example>jpeg, array</example>
            public string Representation { get; set; }
            /// <summary />
            public decimal Width { get; set; }
            /// <summary />
            public decimal Height { get; set; }
        }

        #endregion

        #region Picture Metadata

        /// <summary>
        /// Represents /picture structure.
        /// </summary>
        public class Picture
        {
            /// <summary />
            public FrameItem[] FrameArray { get; set; }
            /// <summary />
            public ViewItem[] ViewArray { get; set; }
            /// <summary />
            public AccelerationItem[] AccelerationArray { get; set; }
            /// <summary>
            /// Required. E.g. great-grandfather, grandfather, father, current. Generated from camera sn, picture index, and nonce.
            /// </summary>
            public string[] DerivationArray { get; set; }
        }

        /// <summary>
        /// Represents /picture/frameArray item structure.
        /// </summary>
        public class FrameItem
        {
            /// <summary />
            public FrameReferences Frame { get; set; }
            /// <summary />
            public FrameParameters Parameters { get; set; }
        }

        /// <summary>
        /// Represents /picture/frameArray/frame structure.
        /// </summary>
        public class FrameReferences
        {
            /// <summary>
            /// Required.
            /// </summary>
            public string MetadataRef { get; set; }
            /// <summary>
            /// Optional, but <see cref="MetadataRef"/> must be specified in either case.
            /// </summary>
            public FrameMetadata Metadata { get; set; }

            /// <summary>
            /// Optional.
            /// </summary>
            public string PrivateMetadataRef { get; set; }
            /// <summary>
            /// Optional, but cannot be present if <see cref="PrivateMetadataRef"/> is not present.
            /// </summary>
            public FrameMetadata PrivateMetadata { get; set; }

            /// <summary>
            /// Required.
            /// </summary>
            public string ImageRef { get; set; }

            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Description { get; set; }
        }

        /// <summary>
        /// Represents /picture/frameArray/parameters structure.
        /// </summary>
        public class FrameParameters
        {
            /// <summary />
            [JsonMemberTypeInKey]
            public Dictionary<string, object> VendorContent { get; set; }
        }

        /// <summary>
        /// Represents /viewArray item structure.
        /// </summary>
        public class ViewItem
        {
            /// <summary />
            public string Type { get; set; }

            /// <summary />
            [JsonMemberTypeIn("Type")]
            public object VendorContent { get; set; }
        }

        /// <summary>
        /// Represents /accelerationArray item structure.
        /// </summary>
        public class AccelerationItem
        {
            /// <summary>
            /// Uniquely identifies the type of this acceleration object.
            /// </summary>
            public string Type { get; set; }
            /// <summary>
            /// Optional, software system used to compute acceleration data (e.g., images).
            /// </summary>
            public string Generator { get; set; }
            /// <summary />
            [JsonMemberTypeIn("Type")]
            public object VendorContent { get; set; }

            /// <summary>
            /// Optional, presumed false if not present.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public bool Keep { get; set; }
        }

        /// <summary>
        /// Represents /version structure.
        /// </summary>
        public class Version
        {
            /// <summary>
            /// Initially 1, incremented for incompatible versions.
            /// </summary>
            public decimal Major { get; set; }
            /// <summary>
            /// Initially 0, incremented for forward-compatible versions, reset when major is incremented.
            /// </summary>
            public decimal Minor { get; set; }
            /// <summary>
            /// Included only if the JSON is formatted to a provisional version. Format is iso8601: yyyy-mm-dd.
            /// </summary>
            public string ProvisionalDate { get; set; }
        }

        #endregion

        #region Vendor Content

        #region com.lytro.tags

        /// <summary>
        /// Represents com.lytro.tags structure.
        /// </summary>
        public class LytroTags
        {
            /// <summary>
            /// JSON type name of this structure.
            /// </summary>
            public const string Key = "com.lytro.tags";

            /// <summary />
            public bool DarkFrame { get; set; }
            /// <summary />
            public bool ModulationFrame { get; set; }
        }

        #endregion

        #region com.lytro.stars

        /// <summary>
        /// Represents com.lytro.stars structure.
        /// </summary>
        public class LytroStars
        {
            /// <summary>
            /// JSON type name of this structure.
            /// </summary>
            public const string Key = "com.lytro.stars";

            /// <summary />
            public bool Starred { get; set; }
        }

        #endregion

        #region com.lytro.acceleration.refocusStack

        /// <summary>
        /// Represents com.lytro.acceleration.refocusStack structure.
        /// </summary>
        public class LytroRefocusStack
        {
            /// <summary>
            /// JSON type name of this structure.
            /// </summary>
            public const string Key = "com.lytro.acceleration.refocusStack";

            /// <summary />
            public Dictionary<string, object> ViewParameters { get; set; }
            /// <summary />
            public DisplayParameters DisplayParameters { get; set; }
            /// <summary />
            public decimal DefaultLambda { get; set; }
            /// <summary />
            public DepthLut DepthLut { get; set; }
            /// <summary />
            public RefocusImageItem[] ImageArray { get; set; }
            /// <summary />
            public BlockOfRefocusImages BlockOfImages { get; set; }
        }

        /// <summary>
        /// Represents com.lytro.acceleration.refocusStack/displayParameters structure.
        /// </summary>
        public class DisplayParameters
        {
            /// <summary />
            public DisplayDimensions DisplayDimensions { get; set; }
        }

        /// <summary>
        /// Represents com.lytro.acceleration.refocusStack/displayParameters/displayDimensions structure.
        /// </summary>
        public class DisplayDimensions
        {
            /// <summary />
            public string Mode { get; set; }
            /// <summary />
            public DimensionsValue Value { get; set; }
        }

        /// <summary>
        /// Represents com.lytro.acceleration.refocusStack/displayParameters/displayDimensions/value structure.
        /// </summary>
        public class DimensionsValue
        {
            /// <summary />
            public decimal Width { get; set; }
            /// <summary />
            public decimal Height { get; set; }
        }

        /// <summary>
        /// Represents com.lytro.acceleration.refocusStack/depthLut structure.
        /// </summary>
        public class DepthLut
        {
            /// <summary />
            public string Representation { get; set; }
            /// <summary />
            public decimal Width { get; set; }
            /// <summary />
            public decimal Height { get; set; }
            /// <summary />
            public string ImageRef { get; set; }
        }

        /// <summary>
        /// Represents com.lytro.acceleration.refocusStack/imageArray item structure.
        /// </summary>
        public class RefocusImageItem
        {
            /// <summary />
            public string Representation { get; set; }
            /// <summary />
            public decimal Width { get; set; }
            /// <summary />
            public decimal Height { get; set; }
            /// <summary />
            public decimal Lambda { get; set; }
            /// <summary />
            public string ImageRef { get; set; }
        }

        /// <summary>
        /// Represents com.lytro.acceleration.refocusStack/blockOfImages structure.
        /// </summary>
        public class BlockOfRefocusImages
        {
            /// <summary />
            public string BlockOfImagesRef { get; set; }
            /// <summary />
            public string Representation { get; set; }
            /// <summary />
            public BlockRefocusMetadata[] MetadataArray { get; set; }
        }

        /// <summary>
        /// Represents com.lytro.acceleration.refocusStack/blockOfImages/metadataArray item structure.
        /// </summary>
        public class BlockRefocusMetadata
        {
            /// <summary />
            public decimal Width { get; set; }
            /// <summary />
            public decimal Height { get; set; }
            /// <summary />
            public decimal Lambda { get; set; }
        }

        #endregion

        #region com.lytro.acceleration.depthMap

        /// <summary>
        /// Represents com.lytro.acceleration.depthMap structure.
        /// </summary>
        public class LytroDepthMap
        {
            /// <summary>
            /// JSON type name of this structure.
            /// </summary>
            public const string Key = "com.lytro.acceleration.depthMap";

            /// <summary />
            public decimal Width { get; set; }
            /// <summary />
            public decimal Height { get; set; }
            /// <summary />
            public decimal MinLambda { get; set; }
            /// <summary />
            public decimal MaxLambda { get; set; }
            /// <summary />
            public DepthMap DepthMap { get; set; }
            /// <summary />
            public ConfidenceMap ConfidenceMap { get; set; }
        }

        /// <summary>
        /// Represents com.lytro.acceleration.depthMap/DepthMap structure.
        /// </summary>
        public class DepthMap
        {
            /// <summary />
            public string Representation { get; set; }
            /// <summary />
            public string ImageRef { get; set; }
        }

        /// <summary>
        /// Represents com.lytro.acceleration.depthMap/ConfidenceMap structure.
        /// </summary>
        public class ConfidenceMap
        {
            /// <summary />
            public string Representation { get; set; }
            /// <summary />
            public string ImageRef { get; set; }
        }

        #endregion

        #region com.lytro.acceleration.edofParallax

        /// <summary>
        /// Represents com.lytro.acceleration.edofParallax structure.
        /// </summary>
        public class LytroEdofParallax
        {
            /// <summary>
            /// JSON type name of this structure.
            /// </summary>
            public const string Key = "com.lytro.acceleration.edofParallax";

            /// <summary />
            public DisplayParameters DisplayParameters { get; set; }
            /// <summary />
            public ParallaxImageItem[] ImageArray { get; set; }
            /// <summary />
            public BlockOfParallaxImages BlockOfImages { get; set; }
        }

        /// <summary>
        /// Represents com.lytro.acceleration.edofParallax/imageArray item structure.
        /// </summary>
        public class ParallaxImageItem
        {
            /// <summary />
            public string Representation { get; set; }
            /// <summary />
            public decimal Width { get; set; }
            /// <summary />
            public decimal Height { get; set; }
            /// <summary />
            public Coordinate2D Coord { get; set; }
            /// <summary />
            public string ImageRef { get; set; }
        }

        /// <summary>
        /// Represents 2D coordinate.
        /// </summary>
        public class Coordinate2D : XYValue { }

        /// <summary>
        /// Represents com.lytro.acceleration.edofParallax/blockOfImages structure.
        /// </summary>
        public class BlockOfParallaxImages
        {
            /// <summary />
            public string BlockOfImagesRef { get; set; }
            /// <summary />
            public string Representation { get; set; }
            /// <summary />
            public BlockParallaxMetadata[] MetadataArray { get; set; }
        }

        /// <summary>
        /// Represents com.lytro.acceleration.edofParallax/blockOfImages/metadataArray item structure.
        /// </summary>
        public class BlockParallaxMetadata
        {
            /// <summary />
            public decimal Width { get; set; }
            /// <summary />
            public decimal Height { get; set; }
            /// <summary />
            public Coordinate2D Coord { get; set; }
        }

        #endregion

        #region com.lytro.parameters

        /// <summary>
        /// Represents com.lytro.parameters structure.
        /// </summary>
        public class LytroParameters
        {
            /// <summary>
            /// JSON type name of this structure.
            /// </summary>
            public const string Key = "com.lytro.parameters";

            /// <summary />
            public Event[] EventArray { get; set; }
        }

        /// <summary>
        /// Represents com.lytro.parameters/eventArray item structure.
        /// </summary>
        public class Event
        {
            /// <summary />
            public string ZuluTime { get; set; }
            /// <summary />
            public RatioXY ViewPitchRatio { get; set; }
            /// <summary />
            public decimal ViewRefocusLambda { get; set; }
            /// <summary />
            public string ViewRefocusDof { get; set; }
            /// <summary />
            public Coordinate2D ViewParallaxCoord { get; set; }
            /// <summary />
            public decimal ViewParallaxLambda { get; set; }
            /// <summary />
            public decimal ViewParallaxOverscan { get; set; }
            /// <summary />
            public decimal ViewStereoBaselineLength { get; set; }
            /// <summary />
            public decimal ViewStereoBaselineAngle { get; set; }
            /// <summary />
            public decimal ViewOrientation { get; set; }
        }

        /// <summary>
        /// Represents 2D ratio.
        /// </summary>
        public class RatioXY : XYValue { }

        #endregion

        #endregion

        #region Frame Metadata

        /// <summary>
        /// Represents the frame: root metadata structure.
        /// </summary>
        public class FrameMetadata
        {
            /// <summary />
            /// <example>2d, lightField</example>
            public string Type { get; set; }
            /// <summary />
            public FrameImage Image { get; set; }
            /// <summary />
            public Devices Devices { get; set; }
            /// <summary />
            public Modes Modes { get; set; }
            /// <summary />
            public Camera Camera { get; set; }
            /// <summary />
            public string CalibrationDataRef { get; set; }
            /// <summary />
            public CompressionDetails CompressionDetails { get; set; }

            /// <summary>
            /// This is private metadata. References only, no actual data.
            /// </summary>
            public FrameReferences[] DerivationArray { get; set; }
        }

        /// <summary>
        /// Represents frame:image structure.
        /// </summary>
        public class FrameImage
        {
            /// <summary>
            /// E.g. 3280.
            /// </summary>
            public decimal Width { get; set; }
            /// <summary>
            /// E.g. 3280.
            /// </summary>
            public decimal Height { get; set; }
            /// <summary />
            public decimal Orientation { get; set; }
            /// <summary />
            /// <example>jpeg, jpegRaw, dng, raw</example>
            public string Representation { get; set; }
            /// <summary>
            /// Required for <see cref="Representation"/> values jpegRaw and raw, not present otherwise.
            /// </summary>
            public RawDetails RawDetails { get; set; }
            /// <summary />
            public Color Color { get; set; }
            /// <summary />
            public decimal ModulationExposureBias { get; set; }
            /// <summary />
            public decimal LimitExposureBias { get; set; }
        }

        /// <summary>
        /// Represents frame:image/rawDetails structure.
        /// </summary>
        public class RawDetails
        {
            /// <summary>
            /// How to interpret the value of an individual pixel (after it is unpacked from the raw data).
            /// </summary>
            public PixelFormat PixelFormat { get; set; }
            /// <summary>
            /// How individual pixels are packed into the raw data.
            /// </summary>
            public PixelPacking PixelPacking { get; set; }
            /// <summary>
            /// Describes the pattern of pixels in the raw image.
            /// </summary>
            public Mosaic Mosaic { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            public Crop CropFromSensor { get; set; }
        }

        /// <summary>
        /// Represents frame:image/rawDetails/pixelFormat structure.
        /// </summary>
        public class PixelFormat
        {
            /// <summary>
            /// Shift required to right-justify the valid bitsPerPixel (may not be negative).
            /// </summary>
            public decimal RightShift { get; set; }
            /// <summary>
            /// Black value (values below this are negative values, due to noise).
            /// </summary>
            /// <remarks>
            /// This member is post-shift pixel value (numeric) as per WO 2012/170111 A1.
            /// </remarks>
            public BayerValue Black { get; set; }
            /// <summary>
            /// White value (values above this are special reserved values).
            /// </summary>
            /// <remarks>
            /// This member is post-shift pixel value (numeric) as per WO 2012/170111 A1.
            /// </remarks>
            public BayerValue White { get; set; }

            /// <summary>
            /// Value for defective pixels.
            /// </summary>
            /// <remarks>
            /// This member was added as per WO 2012/170111 A1 only, where it is post-shift pixel value (numeric).
            /// </remarks>
            public BayerValue Defect { get; set; }
        }

        /// <summary>
        /// Represents a Bayer filtered pixel value
        /// </summary>
        public class BayerValue
        {
            /// <summary />
            public decimal R { get; set; }
            /// <summary />
            public decimal Gr { get; set; }
            /// <summary />
            public decimal Gb { get; set; }
            /// <summary />
            public decimal B { get; set; }
        }

        /// <summary>
        /// Represents frame:image/rawDetails/pixelPacking structure.
        /// </summary>
        public class PixelPacking
        {
            /// <summary>Data are a byte stream. Little endian means LSBs come before MSBs. Big means the opposite.</summary>
            /// <example>little, big</example>
            public string Endianness { get; set; }
            /// <summary>
            /// Bits per pixel in the raw stream. Greater than or equal to <see cref="Sensor.BitsPerPixel"/>.
            /// </summary>
            public decimal BitsPerPixel { get; set; }
        }

        /// <summary>
        /// Represents frame:image/rawDetails/mosaic structure.
        /// </summary>
        public class Mosaic
        {
            /// <summary>
            /// Name of tile pattern.
            /// </summary>
            /// <example>r,gr:gb,b</example>
            public string Tile { get; set; }
            /// <summary>
            /// Name of pixel in tile pattern that is upper left in the image.
            /// </summary>
            /// <example>r, gr, gb, b</example>
            public string UpperLeftPixel { get; set; }
        }

        /// <summary>
        /// Represents frame:image/rawDetails/cropFromSensor structure.
        /// </summary>
        public class Crop
        {
            /// <summary>
            /// Number of columns of pixels removed, from the left side of the sensor image.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public decimal Left { get; set; }
            /// <summary>
            /// Number of columns of pixels removed, from the right side of the sensor image.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public decimal Right { get; set; }
            /// <summary>
            /// Number of rows of pixels removed, from the top of the sensor image.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public decimal Top { get; set; }
            /// <summary>
            /// Number of rows of pixels removed, from the bottom of the sensor image.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public decimal Bottom { get; set; }
        }

        /// <summary>
        /// Represents frame:image/color structure.
        /// </summary>
        public class Color
        {
            /// <summary>
            /// Optional if <see cref="Mode"/> is recommended, 3x3 array, row major, premultiplies color vector (r,g,b)T.
            /// </summary>
            public decimal[] CcmRgbToSrgbArray { get; set; }
            /// <summary>
            /// Optional if <see cref="Mode"/> is recommended.
            /// </summary>
            public decimal Gamma { get; set; }
            /// <summary />
            public Dictionary<string, object> Applied { get; set; }
            /// <summary>
            /// Optional if <see cref="Mode"/> is recommended. 
            /// </summary>
            /// <remarks>
            /// This value is RGB structure only as per WO 2012/170111 A1.
            /// </remarks>
            public BayerValue WhiteBalanceGain { get; set; }

            /// <summary />
            /// <example>bakedIntoFrame, recommended</example>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Mode { get; set; }
            /// <summary>
            /// Optional (never baked in), can look-up in XYZ, then convert to sRGB if required.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public decimal CctKelvin { get; set; }
        }

        /// <summary>
        /// Represents frame:devices structure.
        /// </summary>
        public class Devices
        {
            /// <summary />
            public Clock Clock { get; set; }
            /// <summary />
            public Sensor Sensor { get; set; }
            /// <summary />
            public Lens Lens { get; set; }
            /// <summary />
            public NDFilter NDFilter { get; set; }
            /// <summary />
            public Shutter Shutter { get; set; }
            /// <summary />
            public Soc Soc { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            public Accelerometer Accelerometer { get; set; }
            /// <summary />
            public Mla Mla { get; set; }
            
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public Gps Gps { get; set; }
            /// <summary>
            /// May be empty.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public FlashItem[] FlashArray { get; set; }
        }

        /// <summary>
        /// Represents frame:devices/clock structure.
        /// </summary>
        public class Clock
        {
            /// <summary>
            /// ISO 8601, e.g., "2011-03-30T18:07:25.134Z", fraction to millisecond, Zulu time (no local offset).
            /// </summary>
            public string ZuluTime { get; set; }

            /// <summary>
            /// Optional, e.g., "2011-03-30T10:07:25.134Z", fraction to millisecond, no trailing "Z".
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string LocalTime { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Make { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Model { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Firmware { get; set; }
        }

        /// <summary>
        /// Represents frame:devices/sensor structure.
        /// </summary>
        public class Sensor
        {
            /// <summary />
            [JsonIgnoreDefault]
            public decimal BitsPerPixel { get; set; }
            /// <summary />
            public Mosaic Mosaic { get; set; }
            /// <summary />
            [JsonIgnoreDefault]
            public decimal Iso { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            public BayerValue AnalogGain { get; set; }
            /// <summary />
            [JsonIgnoreDefault]
            public decimal PixelPitch { get; set; }
            /// <summary>
            ///  This is private metadata.
            /// </summary>
            public string SensorSerial { get; set; }

            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Make { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Model { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Firmware { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public BayerValue DigitalGain { get; set; }
        }

        /// <summary>
        /// Represents frame:devices/lens structure.
        /// </summary>
        public class Lens
        {
            /// <summary>
            /// Distance ahead of the MLA that is in focus at infinity (along optical axis).
            /// </summary>
            public decimal InfinityLambda { get; set; }
            /// <summary>
            /// Optional. Focal length of the lens (taking zoom into account).
            /// </summary>
            public decimal FocalLength { get; set; }
            /// <summary>
            /// Optional, zoom stepper-motor position.
            /// </summary>
            public decimal ZoomStep { get; set; }
            /// <summary>
            /// Optional, focus stepper-motor position.
            /// </summary>
            public decimal FocusStep { get; set; }
            /// <summary />
            public decimal FNumber { get; set; }
            /// <summary>
            /// Optional, in degrees of Celsius.
            /// </summary>
            public decimal Temperature { get; set; }
            /// <summary />
            public decimal TemperatureAdc { get; set; }
            /// <summary />
            public decimal ZoomStepperOffset { get; set; }
            /// <summary />
            public decimal FocusStepperOffset { get; set; }
            /// <summary />
            public OffsetXYZ ExitPupilOffset { get; set; }

            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Make { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Model { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Firmware { get; set; }
            /// <summary>
            /// Object-coordinate focus distance of the lens to the MLA, in meters.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public decimal FocusDistance { get; set; }
            /// <summary>
            /// Ratio of focal length to the diameter of the entrance pupil (traditional f-number).
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public decimal EntranceFNumber { get; set; }
            /// <summary>
            /// Ratio of focal length to the diameter of the exit pupil.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public decimal ExitFNumber { get; set; }
        }

        /// <summary>
        /// Represents 2D offset value.
        /// </summary>
        public class OffsetXY : XYValue { }

        /// <summary>
        /// Represents 1D offset value.
        /// </summary>
        public class OffsetZ : ZValue { }

        /// <summary>
        /// Represents 3D offset value.
        /// </summary>
        public class OffsetXYZ : XYZValue { }

        /// <summary>
        /// Represents frame:devices/ndfilter structure.
        /// </summary>
        public class NDFilter
        {
            /// <summary />
            public decimal ExposureBias { get; set; }

            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Make { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Model { get; set; }
            /// <summary>
            /// Effective light blockage for this frame.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public decimal Stops { get; set; }
        }

        /// <summary>
        /// Represents frame:devices/shutter structure.
        /// </summary>
        public class Shutter
        {
            /// <summary />
            /// <example>sensorRolling, sensorOpenApertureClose, apertureOpenClose</example>
            public string Mechanism { get; set; }
            /// <summary>
            /// Approximate exposure time from start of first-pixel exposure to end of last-pixel exposure.
            /// </summary>
            public decimal FrameExposureDuration { get; set; }
            /// <summary>
            /// Approximate exposure time of an individual pixel.
            /// </summary>
            public decimal PixelExposureDuration { get; set; }

            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Make { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Model { get; set; }
        }

        /// <summary>
        /// Represents frame:devices/soc structure.
        /// </summary>
        public class Soc
        {
            /// <summary>
            /// Optional, in degrees of Celsius.
            /// </summary>
            public decimal Temperature { get; set; }
            /// <summary />
            public decimal TemperatureAdc { get; set; }


            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Make { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Model { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Firmware { get; set; }
        }

        /// <summary>
        /// Represents frame:
        /// </summary>
        public class Accelerometer
        {
            /// <summary>
            /// Must not be empty.
            /// </summary>
            public AccelerometerValue[] SampleArray { get; set; }

            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Make { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Model { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Firmware { get; set; }
        }

        /// <summary>
        /// Represents an accelerometer sensor reading.
        /// </summary>
        public class AccelerometerValue : XYZValue
        {
            /// <summary>
            /// Acceleration in the camera's positive x direction (right).
            /// </summary>
            public override decimal X { get; set; }
            /// <summary>
            /// Acceleration in the camera's positive y direction (up).
            /// </summary>
            public override decimal Y { get; set; }
            /// <summary>
            /// Acceleration in the camera's positive z direction (backward).
            /// </summary>
            public override decimal Z { get; set; }
            /// <summary>
            /// Measured from clock value (negative before, positive after), in seconds.
            /// </summary>
            public decimal Time { get; set; }
        }

        /// <summary>
        /// Represents frame:devices/mla structure.
        /// </summary>
        public class Mla
        {
            /// <summary>
            /// The pattern of the microlens array.
            /// </summary>
            /// <example>squareUniform, hexUniformRowMajor, hexUniformColumnMajor</example>
            public string Tiling { get; set; }
            /// <summary>
            /// The pitch of the lenslets in a microlens array (the distance between the centers of neighboring microlenses, in meters).
            /// </summary>
            public decimal LensPitch { get; set; }
            /// <summary>
            /// Positive values indicate CW rotation, viewed from the optical axis in front of the camera, in radians.
            /// </summary>
            public decimal Rotation { get; set; }

            /// <summary>
            /// May be empty.
            /// </summary>
            public DefectXY[] DefectArray { get; set; }
            /// <summary />
            public string Config { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            public ScaleXY ScaleFactor { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            public OffsetXYZ SensorOffset { get; set; }

            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Make { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Model { get; set; }
        }

        /// <summary>
        /// Defect 2D value.
        /// </summary>
        public class DefectXY : XYValue
        {
            /// <summary>
            /// Increases left-to-right.
            /// </summary>
            public override decimal X { get; set; }
            /// <summary>
            /// Increases top-to-bottom in square grid, 60 degrees down and to the right in hex grid.
            /// </summary>
            public override decimal Y { get; set; }
            /// <summary />
            public string Type { get; set; }
        }

        /// <summary>
        /// Scaling 2D value.
        /// </summary>
        public class ScaleXY : XYValue { }

        /// <summary>
        /// Represents frame:devices/gps structure.
        /// </summary>
        public class Gps
        {
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Make { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Model { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Firmware { get; set; }
        }

        /// <summary>
        /// Represents frame:devices/flashArray item structure.
        /// </summary>
        public class FlashItem
        {
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Make { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Model { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public string Firmware { get; set; }
            /// <summary />
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public decimal Brightness { get; set; }
            /// <summary>
            /// In seconds.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public decimal Duration { get; set; }
        }

        /// <summary>
        /// Represents frame:modes structure.
        /// </summary>
        public class Modes
        {
            /// <summary />
            public string Creative { get; set; }
            /// <summary />
            public RegionOfInterest[] RegionOfInterestArray { get; set; }
            /// <summary />
            public bool ManualControls { get; set; }
            /// <summary />
            public string ExposureDurationMode { get; set; }
            /// <summary />>
            public decimal ExposureDurationSpec { get; set; }
            /// <summary />
            public string IsoMode { get; set; }
            /// <summary />
            public decimal IsoSpec { get; set; }
            /// <summary />
            public string NDFilterMode { get; set; }
            /// <summary />
            public bool NDFilterSpec { get; set; }
            /// <summary />
            public bool ExposureLock { get; set; }
            /// <summary />
            public decimal Overscan { get; set; }

            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public bool AutoRotate { get; set; }
            /// <summary>
            /// Optional.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public decimal DefaultCameraTurns { get; set; }
            /// <summary>
            /// Camera turns, CCW when viewing the display.
            /// </summary>
            /// <remarks>This member was added as per WO 2012/170111 A1 only.</remarks>
            public decimal SelectedCameraTurns { get; set; }
        }

        /// <summary>
        /// Represents frame:modes/regionOfInterestArray item.
        /// </summary>
        public class RegionOfInterest : XYValue
        {
            /// <summary />
            public string Type { get; set; }
        }

        /// <summary>
        /// Represents frame:camera structure.
        /// </summary>
        public class Camera
        {
            /// <summary>
            /// E.g. Lytro.
            /// </summary>
            public string Make { get; set; }
            /// <summary>
            /// E.g. Firefly 1.0.
            /// </summary>
            public string Model { get; set; }
            /// <summary>
            /// E.g. 2.0.34.
            /// </summary>
            public string Firmware { get; set; }

            /// <summary>
            /// This is private metadata.
            /// </summary>
            public string SerialNumber { get; set; }
        }

        /// <summary>
        /// Represents frame:compressionDetails structure.
        /// </summary>
        public class CompressionDetails
        {
            /// <summary />
            public string Version { get; set; }
            /// <summary />
            public StrideXY Stride { get; set; }
            /// <summary />
            public PaddingXY PaddingPerTile { get; set; }
            /// <summary />
            public decimal[] Codes { get; set; }
        }

        /// <summary>
        /// Stride 2D value.
        /// </summary>
        public class StrideXY : XYValue { }

        /// <summary>
        /// Padding 2D value.
        /// </summary>
        public class PaddingXY : XYValue { }

        #endregion
    }
}
