using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace JournalIt.View
{
    using System.Windows.Media.Animation;

    public partial class FrmJournalIt : Window
    {
        private readonly List<ButtonStopwatch> _buttonStopwatches = new List<ButtonStopwatch>();
        private ViewModel.CogBox _cogBox;

        private bool _isMouseDown;

        private static readonly Action EmptyDelegate = delegate { };

        // Used to animate the message box
        public DoubleAnimation FadeOutAnimation = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(1)));
        //public DoubleAnimation FadeInAnimation = new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(200)));



        public void Refresh(UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
        }



        public FrmJournalIt()
        {
           // ConsoleManager.Show();

            FadeOutAnimation.Completed += fadeOutAnimation_Completed;


            if (Properties.UI.Default.FormWidth != 0)
            {
                Width = Properties.UI.Default.FormWidth;
            }

            InitializeComponent();

            Loaded += (e, s) =>
                {
                    try
                    {
                        MinHeight = ActualHeight;

                        _cogBox = new ViewModel.CogBox(this);
                        DataContext = _cogBox;


                        _cogBox.Common.UI.PropertyChanged += UI_PropertyChanged;


                        SetPositionAndSize();

                        var i = 0;
                        foreach (var stopwatch in _cogBox.Stopwatches)
                        {
                            i++;
                            var btnStopwatch = AddEntryButton(i, stopwatch);
                        }

                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message, 
                                        _cogBox.Common.UI.ProductName);
                    }
                };
        }

        private void ctlWrapPanelStopwatches_CollectionOrderChanged(object sender, EventArgs e)
        {
            var stopwatches = new List<ViewModel.Stopwatch>();

            foreach (FrameworkElement child in ctlWrapPanelStopwatches.Children)
                if (child is ButtonStopwatch buttonStopwatch)
                    stopwatches.Add(buttonStopwatch.Stopwatch);


            _cogBox.SaveAll(stopwatches);
        }




        private ButtonStopwatch AddEntryButton(int i, 
                                               ViewModel.Stopwatch stopwatch)
        {

            var btnStopwatch = new ButtonStopwatch(stopwatch);
            ctlWrapPanelStopwatches.Children.Add(btnStopwatch);
            
            btnStopwatch.ContextMenu = ContextMenu;
            btnStopwatch.Name = "btnStopwatch" + i;
            
            btnStopwatch.Height = stopwatch.CogBox.Common.UI.StopwatchHeight;
            btnStopwatch.TabIndex = i;
            btnStopwatch.Margin = new Thickness(btnAdd.Margin.Left, 
                                                btnAdd.Margin.Top, 
                                                btnAdd.Margin.Left, 
                                                btnOptions.Margin.Bottom);

            btnStopwatch.ContextMenu = (ContextMenu)FindResource("contractMenu");

            btnStopwatch.Click += btnStopwatch_Click;

            _buttonStopwatches.Add(btnStopwatch);

            TurnOffOtherStopwatches(btnStopwatch);

            return btnStopwatch;
        }
        #region Methods


        public void SetPositionAndSize()
        {
            //Check the window is in the screen bounds and set position
            var widthScreen = SystemParameters.WorkArea.Width;
            var heightScreen = SystemParameters.WorkArea.Height;



            // Load the position
            var left = _cogBox.Common.UI.JournalIt.Left;
            var top = _cogBox.Common.UI.JournalIt.Top;



            // Load the saved size
            var width = _cogBox.Common.UI.JournalIt.Width;
            var height = _cogBox.Common.UI.JournalIt.Height;


            if (width > 0 || height > 0)
            {
                SizeToContent = SizeToContent.Manual;
                Width = width;
                Height = height;
            }
            //else
            //    ShrinkToFit();



            // Check the position
            if (left < 0
            || left + width > widthScreen)
                left = 0;


            //if (top < 0
            //|| top + height > heightScreen)
            //    top = heightScreen - ActualHeight;

            if (top < 0
            || top + height > heightScreen)
                top = 0;


            // Set the position
            Left = left;
            Top = top;
        }


        // Save the positon to the settings
        public void SavePosition()
        {
            _cogBox.Common.UI.JournalIt.Left = Left;
            _cogBox.Common.UI.JournalIt.Top = Top;
            _cogBox.Common.UI.JournalIt.Save();
        }


        // Save the size to the settings
        public void SaveSize()
        {
            _cogBox.Common.UI.JournalIt.Width = ActualWidth;
            _cogBox.Common.UI.JournalIt.Height = ActualHeight;
            _cogBox.Common.UI.JournalIt.Save();
        }


        private void SetWrapPanelMax()
        {
            if (!_isMouseDown)
                return;

            double width = 0;


            var pointWrapPanelStopwatches = ctlWrapPanelStopwatches.PointToScreen(new Point(0, 0));

            foreach (FrameworkElement child in ctlWrapPanelStopwatches.Children)
            {
                var point = child.PointToScreen(new Point(0, 0));

                var vector = point - pointWrapPanelStopwatches;

                if (width < vector.X + child.RenderSize.Width + child.Margin.Left)
                    width = vector.X + child.RenderSize.Width + child.Margin.Left;
            }

            ctlWrapPanelStopwatches.MaxWidth = width;
        }

       




        public void ShrinkToFit()
        {
            // UI
            const double marginRight = 14;
            const double marginBottom = 14;



            // Refresh the controls
            Refresh(ctlWrapPanelStopwatches);

            var scale = _cogBox.Common.UI.Scale+0.031;

            // Calculate the form width
            double widthNew = 0;
            foreach (FrameworkElement child in ctlWrapPanelStopwatches.Children)
            {
                var point = child.TransformToAncestor(this).Transform(new Point(0,0));

                if (widthNew < point.X + Math.Ceiling(child.RenderSize.Width * scale) + child.Margin.Right)
                    widthNew = point.X + Math.Ceiling(child.RenderSize.Width * scale) + child.Margin.Right;
            }

            widthNew += marginRight;




            // Calculate the form height
            var pointWrapPanel = ctlWrapPanelStopwatches.TransformToAncestor(this).Transform(new Point(0, 0));
            var heightNew = pointWrapPanel.Y
                            + ctlWrapPanelStopwatches.ActualHeight * scale
                            + ctlWrapPanelStopwatches.Margin.Top
                            + ctlWrapPanelStopwatches.Margin.Bottom
                            + marginBottom;



            // Apply the values
            SizeToContent = SizeToContent.Manual;

            Width = widthNew;
            Height = heightNew;

            ctlWrapPanelStopwatches.MaxWidth = widthNew;
        }

        //public void FadeInMessage()
        //{
        //    if (ctrlMessage.Visibility == System.Windows.Visibility.Collapsed)
        //    {
        //        ctrlMessage.Visibility = System.Windows.Visibility.Visible;

        //        ctrlMessage.BeginAnimation(CtrlMessage.OpacityProperty, (AnimationTimeline)FadeInAnimation);
        //        frmJournalIt.FadeInAnimation.BeginTime = TimeSpan.FromSeconds(0);
        //    }
        //}

        public void FadeOutMessage()
        {
            if (ctrlMessage.Visibility == Visibility.Visible)
            {
                ctrlMessage.BeginAnimation(CtrlMessage.OpacityProperty, FadeOutAnimation);
                frmJournalIt.FadeOutAnimation.BeginTime = TimeSpan.FromSeconds(1);
            }
        }

        public void FadeOutMessageCancel()
        {
            FadeOutAnimation.FillBehavior = FillBehavior.Stop;
            FadeOutAnimation.BeginTime = null;
            ctrlMessage.Visibility = Visibility.Visible;
            ctrlMessage.Opacity = 1;
            Refresh(ctrlMessage);
        }

        void fadeOutAnimation_Completed(object sender, EventArgs e)
        {
            ctrlMessage.Visibility = Visibility.Collapsed;
            ctrlMessage.Opacity = 1;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            FadeOutMessage();
        }

        #endregion



        private ButtonStopwatch GetButtonStopwatch(object sender)
        {
            //ContextMenu menuItem = (ContextMenu)sender;
            //return (ButtonStopwatch)menuItem.Parent;
            ButtonStopwatch btn = null;

            var mi = sender as MenuItem;

            if (mi == null)
                return null;

            var cm = mi.CommandParameter as ContextMenu;
            if (cm != null)
            {
                btn = cm.PlacementTarget as ButtonStopwatch;
            }
            

            return btn;
        }

        private void TurnOffOtherStopwatches(ButtonStopwatch btnCurrent)
        {
            //Turn off the other stopwatches
            foreach (var oButton in _buttonStopwatches.Where(x => x != btnCurrent))
                oButton.IsChecked = false;
        }


        private void btnStopwatch_Click(object sender, 
                                        EventArgs e)
        {
            var btn = (ButtonStopwatch)sender;
            if (btn.Stopwatch.IsOn)
            {
                _cogBox.StopwatchSelected = btn.Stopwatch;
                _cogBox.Start();

                TurnOffOtherStopwatches(btn);
            }
            else
            {
                _cogBox.Stop();
                _cogBox.StopwatchSelected = null;
            }
        }

        private void FrmJournalIt_Closing(object sender, 
                                          System.ComponentModel.CancelEventArgs e)
        {
            ViewModel.Stopwatch.SaveAll();
            SaveSize();
        }

        private void btnAdd_Click(object sender, 
                                  RoutedEventArgs e)
        {
           
            // AddChild a button
            var stopwatch = ViewModel.Stopwatch.New(_cogBox);
            stopwatch.IsOn = true;

            AddEntryButton(_cogBox.Stopwatches.Count, 
                                stopwatch);



            ShrinkToFit();
            ctlWrapPanelStopwatches.MaxWidth = double.MaxValue;

        }

        private void frmJournalIt_LocationChanged(object sender, 
                                                  EventArgs e)
        {
            SavePosition();
        }

        #region Event - context Menu
        private void mnuSave_Click(object sender, RoutedEventArgs e)
        {
            var btn = GetButtonStopwatch(sender);
            btn.Stopwatch.Save();
        }

        private void mnuReset_Click(object sender, RoutedEventArgs e)
        {
            var btn = GetButtonStopwatch(sender);


            if (MessageBox.Show("Are you sure you want to reset this stopwatch?",
                _cogBox.Common.UI.ProductName,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                btn.Stopwatch.Reset();
            }
        }


        private void mnuDelete_Click(object sender, RoutedEventArgs e)
        {
            var btn = GetButtonStopwatch(sender);

            if (MessageBox.Show("Are you sure you want to delete this stopwatch?",
                    _cogBox.Common.UI.ProductName,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.No) != MessageBoxResult.Yes)
                return;


            if (_cogBox.StopwatchSelected == btn.Stopwatch)
                _cogBox.StopwatchSelected = null;



            btn.Stopwatch.Delete();
            _cogBox.Stopwatches.Remove(btn.Stopwatch);
            _buttonStopwatches.Remove(btn);

            _cogBox.OnPropertyChanged("Title");


            ctlWrapPanelStopwatches.Children.Remove(btn);

            ShrinkToFit();
        }
        #endregion

        private void mnuExport_Click(object sender, 
                                     RoutedEventArgs e)
        {
            var btn = GetButtonStopwatch(sender);

            Export(btn);
        }

        private void btnExportAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ViewModel.Stopwatch.ExportAll())
                    return;


                foreach (var stopwatch in ViewModel.Stopwatch.Stopwatches)
                {
                    if (stopwatch.CanExport())
                        stopwatch.Reset();
                    else
                        Edit(stopwatch);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                                _cogBox.Common.UI.ProductName);
            }
        }

        private void Export(ButtonStopwatch btn)
        {
            if (btn.Stopwatch.CanExport())
            {
                try
                {
                    if (btn.Stopwatch.Export())
                        btn.Stopwatch.Reset();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,
                                    _cogBox.Common.UI.ProductName);
                }
            }
            else
                Edit(btn.Stopwatch);
        }

        private void mnuEdit_Click(object sender, 
                                   RoutedEventArgs e)
        {
            var btn = GetButtonStopwatch(sender);
            Edit(btn.Stopwatch);
        }

        private void Edit(ViewModel.Stopwatch stopwatch)
        {
            _cogBox.StopwatchSelected = stopwatch;
            stopwatch.OpenDialogue();
        }

        private void btnOptions_Click(object sender, 
                                      RoutedEventArgs e)
        {
            _cogBox.OpenOptionsDialogue();
        }





        
        private void frmJournalIt_PreviewMouseLeftButtonDown(object sender, 
                                                             MouseButtonEventArgs e)
        {
            StopOffScreenOverflow();
        }

        private void StopOffScreenOverflow()
        {
            var screenWidth = SystemParameters.PrimaryScreenWidth;

            var left = Left;
            var width = Width;

            if (ActualHeight < _cogBox.Common.UI.StopwatchHeight*2)
            {
                // We only have one line.
                var rightNew = left + width + _cogBox.Common.UI.StopwatchWidth;

                if( screenWidth < rightNew )
                    ctlWrapPanelStopwatches.MaxWidth = ActualWidth;
            }
            else
            {
                // We have more than one line.
                ctlWrapPanelStopwatches.MaxWidth = ActualWidth;
            }
        }








        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            SetWrapPanelMax();
        }

        private void frmJournalIt_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = true;
        }
        
        private void frmJournalIt_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = false;
            ShrinkToFit();
        }

        private void UI_PropertyChanged(object sender,
                                    System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Scale" || e.PropertyName == "StopwatchWidth")
                ShrinkToFit();
        }




    }
}
