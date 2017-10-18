using System;
using System.Collections.Generic;
using System.Linq;
using UAM.InformatiX;

namespace UAM.Optics.LightField.Lytro.Metadata
{
    /// <summary>
    /// Represents a file list part of the package metadata. 
    /// </summary>
    public class FilesMetadata
    {
        /// <summary>
        /// Represents an entry in the file list part of the package metadata.
        /// </summary>
        public class File
        {
            internal Json.File JsonFile;
            /// <summary>
            /// Initializes a new instance of the <see cref="File" /> class.
            /// </summary>
            public File()
            {
                JsonFile = new Json.File();
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="File"/> class with an existing <see cref="Json.File"/> storage.
            /// </summary>
            /// <param name="file">A <see cref="Json.File"/> to use as a storage for the entry.</param>
            public File(Json.File file)
            {
                JsonFile = file;
            }

            /// <summary>
            /// Gets or sets the file name of this entry.
            /// </summary>
            public string FileName { get { return JsonFile.Name; } set { JsonFile.Name = value; } }
            /// <summary>
            /// Gets or sets the reference identifier of this entry.
            /// </summary>
            public string Reference { get { return JsonFile.DataRef; } set { JsonFile.DataRef = value; } }
        }
        /// <summary>
        /// Represents a collection of <see cref="File"/> objects.
        /// </summary>
        public class FileCollection : ICollection<File>
        {
            private Json.Master _master;
            /// <summary>
            /// Initializes a new instance of the <see cref="FileCollection"/> class with an existing <see cref="Json.Master"/> storage.
            /// </summary>
            /// <param name="Master">A <see cref="Json.Master"/> to use as a storage for the collection.</param>
            public FileCollection (Json.Master Master)
	        {
                _master = Master;
                if (_master.Files == null)
                    _master.Files = new Json.File[0];
	        }

            /// <summary>
            /// Adds a <see cref="File"/> to the end of the collection.
            /// </summary>
            /// <param name="item">The <see cref="File"/> to add to the end of the collection.</param>
            public void Add(File item)
            {
                _master.Files = _master.Files.Append(item.JsonFile);
            }

            /// <summary>
            /// Removes all <see cref="File"/> objects from the collection.
            /// </summary>
            public void Clear()
            {
                _master.Files = new Json.File[0];
            }

            /// <summary>
            /// Returns a value that indicates whether the collection contains the specified <see cref="File" />.
            /// </summary>
            /// <param name="item">The <see cref="File"/> to locate in the collection. The value can be null.</param>
            /// <returns>true if <paramref name="item" /> is found in the <see cref="FileCollection" />; otherwise, false.</returns>
            public bool Contains(File item)
            {
                return _master.Files.Contains(item.JsonFile);
            }

            /// <summary>Copies all of the <see cref="File"/> objects in a collection to a specified array.</summary>
            /// <param name="array">Identifies the array to which content is copied.</param>
            /// <param name="arrayIndex">Index position in the array to which the contents of the collection are copied.</param>
            public void CopyTo(File[] array, int arrayIndex)
            {
                Files.ToArray().CopyTo(array, arrayIndex);
            }

            /// <summary>
            /// Gets the number of files contained in the <see cref="FileCollection"/>.
            /// </summary>
            /// <returns>the number of files contained in the <see cref="FileCollection"/>.</returns>
            public int Count
            {
                get { return _master.Files.Length; }
            }

            bool ICollection<File>.IsReadOnly
            {
                get { return false; }
            }

            /// <summary>
            /// Removes the first occurrence of the specified <see cref="File"/> from this <see cref="FileCollection"/>.
            /// </summary>
            /// <param name="item">The <see cref="File"/> to remove from this <see cref="FileCollection"/>.</param>
            /// <returns>true if <paramref name="item" /> was removed from the collection; otherwise, false.</returns>
            public bool Remove(File item)
            {
                int i = Array.IndexOf(_master.Files, item.JsonFile);
                if (i >= 0)
                {
                    _master.Files = _master.Files.RemoveAt(i);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Returns an enumerator that can iterate through the collection.
            /// </summary>
            /// <returns>An enumerator that can iterate through the collection.</returns>
            public IEnumerator<File> GetEnumerator()
            {
                return Files.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private IEnumerable<File> Files
            {
                get { return _master.Files.Select(f => new File(f)); }
            }
        }

        internal Json.Master JsonMaster;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesMetadata"/> class.
        /// </summary>
        public FilesMetadata()
        {
            JsonMaster = new Json.Master();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FilesMetadata"/> class with an existing <see cref="Json.Master"/> storage.
        /// </summary>
        /// <param name="Master">A <see cref="Json.Master"/> to use as storage of the collection.</param>
        public FilesMetadata(Json.Master Master)
        {
            JsonMaster = Master;
        }

        /// <summary>
        /// Gets a collection of file entries in the package.
        /// </summary>
        public FileCollection Files
        {
            get { return new FileCollection(JsonMaster); }
        }

        /// <summary>
        /// Gets a file name of the package containing rest of the file entries.
        /// </summary>
        public string NextFileName
        {
            get { return JsonMaster.NextFile; }
            set { JsonMaster.NextFile = value; }
        }
    }
}
