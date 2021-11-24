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

using System.Threading;

namespace LHQ.Utils.Utilities
{
    /// <summary>
    /// Code from: http://www.codeproject.com/Tips/375559/Implement-Thread-Safe-One-shot-Bool-Flag-with-Inte
    /// Thread safe enter once into a code block:
    /// the first call to CheckAndSetFirstCall returns always true,
    /// all subsequent call return false.
    /// </summary>
    public class ThreadSafeSingleShotGuard
    {
        private const int SignalNotset = 0;
        private const int SignalSet = 1;

        private int _state = SignalNotset;

        private ThreadSafeSingleShotGuard(bool signaled)
        {
            if (signaled)
            {
                SetSignal();
            }
            else
            {
                ResetSignal();
            }
        }

        public static ThreadSafeSingleShotGuard Create(bool signaled)
        {
            return new ThreadSafeSingleShotGuard(signaled);
        }

        /// <summary>
        /// Set signal.
        /// </summary>
        /// <returns>
        /// Returns <c>true</c> if set signal was successful.
        /// </returns>
        public bool SetSignal()
        {
            return Interlocked.Exchange(ref _state, SignalSet) == SignalNotset;
        }

        public void ResetSignal()
        {
            Interlocked.Exchange(ref _state, SignalNotset);
        }

        public bool IsSignalSet()
        {
            int res = Interlocked.CompareExchange(ref _state, SignalSet, SignalSet);
            return res == SignalSet;
        }
    }
}