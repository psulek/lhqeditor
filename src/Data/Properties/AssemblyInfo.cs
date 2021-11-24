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

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LHQ.Data.Attributes;
using LHQ.Data.CodeGenerator;
using LHQ.Data.Metadata;
using LHQ.Data.ModelStorage.Serializers;

[assembly: AssemblyTitle("LHQ Model library")]
[assembly: AssemblyProduct("Model")]

[assembly: ComVisible(false)]

[assembly: Guid("ee616feb-a683-41ed-95ca-e6a555292239")]

[assembly: ModelMetadataDescriptor(typeof(CodeGeneratorMetadataDescriptor))]

[assembly: ModelSerializer(typeof(ModelSerializer_v1))]

[assembly: InternalsVisibleTo("LHQ.App, PublicKey=002400000480000094000000060200000024000052534131000400000100010045B61101054DC9FC021CFC4372901DA828477699C3AEB1A53E19F874D45AF79F6D62386966A78761E97DFB97D389A54A64314F7B251745D033A711E0C9C6413E28B015B0C432B95ED4B18981FD44856ADEBE5588938EEB23ABA56BD91CA023709E3382E9B5359821A3761C8C8A43CD1E443E8BAD38C41BC4273726CC5DA0B9C2")]
[assembly: InternalsVisibleTo("LHQ.UnitTests, PublicKey=002400000480000094000000060200000024000052534131000400000100010045B61101054DC9FC021CFC4372901DA828477699C3AEB1A53E19F874D45AF79F6D62386966A78761E97DFB97D389A54A64314F7B251745D033A711E0C9C6413E28B015B0C432B95ED4B18981FD44856ADEBE5588938EEB23ABA56BD91CA023709E3382E9B5359821A3761C8C8A43CD1E443E8BAD38C41BC4273726CC5DA0B9C2")]