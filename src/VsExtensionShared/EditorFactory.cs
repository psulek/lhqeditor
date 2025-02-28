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
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace LHQ.VsExtension
{
    /// <summary>
    ///     Factory for creating our editor object. Extends from the IVsEditoryFactory interface
    /// </summary>
    [Guid(PackageGuids.guidLhqEditorString)]
    public sealed class EditorFactory : IVsEditorFactory, IDisposable
    {
        private readonly VsPackage _editorPackage;
        private ServiceProvider _vsServiceProvider;

        public EditorFactory(VsPackage package)
        {
            _editorPackage = package;
        }

        /// <summary>
        ///     Since we create a ServiceProvider which implements IDisposable we
        ///     also need to implement IDisposable to make sure that the ServiceProvider's
        ///     Dispose method gets called.
        /// </summary>
        public void Dispose()
        {
            _vsServiceProvider?.Dispose();
        }

        /// <summary>
        ///     Used for initialization of the editor in the environment
        /// </summary>
        /// <param name="psp">
        ///     pointer to the service provider. Can be used to obtain instances of other interfaces
        /// </param>
        /// <returns></returns>
        public int SetSite(IServiceProvider psp)
        {
            _vsServiceProvider = new ServiceProvider(psp);
            return VSConstants.S_OK;
        }

        public object GetService(Type serviceType)
        {
            return _vsServiceProvider.GetService(serviceType);
        }

        public int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
        {
            pbstrPhysicalView = null; // initialize out parameter

            // we support only a single physical view
            if (VSConstants.LOGVIEWID_Primary == rguidLogicalView)
            {
                return VSConstants.S_OK; // primary view uses NULL as pbstrPhysicalView
            }
            if (VSConstants.LOGVIEWID.TextView_guid == rguidLogicalView)
            {
                // Our editor supports FindInFiles, therefore we need to declare support for LOGVIEWID_TextView.
                // In addition our EditorPane implements IVsCodeWindow and we also provide the 
                // VSSettings (pkgdef) metadata statement that we support LOGVIEWID_TextView via the following
                // attribute on our Package class:
                // [ProvideEditorLogicalView(typeof(EditorFactory), VSConstants.LOGVIEWID.TextView_string)]

                pbstrPhysicalView = null; // our primary view implements IVsCodeWindow
                return VSConstants.S_OK;
            }
            return VSConstants.E_NOTIMPL; // you must return E_NOTIMPL for any unrecognized rguidLogicalView values
        }

        public int Close()
        {
            return VSConstants.S_OK;
        }

        /// <summary>
        ///     Used by the editor factory to create an editor instance. the environment first determines the
        ///     editor factory with the highest priority for opening the file and then calls
        ///     IVsEditorFactory.CreateEditorInstance. If the environment is unable to instantiate the document data
        ///     in that editor, it will find the editor with the next highest priority and attempt to so that same
        ///     thing.
        ///     NOTE: The priority of our editor is 32 as mentioned in the attributes on the package class.
        ///     Since our editor supports opening only a single view for an instance of the document data, if we
        ///     are requested to open document data that is already instantiated in another editor, or even our
        ///     editor, we return a value VS_E_INCOMPATIBLEDOCDATA.
        /// </summary>
        /// <param name="grfCreateDoc">
        ///     Flags determining when to create the editor. Only open and silent flags
        ///     are valid
        /// </param>
        /// <param name="pszMkDocument">path to the file to be opened</param>
        /// <param name="pszPhysicalView">name of the physical view</param>
        /// <param name="pvHier">pointer to the IVsHierarchy interface</param>
        /// <param name="itemid">Item identifier of this editor instance</param>
        /// <param name="punkDocDataExisting">
        ///     This parameter is used to determine if a document buffer
        ///     (DocData object) has already been created
        /// </param>
        /// <param name="ppunkDocView">Pointer to the IUnknown interface for the DocView object</param>
        /// <param name="ppunkDocData">Pointer to the IUnknown interface for the DocData object</param>
        /// <param name="pbstrEditorCaption">Caption mentioned by the editor for the doc window</param>
        /// <param name="pguidCmdUI">
        ///     the Command UI Guid. Any UI element that is visible in the editor has
        ///     to use this GUID. This is specified in the .vsct file
        /// </param>
        /// <param name="pgrfCDW">Flags for CreateDocumentWindow</param>
        /// <returns></returns>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public int CreateEditorInstance(
            uint grfCreateDoc,
            string pszMkDocument,
            string pszPhysicalView,
            IVsHierarchy pvHier,
            uint itemid,
            IntPtr punkDocDataExisting,
            out IntPtr ppunkDocView,
            out IntPtr ppunkDocData,
            out string pbstrEditorCaption,
            out Guid pguidCmdUI,
            out int pgrfCDW)
        {
            //DebugUtils.Log(string.Format(CultureInfo.CurrentCulture, "Entering {0} CreateEditorInstace()", ToString()));

            // Initialize to null
            ppunkDocView = IntPtr.Zero;
            ppunkDocData = IntPtr.Zero;
            //pguidCmdUI = GuidList.GuidEditorFactory;
            pguidCmdUI = PackageGuids.guidLhqEditor;
            pgrfCDW = 0;
            pbstrEditorCaption = null;

            // Validate inputs
            if ((grfCreateDoc & (VSConstants.CEF_OPENFILE | VSConstants.CEF_SILENT)) == 0)
            {
                return VSConstants.E_INVALIDARG;
            }
            if (punkDocDataExisting != IntPtr.Zero)
            {
                return VSConstants.VS_E_INCOMPATIBLEDOCDATA;
            }

            // Create the Document (editor)
            var editorPane = new EditorPane(_editorPackage);
            ppunkDocView = Marshal.GetIUnknownForObject(editorPane);
            ppunkDocData = Marshal.GetIUnknownForObject(editorPane);
            pbstrEditorCaption = "";
            return VSConstants.S_OK;
        }
    }
}
