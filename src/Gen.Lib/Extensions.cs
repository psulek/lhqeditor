#region License

// Copyright (c) 2025 Peter Šulek / ScaleHQ Solutions s.r.o.
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

using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LHQ.Gen.Lib
{
    public static class Extensions
    {
        private static readonly Encoding _noBomEncoding = new UTF8Encoding(false);

        private const int BufferSize = 4096;

        public static async Task WriteToDiskAsync(this GeneratedFile generatedFile, string outputPath = null)
        {
            var encoding = generatedFile.Bom ? Encoding.UTF8 : _noBomEncoding;
            byte[] encodedText = encoding.GetBytes(generatedFile.GetContent(true));

            var fileName = string.IsNullOrEmpty(outputPath) ? generatedFile.FileName : Path.Combine(outputPath, generatedFile.FileName);

            using (var sourceStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize, true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            }
        }

        public static void WriteToDisk(this GeneratedFile generatedFile, string outputPath = null)
        {
            var encoding = generatedFile.Bom ? Encoding.UTF8 : _noBomEncoding;
            byte[] encodedText = encoding.GetBytes(generatedFile.GetContent(true));

            var fileName = string.IsNullOrEmpty(outputPath) ? generatedFile.FileName : Path.Combine(outputPath, generatedFile.FileName);

            using (var sourceStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize, true))
            {
                sourceStream.Write(encodedText, 0, encodedText.Length);
            }
        }
    }
}
