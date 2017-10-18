namespace UAM.Optics.LightField.Lytro.Net
{
    /// <summary>
    /// Contains known networking commands.
    /// </summary>
    public enum LytroCommand : ulong
    {
        /// <summary>
        /// Loads basic information about the camera.
        /// </summary>
        LoadHardwareInfo    = 0x00000000000000c2U,
        /// <summary>
        /// Loads a file from the camera.
        /// </summary>
        LoadFile            = 0x00000000000100c2U,
        /// <summary>
        /// Loads a list of pictures available on the camera.
        /// </summary>
        LoadPictureList     = 0x00000000000200c2U,
        /// <summary>
        /// Loads a picture from the camera.
        /// </summary>
        LoadPicture         = 0x00000000000500c2U,
        /// <summary>
        /// Deletes selected photo from camera. 
        /// </summary>
        ///
        DeletePicture       = 0x00000000000500c0U,
        /// <summary>
        /// Loads the calibration data minimum (set of files). 
        /// </summary>
        ///  
        LoadCalibrationData = 0x00000000000600c2U,
        /// <summary>
        /// Loads a picture in the <see cref="UAM.Optics.LightField.Lytro.Metadata.Representation.RawPackedJpegCompressed"/> representation.
        /// </summary>
        LoadPictureRawJpeg  = 0x00000000000700c2U,

        /// <summary>
        /// Retrieves the loaded content from camera.
        /// </summary>
        Download            = 0x00000000000000c4U,
        /// <summary>
        /// Sends the content to camera.
        /// </summary>
        Upload              = 0x00000000000000c5U,

        /// <summary>
        /// Returns the loaded content length.
        /// </summary>
        QueryContentLength  = 0x00000000000000c6U,
        /// <summary>
        /// Returns current camera time.
        /// </summary>
        QueryCameraTime     = 0x00000000000300c6U,
        /// <summary>
        /// Returns camera battery level (as percentage).
        /// </summary>
        QueryBatteryLevel   = 0x00000000000600c6U,

        /// <summary>
        /// Take a picture.
        /// </summary>
        TakePicture         = 0x00000000000000c0U,
        /// <summary>
        /// Sets current camera time.
        /// </summary>
        SetCameraTime       = 0x00000000000400c0U
    }
}
