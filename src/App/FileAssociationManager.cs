#region License
// Copyright (c) 2026 Peter Šulek / ScaleHQ Solutions s.r.o.
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

using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace LHQ.App
{
    public static class FileAssociationManager
    {
        // Import SHChangeNotify to refresh the shell
        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        private const uint SHCNE_ASSOCCHANGED = 0x08000000;
        private const uint SHCNF_IDLIST = 0x0000;

        /// <summary>
        /// Registers a file association for your application
        /// </summary>
        /// <param name="extension">File extension (e.g., ".myfile")</param>
        /// <param name="progId">Programmatic identifier (e.g., "MyApp.Document")</param>
        /// <param name="description">Description of the file type</param>
        /// <param name="executablePath">Full path to your executable</param>
        /// <param name="iconPath">Optional: Path to icon file (can be exe with icon index, e.g., "app.exe,0")</param>
        public static void RegisterFileAssociation(
            string extension, 
            string progId, 
            string description, 
            string executablePath,
            string iconPath = null)
        {
            try
            {
                // Ensure extension starts with a dot
                if (!extension.StartsWith("."))
                    extension = "." + extension;

                // Create or open the extension key
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey($@"Software\Classes\{extension}"))
                {
                    if (key != null)
                    {
                        key.SetValue("", progId);
                    }
                }

                // Create or open the ProgID key
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey($@"Software\Classes\{progId}"))
                {
                    if (key != null)
                    {
                        key.SetValue("", description);

                        // Set the icon if provided
                        if (!string.IsNullOrEmpty(iconPath))
                        {
                            using (RegistryKey iconKey = key.CreateSubKey("DefaultIcon"))
                            {
                                iconKey?.SetValue("", iconPath);
                            }
                        }

                        // Set the command to open files with -m argument
                        using (RegistryKey commandKey = key.CreateSubKey(@"shell\open\command"))
                        {
                            commandKey?.SetValue("", $"\"{executablePath}\" -m \"%1\"");
                        }
                    }
                }

                // Notify the shell that file associations have changed
                SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to register file association: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Unregisters a file association
        /// </summary>
        public static void UnregisterFileAssociation(string extension, string progId)
        {
            try
            {
                if (!extension.StartsWith("."))
                    extension = "." + extension;

                // Remove extension key
                Registry.CurrentUser.DeleteSubKeyTree($@"Software\Classes\{extension}", false);

                // Remove ProgID key
                Registry.CurrentUser.DeleteSubKeyTree($@"Software\Classes\{progId}", false);

                // Notify the shell
                SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to unregister file association: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Checks if a file association already exists
        /// </summary>
        public static bool IsAssociated(string extension, string progId)
        {
            try
            {
                if (!extension.StartsWith("."))
                    extension = "." + extension;

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey($@"Software\Classes\{extension}"))
                {
                    if (key != null)
                    {
                        string value = key.GetValue("")?.ToString();
                        return value == progId;
                    }
                }
            }
            catch
            {
                // Ignore errors
            }

            return false;
        }

        /// <summary>
        /// Checks if running with administrator privileges
        /// </summary>
        public static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}
