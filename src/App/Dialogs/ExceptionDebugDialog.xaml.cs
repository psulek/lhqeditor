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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using LHQ.App.Code;
using LHQ.App.Localization;
using LHQ.App.Services.Interfaces;
using LHQ.Utils.Utilities;

// ReSharper disable UnusedAutoPropertyAccessor.Local

// code taken from - http://www.codeproject.com/Tips/469452/WPF-ExceptionViewer

namespace LHQ.App.Dialogs
{
    /// <summary>
    ///     A WPF window for viewing Exceptions and inner Exceptions, including all their properties.
    /// </summary>
    public partial class ExceptionDebugDialog
    {
        private static string _defaultTitle;
        private static string _product;

        // Font sizes based on the "normal" size.
        private readonly double _small;

        private readonly double _med;
        private readonly double _large;

        // This is used to dynamically calculate the mainGrid.MaxWidth when the Window is resized,
        // since I can't quite get the behavior I want without it.  See CalcMaxTreeWidth().
        private double _chromeWidth;

        /// <summary>
        ///     The exception and header message cannot be null.  If owner is specified, this window
        ///     uses its Style and will appear centered on the Owner.  You can override this before
        ///     calling ShowDialog().
        /// </summary>
        public ExceptionDebugDialog(Exception e, Window owner)
        {
            InitializeComponent();

            if (owner != null)
            {
                // This hopefully makes our window look like it belongs to the main app.
                //Style = owner.Style;

                // This seems to make the window appear on the same monitor as the owner.
                Owner = owner;

                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            if (DefaultPaneBrush != null)
            {
                TreeView1.Background = DefaultPaneBrush;
            }

            DocViewer.Background = TreeView1.Background;

            // We use three font sizes.  The smallest is based on whatever the "standard"
            // size is for the current system/app, taken from an arbitrary control.

            _small = TreeView1.FontSize;
            _med = _small * 1.1;
            _large = _small * 1.2;

            Title = DefaultTitle;

            BuildTree(e, Strings.Dialogs.ExceptionViewer.DialogTitle);
        }

        /// <summary>
        ///     The default title to use for the ExceptionViewer window.  Automatically initialized
        ///     to "Error - [ProductName]" where [ProductName] is taken from the application's
        ///     AssemblyProduct attribute (set in the AssemblyInfo.cs file).  You can change this
        ///     default, or ignore it and set Title yourself before calling ShowDialog().
        /// </summary>
        private static string DefaultTitle
        {
            get
            {
                if (_defaultTitle == null)
                {
                    string error = Strings.Common.Error;

                    if (string.IsNullOrEmpty(Product))
                    {
                        _defaultTitle = error;
                    }
                    else
                    {
                        _defaultTitle = error + " - " + Product;
                    }
                }

                return _defaultTitle;
            }
        }

        private static Brush DefaultPaneBrush { get; set; }

        /// <summary>
        ///     Gets the value of the AssemblyProduct attribute of the app.
        ///     If unable to lookup the attribute, returns an empty string.
        /// </summary>
        private static string Product => _product ?? (_product = GetProductName());

        public static void DialogShow(IAppContext appContext, Exception exception)
        {
            ArgumentValidator.EnsureArgumentNotNull(exception, "exception");

            var currentMainWindow = Application.Current?.MainWindow;
            var ownerWindow = currentMainWindow == null ? null : GetWindow(currentMainWindow);
            var dialog = new ExceptionDebugDialog(exception, ownerWindow);

            dialog.Closed += (sender, args) => { dialog.DataContext = null; };

            dialog.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // The grid column used for the tree started with Width="Auto" so it is now exactly
            // wide enough to fit the longest exception (up to the MaxWidth set in XAML).
            // Changing the width to a fixed pixel value prevents it from changing if the user
            // resizes the window.

            TreeCol.Width = new GridLength(TreeCol.ActualWidth, GridUnitType.Pixel);
            _chromeWidth = ActualWidth - MainGrid.ActualWidth;
            CalcMaxTreeWidth();
        }

        // Initializes the Product property.
        private static string GetProductName()
        {
            var result = "";

            try
            {
                Assembly appAssembly = GetAppAssembly();

                object[] customAttributes = appAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);

                if (customAttributes.Length > 0)
                {
                    result = ((AssemblyProductAttribute)customAttributes[0]).Product;
                }
            }
            catch
            {
                // ignored
            }

            return result;
        }

        // Tries to get the assembly to extract the product name from.
        private static Assembly GetAppAssembly()
        {
            Assembly appAssembly = null;

            try
            {
                // This is supposedly how Windows.Forms.Application does it.
                Window currentMainWindow = Application.Current?.MainWindow;
                appAssembly = currentMainWindow?.GetType().Assembly;
            }
            catch
            {
                // ignored
            }

            // If the above didn't work, try less desirable ways to get an assembly.

            if (appAssembly == null)
            {
                appAssembly = Assembly.GetEntryAssembly();
            }

            return appAssembly ?? Assembly.GetExecutingAssembly();
        }

        // Builds the tree in the left pane.
        // Each TreeViewItem.Tag will contain a list of Inlines
        // to display in the right-hand pane When it is selected.
        private void BuildTree(Exception e, string summaryMessage)
        {
            // The first node in the tree contains the summary message and all the
            // nested exception messages.

            var inlines = new List<Inline>();
            var firstItem = new TreeViewItem
            {
                Header = Strings.Dialogs.ExceptionViewer.AllMessages
            };
            TreeView1.Items.Add(firstItem);

            var inline = new Bold(new Run(summaryMessage)) { FontSize = _large };
            inlines.Add(inline);

            // Now add top-level nodes for each exception while building
            // the contents of the first node.
            while (e != null)
            {
                inlines.Add(new LineBreak());
                inlines.Add(new LineBreak());
                AddLines(inlines, e.Message);

                AddException(e);
                e = e.InnerException;
            }

            firstItem.Tag = inlines;
            firstItem.IsSelected = true;
        }

        private void AddProperty(List<Inline> inlines, string propName, object propVal)
        {
            inlines.Add(new LineBreak());
            inlines.Add(new LineBreak());
            var inline = new Bold(new Run(propName + ":")) { FontSize = _med };
            inlines.Add(inline);
            inlines.Add(new LineBreak());

            if (propVal is string s)
            {
                // Might have embedded newlines.

                AddLines(inlines, s);
            }
            else
            {
                inlines.Add(new Run(propVal.ToString()));
            }
        }

        // Adds the string to the list of Inlines, substituting
        // LineBreaks for an newline chars found.
        private void AddLines(List<Inline> inlines, string str)
        {
            string[] lines = str.Split('\n');

            inlines.Add(new Run(lines[0].Trim('\r')));

            foreach (string line in lines.Skip(1))
            {
                inlines.Add(new LineBreak());
                inlines.Add(new Run(line.Trim('\r')));
            }
        }

        // Adds the exception as a new top-level node to the tree with child nodes
        // for all the exception's properties.
        private void AddException(Exception e)
        {
            // Create a list of Inlines containing all the properties of the exception object.
            // The three most important properties (message, type, and stack trace) go first.

            var exceptionItem = new TreeViewItem();
            var inlines = new List<Inline>();
            PropertyInfo[] properties = e.GetType().GetProperties();

            exceptionItem.Header = e.GetType();
            exceptionItem.Tag = inlines;
            TreeView1.Items.Add(exceptionItem);

            Inline inline = new Bold(new Run(e.GetType().ToString()));
            inline.FontSize = _large;
            inlines.Add(inline);

            AddProperty(inlines, "Message", e.Message);
            AddProperty(inlines, "Stack Trace", e.StackTrace);

            foreach (PropertyInfo info in properties)
            {
                // Skip InnerException because it will get a whole
                // top-level node of its own.

                if (info.Name != "InnerException")
                {
                    object value = info.GetValue(e, null);

                    if (value != null)
                    {
                        if (value is string s)
                        {
                            if (string.IsNullOrEmpty(s))
                            {
                                continue;
                            }
                        }
                        else if (value is IDictionary dictionary)
                        {
                            value = RenderDictionary(dictionary);
                            if (string.IsNullOrEmpty((string)value))
                            {
                                continue;
                            }
                        }
                        else if (value is IEnumerable enumerable)
                        {
                            value = RenderEnumerable(enumerable);
                            if (string.IsNullOrEmpty((string)value))
                            {
                                continue;
                            }
                        }

                        if (info.Name != "Message" &&
                            info.Name != "StackTrace")
                        {
                            // Add the property to list for the exceptionItem.
                            AddProperty(inlines, info.Name, value);
                        }

                        // Create a TreeViewItem for the individual property.
                        var propertyItem = new TreeViewItem();
                        var propertyInlines = new List<Inline>();

                        propertyItem.Header = info.Name;
                        propertyItem.Tag = propertyInlines;
                        exceptionItem.Items.Add(propertyItem);
                        AddProperty(propertyInlines, info.Name, value);
                    }
                }
            }
        }

        private static string RenderEnumerable(IEnumerable data)
        {
            var result = new StringBuilder();

            foreach (object obj in data)
            {
                result.AppendFormat("{0}\n", obj);
            }

            if (result.Length > 0)
            {
                result.Length = result.Length - 1;
            }
            return result.ToString();
        }

        private static string RenderDictionary(IDictionary data)
        {
            var result = new StringBuilder();

            foreach (object key in data.Keys)
            {
                if (key != null && data[key] != null)
                {
                    result.AppendLine(key + " = " + data[key]);
                }
            }

            if (result.Length > 0)
            {
                result.Length = result.Length - 1;
            }
            return result.ToString();
        }

        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ShowCurrentItem();
        }

        private void ShowCurrentItem()
        {
            if ((TreeView1.SelectedItem as TreeViewItem)?.Tag is List<Inline> inlines)
            {
                var doc = new FlowDocument
                {
                    FontSize = _small,
                    FontFamily = TreeView1.FontFamily,
                    TextAlignment = TextAlignment.Left,
                    Background = DocViewer.Background
                };

                if (ChkWrap.IsChecked == false)
                {
                    doc.PageWidth = CalcNoWrapWidth(inlines) + 50;
                }

                var para = new Paragraph();
                para.Inlines.AddRange(inlines);
                doc.Blocks.Add(para);
                DocViewer.Document = doc;
            }
        }

        // Determines the page width for the Inlilness that causes no wrapping.
        private double CalcNoWrapWidth(IEnumerable<Inline> inlines)
        {
            double pageWidth = 0;
            var tb = new TextBlock();
            var size = new Size(double.PositiveInfinity, double.PositiveInfinity);

            foreach (Inline inline in inlines)
            {
                tb.Inlines.Clear();
                tb.Inlines.Add(inline);
                tb.Measure(size);

                if (tb.DesiredSize.Width > pageWidth)
                {
                    pageWidth = tb.DesiredSize.Width;
                }
            }

            return pageWidth;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            // Build a FlowDocument with Inlines from all top-level tree items.

            var inlines = new List<Inline>();
            var doc = new FlowDocument();
            var para = new Paragraph();

            doc.FontSize = _small;
            doc.FontFamily = TreeView1.FontFamily;
            doc.TextAlignment = TextAlignment.Left;

            Version appVersion = GetType().Assembly.GetName().Version;
            var labelCurrentTime = Strings.Dialogs.ExceptionViewer.CurrentTime;
            var labelAppVersion = Strings.Dialogs.ExceptionViewer.AppVersion;
            var date = DateTimeHelper.UtcNow.ToString("O");

            inlines.Add(new Run($"{labelCurrentTime} {date}"));
            inlines.Add(new LineBreak());
            inlines.Add(new Run($"{labelAppVersion} {appVersion}"));
            inlines.Add(new LineBreak());

            foreach (TreeViewItem treeItem in TreeView1.Items)
            {
                if (inlines.Any())
                {
                    // Put a line of underscores between each exception.

                    inlines.Add(new LineBreak());
                    inlines.Add(new Run("____________________________________________________"));
                    inlines.Add(new LineBreak());
                }

                if (treeItem.Tag is List<Inline> treeItemLines)
                {
                    inlines.AddRange(treeItemLines);
                }
            }

            para.Inlines.AddRange(inlines);
            doc.Blocks.Add(para);

            // Now place the doc contents on the clipboard in both
            // rich text and plain text format.

            var range = new TextRange(doc.ContentStart, doc.ContentEnd);
            var data = new DataObject();

            using (Stream stream = new MemoryStream())
            {
                range.Save(stream, DataFormats.Rtf);
                data.SetData(DataFormats.Rtf, Encoding.UTF8.GetString(((MemoryStream)stream).ToArray()));
            }

            data.SetData(DataFormats.StringFormat, range.Text);
            Clipboard.SetDataObject(data);

            // The Inlines that were being displayed are now in the temporary document we just built,
            // causing them to disappear from the viewer.  This puts them back.

            ShowCurrentItem();
        }

        private void chkWrap_Checked(object sender, RoutedEventArgs e)
        {
            ShowCurrentItem();
        }

        private void chkWrap_Unchecked(object sender, RoutedEventArgs e)
        {
            ShowCurrentItem();
        }

        private void ExpressionViewerWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                CalcMaxTreeWidth();
            }
        }

        private void CalcMaxTreeWidth()
        {
            // This prevents the GridSplitter from being dragged beyond the right edge of the window.
            // Another way would be to use star sizing for all Grid columns including the left 
            // Grid column (i.e. treeCol), but that causes the width of that column to change when the
            // window's width changes, which I don't like.

            MainGrid.MaxWidth = ActualWidth - _chromeWidth;
            TreeCol.MaxWidth = MainGrid.MaxWidth - TextCol.MinWidth;
        }
    }
}
