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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LHQ.Utils.Extensions;
// ReSharper disable UnusedMember.Global

namespace LHQ.Utils.Utilities
{
    public static class ArgumentValidator
    {
        /// <summary>
        ///     Asserts the not null.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="paramName">Name of the param.</param>
        public static void EnsureArgumentNotNull(object parameter, string paramName)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        ///     Asserts the not null.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="paramName">Name of the param.</param>
        public static void EnsureArgumentNotNullOrEmpty(string parameter, string paramName)
        {
            if (parameter.IsNullOrEmpty())
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        ///     Asserts the is null.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="paramName">Name of the param.</param>
        public static void EnsureArgumentNull(object parameter, string paramName)
        {
            if (parameter != null)
            {
                throw new ArgumentException("parameter must be null", paramName);
            }
        }

        /// <summary>
        ///     Asserts the not null GUID.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="paramName">Name of the param.</param>
        public static void EnsureArgumentNotEmptyGuid(Guid parameter, string paramName)
        {
            if (parameter.IsEmpty())
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        ///     Asserts the is true.
        /// </summary>
        /// <param name="parameter">if set to <c>true</c> [expression].</param>
        /// <param name="message">The message.</param>
        public static void EnsureArgumentTrue(bool parameter, string message)
        {
            if (!parameter)
            {
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        ///     Asserts the is false.
        /// </summary>
        /// <param name="parameter">if set to <c>true</c> [expression].</param>
        /// <param name="message">The message.</param>
        public static void EnsureArgumentFalse(bool parameter, string message)
        {
            if (parameter)
            {
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        ///     Asserts the is greater than zero.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="paramName">Name of the param.</param>
        public static void EnsureArgumentGreaterThanZero(int parameter, string paramName)
        {
            if (parameter < 1)
            {
                throw new ArgumentOutOfRangeException(paramName, parameter, "Must be greater than zero");
            }
        }

        /// <summary>
        ///     Asserts the is greater than zero.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="paramName">Name of the param.</param>
        public static void EnsureArgumentGreaterThanZero(long parameter, string paramName)
        {
            if (parameter < 1)
            {
                throw new ArgumentOutOfRangeException(paramName, parameter, "Must be greater than zero");
            }
        }

        /// <summary>
        ///     Asserts the is greater or equal than zero.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="paramName">Name of the param.</param>
        public static void EnsureArgumentGreaterOrEqualThanZero(int parameter, string paramName)
        {
            if (parameter < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, parameter,
                    "Must be greater than zero or must equals to zero");
            }
        }

        /// <summary>
        ///     Asserts the is greater or equal than zero.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="paramName">Name of the param.</param>
        public static void EnsureArgumentGreaterOrEqualThanZero(long parameter, string paramName)
        {
            if (parameter < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, parameter,
                    "Must be greater than zero or must equals to zero");
            }
        }

        public static void EnsureArgumentNotEmptyCollection(ICollection parameter, string paramName)
        {
            EnsureArgumentNotNull(parameter, paramName);
            if (parameter.Count == 0)
            {
                throw new ArgumentOutOfRangeException(paramName, parameter, "Collection must not be empty");
            }
        }

        public static void EnsureArgumentNotEmptyCollection<T>(T[] parameter, string paramName)
        {
            EnsureArgumentNotNull(parameter, paramName);
            if (parameter.Length == 0)
            {
                throw new ArgumentOutOfRangeException(paramName, parameter, "Collection must not be empty");
            }
        }

        public static void EnsureArgumentNotEmptyCollection<T>(IEnumerable<T> parameter, string paramName)
        {
            EnsureArgumentNotNull(parameter, paramName);

            if (!parameter.Any())
            {
                throw new ArgumentOutOfRangeException(paramName, parameter, "Collection must not be empty");
            }
        }

        public static void ThrowError(string message)
        {
            throw new ArgumentException(message);
        }
    }
}
