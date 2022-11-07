using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Common.Utilities;


namespace Common.WindowStyle
{
    public partial class WindowCustomBorderStyle : ResourceDictionary
    {
    public WindowCustomBorderStyle()
    {
        InitializeComponent();

        // Get Operating system information.
        var os = Environment.OSVersion;

        // Get version information about the os.
        var vs = os.Version;

        // Load the OS dependent styles
        var osDependantResources = new ResourceDictionary
        {
            Source = vs.Major <= 5 ? 
                new Uri( "/View/Themes/WindowStyleXP.xaml", UriKind.Relative ) : 
                new Uri( "/View/Themes/WindowStyleWin7.xaml", UriKind.Relative )
        };

        MergedDictionaries.Add( osDependantResources );
        
    }




    /// <summary>
    /// Handles the MouseLeftButtonDown event. This event handler is used here to facilitate
    /// dragging of the Window.
    /// </summary>
    private void MoveWindow(object sender, MouseButtonEventArgs e)
    {
        var window = (Window)((FrameworkElement)sender).TemplatedParent;

        // Check if the control have been double clicked.
        if ( e.ClickCount == 2 )
        {
            // If double clicked then maximize the window.
            MaximizeWindow( sender, e );
        }
        else
        {
            // If not double clicked then just drag the window around.
            window.DragMove();
        }
    }

    /// <summary>
    /// Fires when the user clicks the Close button on the window's custom title bar.
    /// </summary>
    private void CloseWindow(object sender, RoutedEventArgs e)
    {
        var window = (Window)((FrameworkElement)sender).TemplatedParent;
        window.Close();
    }

    /// <summary>
    /// Fires when the user clicks the minimize button on the window's custom title bar.
    /// </summary>
    private void MinimizeWindow(object sender, RoutedEventArgs e)
    {
        var window = (Window)((FrameworkElement)sender).TemplatedParent;
        window.WindowState = WindowState.Minimized;
    }

    /// <summary>
    /// Fires when the user clicks the maximize button on the window's custom title bar.
    /// </summary>
    private void MaximizeWindow(object sender, RoutedEventArgs e)
    {
        var window = (Window)((FrameworkElement)sender).TemplatedParent;

        // Check the current state of the window. If the window is currently maximized, return the
        // window to it's normal state when the maximize button is clicked, otherwise maximize the window.
        if ( window.WindowState == WindowState.Maximized )
        {
            window.WindowState = WindowState.Normal;
        }
        else
        {
            window.Focus();
            window.WindowState = WindowState.Maximized; 
        }
    }

    /// <summary>
    /// Called when the window gets resized.
    /// </summary>
    private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var window = (Window)((FrameworkElement)sender).TemplatedParent;

        // Update window's contraints like max height and width.
        UpdateWindowConstraints( window );

        // Get window sub parts
        var icon = (Image)window.Template.FindName( "IconApp", window );
        var windowRoot = (Grid)window.Template.FindName( "WindowRoot", window );
        var windowFrame = (Border)window.Template.FindName( "WindowFrame", window );
        var windowLayout = (Grid)window.Template.FindName( "WindowLayout", window );

        // Adjust the window icon size
        if (icon == null)
            return;

        if (window.WindowState != WindowState.Maximized)
        {
            icon.Height = 24;
            icon.Width = 24;
            icon.Margin = new Thickness(10, 3, 0, 0);
        }
        else
        {
            icon.Height = 20;
            icon.Width = 20;
            icon.Margin = new Thickness(10, 5, 0, 0);
        }
    }

    /// <summary>
    /// Called when a window gets loaded.
    /// We initialize resizers and update constraints.
    /// </summary>
    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        var window = (Window)((FrameworkElement)sender).TemplatedParent;

        // Update constraints.
        UpdateWindowConstraints(window);

        // Attach resizer
        WindowResizer wr = new WindowResizer( window );
        wr.AddResizerRight( (Rectangle)window.Template.FindName( "rightSizeGrip", window ) );
        wr.AddResizerLeft( (Rectangle)window.Template.FindName( "leftSizeGrip", window ) );
        wr.AddResizerUp( (Rectangle)window.Template.FindName( "topSizeGrip", window ) );
        wr.AddResizerDown( (Rectangle)window.Template.FindName( "bottomSizeGrip", window ) );
        wr.AddResizerLeftUp( (Rectangle)window.Template.FindName( "topLeftSizeGrip", window ) );
        wr.AddResizerRightUp( (Rectangle)window.Template.FindName( "topRightSizeGrip", window ) );
        wr.AddResizerLeftDown( (Rectangle)window.Template.FindName( "bottomLeftSizeGrip", window ) );
        wr.AddResizerRightDown( (Rectangle)window.Template.FindName( "bottomRightSizeGrip", window ) );


    }


    /// <summary>
    /// Called when the user drags the title bar when maximized.
    /// </summary>
    private void OnBorderMouseMove(object sender, MouseEventArgs e)
    {
        var window = (Window)((FrameworkElement)sender).TemplatedParent;

        if (window == null)
            return;

        if (e.LeftButton != MouseButtonState.Pressed || window.WindowState != WindowState.Maximized)
            return;


        var maxSize = new Size( window.ActualWidth, window.ActualHeight );
        var resSize = window.RestoreBounds.Size;

        var curX = e.GetPosition( window ).X;
        var curY = e.GetPosition( window ).Y;

        var newX = curX / maxSize.Width * resSize.Width;
        var newY = curY;

        window.WindowState = WindowState.Normal;

        window.Left = curX - newX;
        window.Top = curY - newY;
        window.DragMove();
    }

    /// <summary>
    /// Updates the window constraints based on its state.
    /// For instance, the max width and height of the window is set to prevent overlapping over the taskbar.
    /// </summary>
    /// <param name="window">Window to set properties</param>
    private static void UpdateWindowConstraints(Window window)
    {
        if (window == null)
            return;

        // Make sure we don't bump the max width and height of the desktop when maximized
        var borderWidth = (GridLength)window.FindResource( "BorderWidth" );
        window.MaxHeight = SystemParameters.WorkArea.Height + borderWidth.Value * 2;
        window.MaxWidth = SystemParameters.WorkArea.Width + borderWidth.Value * 2;
    }

    private void OnThemeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var themeXamlFileName = @"";

        // Get combo box item tag name
        if (e.AddedItems.Count <= 0)
            return;

        var selectedItem = (ComboBoxItem)e.AddedItems[0];
        themeXamlFileName = (string)selectedItem.Tag;
        var skin = new ResourceDictionary
        {
            Source = new Uri($@"View\Themes\Skins\{themeXamlFileName}.xaml", UriKind.Relative )
        };

        Application.Current.Resources.MergedDictionaries[0] = skin;
    }

    }
}