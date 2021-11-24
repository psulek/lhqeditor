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
using LHQ.App.Code;
using LHQ.App.Localization;
using LHQ.App.Model;
using LHQ.Data;
using LHQ.Data.ModelStorage;
// ReSharper disable UnusedMember.Global

namespace LHQ.App.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDisplayName(this TreeElementType elementType)
        {
            switch (elementType)
            {
                case TreeElementType.Model:
                {
                    return Strings.Enums.TreeElementType.Model;
                }
                case TreeElementType.Category:
                {
                    return Strings.Enums.TreeElementType.Category;
                }
                case TreeElementType.Resource:
                {
                    return Strings.Enums.TreeElementType.Resource;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(elementType));
                }
            }
        }

        public static string ToDisplayName(this ResourceElementTranslationState state)
        {
            switch (state)
            {
                case ResourceElementTranslationState.New:
                {
                    return Strings.Enums.ResourceElementTranslationState.New;
                }
                case ResourceElementTranslationState.Edited:
                {
                    return Strings.Enums.ResourceElementTranslationState.Edited;
                }
                case ResourceElementTranslationState.NeedsReview:
                {
                    return Strings.Enums.ResourceElementTranslationState.NeedsReview;
                }
                case ResourceElementTranslationState.Final:
                {
                    return Strings.Enums.ResourceElementTranslationState.Final;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }
            }
        }

        public static string ToDisplayName(this ModelLoadStatus modelLoadStatus)
        {
            switch (modelLoadStatus)
            {
                case ModelLoadStatus.Success:
                {
                    return Strings.Enums.ModelLoadStatus.Success;
                }
                case ModelLoadStatus.ModelSourceInvalid:
                {
                    return Strings.Enums.ModelLoadStatus.ModelSourceInvalid;
                }
                case ModelLoadStatus.ModelUpgraderRequired:
                {
                    return Strings.Enums.ModelLoadStatus.ModelUpgraderRequired;
                }
                case ModelLoadStatus.LoadError:
                {
                    return Strings.Enums.ModelLoadStatus.LoadError;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(modelLoadStatus));
                }
            }
        }

        public static string ToDisplayName(this ProjectLoadStatus status, string fileName)
        {
            switch (status)
            {
                case ProjectLoadStatus.Success:
                {
                    return Strings.Enums.StartupModelLoadStatus.Success(fileName);
                }
                case ProjectLoadStatus.FileNotFoundOrAccessDenied:
                {
                    return Strings.Enums.StartupModelLoadStatus.FileNotFoundOrAccessDenied(fileName);
                }
                case ProjectLoadStatus.ModelLoadError:
                {
                    return Strings.Enums.StartupModelLoadStatus.ModelLoadError(fileName);
                }
                case ProjectLoadStatus.ModelUpgradeRequired:
                {
                    return Strings.Enums.StartupModelLoadStatus.ModelUpgradeRequired(fileName);
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(status));
                }
            }
        }

        public static string ToDisplayName(this ProjectBusyOperationType busyOperationType)
        {
            switch (busyOperationType)
            {
                case ProjectBusyOperationType.None:
                {
                    return string.Empty;
                }
                case ProjectBusyOperationType.OpenProject:
                {
                    return Strings.Enums.ProjectBusyOperationType.OpenProject;
                }
                case ProjectBusyOperationType.SaveProject:
                {
                    return Strings.Enums.ProjectBusyOperationType.SaveProject;
                }
                case ProjectBusyOperationType.Export:
                {
                    return Strings.Enums.ProjectBusyOperationType.Export;
                }
                case ProjectBusyOperationType.Import:
                {
                    return Strings.Enums.ProjectBusyOperationType.Import;
                }
                case ProjectBusyOperationType.TreeSearch:
                {
                    return Strings.Enums.ProjectBusyOperationType.TreeSearch;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(busyOperationType), busyOperationType, null);
                }
            }
        }
    }
}
