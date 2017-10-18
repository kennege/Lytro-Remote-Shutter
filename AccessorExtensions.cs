namespace UAM.Optics.LightField.Lytro.IO
{
    /// <summary>
    /// Provides extension methods for package accesors.
    /// </summary>
    public static class AccessorExtensions
    {
        /// <summary>
        /// Provides access to the files in the package.
        /// </summary>
        /// <param name="package">The package containing files to access.</param>
        /// <returns>an instance of the <see cref="FilesPackageAccessor"/> providing access to the files in the <paramref name="package"/>.</returns>
        public static FilesPackageAccessor AccessFiles(this LightFieldPackage package)
        {
            return new FilesPackageAccessor(package);
        }

        /// <summary>
        /// Provides access to the raw images in the package.
        /// </summary>
        /// <param name="package">The package with raw images.</param>
        /// <returns>an instance of the <see cref="RawPackageAccessor"/> providing access to the raw images in the <paramref name="package"/>.</returns>
        public static RawPackageAccessor AccessRaw(this LightFieldPackage package)
        {
            return new RawPackageAccessor(package);
        }

        /// <summary>
        /// Provides access to the prerendered stacks in the package.
        /// </summary>
        /// <param name="package">The package with stacks.</param>
        /// <returns>an instance of the <see cref="StackPackageAccessor"/> providing access to the stacks in the <paramref name="package"/>.</returns>
        public static StackPackageAccessor AccessStacks(this LightFieldPackage package)
        {
            return new StackPackageAccessor(package);
        }

        /// <summary>
        /// Provides access to the depth and confidence maps in the package.
        /// </summary>
        /// <param name="package">The package with depth and confidence maps.</param>
        /// <returns>an instance of the <see cref="DepthPackageAccessor"/> providing access to the depth and confidence maps in the <paramref name="package"/>.</returns>
        public static DepthPackageAccessor AccessDepth(this LightFieldPackage package)
        {
            return new DepthPackageAccessor(package);
        }
    }
}
