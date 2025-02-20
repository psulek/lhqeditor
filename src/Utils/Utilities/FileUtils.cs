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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global

namespace LHQ.Utils.Utilities
{
    public static class FileUtils
    {
        private static readonly Encoding NoBomEncoding = new UTF8Encoding(false);

        private const int BufferSize = 0x1000;

        public static Encoding DefaultEncoding => new UTF8Encoding(true);

        public static async Task<string> ReadAllTextAsync(string fileName)
        {
            using (FileStream sourceStream = new FileStream(fileName,
                FileMode.Open, FileAccess.Read, FileShare.Read, 
                BufferSize, true))
            {
                StringBuilder sb = new StringBuilder();

                byte[] buffer = new byte[BufferSize];
                int numRead;
                while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string text = Encoding.UTF8.GetString(buffer, 0, numRead);
                    sb.Append(text);
                }

                return sb.ToString();
            }
        }

        // public static string ToLinuxLineEndings(string str)
        // {
        //     return str.Replace("\r\n", "\n").Replace('\r', '\n');
        // }

        public static async Task WriteAllTextAsync(string fileName, string text, bool useBOM = false)
        {
            var encoding = useBOM ? Encoding.UTF8 : NoBomEncoding;
            byte[] encodedText = encoding.GetBytes(text);

            using (FileStream sourceStream = new FileStream(fileName,
                FileMode.Create, FileAccess.Write, FileShare.None,
                BufferSize, true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            }
        }

        public static void WriteAllText(string fileName, string text)
        {
            File.WriteAllText(fileName, text, DefaultEncoding);
        }

        public static string ReadAllText(string fileName)
        {
            return File.ReadAllText(fileName, DefaultEncoding);
        }

        public static void DeleteFolder(string folder, bool recursive, 
            Func<DirectoryInfo, int, bool> directoryPredicate = null,
            Func<FileInfo, int, bool> filePredicate = null)
        {
            void Process(string sourceFolder, int depth)
            {
                if (Directory.Exists(sourceFolder))
                {
                    foreach (var directory in Directory.GetDirectories(sourceFolder).Select(x => new DirectoryInfo(x)))
                    {
                        if (directoryPredicate == null || directoryPredicate(directory, depth))
                        {
                            Process(directory.FullName, depth+1);
                            directory.Delete();
                        }
                    }

                    foreach (var fileInfo in Directory.GetFiles(sourceFolder, "*.*", SearchOption.TopDirectoryOnly).Select(x => new FileInfo(x)))
                    {
                        if (filePredicate == null || filePredicate(fileInfo, depth))
                        {
                            fileInfo.Delete();
                        }
                    }

                    Directory.Delete(sourceFolder);
                }
            }

            Process(folder, 0);
        }

        public static bool MoveFolderRecursive(string sourceFolder, string targetFolder, bool overwrite = false)
        {
            bool exists = Directory.Exists(sourceFolder);
            if (exists)
            {
                foreach (var directory in Directory.GetDirectories(sourceFolder).Select(x => new DirectoryInfo(x)))
                {
                    try
                    {
                        string sourceDir = directory.FullName;
                        string destDirName = Path.Combine(targetFolder, directory.Name);

                        if (!Directory.Exists(targetFolder))
                        {
                            Directory.CreateDirectory(targetFolder);
                        }

                        if (Directory.Exists(destDirName) && overwrite)
                        {
                            Directory.Delete(destDirName, true);
                        }

                        Directory.Move(sourceDir, destDirName);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                foreach (string sourceFile in Directory.GetFiles(sourceFolder, "*.*", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        string baseName = Path.GetFileName(sourceFile);
                        string destFileName = Path.Combine(targetFolder, baseName);

                        if (File.Exists(destFileName) && overwrite)
                        {
                            File.Delete(destFileName);
                        }

                        File.Move(sourceFile, destFileName);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }

            return exists;
        }
    }
}
