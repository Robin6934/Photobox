﻿namespace Photobox.UI.Lib.Camera
{
    /// <summary>
    /// A stream that does nothing more but wrap another stream (needed for a WPF memory leak)
    /// </summary>
    /// <remarks>
    /// Creates a new instance of the <see cref="WrapStream"/> class.
    /// </remarks>
    /// <param name="inStream">The stream that gets wrapped</param>
    public sealed class WrapStream(Stream inStream) : Stream
    {
        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead => inStream.CanRead;

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek => inStream.CanSeek;

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite => inStream.CanWrite;

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        public override long Length => inStream.Length;

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        public override long Position
        {
            get => inStream.Position;
            set => inStream.Position = value;
        }

        /// <summary>
        /// reads a sequence of bytes from the current stream and advances
        /// the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified
        /// byte array with the values between offset and (offset + count - 1) replaced
        /// by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read
        /// from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. This can be less than the
        /// number of bytes requested if that many bytes are not currently available,
        /// or zero (0) if the end of the stream has been reached.</returns>
        public override int Read(byte[] buffer, int offset, int count) =>
            inStream.Read(buffer, offset, count);

        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current
        /// stream and advances the current position within this stream by the number
        /// of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the
        /// current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count) =>
            inStream.Write(buffer, offset, count);

        /// <summary>
        /// sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type System.IO.SeekOrigin indicating the reference
        /// used
        /// to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, SeekOrigin origin) => inStream.Seek(offset, origin);

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush() => inStream.Flush();

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        public override void SetLength(long value) => inStream.SetLength(value);
    }
}
