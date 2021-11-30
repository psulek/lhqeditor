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
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace ScaleHQ.AspNetCore.LHQ
{
    /// <summary>
    /// Extension methods for ASP.NET Core DI container which register LHQ related types.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Register LHQ type string localizer into ASP.NET Core.
        /// </summary>
        /// <typeparam name="TStringLocalizer">Type of string localizer.</typeparam>
        /// <typeparam name="TTypedStrings">Type of typed strings class.</typeparam>
        /// <param name="services">Services container where to register LHQ strings localizer.</param>
        public static void AddTypedStringsLocalizer<TStringLocalizer, TTypedStrings>(this IServiceCollection services)
            where TStringLocalizer : class, IStringLocalizer
            where TTypedStrings : class
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            services.AddOptions();

            services.TryAddSingleton<IStringLocalizerFactory, StringLocalizerFactory>();
            services.TryAddSingleton<IStringLocalizer, TStringLocalizer>();
            services.TryAddSingleton<TTypedStrings>();
        }

        /// <summary>
        /// Register LHQ types into ASP.NET Core model metadata and data annotations (NET Core &gt;= 2.x).
        /// </summary>
        /// <param name="mvcBuilder">Services container where to register LHQ strings localizer.</param>
        public static IMvcBuilder AddMvcTypedStringsLocalizer(this IMvcBuilder mvcBuilder)
        {
#if !ASP_NET_CORE1
            mvcBuilder.Services.Configure<MvcOptions>(options => { options.ModelMetadataDetailsProviders.Add(new DisplayNameDetailsProvider()); });

            mvcBuilder.AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, localizerFactory) => localizerFactory.Create(type);
                });
#endif
            return mvcBuilder;
        }

        /// <summary>
        /// Using LHQ types in ASP.NET Core app.
        /// </summary>
        /// <typeparam name="TStringLocalizer">Type of string localizer.</typeparam>
        /// <typeparam name="TTypedStrings">Type of typed strings class.</typeparam>
        /// <param name="app">Application where to use LHQ types.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseTypedStringsLocalizer<TStringLocalizer, TTypedStrings>(this IApplicationBuilder app)
            where TStringLocalizer : class, IStringLocalizer
            where TTypedStrings : class
        {
            string methodName = typeof(DependencyInjectionExtensions).FullName + "." + nameof(AddTypedStringsLocalizer);

            var stringLocalizer = app.ApplicationServices.GetService<IStringLocalizer>();
            if (stringLocalizer == null)
            {
                throw new InvalidOperationException(
                    $"Service '{typeof(IStringLocalizer)}' was not registered, call method {methodName} in begin of Startup.ConfigureServices()!");
            }

            var stringLocalizerFactory = app.ApplicationServices.GetService<IStringLocalizerFactory>();
            if (stringLocalizerFactory == null)
            {
                throw new InvalidOperationException(
                    $"Service '{typeof(IStringLocalizerFactory)}' was not registered, call method {methodName} in begin of Startup.ConfigureServices()!");
            }

#if !ASP_NET_CORE1
            DisplayNameDetailsProvider.SetStringLocalizer(stringLocalizer);
#endif

            // initialize type string object!
            app.ApplicationServices.GetService<TTypedStrings>();

            return app;
        }
    }
}
