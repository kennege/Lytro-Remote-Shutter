using System;
using System.Collections.Generic;
using System.Linq;
using UAM.Optics.LightField.Lytro.Metadata;

namespace UAM.Optics.LightField.Lytro.IO
{
    /// <summary>
    /// Provides access to the files in a <see cref="LightFieldPackage"/>.
    /// </summary>
    public class FilesPackageAccessor : PackageAccessor
    {
        private Dictionary<string, int> _filesByPath;
        private Dictionary<string, int> _filesByReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesPackageAccessor"/> class.
        /// </summary>
        /// <param name="package">The package containing files to access.</param>
        public FilesPackageAccessor(LightFieldPackage package) : base(package)
        {

        }

        /// <summary>
        /// When overriden in derived class, initializes the accessor and returns whether any content is available.
        /// </summary>
        /// <returns>true if the accessor contains any files; false otherwise.</returns>
        protected override bool Initialize()
        {
            if (Metadata == null)
                return false;

            Json.File[] files = Metadata.Files;
            if (files == null || files.Length < 1)
                return false;

            _filesByPath = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            _filesByReference = new Dictionary<string, int>();

            for (int i = 0; i < files.Length; i++)
            {
                Json.File file = files[i];

                _filesByPath[file.Name] = i;
                _filesByReference[file.DataRef] = i;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the package contains a file with the specified name. 
        /// </summary>
        /// <param name="name">Name of the file.</param>
        /// <returns>true if the package contains a file with the specified name; false otherwise.</returns>
        public bool ContainsFile(string name)
        {
            if (!HasContent)
                return false;

            return _filesByPath.ContainsKey(name);
        }

        /// <summary>
        /// Determines whether the package contains a file with the specified reference. 
        /// </summary>
        /// <param name="reference">Reference of the file.</param>
        /// <returns>true if the package contains a file with the specified reference; false otherwise.</returns>
        public bool ContainsReference(string reference)
        {
            if (!HasContent)
                return false;

            return _filesByReference.ContainsKey(reference);
        }

        /// <summary>
        /// Gets a <see cref="LightFieldComponent"/> of a file with the specified name.
        /// </summary>
        /// <param name="name">Name of the file.</param>
        /// <returns>a <see cref="LightFieldComponent"/> of a file with the specified name if found in the package; null otherwise.</returns>
        public LightFieldComponent GetFile(string name)
        {
            if (!HasContent)
                return null;

            int index;
            if (!_filesByPath.TryGetValue(name, out index))
                return null;

            return Package.GetComponent(Metadata.Files[index].DataRef).FirstOrDefault();
        }

        /// <summary>
        /// Gets file name of a file with the specified reference.
        /// </summary>
        /// <param name="reference">Reference of the file.</param>
        /// <returns>a file name of a file with the specified reference if found in the package; null otherwise.</returns>
        public string GetFileName(string reference)
        {
            if (!HasContent)
                return null;

            int index;
            if (!_filesByReference.TryGetValue(reference, out index))
                return null;

            return Metadata.Files[index].Name;
        }

        /// <summary>
        /// Gets the number of files in the package.
        /// </summary>
        public int FileCount
        {
            get
            {
                if (!HasContent)
                    return 0;

                return Metadata.Files.Length;
            }
        }

        /// <summary>
        /// Gets list of the files in the package.
        /// </summary>
        /// <returns>list of the files in the package.</returns>
        public string[] GetFiles()
        {
            string[] files = new string[FileCount];

            for (int i = 0; i < files.Length; i++)
                files[i] = Metadata.Files[i].Name;

            return files;
        }

        /// <summary>
        /// Gets the files metadata.
        /// </summary>
        /// <returns>the <see cref="FilesMetadata"/> of the package if available; null otherwise.</returns>
        public FilesMetadata GetFilesMetadata()
        {
            if (Metadata == null)
                return null;

            return new FilesMetadata(Metadata);
        }
    }
}
