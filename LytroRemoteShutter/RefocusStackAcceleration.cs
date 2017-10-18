using System;
using System.Collections.Generic;
using System.Linq;
using UAM.InformatiX;

namespace UAM.Optics.LightField.Lytro.Metadata
{
    /// <summary>
    /// Represents Lytro's refocus acceleration data.
    /// </summary>
    public class RefocusStackAcceleration
    {
        /// <summary>
        /// Represents a depth lookup table
        /// </summary>
        public class DepthTable
        {
            internal Json.DepthLut JsonDepthLut;
            /// <summary>
            /// Initializes a new instance of the <see cref="DepthTable"/> class.
            /// </summary>
            public DepthTable()
            {
                JsonDepthLut = new Json.DepthLut();
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="DepthTable"/> class with an existing <see cref="Json.DepthLut"/> storage.
            /// </summary>
            /// <param name="depthLut">A <see cref="Json.DepthLut"/> to use as a storage for the lookup table.</param>
            public DepthTable(Json.DepthLut depthLut)
            {
                JsonDepthLut = depthLut;
            }

            /// <summary>
            /// Gets or sets the number of columns in the lookup table.
            /// </summary>
            public int Width
            {
                get { return (int)JsonDepthLut.Width; }
                set { JsonDepthLut.Width = value; }
            }
            /// <summary>
            /// Gets or sets the number of rows in the lookup table.
            /// </summary>
            public int Height
            {
                get { return (int)JsonDepthLut.Height; }
                set { JsonDepthLut.Height = value; }
            }
            /// <summary>
            /// Gets or sets the format of the lookup table data.
            /// </summary>
            public Representation Representation
            {
                get { return EnumEx.Parse<Representation>(JsonDepthLut.Representation, true); }
                set { JsonDepthLut.Representation = value.ToString().ToLowerInvariant(); }
            }
            /// <summary>
            /// Gets or sets the lookup table reference identifier.
            /// </summary>
            public string Reference
            {
                get { return JsonDepthLut.ImageRef; }
                set { JsonDepthLut.ImageRef = value; }
            }
        }
        /// <summary>
        /// Represents an entry in the collection of prerendered images. 
        /// </summary>
        public class Image
        {
            internal Json.RefocusImageItem JsonImage;
            /// <summary>
            /// Initializes a new instance of the <see cref="Image"/> class.
            /// </summary>
            public Image()
            {
                JsonImage = new Json.RefocusImageItem();
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="Image"/> class with an existing <see cref="Json.RefocusImageItem"/> storage.
            /// </summary>
            /// <param name="image">A <see cref="Json.RefocusImageItem"/> to use a storage for the entry.</param>
            public Image(Json.RefocusImageItem image)
            {
                JsonImage = image;
            }

            /// <summary>
            /// Gets or sets the width of the prerendered image.
            /// </summary>
            public int Width
            {
                get { return (int)JsonImage.Width; }
                set { JsonImage.Width = value; }
            }
            /// <summary>
            /// Gets or sets the height of the prerendered image.
            /// </summary>
            public int Height
            {
                get { return (int)JsonImage.Height; }
                set { JsonImage.Height = value; }
            }
            /// <summary>
            /// Gets or sets the format of the prerendered image data.
            /// </summary>
            public Representation Representation
            {
                get { return EnumEx.Parse<Representation>(JsonImage.Representation, true); }
                set { JsonImage.Representation = value.ToString().ToLowerInvariant(); }
            }
            /// <summary>
            /// Gets or sets the prerendered image reference identifier.
            /// </summary>
            public string Reference
            {
                get { return JsonImage.ImageRef; }
                set { JsonImage.ImageRef = value; }
            }
            /// <summary>
            /// Gets or sets the prerendered image depth value.
            /// </summary>
            public double Lambda
            {
                get { return (double)JsonImage.Lambda; }
                set { JsonImage.Lambda = (decimal)value; }
            }
        }
        /// <summary>
        /// Represents a collection of <see cref="Image"/> objects.
        /// </summary>
        public class ImageCollection : IList<Image>
        {
            private Json.LytroRefocusStack _stack;
            /// <summary>
            /// Initializes a new instance of the <see cref="ImageCollection"/> class with an existing <see cref="Json.LytroRefocusStack"/> as storage.
            /// </summary>
            /// <param name="stack">A <see cref="Json.LytroRefocusStack"/> to use as a storage for the collection.</param>
            public ImageCollection(Json.LytroRefocusStack stack)
            {
                _stack = stack;
                if (_stack.ImageArray == null)
                    _stack.ImageArray = new Json.RefocusImageItem[0];
            }

            /// <summary>
            /// Adds an <see cref="Image"/> to the end of the collection.
            /// </summary>
            /// <param name="item">The <see cref="Image"/> to add.</param>
            public void Add(Image item)
            {
                _stack.ImageArray = _stack.ImageArray.Append(item.JsonImage);
            }
            
            /// <summary>
            /// Removes all <see cref="Image"/> objects from the collection.
            /// </summary>
            public void Clear()
            {
                _stack.ImageArray = new Json.RefocusImageItem[0];
            }

            /// <summary>
            /// Returns a value that indicates whether the collection contains the specified <see cref="Image"/>.
            /// </summary>
            /// <param name="item">The <see cref="Image"/> to locate in the collection. The value can be null.</param>
            /// <returns>true if <paramref name="item"/> is found in the <see cref="ImageCollection"/>; otherwise, false.</returns>
            public bool Contains(Image item)
            {
                return _stack.ImageArray.Contains(item.JsonImage);
            }

            /// <summary>
            /// Copies all the <see cref="Image"/> objects in a collection to a specified array. 
            /// </summary>
            /// <param name="array">Identifies the array to which content is copied.</param>
            /// <param name="arrayIndex">Index position in the array to which the contents of the collection are copied.</param>
            public void CopyTo(Image[] array, int arrayIndex)
            {
                Images.ToArray().CopyTo(array, arrayIndex);
            }

            /// <summary>
            /// Gets the number of images contained in the <see cref="ImageCollection"/>.
            /// </summary>
            /// <returns>the number of files contained in the <see cref="ImageCollection"/>.</returns>
            public int Count
            {
                get { return _stack.ImageArray.Length; }
            }

            bool ICollection<Image>.IsReadOnly
            {
                get { return false; }
            }

            /// <summary>
            /// Removes the first occurrence of an <see cref="Image"/> from the collection.
            /// </summary>
            /// <param name="item">The <see cref="Image"/> to remove from the collection.</param>
            /// <returns>true if <paramref name="item" /> was removed from the collection; otherwise, false.</returns>
            public bool Remove(Image item)
            {
                int i = Array.IndexOf(_stack.ImageArray, item.JsonImage);
                if (i >= 0)
                {
                    _stack.ImageArray = _stack.ImageArray.RemoveAt(i);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Returns an enumerator that can iterate through the collection.
            /// </summary>
            /// <returns>An enumerator that can iterate through the collection.</returns>
            public IEnumerator<Image> GetEnumerator()
            {
                return Images.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private IEnumerable<Image> Images
            {
                get { return _stack.ImageArray.Select(i => new Image(i)); }
            }

            /// <summary>
            /// Returns the index of the first occurrence of an <see cref="Image"/> in the collection.
            /// </summary>
            /// <param name="item">The <see cref="Image"/> to insert.</param>
            /// <returns>The index of the first occurrence of the specified <see cref="Image"/> in the collection.</returns>
            public int IndexOf(Image item)
            {
                return Array.IndexOf(_stack.ImageArray, item.JsonImage);
            }

            /// <summary>
            /// Inserts an <see cref="Image"/> into the collection.
            /// </summary>
            /// <param name="index">The index to insert the <paramref name="item"/> at.</param>
            /// <param name="item">The <see cref="Image"/> to insert.</param>
            /// <exception cref="ArgumentOutOfRangeException">The <paramref name="index"/> is less than zero or bigger than the <see cref="Count"/>.</exception>
            public void Insert(int index, Image item)
            {
               _stack.ImageArray = _stack.ImageArray.Insert(index, item.JsonImage);
            }

            /// <summary>
            /// Removes an <see cref="Image"/> at specified index.
            /// </summary>
            /// <param name="index">The index to remove the <see cref="Image"/> at.</param>
            /// <exception cref="ArgumentOutOfRangeException">The <paramref name="index"/> is less than zero or bigger than or equal to the <see cref="Count"/>.</exception>
            public void RemoveAt(int index)
            {
                _stack.ImageArray = _stack.ImageArray.RemoveAt(index);
            }

            /// <summary>
            /// Gets or sets an <see cref="Image"/> at the specified index.
            /// </summary>
            /// <param name="index">The index at which the <see cref="Image"/> should be returned or set.</param>
            /// <returns>An <see cref="Image"/> at the specified index.</returns>
            public Image this[int index]
            {
                get
                {
                    return new Image(_stack.ImageArray[index]);
                }
                set
                {
                    _stack.ImageArray[index] = value.JsonImage;
                }
            }
        }

        /// <summary>
        /// Represents an entry in the block of prerendered images. 
        /// </summary>
        public class BlockImage
        {
            internal Json.BlockRefocusMetadata JsonImage;
            /// <summary>
            /// Initializes a new instance of the <see cref="BlockImage"/> class.
            /// </summary>
            public BlockImage()
            {
                JsonImage = new Json.BlockRefocusMetadata();
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="BlockImage"/> class with an existing <see cref="Json.BlockRefocusMetadata"/> storage.
            /// </summary>
            /// <param name="image">A <see cref="Json.BlockRefocusMetadata"/> to use a storage for the entry.</param>
            public BlockImage(Json.BlockRefocusMetadata image)
            {
                JsonImage = image;
            }

            /// <summary>
            /// Gets or sets the width of the prerendered image.
            /// </summary>
            public int Width
            {
                get { return (int)JsonImage.Width; }
                set { JsonImage.Width = value; }
            }
            /// <summary>
            /// Gets or sets the height of the prerendered image.
            /// </summary>
            public int Height
            {
                get { return (int)JsonImage.Height; }
                set { JsonImage.Height = value; }
            }
            /// <summary>
            /// Gets or sets the prerendered image depth value.
            /// </summary>
            public double Lambda
            {
                get { return (double)JsonImage.Lambda; }
                set { JsonImage.Lambda = (decimal)value; }
            }
        }
        /// <summary>
        /// Represents a collection of <see cref="BlockImage"/> objects.
        /// </summary>
        public class BlockImageCollection : IList<BlockImage>
        {
            private Json.LytroRefocusStack _stack;
            /// <summary>
            /// Initializes a new instance of the <see cref="BlockImageCollection"/> class with an existing <see cref="Json.LytroRefocusStack"/> as storage.
            /// </summary>
            /// <param name="stack">A <see cref="Json.LytroRefocusStack"/> to use as a storage for the collection.</param>
            public BlockImageCollection(Json.LytroRefocusStack stack)
            {
                _stack = stack;
                if (_stack.BlockOfImages == null)
                    _stack.BlockOfImages = new Json.BlockOfRefocusImages();

                if (_stack.BlockOfImages.MetadataArray == null)
                    _stack.BlockOfImages.MetadataArray = new Json.BlockRefocusMetadata[0];
            }

            /// <summary>
            /// Adds an <see cref="BlockImage"/> to the end of the collection.
            /// </summary>
            /// <param name="item">The <see cref="BlockImage"/> to add.</param>
            public void Add(BlockImage item)
            {
                _stack.BlockOfImages.MetadataArray = _stack.BlockOfImages.MetadataArray.Append(item.JsonImage);
            }

            /// <summary>
            /// Removes all <see cref="BlockImage"/> objects from the collection.
            /// </summary>
            public void Clear()
            {
                _stack.BlockOfImages.MetadataArray = new Json.BlockRefocusMetadata[0];
            }

            /// <summary>
            /// Returns a value that indicates whether the collection contains the specified <see cref="BlockImage"/>.
            /// </summary>
            /// <param name="item">The <see cref="BlockImage"/> to locate in the collection. The value can be null.</param>
            /// <returns>true if <paramref name="item"/> is found in the <see cref="BlockImageCollection"/>; otherwise, false.</returns>
            public bool Contains(BlockImage item)
            {
                return _stack.BlockOfImages.MetadataArray.Contains(item.JsonImage);
            }

            /// <summary>
            /// Copies all the <see cref="BlockImage"/> objects in a collection to a specified array. 
            /// </summary>
            /// <param name="array">Identifies the array to which content is copied.</param>
            /// <param name="arrayIndex">Index position in the array to which the contents of the collection are copied.</param>
            public void CopyTo(BlockImage[] array, int arrayIndex)
            {
                Images.ToArray().CopyTo(array, arrayIndex);
            }

            /// <summary>
            /// Gets the number of images contained in the <see cref="BlockImageCollection"/>.
            /// </summary>
            /// <returns>the number of files contained in the <see cref="BlockImageCollection"/>.</returns>
            public int Count
            {
                get { return _stack.BlockOfImages.MetadataArray.Length; }
            }

            bool ICollection<BlockImage>.IsReadOnly
            {
                get { return false; }
            }

            /// <summary>
            /// Removes the first occurrence of an <see cref="BlockImage"/> from the collection.
            /// </summary>
            /// <param name="item">The <see cref="BlockImage"/> to remove from the collection.</param>
            /// <returns>true if <paramref name="item" /> was removed from the collection; otherwise, false.</returns>
            public bool Remove(BlockImage item)
            {
                int i = Array.IndexOf(_stack.BlockOfImages.MetadataArray, item.JsonImage);
                if (i >= 0)
                {
                    _stack.BlockOfImages.MetadataArray = _stack.BlockOfImages.MetadataArray.RemoveAt(i);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Returns an enumerator that can iterate through the collection.
            /// </summary>
            /// <returns>An enumerator that can iterate through the collection.</returns>
            public IEnumerator<BlockImage> GetEnumerator()
            {
                return Images.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private IEnumerable<BlockImage> Images
            {
                get { return _stack.BlockOfImages.MetadataArray.Select(i => new BlockImage(i)); }
            }

            /// <summary>
            /// Returns the index of the first occurrence of an <see cref="BlockImage"/> in the collection.
            /// </summary>
            /// <param name="item">The <see cref="BlockImage"/> to insert.</param>
            /// <returns>The index of the first occurrence of the specified <see cref="BlockImage"/> in the collection.</returns>
            public int IndexOf(BlockImage item)
            {
                return Array.IndexOf(_stack.BlockOfImages.MetadataArray, item.JsonImage);
            }

            /// <summary>
            /// Inserts an <see cref="BlockImage"/> into the collection.
            /// </summary>
            /// <param name="index">The index to insert the <paramref name="item"/> at.</param>
            /// <param name="item">The <see cref="BlockImage"/> to insert.</param>
            /// <exception cref="ArgumentOutOfRangeException">The <paramref name="index"/> is less than zero or bigger than the <see cref="Count"/>.</exception>
            public void Insert(int index, BlockImage item)
            {
                _stack.BlockOfImages.MetadataArray = _stack.BlockOfImages.MetadataArray.Insert(index, item.JsonImage);
            }

            /// <summary>
            /// Removes an <see cref="BlockImage"/> at specified index.
            /// </summary>
            /// <param name="index">The index to remove the <see cref="BlockImage"/> at.</param>
            /// <exception cref="ArgumentOutOfRangeException">The <paramref name="index"/> is less than zero or bigger than or equal to the <see cref="Count"/>.</exception>
            public void RemoveAt(int index)
            {
                _stack.BlockOfImages.MetadataArray = _stack.BlockOfImages.MetadataArray.RemoveAt(index);
            }

            /// <summary>
            /// Gets or sets an <see cref="BlockImage"/> at the specified index.
            /// </summary>
            /// <param name="index">The index at which the <see cref="BlockImage"/> should be returned or set.</param>
            /// <returns>An <see cref="BlockImage"/> at the specified index.</returns>
            public BlockImage this[int index]
            {
                get
                {
                    return new BlockImage(_stack.BlockOfImages.MetadataArray[index]);
                }
                set
                {
                    _stack.BlockOfImages.MetadataArray[index] = value.JsonImage;
                }
            }

            /// <summary>
            /// Gets or sets the format of the block of images.
            /// </summary>
            public Representation Representation
            {
                get { return EnumEx.Parse<Representation>(_stack.BlockOfImages.Representation, true); }
                set { _stack.BlockOfImages.Representation = value.ToString().ToLowerInvariant(); }
            }
            /// <summary>
            /// Gets or sets the reference identifier of the block of images.
            /// </summary>
            public string Reference
            {
                get { return _stack.BlockOfImages.BlockOfImagesRef; }
                set { _stack.BlockOfImages.BlockOfImagesRef = value; }
            }

        }

        internal Json.LytroRefocusStack JsonRefocusStack;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefocusStackAcceleration"/> class.
        /// </summary>
        public RefocusStackAcceleration()
        {
            JsonRefocusStack = new Json.LytroRefocusStack();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="RefocusStackAcceleration"/> class with an existing <see cref="Json.LytroRefocusStack"/> storage.
        /// </summary>
        /// <param name="stack">A <see cref="Json.LytroRefocusStack"/> to use as a storage for the parallax.</param>
        public RefocusStackAcceleration(Json.LytroRefocusStack stack)
        {
            if (stack == null)
                throw new ArgumentNullException("stack");

            JsonRefocusStack = stack;
        }

        /// <summary>
        /// Gets or sets the parallax generator.
        /// </summary>
        public string Generator { get; set; }

        /// <summary>
        /// Gets or sets the initial focus depth.
        /// </summary>
        public double DefaultLambda
        {
            get { return (double)JsonRefocusStack.DefaultLambda; }
            set { JsonRefocusStack.DefaultLambda = (decimal)value; }
        }

        private DepthTable _depthTable;
        /// <summary>
        /// Gets or sets the lookup table properties.
        /// </summary>
        public DepthTable LookupTable
        {
            get
            {
                if (_depthTable != null)
                    return _depthTable;

                Json.DepthLut depthTable = JsonRefocusStack.DepthLut;
                if (depthTable != null)
                    return _depthTable = new DepthTable(depthTable);

                return null;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                JsonRefocusStack.DepthLut = value.JsonDepthLut;
                _depthTable = value;
            }
        }

        /// <summary>
        /// Gets the collection of prerendered images.
        /// </summary>
        public ImageCollection Images
        {
            get { return new ImageCollection(JsonRefocusStack); }
        }

        /// <summary>
        /// Gets the presence of image collection accessible by <see cref="Images"/>.
        /// </summary>
        public bool HasImages
        {
            get { return JsonRefocusStack.ImageArray != null && JsonRefocusStack.ImageArray.Length > 0; }
        }

        /// <summary>
        /// Gets the collection of prerendered images in a block.
        /// </summary>
        public BlockImageCollection BlockImages
        {
            get { return new BlockImageCollection(JsonRefocusStack); }
        }

        /// <summary>
        /// Gets the presence of image collection in a block accessible by <see cref="BlockImages"/>.
        /// </summary>
        public bool HasBlockImages
        {
            get { return JsonRefocusStack.BlockOfImages != null && JsonRefocusStack.BlockOfImages.MetadataArray != null && JsonRefocusStack.BlockOfImages.MetadataArray.Length > 0; }
        }

        /// <summary>
        /// Gets or sets the prerendered images width.
        /// </summary>
        public int DisplayWidth
        {
            get { return IsDisplayDimensionsAvailable ? (int)JsonRefocusStack.DisplayParameters.DisplayDimensions.Value.Width : 0; }
            set { EnsureDisplayDimensions(); JsonRefocusStack.DisplayParameters.DisplayDimensions.Value.Width = value; }
        }
        /// <summary>
        /// Gets or sets the prerendered images height.
        /// </summary>
        public int DisplayHeight
        {
            get { return IsDisplayDimensionsAvailable ? (int)JsonRefocusStack.DisplayParameters.DisplayDimensions.Value.Height : 0; }
            set { EnsureDisplayDimensions(); JsonRefocusStack.DisplayParameters.DisplayDimensions.Value.Height = value; }
        }

        /// <summary>
        /// Returns whether the Json.LytroRefocusStack.DisplayParameters.DisplayDimensions.Value instance is available.
        /// </summary>
        protected bool IsDisplayDimensionsAvailable
        {
            get
            {
                if (JsonRefocusStack.DisplayParameters == null) return false;
                if (JsonRefocusStack.DisplayParameters.DisplayDimensions == null) return false;
                if (JsonRefocusStack.DisplayParameters.DisplayDimensions.Value == null) return false;

                return true;
            }
        }

        /// <summary>
        /// Ensures the Json.LytroRefocusStack.DisplayParameters.DisplayDimensions.Value instance.
        /// </summary>
        protected void EnsureDisplayDimensions()
        {
            if (JsonRefocusStack.DisplayParameters == null)
                JsonRefocusStack.DisplayParameters = new Json.DisplayParameters
                {
                    DisplayDimensions = new Json.DisplayDimensions
                    {
                        Value = new Json.DimensionsValue()
                    }
                };
            else if (JsonRefocusStack.DisplayParameters.DisplayDimensions == null)
                JsonRefocusStack.DisplayParameters.DisplayDimensions = new Json.DisplayDimensions
                {
                    Value = new Json.DimensionsValue()
                };
            else if (JsonRefocusStack.DisplayParameters.DisplayDimensions.Value == null)
            {
                JsonRefocusStack.DisplayParameters.DisplayDimensions.Value = new Json.DimensionsValue();
            }
        }
    }
}
