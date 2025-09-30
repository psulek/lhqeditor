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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using LHQ.Core.DependencyInjection;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace LHQ.App.Services.Implementation
{
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class ServiceContainer : DisposableObject, IServiceContainer
    {
        private Dictionary<Type, ServiceRegistration> _serviceRegistrations = new Dictionary<Type, ServiceRegistration>();

        private Dictionary<ServiceRegistration, IService> _singletonInstances =
            new Dictionary<ServiceRegistration, IService>();

        private Action<IService> _onObjectCreated;

        public int ServicesCount => _serviceRegistrations.Count;

        public static ServiceContainer Create(Action<IService> onObjectCreated = null)
        {
            return Create(null, onObjectCreated);
        }

        private static ServiceContainer Create(IEnumerable<ServiceRegistration> registrations, Action<IService> onObjectCreated)
        {
            var container = new ServiceContainer
            {
                _onObjectCreated = onObjectCreated
            };
            if (registrations != null)
            {
                container.Initialize(registrations);
            }
            return container;
        }

        public List<T> GetAllImplements<T>() where T: IService
        {
            Type interfaceType = typeof(T);
            if (!interfaceType.IsInterface)
            {
                throw new InvalidOperationException("Type must be interface!");
            }

            var result = new List<T>();
            foreach (ServiceRegistration registration in _serviceRegistrations.Values
                .Where(x => interfaceType.IsAssignableFrom(x.ImplementationType)))
            {
                result.Add(GetInstance<T>(registration));
            }

            return result;
        }

        private void InvokeObjectCreated(IService service)
        {
            _onObjectCreated?.Invoke(service);
        }

        private void Initialize(IEnumerable<ServiceRegistration> registrations)
        {
            foreach (ServiceRegistration registration in registrations)
            {
                Register(registration);
            }
        }

        public void Load(Action<IService> loadTask)
        {
            var services = _serviceRegistrations.Values.Select(GetInstance).ToList();

            foreach (IService service in services)
            {
                service.ConfigureDependencies(this);
            }

            foreach (IService service in services)
            {
                loadTask(service);
            }
        }

        private ServiceRegistration FindRegistration(Type contractType)
        {
            _serviceRegistrations.TryGetValue(contractType, out ServiceRegistration result);
            return result;
        }

        private T CreateInstance<T>(ServiceRegistration registration) where T: IService
        {
            Type implementationType = registration.ImplementationType;

            ConstructorInfo[] constructors = implementationType.GetConstructors();
            if (constructors == null || constructors.Length == 0)
            {
                throw new InvalidOperationException("Type '{0}' does not have public constructor!".FormatWith(implementationType));
            }
            ConstructorInfo constructor = constructors[0];

            List<ParameterInfo> constructorParameters = constructor.GetParameters().ToList();

            var instance = (T)(constructorParameters.Count == 0
                ? Activator.CreateInstance(implementationType)
                : CreateObjectInstance(constructor));

            if (instance != null)
            {
                InvokeObjectCreated(instance);
            }

            return instance;
        }

        protected virtual object CreateObjectInstance(ConstructorInfo constructor)
        {
            List<ParameterInfo> constructorParameters = constructor.GetParameters().ToList();
            var parameters = new List<object>(constructorParameters.Count);
            parameters.AddRange(constructorParameters.Select(parameterInfo => Get(parameterInfo.ParameterType)));
            return constructor.Invoke(parameters.ToArray());
        }

        private T GetInstance<T>(ServiceRegistration registration) where T: IService
        {
            T singletonInstance;
            if (_singletonInstances.TryGetValue(registration, out var value))
            {
                singletonInstance = (T)value;
            }
            else
            {
                singletonInstance = CreateInstance<T>(registration);
                _singletonInstances.Add(registration, singletonInstance);
            }

            return singletonInstance;
        }

        private IService GetInstance(ServiceRegistration registration)
        {
            return GetInstance<IService>(registration);
        }

        private void CheckInitialized()
        {
            if (_serviceRegistrations.Count == 0)
            {
                throw new InvalidOperationException("Service Container need to be initialized by calling Initialize(...) before usage.");
            }
        }

        public IServiceContainer ReplaceRegistration<TContract, TImplementation>()
            where TContract : IService
        {
            Unregister<TContract>();
            return Register<TContract, TImplementation>();
        }

        public IServiceContainer Register<TContract, TImplementation>() where TContract: IService
        {
            ServiceRegistration serviceRegistration = ServiceRegistration.Create<TContract, TImplementation>();
            return Register(serviceRegistration);
        }

        private IServiceContainer Register(ServiceRegistration registration)
        {
            _serviceRegistrations.Add(registration.ContractType, registration);
            return this;
        }

        private void Unregister<TContract>()
        {
            Unregister(typeof(TContract));
        }

        private void Unregister(Type contractType)
        {
            ServiceRegistration serviceRegistration = FindRegistration(contractType);
            if (serviceRegistration != null)
            {
                Unregister(serviceRegistration);
            }
        }

        private void Unregister(ServiceRegistration registration)
        {
            if (_serviceRegistrations.ContainsKey(registration.ContractType))
            {
                _serviceRegistrations.Remove(registration.ContractType);
            }
        }

        public T Get<T>() where T : IService
        {
            CheckInitialized();

            Type contract = typeof(T);
            ServiceRegistration registration = FindRegistration(contract);
            ArgumentValidator.EnsureArgumentNotNull(registration, "registration");
            return GetInstance<T>(registration);
        }

        public object Get(Type contract)
        {
            CheckInitialized();

            ServiceRegistration registration = FindRegistration(contract);
            ArgumentValidator.EnsureArgumentNotNull(registration, "registration");
            return GetInstance(registration);
        }

        protected override void DoDispose()
        {
            _serviceRegistrations.Clear();
            _serviceRegistrations = null;

            _singletonInstances.Clear();
            _singletonInstances = null;
        }
    }
}
