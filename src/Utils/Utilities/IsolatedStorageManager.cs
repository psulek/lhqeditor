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
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Text;
using LHQ.Utils.Extensions;
// ReSharper disable UnusedMember.Global

namespace LHQ.Utils.Utilities
{
    internal sealed class IsolatedStorageManager
    {
        private readonly Random _random;
        private readonly int _typeIntSize;

        public IsolatedStorageManager()
        {
            _random = new Random((int)DateTime.UtcNow.Ticks);
            _typeIntSize = sizeof(int);
        }

        public void RemoveDirectory(string directory)
        {
            using (IsolatedStorageFile storageFile = GetIsolatedStorage())
            {
                if (storageFile.DirectoryExists(directory))
                {
                    string mask = directory;
                    mask += directory.EndsWith("\\") ? string.Empty : "\\";
                    mask += "*.*";
                    string[] allFiles = storageFile.GetFileNames(mask);
                    if (allFiles.Length > 0)
                    {
                        foreach (string file in allFiles)
                        {
                            string fileName = Path.Combine(directory, file);
                            if (storageFile.FileExists(fileName))
                            {
                                storageFile.DeleteFile(fileName);
                            }
                        }
                    }

                    storageFile.DeleteDirectory(directory);
                }
            }
        }

        public void Remove(string directory, string storageFileName)
        {
            using (IsolatedStorageFile storageFile = GetIsolatedStorage())
            {
                storageFileName = GetFullFilePath(directory, storageFileName);

                if (storageFile.FileExists(storageFileName))
                {
                    storageFile.DeleteFile(storageFileName);
                }
            }
        }


        public DateTimeOffset? GetLastWriteTime(string directory, string storageFileName)
        {
            DateTimeOffset? result = null;

            try
            {
                using (IsolatedStorageFile storageFile = GetIsolatedStorage())
                {
                    storageFileName = GetFullFilePath(directory, storageFileName);

                    if (storageFile.FileExists(storageFileName))
                    {
                        result = storageFile.GetLastWriteTime(storageFileName);
                    }
                    storageFile.Close();
                }
            }
            catch (Exception e)
            {
                DebugUtils.Error($"IsolatedStorageManager.GetLastWriteTime(directory:'{directory}',storageFileName:'{storageFileName}') failed", e);
            }

            return result;
        }

        public T Load<T>(string directory, string storageFileName, bool unprotectData)
        {
            return Load<T>(directory, storageFileName, unprotectData, out _);
        }

        public T Load<T>(string directory, string storageFileName, bool unprotectData,
            out DateTimeOffset? lastWriteTime)
        {
            T result = default;
            lastWriteTime = null;

            if (LoadBuffer(directory, storageFileName, unprotectData, out var buffer, out var lastWrite))
            {
                string json = buffer == null ? null : Encoding.UTF8.GetString(buffer);
                if (!string.IsNullOrEmpty(json))
                {
                    result = JsonUtils.FromJsonString<T>(json);
                }

                lastWriteTime = lastWrite;
            }

            return result;
        }

        private bool LoadBuffer(string directory, string storageFileName, bool unprotectData,
            out byte[] buffer, out DateTimeOffset? lastWriteTime)
        {
            bool result = false;
            buffer = null;
            lastWriteTime = null;

            try
            {
                using (IsolatedStorageFile storageFile = GetIsolatedStorage())
                {
                    storageFileName = GetFullFilePath(directory, storageFileName);

                    if (storageFile.FileExists(storageFileName))
                    {
                        lastWriteTime = storageFile.GetLastWriteTime(storageFileName);
                        using (IsolatedStorageFileStream fileStream = storageFile.OpenFile(storageFileName, FileMode.Open))
                        {
                            long streamLength = fileStream.Length;

                            using (var reader = new BinaryReader(fileStream))
                            {
                                if (unprotectData)
                                {
                                    int vectorBufferSize = reader.ReadInt32();
                                    byte[] vectorIv = reader.ReadBytes(vectorBufferSize);

                                    int readedBytesLength = _typeIntSize + vectorIv.Length;

                                    if (vectorIv.Length > 0 && streamLength > readedBytesLength)
                                    {
                                        var restBytesCount = (int)(streamLength - readedBytesLength);
                                        byte[] protectedData = reader.ReadBytes(restBytesCount);

                                        buffer =
                                            System.Security.Cryptography.ProtectedData.Unprotect(protectedData,
                                                vectorIv,
                                                DataProtectionScope.CurrentUser);
                                    }
                                }
                                else
                                {
                                    buffer = reader.ReadBytes((int)streamLength);
                                }
                            }
                            fileStream.Close();
                        }

                        result = true;
                    }
                    storageFile.Close();
                }
            }
            catch (Exception e)
            {
                DebugUtils.Error($"IsolatedStorageManager.LoadBuffer(directory:'{directory}',storageFileName:'{storageFileName}',unprotectData:{unprotectData}) failed", e);
            }

            return result;
        }

        public bool Save<T>(string directory, string storageFileName, T content, bool protectData)
        {
            return Save(directory, storageFileName, content, protectData, out _);
        }

        public bool Save<T>(string directory, string storageFileName, T content, bool protectData,
            out DateTimeOffset? lastWriteTime)
        {
            string json = JsonUtils.ToJsonString(content);
            return Save(directory, storageFileName, Encoding.UTF8.GetBytes(json), protectData,
                out lastWriteTime);
        }

        private bool Save(string directory, string storageFileName, byte[] content, bool protectData,
            out DateTimeOffset? lastWriteTime)
        {
            var result = true;
            lastWriteTime = null;

            try
            {
                byte[] protectedData = protectData ? ProtectedData(content) : content;

                using (IsolatedStorageFile storageFile = GetIsolatedStorage())
                {
                    if (!directory.IsNullOrEmpty() && !storageFile.DirectoryExists(directory))
                    {
                        storageFile.CreateDirectory(directory);
                    }

                    storageFileName = GetFullFilePath(directory, storageFileName);

                    using (IsolatedStorageFileStream fileStream = storageFile.OpenFile(storageFileName, FileMode.Create))
                    {
                        fileStream.Write(protectedData, 0, protectedData.Length);
                        fileStream.Close();
                    }

                    lastWriteTime = storageFile.GetLastWriteTime(storageFileName);
                    storageFile.Close();
                }
            }
            catch (Exception e)
            {
                result = false;
                DebugUtils.Error($"IsolatedStorageManager.Save(directory:'{directory}',storageFileName:'{storageFileName}',protectData:{protectData}) failed", e);
            }

            return result;
        }

        private static IsolatedStorageFile GetIsolatedStorage()
        {
            return IsolatedStorageFile.GetUserStoreForAssembly();
        }

        private string GetFullFilePath(string directory, string storageFileName)
        {
            return directory.IsNullOrEmpty() ? storageFileName : Path.Combine(directory, storageFileName);
        }

        private byte[] ProtectedData(byte[] sourceData)
        {
            byte[] vectorIv = GenerateVector();
            byte[] protectedData = System.Security.Cryptography.ProtectedData.Protect(sourceData, vectorIv, DataProtectionScope.CurrentUser);
            var mergedBuffer = new byte[_typeIntSize + vectorIv.Length + protectedData.Length];

            byte[] vectorIVsize = BitConverter.GetBytes(vectorIv.Length);
            vectorIVsize.CopyTo(mergedBuffer, 0);
            vectorIv.CopyTo(mergedBuffer, vectorIVsize.Length);
            protectedData.CopyTo(mergedBuffer, vectorIVsize.Length + vectorIv.Length);
            return mergedBuffer;
        }

        private static byte[] GenerateRandomBytes(int length)
        {
            // Create a buffer
            byte[] randBytes = length >= 1 ? new byte[length] : new byte[1];

            // Create a new RNGCryptoServiceProvider.
            var rand =
                new RNGCryptoServiceProvider();

            // Fill the buffer with random bytes.
            rand.GetBytes(randBytes);

            // return the bytes.
            return randBytes;
        }

        private byte[] GenerateVector()
        {
            int bufferLength = _random.Next(16, 32);
            byte[] buffer = GenerateRandomBytes(bufferLength);
            return buffer;
        }
    }
}
