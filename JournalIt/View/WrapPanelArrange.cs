using System;


namespace JournalIt.View
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class WrapPanelArrange : Panel
    {
        private readonly Border _placeHolder = new Border();
        private int _placeHolderIndex;

        private FrameworkElement _elementCaptured;
        private Point _positionElementStart;

        private Point _mousePosition;
        private Point _mouseDelta;
        private bool _isDragging = false;


        public event EventHandler CollectionOrderChanged;

        // This is used by the render element method.
        private static Action _emptyDelegate = delegate() { };

        public WrapPanelArrange()
            : base()
        {
            // orientation = (Orientation)OrientationProperty.GetDefaultValue(DependencyObjectType);


            _placeHolder.Background = new SolidColorBrush(Color.FromArgb(45, 0, 0, 0));
            _placeHolder.Width = 50;
            _placeHolder.Height = 50;
            _placeHolder.Visibility = Visibility.Collapsed;

            // Wire up the event method handlers.
            PreviewMouseLeftButtonDown += WrapPanelArrange_PreviewMouseLeftButtonDown;
            PreviewMouseLeftButtonUp += WrapPanelArrange_PreviewMouseLeftButtonUp;
            PreviewMouseMove += WrapPanelArrange_PreviewMouseMove;
            MouseLeave += WrapPanelArrange_MouseLeave;
        }


        #region Event Handlers
        void WrapPanelArrange_PreviewMouseLeftButtonDown(object sender,
                                                         System.Windows.Input.MouseButtonEventArgs e)
        {
            _elementCaptured = GetElement(e.Source as FrameworkElement);
            _positionElementStart = _elementCaptured.TranslatePoint(new Point(),
                                                                            this);

            _positionElementStart.X -= _elementCaptured.Margin.Left;
            _positionElementStart.Y -= _elementCaptured.Margin.Top;


            // Get the initial mouse position
            _mousePosition = e.MouseDevice.GetPosition(this);
            e.Handled = true;
        }

        private FrameworkElement GetElement(FrameworkElement frameworkElement)
        {
            return frameworkElement.Parent == this ? 
                frameworkElement : 
                GetElement(frameworkElement.Parent as FrameworkElement);
        }

        void WrapPanelArrange_PreviewMouseMove(object sender,
                                               System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton != System.Windows.Input.MouseButtonState.Pressed)
                return;

            if (_elementCaptured == null)
                return;

            // Get the position of the mouse relative to the mouse down position.
            _mouseDelta.X = e.MouseDevice.GetPosition(this).X - _mousePosition.X;
            _mouseDelta.Y = e.MouseDevice.GetPosition(this).Y - _mousePosition.Y;

            if (!(_mouseDelta.X > SystemParameters.MinimumHorizontalDragDistance) &&
                !(_mouseDelta.X < -SystemParameters.MinimumHorizontalDragDistance) &&
                !(_mouseDelta.Y > SystemParameters.MinimumVerticalDragDistance) &&
                !(_mouseDelta.Y < -SystemParameters.MinimumVerticalDragDistance) && !_isDragging)
                return;

            if (!_isDragging)
            {
                // We have just started dragging
                _isDragging = true;

                _elementCaptured.CaptureMouse();

                // Get the start position of the selected UI Element
                Point point = new Point(_elementCaptured.Margin.Left,
                    _elementCaptured.Margin.Top);

                _elementCaptured.TranslatePoint(point,
                    this);

                ShowPlaceHolder();
            }

            InvalidateVisual();
        }


        private void WrapPanelArrange_MouseLeave(object sender,
                                                 System.Windows.Input.MouseEventArgs e)
        {
            if (!_isDragging)
                return;

            HidePlaseHolder();
            _isDragging = false;
        }

        void WrapPanelArrange_PreviewMouseLeftButtonUp(object sender,
                                                       System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                // End of drag

                HidePlaseHolder();
                _isDragging = false;

                // Raise a rearranged event.
                if (CollectionOrderChanged != null)
                    CollectionOrderChanged(this, null);
            }
            else
            {
                // Run a click event
                //Console.WriteLine("Click event");

                var element = _elementCaptured as UIElement;

                switch (_elementCaptured.DependencyObjectType.BaseType.Name)
                {
                    case "ToggleButton":
                
                        try
                        {
                            var tb = element as System.Windows.Controls.Primitives.ToggleButton;
                            if (tb != null)
                            {
                                tb.IsChecked = !tb.IsChecked;
                                tb.RaiseEvent(
                                    new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent,
                                        element));

                                //tb.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render,
                                //                     EmptyDelegate);

                                tb.UpdateLayout();
                            }

                            ////ForceUIToUpdate();

                            //ViewModel.Stopwatch stopwatch = tb.DataContext as ViewModel.Stopwatch;
                            //stopwatch.OnPropertyChanged("Caption");
                            //stopwatch.OnPropertyChanged("IsOn");

                            //tb.InvalidateArrange();
                        }
                        catch
                        {
                            // ignored
                        }

                        break;

                    case "Button":

                        try
                        {
                            //element.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                            element.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent, element));
                        }
                        catch
                        {
                            // ignored
                        }

                        break;
                }

            }
        }

        //public static void ForceUIToUpdate()
        //{
        //    System.Windows.Threading.DispatcherFrame frame = new System.Windows.Threading.DispatcherFrame();
        //    System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, 
        //                                                                 new System.Windows.Threading.DispatcherOperationCallback(delegate(object parameter)
        //                                                                     {
        //                                                                         frame.Continue =false;
        //                                                                         return null;
        //                                                                     }), null);
        //    System.Windows.Threading.Dispatcher.PushFrame(frame);
        //}
        #endregion


        #region Orientation Property
        //public static readonly DependencyProperty OrientationProperty =
        //StackPanel.OrientationProperty.AddOwner(
        //        typeof(WrapPanel),
        //        new FrameworkPropertyMetadata(
        //                Orientation.Horizontal,
        //                FrameworkPropertyMetadataOptions.AffectsMeasure,
        //                new PropertyChangedCallback(OnOrientationChanged)));

        //public Orientation Orientation
        //{
        //    get { return orientation; }
        //    set { SetValue(OrientationProperty, value); }
        //}

        //private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    WrapPanelArrange p = (WrapPanelArrange)d;
        //    p.orientation = (Orientation)e.NewValue;
        //}

        //private Orientation orientation;
        #endregion

        #region Methods
        private void ShowPlaceHolder()
        {
            if (_placeHolder.Visibility != Visibility.Collapsed || !_isDragging)
                return;

            if (_elementCaptured == null)
                return;

            // Mimic the properties of the captured element.
            _placeHolder.Width = _elementCaptured.RenderSize.Width;
            _placeHolder.Height = _elementCaptured.RenderSize.Height;
            _placeHolder.Margin = _elementCaptured.Margin;
            _placeHolder.VerticalAlignment = _elementCaptured.VerticalAlignment;
            _placeHolder.HorizontalAlignment = _elementCaptured.HorizontalAlignment;


            // Place into the place holder into the collection 
            // and place the caputured element at the end of the collection
            _placeHolderIndex = InternalChildren.IndexOf(_elementCaptured);
            if (_placeHolderIndex <= -1)
                return;

            InternalChildren.Insert(_placeHolderIndex, _placeHolder);
            InternalChildren.Remove(_elementCaptured);
            InternalChildren.Add(_elementCaptured);
            _placeHolder.Visibility = Visibility.Visible;

            _elementCaptured.SetValue(ZIndexProperty, 1);
        }





        private void HidePlaseHolder()
        {
            if (_placeHolder.Visibility != Visibility.Visible || !_isDragging)
                return;

            var position = _placeHolder.TranslatePoint(new Point(0, 0),
                this);

            _elementCaptured.Arrange(new Rect(position.X,
                position.Y,
                _elementCaptured.RenderSize.Width,
                _elementCaptured.RenderSize.Height));

            // Place into the collection
            var index = InternalChildren.IndexOf(_placeHolder);
            InternalChildren.Remove(_elementCaptured);
            InternalChildren.Insert(index, _elementCaptured);
            _elementCaptured.SetValue(ZIndexProperty, 0);
            _elementCaptured.ReleaseMouseCapture();
            _elementCaptured = null;

            InternalChildren.Remove(_placeHolder);

            _placeHolder.Visibility = Visibility.Collapsed;
        }



        protected override Size MeasureOverride(Size constraint)
        {
            // Place the cildren on the panel
            foreach (UIElement child in InternalChildren)
                child.Measure(constraint);

            return MeasureArrange(constraint,
                                  false);
        }



        protected override Size ArrangeOverride(Size constraint)
        {
            return MeasureArrange(constraint,
                                  true);
        }


        private Size MeasureArrange(Size constraint,
                                    bool positionElements)
        {
            var positionNextChild = new Point();
            double rowHeight = 0;

            var panelSize = new Size();

            foreach (FrameworkElement child in InternalChildren)
            {
                #region Fill vertical then horizontal
                /* This will fill columns then rows.
                 * 
                 * --------------
                 * |   1  |  3  |
                 * --------------
                 * |  2   |  4  |
                 * --------------
                 * 
                 */


                // Set the position
                if (rowHeight + child.DesiredSize.Height > constraint.Height)
                {
                    // We cannot fit another child to the bottom of the column.
                    // Set the new column.
                    positionNextChild.X = panelSize.Width;
                    positionNextChild.Y = 0;
                }
                else
                {
                    // Set the height of the next row
                    positionNextChild.Y = rowHeight;
                }




                // Position the child
                if (positionElements)
                    if (child != _elementCaptured)
                    {
                        // Note: Animation could go here.
                        child.Arrange(new Rect(positionNextChild,
                                               child.DesiredSize));

                        if (child == _placeHolder)
                            _placeHolderIndex = InternalChildren.IndexOf(child);
                    }

                rowHeight = positionNextChild.Y + child.DesiredSize.Height;
                #endregion


                // Set the panel size
                if (panelSize.Width < positionNextChild.X + child.DesiredSize.Width)
                    panelSize.Width = positionNextChild.X + child.DesiredSize.Width;

                if (panelSize.Height < positionNextChild.Y + child.DesiredSize.Height)
                    panelSize.Height = positionNextChild.Y + child.DesiredSize.Height;
            }




            // Dragging code
            if (!_isDragging)
                return panelSize;


            if (_elementCaptured == null)
                return panelSize;
            
            //Has the captured element centre overlapped another element, other than the place holder?
            var position = new Point(_positionElementStart.X + _mouseDelta.X,
                _positionElementStart.Y + _mouseDelta.Y);



            _elementCaptured.Arrange(new Rect(position,
                _elementCaptured.DesiredSize));



            // Get the centre of the captured element.
            var centre = new Point(position.X + _elementCaptured.DesiredSize.Width / 2,
                position.Y + _elementCaptured.DesiredSize.Height / 2);



            FrameworkElement childOverlap = null;
            var index = 0;
            foreach (FrameworkElement child in InternalChildren)
            {
                if (child != _placeHolder
                    && child != _elementCaptured)
                {

                    var childPosition = child.TranslatePoint(new Point(),
                        this);


                    if ((centre.X >= childPosition.X && centre.X <= childPosition.X + child.DesiredSize.Width)
                        && (centre.Y >= childPosition.Y && centre.Y <= childPosition.Y + child.DesiredSize.Height))
                    {
                        childOverlap = child;
                        break;
                    }
                }

                index++;
            }

            if (childOverlap != null)
                PositionPlaceHolder(childOverlap, 
                    _placeHolderIndex, 
                    index);
                



            return panelSize;
        }


        private void PositionPlaceHolder(UIElement child,
                                         int placeHolderIndex,
                                         int childIndex)
        {
            var isGreater = placeHolderIndex < childIndex;

            InternalChildren.Remove(_placeHolder);
            placeHolderIndex = InternalChildren.IndexOf(child);

            if (isGreater 
             || _placeHolderIndex == placeHolderIndex)
                placeHolderIndex++;

            InternalChildren.Insert(placeHolderIndex,
                                         _placeHolder);

        }
        #endregion
    }
}
