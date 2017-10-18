namespace UAM.Optics.LightField.Lytro.Net
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UAM.Optics.LightField.Lytro.Camera; 

    /// <summary>
    /// Provides common methods for sending data to and receiving data from a Lytro camera over network connection.
    /// </summary>
    public abstract partial class LytroNetClientPortableBase
    {
        /// <summary>
        /// Maximum buffer size the camera is able to allocate.
        /// </summary>
        public const int MaximumBufferSize = 0x200000;

        private int _downloadBufferSize = MaximumBufferSize;
        private int _uploadBufferSize = MaximumBufferSize;

        /// <summary>
        /// When overriden in a derived class, provides a command stream that supports reading and writing.
        /// </summary>
        /// <returns>a command stream to use by the client.</returns>
        protected abstract Stream GetStream();

        /// <summary>
        /// Called when an <see cref="Exception"/> occurs during communication.
        /// </summary>
        /// <param name="e">The exception thrown.</param>
        /// <param name="stream">The stream in use when the exception occurred.</param>
        protected virtual void OnException(Exception e, Stream stream) { }

        /// <summary>
        /// Gets or sets the maximum buffer size for download.
        /// </summary>
        public int DownloadBufferSize
        {
            get { return _downloadBufferSize; }
            set
            {
                if (value < 1 || value > MaximumBufferSize)
                    throw new ArgumentOutOfRangeException("value");

                _downloadBufferSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum buffer size for upload.
        /// </summary>
        public int UploadBufferSize
        {
            get { return _uploadBufferSize; }
            set
            {
                if (value < 1 || value > MaximumBufferSize)
                    throw new ArgumentOutOfRangeException("value");

                _uploadBufferSize = value;
            }
        }

        /// <summary>
        /// Gets the basic information about the camera.
        /// </summary>
        /// <returns>the basic information about the camera.</returns>
        public HardwareInfo GetHardwareInfo()
        {
            Stream stream = GetStream();
            try
            {
                lock (stream)
                {
                    LytroRequest request = LytroRequest.Create(LytroCommand.LoadHardwareInfo);
                    LytroResponse response = request.GetResponse(stream);

                    byte[] content = DownloadData(stream);
                    return new HardwareInfo(content, 0);
                }
            }
            catch (Exception e) { OnException(e, stream); throw; }
        }

        /// <summary>
        /// Gets the camera's current battery level (as percentage).
        /// </summary>
        /// <returns>the camera's current battery level (as percentage).</returns>
        public float GetBatteryLevel()
        {
            Stream stream = GetStream();
            try
            {
                lock (stream)
                {
                    LytroRequest request = LytroRequest.Create(LytroCommand.QueryBatteryLevel);
                    LytroResponse response = request.GetResponse(stream, 4);

                    if (response.ContentLength != 4)
                        throw new LytroNetProtocolViolationException("Query battery level failed.");

                    return BitConverter.ToSingle(response.Content, 0);
                }

            }
            catch (Exception e) { OnException(e, stream); throw; }
        }

        /// <summary>
        /// Gets the camera's current date and time.
        /// </summary>
        /// <returns>the camera's current date and time.</returns>
        /// <remarks>Milliseconds are currently not reported (the value is zero).</remarks>
        public DateTimeOffset GetCameraTime()
        {
            Stream stream = GetStream();
            try
            {
                lock (stream)
                {
                    LytroRequest request = LytroRequest.Create(LytroCommand.QueryCameraTime);
                    LytroResponse response = request.GetResponse(stream, 14);

                    if (response.ContentLength != 14)
                        throw new LytroNetProtocolViolationException("Query camera time failed.");

                    return ToDateTimeOffset(response.Content, 0);
                }

            }
            catch (Exception e) { OnException(e, stream); throw; }
        }

        /// <summary>
        /// Sets the camera's current date and time.
        /// </summary>
        /// <param name="time">The date and time to set.</param>
        /// <returns>new camera time.</returns>
        /// <remarks>Setting the camera time can be logged by the camera.</remarks>
        public DateTimeOffset SetCameraTime(DateTimeOffset time)
        {
            Stream stream = GetStream();
            try
            {
                lock (stream)
                {
                    byte[] content = ToCameraTime(time);

                    LytroRequest request = LytroRequest.Create(LytroCommand.SetCameraTime, content);
                    LytroResponse response = request.GetResponse(stream, 14);

                    if (response.ContentLength != 14)
                        throw new LytroNetProtocolViolationException("Set camera time failed.");

                    return ToDateTimeOffset(response.Content, 0);
                }

            }
            catch (Exception e) { OnException(e, stream); throw; }
        }

        /// <summary>
        /// Triggers the camera's shutter.
        /// </summary>
        public void TakePicture()
        {
            Stream stream = GetStream();
            try
            {
                lock (stream)
                {
                    LytroRequest request = LytroRequest.Create(LytroCommand.TakePicture);
                    LytroResponse response = request.GetResponse(stream);
                }

            }
            catch (Exception e) { OnException(e, stream); throw; }
        }

        public void DeletePicture(PictureListEntry entry)
        {
            Stream stream = GetStream();
            try
            {
                lock (stream)
                {
                    byte[] content = new byte[24];
                    Encoding.UTF8.GetBytes(entry.FolderName).CopyTo(content, 0x00);
                    Encoding.UTF8.GetBytes(entry.FileName).CopyTo(content, 0x08);
                    BitConverter.GetBytes(entry.FolderNumber).CopyTo(content, 0x10);
                    BitConverter.GetBytes(entry.FileNumber).CopyTo(content, 0x14);

                    LytroRequest request = LytroRequest.Create(LytroCommand.DeletePicture, content);
                    LytroResponse response = request.GetResponse(stream);
                }
            }
            catch (Exception e) { OnException(e, stream); throw; }
        }

        /// <summary>
        /// Gets a list of pictures available on the camera.
        /// </summary>
        /// <returns>a list of pictures available on the camera.</returns>
        public PictureList DownloadPictureList()
        {
            Stream stream = GetStream();
            try
            {
                lock (stream)
                {
                    LytroRequest request = LytroRequest.Create(LytroCommand.LoadPictureList);
                    LytroResponse response = request.GetResponse(stream);

                    byte[] content = DownloadData(stream);
                    return new PictureList(content, 0);
                }

            }
            catch (Exception e) { OnException(e, stream); throw; }
        }

        /// <summary>
        /// Downloads a file from the camera.
        /// </summary>
        /// <param name="path">The file to download.</param>
        /// <returns>the file contents.</returns>
        /// <exception cref="FileNotFoundException">The file specified in <paramref name="path"/> was not found on the camera.</exception>
        public byte[] DownloadFile(string path)
        {
            Stream stream = GetStream();
            try
            {
                lock (stream)
                {
                    LytroRequest request = LytroRequest.Create(LytroCommand.LoadFile, path + "\0");
                    LytroResponse response = request.GetResponse(stream);

                    if (response.ContentLength == 0)
                        throw new FileNotFoundException("Could not find file '" + path + "'.");

                    return DownloadData(stream);
                }

            }
            catch (Exception e) { OnException(e, stream); throw; }
        }

        /// <summary>
        /// Determines whether the specified file exists on camera.
        /// </summary>
        /// <param name="path">The file to check.</param>
        /// <returns>true if the <paramref name="path" /> contains the name of an existing file; otherwise, false.</returns>
        public bool FileExists(string path)
        {
            Stream stream = GetStream();
            try
            {
                lock (stream)
                {
                    LytroRequest request = LytroRequest.Create(LytroCommand.LoadFile, path + "\0");
                    LytroResponse response = request.GetResponse(stream);

                    return response.ContentLength > 0;
                }

            }
            catch (Exception e) { OnException(e, stream); throw; }
        }

        /// <summary>
        /// Determines size of the specified file on camera.
        /// </summary>
        /// <param name="path">The file whose length should be determined.</param>
        /// <returns>the length of the file.</returns>
        /// <exception cref="FileNotFoundException">The file specified in <paramref name="path"/> was not found on the camera.</exception>
        public int GetFileLength(string path)
        {
            int length;
            if (!TryGetFileLength(path, out length))
                throw new FileNotFoundException("Could not find file '" + path + "'.");

            return length;
        }
        
        /// <summary>
        /// Tries to determine size of the specified file on camera.
        /// </summary>
        /// <param name="path">The file whose length should be determined.</param>
        /// <param name="length">When this method returns, contains the length of the file specified in <paramref name="path"/>.</param>
        /// <returns>true if the file specified in <paramref name="path"/> exists and <paramref name="length"/> contain its size; false otherwise.</returns>
        public bool TryGetFileLength(string path, out int length)
        {
            Stream stream = GetStream();
            try
            {
                lock (stream)
                {
                    length = 0;
                    LytroRequest request = LytroRequest.Create(LytroCommand.LoadFile, path + "\0");
                    LytroResponse response = request.GetResponse(stream);

                    if (response.ContentLength == 0)
                        return false;

                    length = DownloadLength(stream);
                    return true;
                }

            }
            catch (Exception e) { OnException(e, stream); throw; }
        }

        /// <summary>
        /// Downloads a picture from the camera.
        /// </summary>
        /// <param name="id">ID of the picture to be downloaded.</param>
        /// <param name="format">Format of the picture to be downloaded.</param>
        /// <returns>picture data in specified <paramref name="format"/>, or an empty array if not available.</returns>
        /// <exception cref="FileNotFoundException">The picture with specified <paramref name="id"/> was not found on the camera.</exception>
        public byte[] DownloadPicture(string id, LoadPictureFormat format)
        {
            Stream stream = GetStream();
            try
            {
                lock (stream)
                {
                    LytroRequest request = LytroRequest.Create(LytroCommand.LoadPicture, id + (char)format + "\0");
                    LytroResponse response = request.GetResponse(stream);

                    if (response.ContentLength == 0)
                        throw new FileNotFoundException("Could not find picture '" + id + "'.");

                    return DownloadData(stream, 4);
                }

            }
            catch (Exception e) { OnException(e, stream); throw; }
        }

        /// <summary>
        /// Downloads a picture from the camera in the <see cref="UAM.Optics.LightField.Lytro.Metadata.Representation.RawPackedJpegCompressed"/> format.
        /// </summary>
        /// <param name="id">ID of the picture to be downloaded.</param>
        /// <returns>picture data in the <see cref="UAM.Optics.LightField.Lytro.Metadata.Representation.RawPackedJpegCompressed"/> format.</returns>
        /// <exception cref="FileNotFoundException">The picture with specified <paramref name="id"/> was not found on the camera.</exception>
        public byte[] DownloadPictureRawJpeg(string id)
        {
            Stream stream = GetStream();
            try
            {
                lock (stream)
                {
                    LytroRequest request = LytroRequest.Create(LytroCommand.LoadPictureRawJpeg, id + "\0");
                    LytroResponse response = request.GetResponse(stream);

                    if (response.ContentLength == 0)
                        throw new FileNotFoundException("Could not find picture '" + id + "'.");

                    return DownloadData(stream);
                }

            }
            catch (Exception e) { OnException(e, stream); throw; }
        }

        /// <summary>
        /// Downloads calibration data minimum from the camera.
        /// </summary>
        /// <returns>a dictionary containing calibration files grouped by their path.</returns>
        public Dictionary<string, byte[]> DownloadCalibrationData()
        {
            byte[] content;
            int contentLength;

            Stream stream = GetStream();
            try
            {
                lock (stream)
                {
                    LytroRequest request = LytroRequest.Create(LytroCommand.LoadCalibrationData);
                    LytroResponse response = request.GetResponse(stream);

                    content = DownloadData(stream);
                    contentLength = content.Length;
                }

            }
            catch (Exception e) { OnException(e, stream); throw; }

            const int headerSize = 36;
            int requiredLength = 0;
            int contentOffset = 0;
            int fileCount = 0;

            for (; contentOffset + requiredLength < contentLength; contentOffset += headerSize, fileCount++)
                requiredLength += BitConverter.ToInt32(content, contentOffset);

            Dictionary<string, byte[]> data = new Dictionary<string, byte[]>(fileCount);

            for (int f = 0; f < fileCount; f++)
            {
                int length = BitConverter.ToInt32(content, f * headerSize);
                string path = Encoding.UTF8.GetString(content, f * headerSize + 4, 32).TrimEnd('\0');

                byte[] file = new byte[length];
                Array.Copy(content, contentOffset, file, 0, length);

                data[path] = file;
                contentOffset += length;
            }

            return data;
        }

        private byte[] DownloadData(Stream stream, int startOffset = 0)
        {
            int length = DownloadLength(stream);
            MemoryStream content = new MemoryStream(length);

            int offset = startOffset;
            while (offset < length)
            {
                ThrowIfCancelled(offset, length);

                LytroRequest request = LytroRequest.CreateDownload(offset);
                LytroResponse response = request.GetResponse(stream, _downloadBufferSize);

                if (response.ContentLength > 0)
                    content.Write(response.Content, 0, response.Content.Length);

                offset += response.ContentLength;

                if (response.ContentLength < _downloadBufferSize)
                    break;
            }

            ThrowIfCancelled(offset, length);
            return content.ToArray();
        }

        private int DownloadLength(Stream stream)
        {
            LytroRequest request = LytroRequest.Create(LytroCommand.QueryContentLength);
            LytroResponse response = request.GetResponse(stream, 4);

            int length = -1;

            if (response.ContentLength == 4)
                length = response.GetContentAsInt32();

            if (length < 0)
                throw new LytroNetProtocolViolationException("Query content length failed.");

            return length;
        }

        private void ThrowIfCancelled(int transferred, int total)
        {
            LytroNetProgressChangedHandler handler = ProgressChanged;
            if (handler != null)
            {
                LytroNetProgressChangedEventArgs args = new LytroNetProgressChangedEventArgs(transferred, total);
                handler(this, args);

                if (args.Cancel)
                    throw new LytroNetCanceledException();
            }
        }

        /// <summary>
        /// Occurs during downloading data from or uploading data to the camera.
        /// </summary>
        public event LytroNetProgressChangedHandler ProgressChanged;

        private static byte[] ToCameraTime(DateTimeOffset time)
        {
            time = time.UtcDateTime;
            byte[] content = new byte[14];
         
            BitConverter.GetBytes((short)time.Year).CopyTo(content, 0);
            BitConverter.GetBytes((short)time.Month).CopyTo(content, 2);
            BitConverter.GetBytes((short)time.Day).CopyTo(content, 4);
            BitConverter.GetBytes((short)time.Hour).CopyTo(content, 6);
            BitConverter.GetBytes((short)time.Minute).CopyTo(content, 8);
            BitConverter.GetBytes((short)time.Second).CopyTo(content, 10);
            BitConverter.GetBytes((short)time.Millisecond).CopyTo(content, 12);

            return content;
        }

        private static DateTimeOffset ToDateTimeOffset(byte[] buffer, int index)
        {
            global::System.Diagnostics.Debug.Assert(buffer != null, "Buffer cannot be null.");
            global::System.Diagnostics.Debug.Assert(buffer.Length - index >= 14, "Buffer must be have at least 14 bytes from index.");
            global::System.Diagnostics.Debug.Assert(index >= 0, "Index out of range.");

            return new DateTimeOffset(
                BitConverter.ToInt16(buffer, index + 0),
                BitConverter.ToInt16(buffer, index + 2),
                BitConverter.ToInt16(buffer, index + 4),
                BitConverter.ToInt16(buffer, index + 6),
                BitConverter.ToInt16(buffer, index + 8),
                BitConverter.ToInt16(buffer, index + 10),
                BitConverter.ToInt16(buffer, index + 12),
                TimeSpan.Zero);
        }
    }
}
