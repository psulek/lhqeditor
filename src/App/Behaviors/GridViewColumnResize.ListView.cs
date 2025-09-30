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
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace LHQ.App.Behaviors
{
    public static partial class GridViewColumnResize
    {
        /// <summary>
        ///     ListViewResizeBehavior class that gets attached to the ListView control
        /// </summary>
        public class ListViewResizeBehavior
        {
            //private const int Margin = 25;
            private const long RefreshTime = Timeout.Infinite;
            private const long Delay = 150;

            private readonly ListView _element;
            private readonly Timer _timer;
            private bool _resizeInProgress;

            public ListViewResizeBehavior(ListView element)
            {
                _element = element ?? throw new ArgumentNullException(nameof(element));
                _resizeInProgress = false;

                element.Loaded += OnLoaded;

                // Action for resizing and re-enable the size lookup
                // This stops the columns from constantly resizing to improve performance
                void ResizeAndEnableSize()
                {
                    Resize();
                    _element.SizeChanged += OnSizeChanged;
                }

                _timer = new Timer(x => Application.Current?.Dispatcher?.BeginInvoke((Action)ResizeAndEnableSize), null, Delay,
                    RefreshTime);
            }

            public bool Enabled { get; set; }

            private void OnLoaded(object sender, RoutedEventArgs e)
            {
                _element.SizeChanged += OnSizeChanged;
            }

            private void OnSizeChanged(object sender, SizeChangedEventArgs e)
            {
                if (e.WidthChanged && !_resizeInProgress)
                {
                    _element.SizeChanged -= OnSizeChanged;
                    _timer.Change(Delay, RefreshTime);
                }
            }

            private void Resize()
            {
                if (Enabled)
                {
                    _resizeInProgress = true;

                    try
                    {
                        double totalWidth = _element.ActualWidth;
                        if (_element.View is GridView gv)
                        {
                            double allowedSpace = totalWidth - GetAllocatedSpace(gv);
                            //allowedSpace = allowedSpace - Margin;
                            double totalPercentage = GridViewColumnResizeBehaviors(gv).Sum(x => x.Percentage);
                            foreach (GridViewColumnResizeBehavior behavior in GridViewColumnResizeBehaviors(gv))
                            {
                                behavior.SetWidth(allowedSpace, totalPercentage);
                            }
                            //_element.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                        }

                    }
                    finally
                    {
                        _resizeInProgress = false;
                    }
                }
            }

            private static IEnumerable<GridViewColumnResizeBehavior> GridViewColumnResizeBehaviors(GridView gv)
            {
                foreach (GridViewColumn t in gv.Columns)
                {
                    if (t.GetValue(GridViewColumnResizeBehaviorProperty) is GridViewColumnResizeBehavior gridViewColumnResizeBehavior)
                    {
                        yield return gridViewColumnResizeBehavior;
                    }
                }
            }

            private static double GetAllocatedSpace(GridView gv)
            {
                double totalWidth = 0;
                foreach (GridViewColumn t in gv.Columns)
                {
                    if (t.GetValue(GridViewColumnResizeBehaviorProperty) is GridViewColumnResizeBehavior gridViewColumnResizeBehavior)
                    {
                        if (gridViewColumnResizeBehavior.IsStatic)
                        {
                            totalWidth += gridViewColumnResizeBehavior.StaticWidth;
                        }
                    }
                    else
                    {
                        totalWidth += t.ActualWidth;
                    }
                }
                return totalWidth;
            }
        }
    }
}
