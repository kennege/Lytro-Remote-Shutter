namespace UAM.Optics.LightField.Lytro.Camera
{
    /// <summary>
    /// Represents the picture rotation as used by <see cref="PictureListEntry" /> class.
    /// </summary>
    public enum PictureListRotation : int
    {
        /// <summary>
        /// No rotation.
        /// </summary>
        Deg0 = 1,

        /// <summary>
        /// 90° counter-clockwise rotation.
        /// </summary>
        Deg90 = 8,

        /// <summary>
        /// 180° counter-clockwise rotation.
        /// </summary>
        Deg180 = 3,

        /// <summary>
        /// 270° counter-clockwise rotation.
        /// </summary>
        Deg270 = 6
    }
}
