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
using LHQ.Core.DependencyInjection;

// ReSharper disable UnusedMember.Global

namespace LHQ.App.Services.Implementation
{
    public sealed class ServiceRegistration : IEquatable<ServiceRegistration>
    {
        private ServiceRegistration(Type contractType, Type implementationType)
        {
            ContractType = contractType;
            ImplementationType = implementationType;
        }

        public Type ContractType { get; }

        public Type ImplementationType { get; }

        public static ServiceRegistration Create(Type contractType,
            Type implementationType)
        {
            if (!typeof(IService).IsAssignableFrom(contractType))
            {
                throw new InvalidOperationException($"Contract type '{contractType}' must inherit interface '{typeof(IService)}'");
            }

            return new ServiceRegistration(contractType, implementationType);
        }

        public static ServiceRegistration Create<TContract, TImplementation>()
            where TContract : IService
        {
            return new ServiceRegistration(typeof(TContract), typeof(TImplementation));
        }

        public bool Equals(ServiceRegistration other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return ContractType == other.ContractType && ImplementationType == other.ImplementationType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is ServiceRegistration registration && Equals(registration);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ContractType.GetHashCode() * 397) ^ ImplementationType.GetHashCode();
            }
        }
    }
}
