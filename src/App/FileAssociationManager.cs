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
namespace LHQ.App
{
    public static class FileAssociationManager
    {
        // Import SHChangeNotify to refresh the shell
        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        private const uint SHCNE_ASSOCCHANGED = 0x08000000;
        private const uint SHCNF_IDLIST = 0x0000;

        private static RegistryKey GetClassesRoot()
        {
            return Environment.UserInteractive ? Registry.CurrentUser : Registry.LocalMachine;
        }

        public static void RegisterFileAssociation(
            string extension,
            string progId,
            string description,
            string executablePath,
            string iconPath = null)
        {
            try
            {
                if (!extension.StartsWith("."))
                {
                    extension = "." + extension;
                }

                RegistryKey root = GetClassesRoot();

                using (RegistryKey key = root.CreateSubKey($@"Software\Classes\{extension}"))
                {
                    key?.SetValue("", progId);
                }

                using (RegistryKey key = root.CreateSubKey($@"Software\Classes\{progId}"))
                {
                    if (key != null)
                    {
                        key.SetValue("", description);

                        if (!string.IsNullOrEmpty(iconPath))
                        {
                            using (RegistryKey iconKey = key.CreateSubKey("DefaultIcon"))
                            {
                                iconKey?.SetValue("", iconPath);
                            }
                        }

                        using (RegistryKey commandKey = key.CreateSubKey(@"shell\open\command"))
                        {
                            commandKey?.SetValue("", $"\"{executablePath}\" -m \"%1\"");
                        }
                    }
                }

                SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to register file association: {ex.Message}", ex);
            }
        }

        public static void UnregisterFileAssociation(string extension, string progId)
        {
            try
            {
                if (!extension.StartsWith("."))
                {
                    extension = "." + extension;
                }

                RegistryKey root = GetClassesRoot();
                root.DeleteSubKeyTree($@"Software\Classes\{extension}", false);
                root.DeleteSubKeyTree($@"Software\Classes\{progId}", false);

                SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to unregister file association: {ex.Message}", ex);
            }
        }

        public static bool IsAssociated(string extension, string progId)
        {
            try
            {
                if (!extension.StartsWith("."))
                {
                    extension = "." + extension;
                }

                string subKeyPath = $@"Software\Classes\{extension}";

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(subKeyPath))
                {
                    if (key?.GetValue("")?.ToString() == progId)
                        return true;
                }

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(subKeyPath))
                {
                    if (key?.GetValue("")?.ToString() == progId)
                        return true;
                }
            }
            catch
            {
                // Ignore errors
            }

            return false;
        }
    }
}
