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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace LHQ.App.Extensions
{
    public static class VisualTreeHelperExtensions
    {
        public static void RemoveChild(this DependencyObject parent, UIElement child)
        {
            if (parent is Panel panel)
            {
                panel.Children.Remove(child);
                return;
            }

            if (parent is Decorator decorator)
            {
                if (decorator.Child == child)
                {
                    decorator.Child = null;
                }
                return;
            }

            if (parent is ContentPresenter contentPresenter)
            {
                // ReSharper disable once PossibleUnintendedReferenceComparison
                if (contentPresenter.Content == child)
                {
                    contentPresenter.Content = null;
                }
                return;
            }

            if (parent is ContentControl contentControl)
            {
                // ReSharper disable once PossibleUnintendedReferenceComparison
                if (contentControl.Content == child)
                {
                    contentControl.Content = null;
                }
            }
        }

        public static bool IsElementClickable<T>(this UIElement element, UIElement container, out bool isPartiallyClickable)
        {
            isPartiallyClickable = false;
            Rect pos = element.TransformToAncestor(container)
                .TransformBounds(new Rect(0.0, 0.0, element.RenderSize.Width, element.RenderSize.Height));
            bool isTopLeftClickable = GetIsPointClickable<T>(container, element, new Point(pos.TopLeft.X + 1, pos.TopLeft.Y + 1));
            bool isBottomLeftClickable = GetIsPointClickable<T>(container, element, new Point(pos.BottomLeft.X + 1, pos.BottomLeft.Y - 1));
            bool isTopRightClickable = GetIsPointClickable<T>(container, element, new Point(pos.TopRight.X - 1, pos.TopRight.Y + 1));
            bool isBottomRightClickable = GetIsPointClickable<T>(container, element, new Point(pos.BottomRight.X - 1, pos.BottomRight.Y - 1));

            if (isTopLeftClickable || isBottomLeftClickable || isTopRightClickable || isBottomRightClickable)
            {
                isPartiallyClickable = true;
            }

            return isTopLeftClickable && isBottomLeftClickable && isTopRightClickable && isBottomRightClickable; // return if element is fully click-able
        }

        public static bool GetIsPointClickable<T>(UIElement container, UIElement element, Point p)
        {
            DependencyObject hitTestResult = HitTest<T>(p, container);
            if (null != hitTestResult)
            {
                return IsElementChildOfElement(element, hitTestResult);
            }
            return false;
        }

        public static DependencyObject HitTest<T>(Point p, UIElement container)
        {
            PointHitTestParameters parameter = new PointHitTestParameters(p);
            DependencyObject hitTestResult = null;

            HitTestResultBehavior ResultCallback(HitTestResult result)
            {
                // result can be collapsed! Even though documentation indicates otherwise
                if (result.VisualHit is UIElement elemCandidateResult && elemCandidateResult.Visibility == Visibility.Visible)
                {
                    hitTestResult = result.VisualHit;
                    return HitTestResultBehavior.Stop;
                }

                return HitTestResultBehavior.Continue;
            }

            HitTestFilterBehavior FilterCallBack(DependencyObject potentialHitTestTarget)
            {
                if (potentialHitTestTarget is T)
                {
                    hitTestResult = potentialHitTestTarget;
                    return HitTestFilterBehavior.Stop;
                }

                return HitTestFilterBehavior.Continue;
            }

            VisualTreeHelper.HitTest(container, FilterCallBack, ResultCallback, parameter);
            return hitTestResult;
        }

        public static bool IsElementChildOfElement(DependencyObject child, DependencyObject parent)
        {
            if (child.GetHashCode() == parent.GetHashCode())
                return true;
            IEnumerable<DependencyObject> elemList = FindVisualChildren<DependencyObject>(parent);
            foreach (DependencyObject obj in elemList)
            {
                if (obj.GetHashCode() == child.GetHashCode())
                    return true;
            }
            return false;
        }

        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj)
            where T : DependencyObject
        {
            if (depObj != null)
            {
                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child is T children)
                    {
                        yield return children;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static T FindParent<T>(this DependencyObject parent)
            where T : class
        {
            if (parent == null)
            {
                return null;
            }

            if (parent.GetType() == typeof(T))
            {
                return parent as T;
            }

            return FindParent<T>(VisualTreeHelper.GetParent(parent));
        }

        /// <summary>
        ///     Find the specified child
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T FindChild<T>(this FrameworkElement parent, Func<T, bool> predicate = null)
            where T : class
        {
            if (parent == null)
            {
                return null;
            }

            if (parent.GetType() == typeof(T))
            {
                return parent as T;
            }

            int childCount = VisualTreeHelper.GetChildrenCount(parent);

            for (var i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
                var result = FindChild<T>(child);

                if (result != null)
                {
                    bool isValid = predicate == null || predicate(result);

                    if (isValid)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     Finds a Child of a given item in the visual tree.
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>
        ///     The first parent item that matches the submitted type parameter.
        ///     If not matching item can be found,
        ///     a null parent is being returned.
        /// </returns>
        // code from -http://stackoverflow.com/a/1759923/316886
        public static T FindChildByName<T>(this DependencyObject parent, string childName)
            where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null)
            {
                return null;
            }

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                var childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChildByName<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null)
                    {
                        break;
                    }
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    // If the child's name is set for search
                    if (child is FrameworkElement frameworkElement && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public static Point GetTop(Visual child, Visual root)
        {
            return child.TransformToAncestor(root).Transform(new Point(0, 0));
        }
    }
}
