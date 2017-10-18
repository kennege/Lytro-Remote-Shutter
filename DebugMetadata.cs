using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace UAM.Optics.LightField.Lytro.Metadata
{
    /// <summary>
    /// Represents a debug part of the calibration metadata.
    /// </summary>
    public class DebugMetadata
    {
        #pragma warning disable 1591

        /// <summary>
        /// Encapsulates sensor configuration members of debug calibration metadata.
        /// </summary>
        public class SensorConfig
        {
            internal IDictionary<string, object> JsonDebug;

            private int Get(string property)
            {
                object value;

                if (JsonDebug.TryGetValue("sensorConfig.uw" + property, out value))
                    try { return Convert.ToInt32(value); }
                    catch { return default(int); }

                return default(int);
            }
            private void Set(string property, int value)
            {
                JsonDebug["sensorConfig.uw" + property] = (decimal)value;
            }

            /** <summary>Gets or sets the sensorConfig.uwExtClk                      value.</summary> **/ public int ExtClk                      { get { return Get("ExtClk"); } set { Set("ExtClk", value); } }
            /** <summary>Gets or sets the sensorConfig.uwVTotal                      value.</summary> **/ public int VTotal                      { get { return Get("VTotal"); } set { Set("VTotal", value); } }
            /** <summary>Gets or sets the sensorConfig.uwHTotal                      value.</summary> **/ public int HTotal                      { get { return Get("HTotal"); } set { Set("HTotal", value); } }
            /** <summary>Gets or sets the sensorConfig.uwVISActive                   value.</summary> **/ public int VISActive                   { get { return Get("VISActive"); } set { Set("VISActive", value); } }
            /** <summary>Gets or sets the sensorConfig.uwHISActive                   value.</summary> **/ public int HISActive                   { get { return Get("HISActive"); } set { Set("HISActive", value); } }
            /** <summary>Gets or sets the sensorConfig.uwScanMode                    value.</summary> **/ public int ScanMode                    { get { return Get("ScanMode"); } set { Set("ScanMode", value); } }
            /** <summary>Gets or sets the sensorConfig.uwFrameSkew                   value.</summary> **/ public int FrameSkew                   { get { return Get("FrameSkew"); } set { Set("FrameSkew", value); } }
            /** <summary>Gets or sets the sensorConfig.uwCyclicScanStartX            value.</summary> **/ public int CyclicScanStartX            { get { return Get("CyclicScanStartX"); } set { Set("CyclicScanStartX", value); } }
            /** <summary>Gets or sets the sensorConfig.uwCyclicScanStartY            value.</summary> **/ public int CyclicScanStartY            { get { return Get("CyclicScanStartY"); } set { Set("CyclicScanStartY", value); } }
            /** <summary>Gets or sets the sensorConfig.uwCyclicScanStartColor        value.</summary> **/ public int CyclicScanStartColor        { get { return Get("CyclicScanStartColor"); } set { Set("CyclicScanStartColor", value); } }
            /** <summary>Gets or sets the sensorConfig.uwCyclicScanColors            value.</summary> **/ public int CyclicScanColors            { get { return Get("CyclicScanColors"); } set { Set("CyclicScanColors", value); } }
            /** <summary>Gets or sets the sensorConfig.uwHShift                      value.</summary> **/ public int HShift                      { get { return Get("HShift"); } set { Set("HShift", value); } }
            /** <summary>Gets or sets the sensorConfig.uwFrameStartX                 value.</summary> **/ public int FrameStartX                 { get { return Get("FrameStartX"); } set { Set("FrameStartX", value); } }
            /** <summary>Gets or sets the sensorConfig.uwFrameStartY                 value.</summary> **/ public int FrameStartY                 { get { return Get("FrameStartY"); } set { Set("FrameStartY", value); } }
            /** <summary>Gets or sets the sensorConfig.uwFrameWidth                  value.</summary> **/ public int FrameWidth                  { get { return Get("FrameWidth"); } set { Set("FrameWidth", value); } }
            /** <summary>Gets or sets the sensorConfig.uwFrameHeight                 value.</summary> **/ public int FrameHeight                 { get { return Get("FrameHeight"); } set { Set("FrameHeight", value); } }
            /** <summary>Gets or sets the sensorConfig.uwDataMode                    value.</summary> **/ public int DataMode                    { get { return Get("DataMode"); } set { Set("DataMode", value); } }
            /** <summary>Gets or sets the sensorConfig.uwVerticalDecimation          value.</summary> **/ public int VerticalDecimation          { get { return Get("VerticalDecimation"); } set { Set("VerticalDecimation", value); } }
            /** <summary>Gets or sets the sensorConfig.uwHorizontalDecimation        value.</summary> **/ public int HorizontalDecimation        { get { return Get("HorizontalDecimation"); } set { Set("HorizontalDecimation", value); } }
            /** <summary>Gets or sets the sensorConfig.uwFisrtExposure               value.</summary> **/ public int FisrtExposure               { get { return Get("FisrtExposure"); } set { Set("FisrtExposure", value); } }
            /** <summary>Gets or sets the sensorConfig.uwFisrtGain                   value.</summary> **/ public int FisrtGain                   { get { return Get("FisrtGain"); } set { Set("FisrtGain", value); } }
            /** <summary>Gets or sets the sensorConfig.uwTGBurstLine                 value.</summary> **/ public int TGBurstLine                 { get { return Get("TGBurstLine"); } set { Set("TGBurstLine", value); } }
            /** <summary>Gets or sets the sensorConfig.uwFieldsNum                   value.</summary> **/ public int FieldsNum                   { get { return Get("FieldsNum"); } set { Set("FieldsNum", value); } }
            /** <summary>Gets or sets the sensorConfig.uwAptinaVtPixClkDiv           value.</summary> **/ public int AptinaVtPixClkDiv           { get { return Get("AptinaVtPixClkDiv"); } set { Set("AptinaVtPixClkDiv", value); } }
            /** <summary>Gets or sets the sensorConfig.uwAptinaVtSysClkDiv           value.</summary> **/ public int AptinaVtSysClkDiv           { get { return Get("AptinaVtSysClkDiv"); } set { Set("AptinaVtSysClkDiv", value); } }
            /** <summary>Gets or sets the sensorConfig.uwAptinaPrePllClkDiv          value.</summary> **/ public int AptinaPrePllClkDiv          { get { return Get("AptinaPrePllClkDiv"); } set { Set("AptinaPrePllClkDiv", value); } }
            /** <summary>Gets or sets the sensorConfig.uwAptinaPllMultiplier         value.</summary> **/ public int AptinaPllMultiplier         { get { return Get("AptinaPllMultiplier"); } set { Set("AptinaPllMultiplier", value); } }
            /** <summary>Gets or sets the sensorConfig.uwAptinaOpPixClkDiv           value.</summary> **/ public int AptinaOpPixClkDiv           { get { return Get("AptinaOpPixClkDiv"); } set { Set("AptinaOpPixClkDiv", value); } }
            /** <summary>Gets or sets the sensorConfig.uwAptinaOpSysClkDiv           value.</summary> **/ public int AptinaOpSysClkDiv           { get { return Get("AptinaOpSysClkDiv"); } set { Set("AptinaOpSysClkDiv", value); } }
            /** <summary>Gets or sets the sensorConfig.uwAptinaXAddrStart            value.</summary> **/ public int AptinaXAddrStart            { get { return Get("AptinaXAddrStart"); } set { Set("AptinaXAddrStart", value); } }
            /** <summary>Gets or sets the sensorConfig.uwAptinaXAddrEnd              value.</summary> **/ public int AptinaXAddrEnd              { get { return Get("AptinaXAddrEnd"); } set { Set("AptinaXAddrEnd", value); } }
            /** <summary>Gets or sets the sensorConfig.uwAptinaYAddrStart            value.</summary> **/ public int AptinaYAddrStart            { get { return Get("AptinaYAddrStart"); } set { Set("AptinaYAddrStart", value); } }
            /** <summary>Gets or sets the sensorConfig.uwAptinaYAddrEnd              value.</summary> **/ public int AptinaYAddrEnd              { get { return Get("AptinaYAddrEnd"); } set { Set("AptinaYAddrEnd", value); } }
            /** <summary>Gets or sets the sensorConfig.uwXOddIncrement               value.</summary> **/ public int XOddIncrement               { get { return Get("XOddIncrement"); } set { Set("XOddIncrement", value); } }
            /** <summary>Gets or sets the sensorConfig.uwYOddIncrement               value.</summary> **/ public int YOddIncrement               { get { return Get("YOddIncrement"); } set { Set("YOddIncrement", value); } }
            /** <summary>Gets or sets the sensorConfig.uwXYBinEnable                 value.</summary> **/ public int XYBinEnable                 { get { return Get("XYBinEnable"); } set { Set("XYBinEnable", value); } }
            /** <summary>Gets or sets the sensorConfig.uwXBinEnable                  value.</summary> **/ public int XBinEnable                  { get { return Get("XBinEnable"); } set { Set("XBinEnable", value); } }
            /** <summary>Gets or sets the sensorConfig.uwAptinaScale_m               value.</summary> **/ public int AptinaScale_m               { get { return Get("AptinaScale_m"); } set { Set("AptinaScale_m", value); } }
            /** <summary>Gets or sets the sensorConfig.uwAptinaXOutputSize           value.</summary> **/ public int AptinaXOutputSize           { get { return Get("AptinaXOutputSize"); } set { Set("AptinaXOutputSize", value); } }
            /** <summary>Gets or sets the sensorConfig.uwAptinaYOutputSize           value.</summary> **/ public int AptinaYOutputSize           { get { return Get("AptinaYOutputSize"); } set { Set("AptinaYOutputSize", value); } }
            /** <summary>Gets or sets the sensorConfig.uwAptinaLineLengthPck         value.</summary> **/ public int AptinaLineLengthPck         { get { return Get("AptinaLineLengthPck"); } set { Set("AptinaLineLengthPck", value); } }
            /** <summary>Gets or sets the sensorConfig.uwAptinaFrameLengthLines      value.</summary> **/ public int AptinaFrameLengthLines      { get { return Get("AptinaFrameLengthLines"); } set { Set("AptinaFrameLengthLines", value); } }
            /** <summary>Gets or sets the sensorConfig.uwAptinaCoarseIntegrationTime value.</summary> **/ public int AptinaCoarseIntegrationTime { get { return Get("AptinaCoarseIntegrationTime"); } set { Set("AptinaCoarseIntegrationTime", value); } }
            /** <summary>Gets or sets the sensorConfig.uwAptinaFineCorrection        value.</summary> **/ public int AptinaFineCorrection        { get { return Get("AptinaFineCorrection"); } set { Set("AptinaFineCorrection", value); } }
            /** <summary>Gets or sets the sensorConfig.uwAptinaClkPixel              value.</summary> **/ public int AptinaClkPixel              { get { return Get("AptinaClkPixel"); } set { Set("AptinaClkPixel", value); } }

            /// <summary>
            /// Initializes a new instance of the <see cref="SensorConfig"/> class.
            /// </summary>
            /// <param name="debug">The debug part of calibration data that contains sensorConfig members.</param>
            public SensorConfig(IDictionary<string, object> debug)
            {
                JsonDebug = debug;
            }

        }

        #pragma warning restore 1591

        internal IDictionary<string, object> JsonDebug;
        private SensorConfig _sensorConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugMetadata"/> class.
        /// </summary>
        public DebugMetadata()
        {
            JsonDebug = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugMetadata"/> class with an existing property storage.
        /// </summary>
        /// <param name="debug"></param>
        public DebugMetadata(IDictionary<string, object> debug)
        {
            if (debug == null)
                throw new ArgumentNullException("debug");

            JsonDebug = debug;
        }

        /// <summary>
        /// Gest or sets the sensor configuration.
        /// </summary>
        public SensorConfig SensorConfiguration
        {
            get { return _sensorConfiguration ?? (_sensorConfiguration = new SensorConfig(JsonDebug)); }
            set
            {
                foreach (string key in value.JsonDebug.Keys)
                    if (key.StartsWith("sensorConfig"))
                        JsonDebug[key] = value.JsonDebug[key];
            }
        }

        /// <summary>
        /// Gets or sets whether the internal clock is valid.
        /// </summary>
        public bool InternalRTCValid
        {
            get
            {
                object value;
                try { return JsonDebug.TryGetValue("internalRTCValid", out value) && Convert.ToBoolean(value); }
                catch (FormatException) { return false; }
            }
            set { JsonDebug["internalRTCValid"] = value; }
        }
    }
}
