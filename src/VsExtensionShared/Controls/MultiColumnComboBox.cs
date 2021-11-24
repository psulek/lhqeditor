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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using LHQ.Utils.Extensions;

namespace LHQ.VsExtension.Controls
{
    public sealed class MultiColumnComboBox : ComboBox
    {
        private string _columnNameString = "";
        private string _columnWidthString = "";
        private int _linkedColumnIndex;
        private TextBox _linkedTextBox;
        
        public MultiColumnComboBox()
        {
            DrawMode = DrawMode.OwnerDrawVariable;

            // If all of your boxes will be RightToLeft, uncomment 
            // the following line to make RTL the default.
            //RightToLeft = RightToLeft.Yes;

            // Remove the Context Menu to disable pasting 
            ContextMenu = new ContextMenu();
        }

        public bool AutoComplete { get; set; }

        public bool AutoDropdown { get; set; }

        public Color BackColorEven { get; set; } = Color.White;

        public Color BackColorOdd { get; set; } = Color.White;

        public Collection<string> ColumnNameCollection { get; } = new Collection<string>();

        public string ColumnNames
        {
            get => _columnNameString;

            set
            {
                // If the column string is blank, leave it blank.
                // The default width will be used for all columns.
                if (!Convert.ToBoolean(value.Trim().Length))
                {
                    _columnNameString = "";
                }
                else if (value != null)
                {
                    char[] delimiterChars = { ',', ';', ':' };
                    string[] columnNames = value.Split(delimiterChars);

                    if (!DesignMode)
                    {
                        ColumnNameCollection.Clear();
                    }

                    // After splitting the string into an array, iterate
                    // through the strings and check that they're all valid.
                    foreach (string s in columnNames)
                    {
                        // Does it have length?
                        if (Convert.ToBoolean(s.Trim().Length))
                        {
                            if (!DesignMode)
                            {
                                ColumnNameCollection.Add(s.Trim());
                            }
                        }
                        else // The value is blank
                        {
                            throw new NotSupportedException("Column names can not be blank.");
                        }
                    }
                    _columnNameString = value;
                }
            }
        }

        public Collection<int> ColumnWidthCollection { get; } = new Collection<int>();

        public int ColumnWidthDefault { get; set; } = 75;

        public string ColumnWidths
        {
            get => _columnWidthString;

            set
            {
                // If the column string is blank, leave it blank.
                // The default width will be used for all columns.
                if (!Convert.ToBoolean(value.Trim().Length))
                {
                    _columnWidthString = "";
                }
                else if (value != null)
                {
                    char[] delimiterChars = { ',', ';', ':' };
                    string[] columnWidths = value.Split(delimiterChars);
                    var invalidValue = "";
                    int invalidIndex = -1;
                    var idx = 1;
                    int intValue;

                    // After splitting the string into an array, iterate
                    // through the strings and check that they're all integers
                    // or blanks
                    foreach (string s in columnWidths)
                    {
                        // If it has length, test if it's an integer
                        if (Convert.ToBoolean(s.Trim().Length))
                        {
                            // It's not an integer. Flag the offending value.
                            if (!int.TryParse(s, out intValue))
                            {
                                invalidIndex = idx;
                                invalidValue = s;
                            }
                            else // The value was okay. Increment the item index.
                            {
                                idx++;
                            }
                        }
                        else // The value is a space. Use the default width.
                        {
                            idx++;
                        }
                    }

                    // If an invalid value was found, raise an exception.
                    if (invalidIndex > -1)
                    {
                        string errMsg = "Invalid column width '" + invalidValue + "' located at column " + invalidIndex;
                        throw new ArgumentOutOfRangeException(errMsg);
                    }
                    _columnWidthString = value;

                    // Only set the values of the collections at runtime.
                    // Setting them at design time doesn't accomplish 
                    // anything and causes errors since the collections 
                    // don't exist at design time.
                    if (!DesignMode)
                    {
                        ColumnWidthCollection.Clear();
                        foreach (string s in columnWidths)
                        {
                            // Initialize a column width to an integer
                            if (Convert.ToBoolean(s.Trim().Length))
                            {
                                ColumnWidthCollection.Add(Convert.ToInt32(s));
                            }
                            else // Initialize the column to the default
                            {
                                ColumnWidthCollection.Add(ColumnWidthDefault);
                            }
                        }

                        // If the column is bound to data, set the column widths
                        // for any columns that aren't explicitly set by the 
                        // string value entered by the programmer
                        if (DataManager != null)
                        {
                            InitializeColumns();
                        }
                    }
                }
            }
        }

        public new DrawMode DrawMode
        {
            get => base.DrawMode;
            set
            {
                if (value != DrawMode.OwnerDrawVariable)
                {
                    throw new NotSupportedException("Needs to be DrawMode.OwnerDrawVariable");
                }
                base.DrawMode = value;
            }
        }

        public new ComboBoxStyle DropDownStyle
        {
            get => base.DropDownStyle;
            set
            {
                if (value != ComboBoxStyle.DropDown)
                {
                    throw new NotSupportedException("ComboBoxStyle.DropDown is the only supported style");
                }
                base.DropDownStyle = value;
            }
        }

        public int LinkedColumnIndex
        {
            get => _linkedColumnIndex;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("A column index can not be negative");
                }
                _linkedColumnIndex = value;
            }
        }

        public TextBox LinkedTextBox
        {
            get => _linkedTextBox;
            set
            {
                _linkedTextBox = value;

                if (_linkedTextBox != null)
                {
                    // Set any default properties of the Linked Textbox here
                    _linkedTextBox.ReadOnly = true;
                    _linkedTextBox.TabStop = false;
                }
            }
        }

        public int TotalWidth { get; private set; }

        public event EventHandler OpenSearchForm;

        protected override void OnDataSourceChanged(EventArgs e)
        {
            base.OnDataSourceChanged(e);

            InitializeColumns();
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            if (DesignMode)
            {
                return;
            }

            e.DrawBackground();

            Rectangle boundsRect = e.Bounds;
            var lastRight = 0;

            Color brushForeColor;
            if ((e.State & DrawItemState.Selected) == 0)
            {
                // Item is not selected. Use BackColorOdd & BackColorEven
                Color backColor;
                backColor = Convert.ToBoolean(e.Index % 2) ? BackColorOdd : BackColorEven;
                using (var brushBackColor = new SolidBrush(backColor))
                {
                    e.Graphics.FillRectangle(brushBackColor, e.Bounds);
                }
                brushForeColor = Color.Black;
            }
            else
            {
                // Item is selected. Use ForeColor = White
                brushForeColor = Color.White;
            }

            using (var linePen = new Pen(SystemColors.GrayText))
            {
                using (var brush = new SolidBrush(brushForeColor))
                {
                    if (!Convert.ToBoolean(ColumnNameCollection.Count))
                    {
                        e.Graphics.DrawString(Convert.ToString(Items[e.Index]), Font, brush, boundsRect);
                    }
                    else
                    {
                        // If the ComboBox is displaying a RightToLeft language, draw it this way.
                        if (RightToLeft.Equals(RightToLeft.Yes))
                        {
                            // Define a StringFormat object to make the string display RTL.
                            var rtl = new StringFormat();
                            rtl.Alignment = StringAlignment.Near;
                            rtl.FormatFlags = StringFormatFlags.DirectionRightToLeft;

                            // Draw the strings in reverse order from high column index to zero column index.
                            for (int colIndex = ColumnNameCollection.Count - 1; colIndex >= 0; colIndex--)
                            {
                                if (Convert.ToBoolean(ColumnWidthCollection[colIndex]))
                                {
                                    string item = Convert.ToString(FilterItemOnProperty(Items[e.Index], ColumnNameCollection[colIndex]));

                                    boundsRect.X = lastRight;
                                    boundsRect.Width = ColumnWidthCollection[colIndex];
                                    lastRight = boundsRect.Right;

                                    // Draw the string with the RTL object.
                                    e.Graphics.DrawString(item, Font, brush, boundsRect, rtl);

                                    if (colIndex > 0)
                                    {
                                        e.Graphics.DrawLine(linePen, boundsRect.Right, boundsRect.Top, boundsRect.Right, boundsRect.Bottom);
                                    }
                                }
                            }
                        }
                        // If the ComboBox is displaying a LeftToRight language, draw it this way.
                        else
                        {
                            // Display the strings in ascending order from zero to the highest column.
                            for (var colIndex = 0; colIndex < ColumnNameCollection.Count; colIndex++)
                            {
                                if (Convert.ToBoolean(ColumnWidthCollection[colIndex]))
                                {
                                    string item = Convert.ToString(FilterItemOnProperty(Items[e.Index], ColumnNameCollection[colIndex]));

                                    boundsRect.X = lastRight;
                                    boundsRect.Width = ColumnWidthCollection[colIndex];
                                    lastRight = boundsRect.Right;
                                    e.Graphics.DrawString(item, Font, brush, boundsRect);

                                    if (colIndex < ColumnNameCollection.Count - 1)
                                    {
                                        e.Graphics.DrawLine(linePen, boundsRect.Right, boundsRect.Top, boundsRect.Right, boundsRect.Bottom);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            e.DrawFocusRectangle();
        }

        protected override void OnDropDown(EventArgs e)
        {
            base.OnDropDown(e);

            if (TotalWidth > 0)
            {
                if (Items.Count > MaxDropDownItems)
                {
                    // The vertical scrollbar is present. Add its width to the total.
                    // If you don't then RightToLeft languages will have a few characters obscured.
                    DropDownWidth = TotalWidth + SystemInformation.VerticalScrollBarWidth;
                }
                else
                {
                    DropDownWidth = TotalWidth;
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Use the Delete or Escape Key to blank out the ComboBox and
            // allow the user to type in a new value
            if (e.KeyCode == Keys.Delete ||
                e.KeyCode == Keys.Escape)
            {
                SelectedIndex = -1;
                Text = "";
                if (_linkedTextBox != null)
                {
                    _linkedTextBox.Text = "";
                }
            }
            else if (e.KeyCode == Keys.F3)
            {
                // Fire the OpenSearchForm Event
                if (OpenSearchForm != null)
                {
                    OpenSearchForm(this, EventArgs.Empty);
                }
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (DroppedDown)
                {
                    DroppedDown = false;
                }
            }
        }

        // Some of the code for OnKeyPress was derived from some VB.NET code  
        // posted by Laurent Muller as a suggested improvement for another control.
        // http://www.codeproject.com/vb/net/autocomplete_combobox.asp?df=100&forumid=3716&select=579095#xx579095xx
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            int idx = -1;
            string toFind;

            DroppedDown = AutoDropdown && e.KeyChar != (char)Keys.Enter;
            if (!char.IsControl(e.KeyChar))
            {
                if (AutoComplete)
                {
                    toFind = Text.Substring(0, SelectionStart) + e.KeyChar;
                    idx = FindStringExact(toFind);

                    if (idx == -1)
                    {
                        // An exact match for the whole string was not found
                        // Find a substring instead.
                        idx = FindString(toFind);
                    }
                    else
                    {
                        // An exact match was found. Close the dropdown.
                        DroppedDown = false;
                    }

                    if (idx != -1) // The substring was found.
                    {
                        SelectedIndex = idx;
                        SelectionStart = toFind.Length;
                        SelectionLength = Text.Length - SelectionStart;
                    }
                    else // The last keystroke did not create a valid substring.
                    {
                        // If the substring is not found, cancel the keypress
                        e.KeyChar = (char)0;
                    }
                }
                else // AutoComplete = false. Treat it like a DropDownList by finding the
                    // KeyChar that was struck starting from the current index
                {
                    idx = FindString(e.KeyChar.ToString(), SelectedIndex);

                    if (idx != -1)
                    {
                        SelectedIndex = idx;
                    }
                }
            }

            // Do no allow the user to backspace over characters. Treat it like
            // a left arrow instead. The user must not be allowed to change the 
            // value in the ComboBox. 
            if (e.KeyChar == (char)Keys.Back && // A Backspace Key is hit
                AutoComplete && // AutoComplete = true
                Convert.ToBoolean(SelectionStart)) // And the SelectionStart is positive
            {
                // Find a substring that is one character less the the current selection.
                // This mimicks moving back one space with an arrow key. This substring should
                // always exist since we don't allow invalid selections to be typed. If you're
                // on the 3rd character of a valid code, then the first two characters have to 
                // be valid. Moving back to them and finding the 1st occurrence should never fail.
                toFind = Text.Substring(0, SelectionStart - 1);
                idx = FindString(toFind);

                if (idx != -1)
                {
                    SelectedIndex = idx;
                    SelectionStart = toFind.Length;
                    SelectionLength = Text.Length - SelectionStart;
                }
            }

            // e.Handled is always true. We handle every keystroke programatically.
            e.Handled = true;
        }

        protected override void OnSelectedValueChanged(EventArgs e)
        {
            base.OnSelectedValueChanged(e); //Added after version 1.3 on 01/31/2008

            if (_linkedTextBox != null)
            {
                if (_linkedColumnIndex < ColumnNameCollection.Count)
                {
                    _linkedTextBox.Text = Convert.ToString(FilterItemOnProperty(SelectedItem, ColumnNameCollection[_linkedColumnIndex]));
                }
            }
        }

        protected override void OnValueMemberChanged(EventArgs e)
        {
            base.OnValueMemberChanged(e);

            InitializeValueMemberColumn();
        }

        private void InitializeColumns()
        {
            if (!Convert.ToBoolean(_columnNameString.Length))
            {
                PropertyDescriptorCollection propertyDescriptorCollection = DataManager.GetItemProperties();

                TotalWidth = 0;
                ColumnNameCollection.Clear();

                for (var colIndex = 0; colIndex < propertyDescriptorCollection.Count; colIndex++)
                {
                    ColumnNameCollection.Add(propertyDescriptorCollection[colIndex].Name);

                    // If the index is greater than the collection of explicitly
                    // set column widths, set any additional columns to the default
                    if (colIndex >= ColumnWidthCollection.Count)
                    {
                        ColumnWidthCollection.Add(ColumnWidthDefault);
                    }
                    TotalWidth += ColumnWidthCollection[colIndex];
                }
            }
            else
            {
                TotalWidth = 0;

                for (var colIndex = 0; colIndex < ColumnNameCollection.Count; colIndex++)
                {
                    // If the index is greater than the collection of explicitly
                    // set column widths, set any additional columns to the default
                    if (colIndex >= ColumnWidthCollection.Count)
                    {
                        ColumnWidthCollection.Add(ColumnWidthDefault);
                    }
                    TotalWidth += ColumnWidthCollection[colIndex];
                }
            }

            // Check to see if the programmer is trying to display a column
            // in the linked textbox that is greater than the columns in the 
            // ComboBox. I handle this error by resetting it to zero.
            if (_linkedColumnIndex >= ColumnNameCollection.Count)
            {
                _linkedColumnIndex = 0; // Or replace this with an OutOfBounds Exception
            }
        }

        private void InitializeValueMemberColumn()
        {
            var colIndex = 0;
            foreach (string columnName in ColumnNameCollection)
            {
                if (string.Compare(columnName, ValueMember, true, CultureInfo.CurrentUICulture) == 0)
                {
                    //_valueMemberColumnIndex = colIndex;
                    break;
                }
                colIndex++;
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            if (Text.IsNullOrEmpty())
            {
                SelectedIndex = -1;
            }
        }
    }
}
