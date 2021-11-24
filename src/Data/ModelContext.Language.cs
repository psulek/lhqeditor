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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using LHQ.Data.Extensions;

namespace LHQ.Data
{
    public partial class ModelContext
    {
        public void RemoveLanguage(string languageName, bool removeResources)
        {
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(languageName, "languageName");

            LanguageElement language = FindLanguageByName(languageName);
            if (language == null)
            {
                throw new ModelException(Model, Errors.Model.ModelDoesNotContainLanguage(languageName));
            }

            RemoveLanguage(language, removeResources);
        }

        public void RemoveLanguage(LanguageElement language, bool removeResources)
        {
            ArgumentValidator.EnsureArgumentNotNull(language, "language");
            Model.Languages.Remove(language);

            if (removeResources)
            {
                foreach (var resourceValues in Model.FlattenAllResources().Select(x => x.Values))
                {
                    resourceValues.RemoveAll(x => x.LanguageName.EqualsTo(language.Name));
                }
            }
        }

        public LanguageElement AddLanguage(string cultureName, bool isPrimary = false)
        {
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(cultureName, "cultureName");

            var cultureInfo = new CultureInfo(cultureName);
            return AddLanguage(cultureInfo, isPrimary);
        }

        public LanguageElement AddLanguage(CultureInfo cultureInfo, bool isPrimary = false)
        {
            ArgumentValidator.EnsureArgumentNotNull(cultureInfo, "cultureInfo");

            var language = CreateLanguage(cultureInfo);
            AddLanguage(language);

            if (isPrimary)
            {
                SetPrimaryLanguage(language.Name);
            }

            return language;
        }

        public void SetPrimaryLanguage(string languageName)
        {
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(languageName, "languageName");

            LanguageElement language = Model.Languages.FindByName(languageName, true, CultureInfo.InvariantCulture);
            if (language == null)
            {
                throw new ModelException(Model, Errors.Model.ModelDoesNotContainLanguage(languageName));
            }

            LanguageElement primaryLanguage = GetPrimaryLanguage();
            if (primaryLanguage != null && !primaryLanguage.Name.EqualsTo(languageName))
            {
                primaryLanguage.IsPrimary = false;
            }

            language.IsPrimary = true;
        }

        public LanguageElement GetPrimaryLanguage()
        {
            return Model.Languages.SingleOrDefault(x => x.IsPrimary);
        }

        internal bool ContainsLanguage(string languageName)
        {
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(languageName, "languageName");
            return FindLanguageByName(languageName) != null;
        }

        private LanguageElement FindLanguageByName(string languageName)
        {
            return Model.Languages.FindByName(languageName, true, CultureInfo.InvariantCulture);
        }

        public void AddLanguages(List<string> languages)
        {
            foreach (string languageName in languages)
            {
                if (!ContainsLanguage(languageName))
                {
                    var language = CreateLanguage(languageName);
                    AddLanguage(language);
                }
            }
        }

        internal void AddLanguage(LanguageElement language)
        {
            ArgumentValidator.EnsureArgumentNotNull(language, "language");
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(language.Name, "language.Name");

            if (ContainsLanguage(language.Name))
            {
                throw new ModelException(Model, language, Errors.Model.ModelAlreadyContainsLanguage(language.Name));
            }

            LanguageElement primaryLanguage = GetPrimaryLanguage();
            bool hasPrimaryLanguage = primaryLanguage != null;

            // if model already has set some primary language and newly added language has also flag is primary to true, throw ex.
            if (hasPrimaryLanguage && language.IsPrimary)
            {
                throw new ModelException(Model, language,
                    Errors.Model.ModelAlreadyContainOtherLanguageAsPrimary(primaryLanguage.Name));
            }

            Model.Languages.Add(language);
        }


        internal LanguageElement CreateLanguage(string cultureName)
        {
            ArgumentValidator.EnsureArgumentNotNullOrEmpty(cultureName, "cultureName");

            var cultureInfo = new CultureInfo(cultureName);
            return CreateLanguage(cultureInfo);
        }

        internal LanguageElement CreateLanguage(CultureInfo cultureInfo)
        {
            ArgumentValidator.EnsureArgumentNotNull(cultureInfo, "cultureInfo");

            var language = CreateLanguage();
            language.Name = cultureInfo.Name;
            return language;
        }

        internal LanguageElement CreateLanguage()
        {
            return new LanguageElement(GenerateKey());
        }
    }
}