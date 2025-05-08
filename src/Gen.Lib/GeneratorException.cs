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

using System;
using System.Runtime.Serialization;

namespace LHQ.Gen.Lib
{
    /// <summary>
    /// Thrown when generation process of <see cref="Generator.Generate"/> fails on executing handlerbars templates.
    /// </summary>
    public class GeneratorException : Exception
    {
        /// <summary>
        /// Title of the exception.
        /// </summary>
        public string Title { get; set; }

        public GeneratorErrorKind Kind { get; set; } = GeneratorErrorKind.GenericError;

        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorException"/> class.
        /// </summary>
        public GeneratorException()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorException"/> class.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        protected GeneratorException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratorException"/> class.
        /// </summary>
        /// <param name="title">Title of the exception.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public GeneratorException(string title, string message, GeneratorErrorKind kind, string code, Exception innerException) : base(message, innerException)
        {
            Title = title;
            Kind = kind;
            Code = code;
        }
    }
    
    public static class GeneratorErrorCodes
    {
        public const string CsharpNamespaceMissing = "csharp.namespace.missing";
    }
    
    public enum GeneratorErrorKind
    {
        /// <summary>
        ///  The generic error.
        /// </summary>
        GenericError,
        
        /// <summary>
        /// The exception is caused by invalid model schema.
        /// </summary>
        InvalidModelSchema,

        /// <summary>
        /// The exception is caused by invalid template settings.
        /// </summary>
        TemplateValidationError
    }
}
