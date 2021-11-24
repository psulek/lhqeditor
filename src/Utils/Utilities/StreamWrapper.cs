#region License
// Copyright (c) 2021 Peter Šulek / ScaleHQ Solutions s.r.o.
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.IO;
// ReSharper disable UnusedMember.Global

namespace LHQ.Utils.Utilities
{
    public sealed class StreamWrapper<T> : Stream
        where T : Stream
    {
        private readonly bool _leaveOpen;

        public StreamWrapper(T stream)
            : this(stream, true)
        { }

        public StreamWrapper(T stream, bool leaveOpen)
        {
            Stream = stream;
            _leaveOpen = leaveOpen;
        }

        public override bool CanRead => Stream.CanRead;

        public override bool CanSeek => Stream.CanSeek;

        public override bool CanTimeout => Stream.CanTimeout;

        public override bool CanWrite => Stream.CanWrite;

        public override long Length => Stream.Length;

        public override long Position
        {
            get => Stream.Position;
            set => Stream.Position = value;
        }

        public override int ReadTimeout
        {
            get => Stream.ReadTimeout;
            set => Stream.ReadTimeout = value;
        }

        public T Stream { get; }

        public override int WriteTimeout
        {
            get => Stream.WriteTimeout;
            set => Stream.WriteTimeout = value;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return Stream.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return Stream.BeginWrite(buffer, offset, count, callback, state);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_leaveOpen)
            {
                Stream.Dispose();
            }
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return Stream.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            Stream.EndWrite(asyncResult);
        }

        public override void Flush()
        {
            Stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return Stream.Read(buffer, offset, count);
        }

        public override int ReadByte()
        {
            return Stream.ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return Stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            Stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Stream.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            Stream.WriteByte(value);
        }
    }
}
