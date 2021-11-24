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

using LHQ.Utils.Utilities;

namespace LHQ.Core.Model
{
    public class OperationResult
    {
        public OperationResultStatus Status { get; set; }

        public bool IsSuccess => Status == OperationResultStatus.Success;

        public bool IsFailure => Status == OperationResultStatus.Failed;

        public string Message { get; set; }

        public string Detail { get; set; }

        public bool IsWarning { get; set; }

        public object Data { get; set; }

        public OperationResult(OperationResultStatus status) : this(status, null)
        {
        }

        public OperationResult(OperationResultStatus status, string message) : this(status, message, null)
        {
        }

        public OperationResult(OperationResultStatus status, string message, string detail)
        {
            IsWarning = false;
            Status = status;
            Message = message;
            Detail = detail;
        }

        public void SetError(string message, string detail = null)
        {
            Status = OperationResultStatus.Failed;
            Message = message;
            Detail = detail;
        }

        public void SetSuccess(string message = null, string detail = null)
        {
            Status = OperationResultStatus.Success;
            Message = message;
            Detail = detail;
        }

        public void AssignFrom(OperationResult other)
        {
            ArgumentValidator.EnsureArgumentNotNull(other, nameof(other));
            Status = other.Status;
            Message = other.Message;
            Detail = other.Detail;
        }
    }

    public class OperationResult<T> : OperationResult
    {
        public new T Data { get; set; }

        public OperationResult(OperationResultStatus status) : base(status)
        { }

        public OperationResult(OperationResultStatus status, string message) : base(status, message)
        { }

        public OperationResult(OperationResultStatus status, string message, string detail) : base(status, message, detail)
        { }
    }

    public enum OperationResultStatus
    {
        Success,
        Failed
    }
}