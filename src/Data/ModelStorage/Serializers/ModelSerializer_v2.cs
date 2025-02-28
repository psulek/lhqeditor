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
using System.Diagnostics;
using System.Linq;
using LHQ.Data.Extensions;
using LHQ.Data.Interfaces;
using LHQ.Data.Interfaces.Key;
using LHQ.Data.Metadata;
using LHQ.Data.Support;
using LHQ.Utils;
using LHQ.Utils.Extensions;
using LHQ.Utils.Utilities;
using Newtonsoft.Json.Linq;

namespace LHQ.Data.ModelStorage.Serializers
{
    public class ModelSerializer_v2: ModelSerializer_v1
    {
        public override int ModelVersion => 2;

        public override bool Upgrade(ModelContext previousModelContext)
        {
            return previousModelContext.Model.Version == 1;
        }
    }
}
