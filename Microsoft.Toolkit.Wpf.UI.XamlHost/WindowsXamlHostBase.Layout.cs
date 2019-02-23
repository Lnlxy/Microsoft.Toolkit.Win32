// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using windows = Windows;
using WUX = Windows.UI.Xaml;

namespace Microsoft.Toolkit.Wpf.UI.XamlHost
{
    /// <summary>
    /// Integrates UWP XAML in to WPF's layout system
    /// </summary>
    partial class WindowsXamlHostBase
    {
        /// <summary>
        /// Measures wrapped UWP XAML content using passed in size constraint
        /// </summary>
        /// <param name="constraint">Available Size</param>
        /// <returns>XAML DesiredSize</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            var desiredSize = new Size(0, 0);

            if (IsXamlContentLoaded())
            {
                _xamlSource.Content.Measure(new windows.Foundation.Size(constraint.Width, constraint.Height));
                desiredSize.Width = _xamlSource.Content.DesiredSize.Width;
                desiredSize.Height = _xamlSource.Content.DesiredSize.Height;
            }

            desiredSize.Width = Math.Min(desiredSize.Width, constraint.Width);
            desiredSize.Height = Math.Min(desiredSize.Height, constraint.Height);

            return desiredSize;
        }

        /// <summary>
        /// Arranges wrapped UWP XAML content using passed in size constraint
        /// </summary>
        /// <param name="finalSize">Final Size</param>
        /// <returns>Size</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (IsXamlContentLoaded())
            {
                // Arrange is required to support HorizontalAlignment and VerticalAlignment properties
                // set to 'Stretch'.  The UWP XAML content will be 0 in the stretch alignment direction
                // until Arrange is called, and the UWP XAML content is expanded to fill the available space.
                var finalRect = new windows.Foundation.Rect(0, 0, finalSize.Width, finalSize.Height);
                _xamlSource.Content.Arrange(finalRect);
            }

            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Is the Xaml Content loaded and live?
        /// </summary>
        /// <returns>True if the Xaml content is properly loaded</returns>
        private bool IsXamlContentLoaded()
        {
            if (_xamlSource.Content == null)
            {
                return false;
            }

            if (WUX.Media.VisualTreeHelper.GetParent(_xamlSource.Content) == null)
            {
                // If there's no parent to this content, it's not "live" or "loaded" in the tree yet.
                // Performing a measure or arrange in this state may cause unexpected results.
                return false;
            }

            return true;
        }

        /// <summary>
        /// UWP XAML content size changed
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="windows.UI.Xaml.SizeChangedEventArgs"/> instance containing the event data.</param>
        protected void XamlContentSizeChanged(object sender, windows.UI.Xaml.SizeChangedEventArgs e)
        {
            InvalidateMeasure();
        }
    }
}
