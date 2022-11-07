using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;




namespace JournalIt.View
{


    public partial class FrmJournalIt : Window
    {
        private List<ButtonStopwatch> buttonStopwatches = new List<ButtonStopwatch>();
        private ViewModel.CogBox cogBox;

        private bool isMouseDown = false;

        private static Action EmptyDelegate = delegate() { };


        public void Refresh(UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
        }



        public FrmJournalIt()
        {

            if (Properties.UI.Default.FormWidth != 0)
            {
                this.Width = Properties.UI.Default.FormWidth;
            }

            InitializeComponent();

            this.Loaded += (e, s) =>
                {
                    this.MinHeight = this.ActualHeight;

                    cogBox = new ViewModel.CogBox(this);
                    this.DataContext = cogBox;

                    this.SetPositionAndSize();

                    int i = 0;
                    foreach (ViewModel.Stopwatch stopwatch in cogBox.Stopwatches)
                    {                    
                        i++;
                        ButtonStopwatch btnStopwatch = AddEntryButton(i, stopwatch);
                    }
                };
        }






        private ButtonStopwatch AddEntryButton(int i, 
                                               ViewModel.Stopwatch stopwatch)
        {

            ButtonStopwatch btnStopwatch = new ButtonStopwatch(stopwatch);
            this.ctlWrapPanelStopwatches.Children.Add(btnStopwatch);
            
            btnStopwatch.ContextMenu = this.ContextMenu;
            btnStopwatch.Name = "btnStopwatch" + i;
            
            btnStopwatch.Height = stopwatch.CogBox.Common.UI.StopwatchHeight;
            btnStopwatch.TabIndex = i;
            btnStopwatch.Margin = new Thickness(btnAdd.Margin.Left, 
                                                btnAdd.Margin.Top, 
                                                btnAdd.Margin.Left, 
                                                btnOptions.Margin.Bottom);

            btnStopwatch.Click += new System.Windows.RoutedEventHandler(this.btnStopwatch_Click);

            buttonStopwatches.Add(btnStopwatch);

            TurnOffOtherStopwatches(btnStopwatch);

            return btnStopwatch;
        }



        #region Methods


        public void SetPositionAndSize()
        {
            //Check the window is in the screen bounds and set position
            double widthScreen = System.Windows.SystemParameters.WorkArea.Width;
            double heightScreen = System.Windows.SystemParameters.WorkArea.Height;



            // Load the position
            double left = this.cogBox.Common.UI.JournalIt.Left;
            double top = this.cogBox.Common.UI.JournalIt.Top;



            // Load the saved size
            double width = this.cogBox.Common.UI.JournalIt.Width;
            double height = this.cogBox.Common.UI.JournalIt.Height;

            if (width > 0 || height > 0)
            {
                this.SizeToContent = System.Windows.SizeToContent.Manual;
                this.Width = width;
                this.Height = height;
            }
            //else
            //    this.ShrinkToFit();



            // Check the position
            if (left < 0
            || left + width > widthScreen)
                left = 0;


            if (top < 0
            || top + height > heightScreen)
                top = heightScreen - this.ActualHeight;



            // Set the position
            this.Left = left;
            this.Top = top;
        }


        // Save the positon to the settings
        public void SavePosition()
        {
            this.cogBox.Common.UI.JournalIt.Left = this.Left;
            this.cogBox.Common.UI.JournalIt.Top = this.Top;
            this.cogBox.Common.UI.JournalIt.Save();
        }


        // Save the size to the settings
        public void SaveSize()
        {
            this.cogBox.Common.UI.JournalIt.Width = this.ActualWidth;
            this.cogBox.Common.UI.JournalIt.Height = this.ActualHeight;
            this.cogBox.Common.UI.JournalIt.Save();
        }


        private void SetWrapPanelMax()
        {
            if (isMouseDown)
            {

                double width = 0;


                Point pointWrapPanelStopwatches = this.ctlWrapPanelStopwatches.PointToScreen(new Point(0, 0));

                foreach (FrameworkElement child in this.ctlWrapPanelStopwatches.Children)
                {
                    Point point = child.PointToScreen(new Point(0, 0));

                    Vector vector = point - pointWrapPanelStopwatches;

                    if (width < vector.X + child.RenderSize.Width + child.Margin.Left)
                        width = vector.X + child.RenderSize.Width + child.Margin.Left;
                }

                this.ctlWrapPanelStopwatches.MaxWidth = width;
            }
        }

        // Shrink the UI to fit the controls
        //public void ShrinkToFit()
        //{
        //    double widthPanelNew = 0;


        //    Point pointWrapPanelStopwatches = this.ctlWrapPanelStopwatches.PointToScreen(new Point(0, 0));

        //    this.Refresh(this.ctlWrapPanelStopwatches);
        //    foreach (FrameworkElement child in this.ctlWrapPanelStopwatches.Children)
        //    {
        //        Point point = child.PointToScreen(new Point(0, 0));

        //        Vector vector = point - pointWrapPanelStopwatches;

        //        if (widthPanelNew < vector.X + child.RenderSize.Width + child.Margin.Left)
        //            widthPanelNew = vector.X + child.RenderSize.Width + child.Margin.Left;
        //    }


        //    Point pointForm = this.PointToScreen(new Point(0, 0));


        //    double rightPanel = pointWrapPanelStopwatches.X + this.ctlWrapPanelStopwatches.ActualWidth;
        //    double widthForm = rightPanel - pointForm.X + 14;


        //    double bottomPanel = pointWrapPanelStopwatches.Y + this.ctlWrapPanelStopwatches.ActualHeight;
        //    double heightForm = bottomPanel - pointForm.Y + 14;


        //    this.SizeToContent = System.Windows.SizeToContent.Manual;

        //    this.Width = widthForm;
        //    this.Height = heightForm;

        //    this.ctlWrapPanelStopwatches.MaxWidth = widthPanelNew;
        //}

        //public void ShrinkToFit()
        //{
        //    double widthPanelNew = 0;
        //    double heightPanelNew = 0;


        //    // Get the new panel widith
        //    Point pointWrapPanelStopwatches = this.ctlWrapPanelStopwatches.PointToScreen(new Point(0, 0));

        //    this.Refresh(this.ctlWrapPanelStopwatches);
        //    foreach (FrameworkElement child in this.ctlWrapPanelStopwatches.Children)
        //    {
        //        Point point = child.PointToScreen(new Point(0, 0));
        //        Vector childTopLeft = point - pointWrapPanelStopwatches;

        //        if (widthPanelNew < childTopLeft.X + child.RenderSize.Width + child.Margin.Left + child.Margin.Right)
        //            widthPanelNew = childTopLeft.X + child.RenderSize.Width + child.Margin.Left + child.Margin.Right;

        //        if (heightPanelNew < childTopLeft.Y + child.RenderSize.Height + child.Margin.Top + child.Margin.Bottom)
        //            heightPanelNew = childTopLeft.Y + child.RenderSize.Height + child.Margin.Top + child.Margin.Bottom;

        //    }


        //    double widthForm = widthPanelNew 
        //                     + this.ctlWrapPanelStopwatches.Margin.Left
        //                     + this.ctlWrapPanelStopwatches.Margin.Right
        //                     + btnAdd.ActualWidth 
        //                     + btnAdd.Margin.Left 
        //                     + btnAdd.Margin.Right 
        //                     + this.Padding.Left 
        //                     + this.Padding.Right;

        //    double heightForm = heightPanelNew
        //                      + this.ctlWrapPanelStopwatches.Margin.Top
        //                      + this.ctlWrapPanelStopwatches.Margin.Bottom
        //                      + this.Padding.Left 
        //                      + this.Padding.Right;


        //    this.SizeToContent = System.Windows.SizeToContent.Manual;

        //    this.Width = widthForm;
        //    this.Height = heightForm;

        //    this.ctlWrapPanelStopwatches.MaxWidth = widthPanelNew;
        //}

        public void ShrinkToFit()
        {
            double widthPanelNew = 0;


            Point pointWrapPanelStopwatches = this.ctlWrapPanelStopwatches.PointToScreen(new Point(0, 0));

            this.Refresh(this.ctlWrapPanelStopwatches);
            foreach (FrameworkElement child in this.ctlWrapPanelStopwatches.Children)
            {
                Point point = child.PointToScreen(new Point(0, 0));

                Vector childTopLeft = point - pointWrapPanelStopwatches;

                if (widthPanelNew < childTopLeft.X + child.RenderSize.Width + child.Margin.Right)
                    widthPanelNew = childTopLeft.X + child.RenderSize.Width + child.Margin.Right;
            }


            Point pointForm = this.PointToScreen(new Point(0, 0));


            double rightPanel = pointWrapPanelStopwatches.X 
                              + this.ctlWrapPanelStopwatches.ActualWidth 
                              + this.ctlWrapPanelStopwatches.Margin.Left 
                              + this.ctlWrapPanelStopwatches.Margin.Right;

            double widthForm = widthPanelNew
                             + this.ctlWrapPanelStopwatches.Margin.Left
                             + this.ctlWrapPanelStopwatches.Margin.Right
                             + 14
                             + 14
                             + btnAdd.ActualWidth
                             + btnAdd.Margin.Left
                             + btnAdd.Margin.Right;


            double bottomPanel = pointWrapPanelStopwatches.Y 
                               + this.ctlWrapPanelStopwatches.ActualHeight
                               + this.ctlWrapPanelStopwatches.Margin.Top
                               + this.ctlWrapPanelStopwatches.Margin.Bottom;

            double heightForm = bottomPanel - pointForm.Y + 14;


            this.SizeToContent = System.Windows.SizeToContent.Manual;

            

            this.Width = widthForm;
            this.Height = heightForm;

            this.ctlWrapPanelStopwatches.MaxWidth = widthPanelNew;
        }
        #endregion



        private ButtonStopwatch GetButtonStopwatch(object sender)
        {
            //ContextMenu menuItem = (ContextMenu)sender;
            //return (ButtonStopwatch)menuItem.Parent;
            ButtonStopwatch btn = null;

            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                ContextMenu cm = mi.CommandParameter as ContextMenu;
                if (cm != null)
                {
                    btn = cm.PlacementTarget as ButtonStopwatch;
                }
            }

            return btn;
        }

        private void TurnOffOtherStopwatches(ButtonStopwatch btnCurrent)
        {
            //Turn off the other stopwatches
            foreach (ButtonStopwatch oButton in buttonStopwatches)
            {
                if (oButton != btnCurrent)
                    oButton.IsChecked = false;
            }
        }


        private void btnStopwatch_Click(object sender, EventArgs e)
        {
            ButtonStopwatch btn = (ButtonStopwatch)sender;
            if ((bool)btn.Stopwatch.IsOn)
            {
                cogBox.StopwatchSelected = btn.Stopwatch;
                cogBox.Start();

                TurnOffOtherStopwatches(btn);
            }
            else
            {
                cogBox.Stop();
                cogBox.StopwatchSelected = null;
            }
        }

        private void FrmJournalIt_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ViewModel.Stopwatch.SaveAll();
            this.SaveSize();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
           
            // AddChild a button
            ViewModel.Stopwatch stopwatch = ViewModel.Stopwatch.New(this.cogBox);
            stopwatch.IsOn = true;

            this.AddEntryButton(cogBox.Stopwatches.Count, 
                                stopwatch);



            this.ShrinkToFit();
            this.ctlWrapPanelStopwatches.MaxWidth = double.MaxValue;

        }

        private void frmJournalIt_LocationChanged(object sender, EventArgs e)
        {
            this.SavePosition();
        }

        #region Event - context Menu
        private void mnuSave_Click(object sender, RoutedEventArgs e)
        {
            ButtonStopwatch btn = GetButtonStopwatch(sender);
            btn.Stopwatch.Save();
        }

        private void mnuReset_Click(object sender, RoutedEventArgs e)
        {
            ButtonStopwatch btn = GetButtonStopwatch(sender);


            if (MessageBox.Show("Are you sure you want to reset this stopwatch?",
                Properties.Settings.Default.ProductName,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                btn.Stopwatch.Reset();
            }
        }


        private void mnuDelete_Click(object sender, RoutedEventArgs e)
        {
            ButtonStopwatch btn = GetButtonStopwatch(sender);

            if (MessageBox.Show("Are you sure you want to delete this stopwatch?",
                Properties.Settings.Default.ProductName,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                if (this.cogBox.StopwatchSelected == btn.Stopwatch)
                    this.cogBox.StopwatchSelected = null;



                btn.Stopwatch.Delete();
                this.cogBox.Stopwatches.Remove(btn.Stopwatch);
                buttonStopwatches.Remove(btn);

                this.cogBox.OnPropertyChanged("Title");


                this.ctlWrapPanelStopwatches.Children.Remove(btn);

                this.ShrinkToFit();
            }
        }
        #endregion

        private void mnuExport_Click(object sender, RoutedEventArgs e)
        {
            ButtonStopwatch btn = GetButtonStopwatch(sender);

            if (btn.Stopwatch.Company == null
            || btn.Stopwatch.Project == null
            || btn.Stopwatch.Notes == null)
                mnuEdit_Click(sender, e);

            if (btn.Stopwatch.Export())
            {
                btn.Stopwatch.Notes = "";
                btn.Stopwatch.DateTimeStart = new DateTime();
                btn.Stopwatch.Seconds = 0;
                btn.Stopwatch.Save();
                this.cogBox.OnPropertyChanged("Title");
            }
        }

        private void mnuEdit_Click(object sender, RoutedEventArgs e)
        {
            ButtonStopwatch btn = GetButtonStopwatch(sender);
            cogBox.StopwatchSelected = btn.Stopwatch;
            btn.Stopwatch.OpenDialogue();
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            this.cogBox.OpenOptionsDialogue();
        }





        
        private void frmJournalIt_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.StopOffScreenOverflow();
        }

        private void StopOffScreenOverflow()
        {
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;

            double left = this.Left;
            double width = this.Width;

            if (this.ActualHeight < this.cogBox.Common.UI.StopwatchHeight*2)
            {
                // We only have one line.
                double rightNew = left + width + this.cogBox.Common.UI.StopwatchWidth;

                if( screenWidth < rightNew )
                    this.ctlWrapPanelStopwatches.MaxWidth = this.ActualWidth;
            }
            else
            {
                // We have more than one line.
                this.ctlWrapPanelStopwatches.MaxWidth = this.ActualWidth;
            }
        }








        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            this.SetWrapPanelMax();
        }

        private void frmJournalIt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = true;
        }
        
        private void frmJournalIt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = false;
            this.ShrinkToFit();
        }
    }
}
