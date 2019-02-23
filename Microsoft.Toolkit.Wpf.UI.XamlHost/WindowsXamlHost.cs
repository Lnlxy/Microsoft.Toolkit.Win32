// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.Toolkit.Win32.UI.XamlHost;
using WUX = Windows.UI.Xaml;

namespace Microsoft.Toolkit.Wpf.UI.XamlHost
{
    /// <summary>
    /// WindowsXamlHost control hosts UWP XAML content inside the Windows Presentation Foundation
    /// </summary>
    public class WindowsXamlHost : WindowsXamlHostBase
    {
        /// <summary>
        /// Gets XAML Content by type name
        /// </summary>
        public static DependencyProperty InitialTypeNameProperty { get; } = DependencyProperty.Register("InitialTypeName", typeof(string), typeof(WindowsXamlHost));

        /// <summary>
        /// Gets or sets XAML Content by type name
        /// </summary>
        /// <example><code>XamlClassLibrary.MyUserControl</code></example>
        /// <remarks>
        /// Content creation is deferred until after the parent hwnd has been created.
        /// </remarks>
        [Browsable(true)]
        [Category("XAML")]
        public string InitialTypeName
        {
            get => (string)GetValue(InitialTypeNameProperty);

            set => SetValue(InitialTypeNameProperty, value);
        }

        /// <summary>
        /// Gets or sets the root UWP XAML element displayed in the WPF control instance.
        /// </summary>
        /// <remarks>This UWP XAML element is the root element of the wrapped DesktopWindowXamlSource.</remarks>
        [Browsable(true)]
        public WUX.UIElement Child
        {
            get => ChildInternal;

            set => ChildInternal = value;
        }

        /// <summary>
        /// Creates <see cref="WUX.Application" /> object, wrapped <see cref="WUX.Hosting.DesktopWindowXamlSource" /> instance; creates and
        /// sets root UWP XAML element on DesktopWindowXamlSource.
        /// </summary>
        /// <param name="hwndParent">Parent window handle</param>
        /// <returns>Handle to XAML window</returns>
        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            // Create and set initial root UWP XAML content
            if (!string.IsNullOrEmpty(InitialTypeName) && Child == null)
            {
                Child = UWPTypeFactory.CreateXamlContentByType(InitialTypeName);

                var frameworkElement = Child as WUX.FrameworkElement;

                // Default to stretch : UWP XAML content will conform to the size of WindowsXamlHost
                if (frameworkElement != null)
                {
                    frameworkElement.HorizontalAlignment = WUX.HorizontalAlignment.Stretch;
                    frameworkElement.VerticalAlignment = WUX.VerticalAlignment.Stretch;
                }
            }

            return base.BuildWindowCore(hwndParent);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                if (Child is WUX.FrameworkElement frameworkElement)
                {
                    frameworkElement.SizeChanged -= XamlContentSizeChanged;
                }

                base.Dispose(disposing);
            }
        }
    }
}
