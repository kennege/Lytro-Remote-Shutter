using System;
using System.Collections.Generic;
using System.Linq;
using UAM.InformatiX.Text.Json;
using UAM.Optics.LightField.Lytro.Metadata;

namespace UAM.Optics.LightField.Lytro.IO
{
    /// <summary>
    /// Provides base class for package accessors.
    /// </summary>
    public abstract class PackageAccessor
    {
        private List<Exception> _exceptions;
        private LightFieldPackage _package;
        private Json.Master _master;
        private bool _hasContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageAccessor"/> class.
        /// </summary>
        /// <param name="package">The package to be accessed.</param>
        public PackageAccessor(LightFieldPackage package)
        {
            if (package == null)
                throw new ArgumentNullException("package");

            _package = package;

            LightFieldComponent metadata = package.GetMetadata().FirstOrDefault();
            if (metadata != null)
            {
                _master = new Json.Master();
                _master.LoadFromJson(metadata.GetDataAsString());
            }

            _exceptions = new List<Exception>();
            _hasContent = Initialize();
        }

        /// <summary>
        /// When overriden in derived class, initializes the accessor and returns whether any content is available.
        /// </summary>
        /// <returns>true if the accessor can provide any content; false otherwise.</returns>
        protected abstract bool Initialize();

        /// <summary>
        /// Stores an exception that occured during initialization for later retrieval.
        /// </summary>
        /// <param name="e">The exception to store.</param>
        protected void OnException(Exception e)
        {
            _exceptions.Add(e);
        }

        /// <summary>
        /// Gets whether any exceptions occured during initialization.
        /// </summary>
        public bool HasExceptions
        {
            get { return _hasContent && _exceptions.Count > 0; }
        }

        /// <summary>
        /// Gets the list of exceptions that occured during initialization.
        /// </summary>
        /// <returns>the list of exceptions that occured during initialization.</returns>
        public Exception[] GetExceptions()
        {
            if (!_hasContent)
                return new Exception[0];

            Exception[] exceptions = new Exception[_exceptions.Count];
            _exceptions.CopyTo(exceptions);
            return exceptions;
        }

        /// <summary>
        /// Throws the first exception that occured during initialization, if any.
        /// </summary>
        public void ThrowIfExceptionsOccured()
        {
            if (!_hasContent)
                return;

            if (_exceptions.Count > 0)
                throw _exceptions[0];
        }

        /// <summary>
        /// Gets the <see cref="LightFieldPackage"/> associated with this accessor.
        /// </summary>
        public LightFieldPackage Package
        {
            get { return _package; }
        }

        /// <summary>
        /// Gets metadata of the package.
        /// </summary>
        public Json.Master Metadata
        {
            get { return _master; }
        }

        /// <summary>
        /// Gets whether any files are present in the package.
        /// </summary>
        public bool HasContent
        {
            get { return _hasContent; }
        }
    }
}
