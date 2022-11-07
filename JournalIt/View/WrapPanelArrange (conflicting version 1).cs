using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace JournalIt.View
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class WrapPanelArrange : Panel
    {
        private Border placeHolder = new Border();
        private int placeHolderIndex;

        private FrameworkElement elementCapurted;
        private Point positionElementStart;

        private Point positionStart;
        private Point mousePosition;
        private Point mouseDelta;
        private bool isDragging = false;


        public event EventHandler CollectionOrderChanged;

        // This is used by the render element method.
        private static Action EmptyDelegate = delegate() { };

        public WrapPanelArrange()
            : base()
        {
            // this.orientation = (Orientation)OrientationProperty.GetDefaultValue(DependencyObjectType);


            this.placeHolder.Background = new SolidColorBrush(Color.FromArgb(45, 0, 0, 0));
            this.placeHolder.Width = 50;
            this.placeHolder.Height = 50;
            placeHolder.Visibility = System.Windows.Visibility.Collapsed;

            // Wire up the event method handlers.
            this.PreviewMouseLeftButtonDown += WrapPanelArrange_PreviewMouseLeftButtonDown;
            this.PreviewMouseLeftButtonUp += WrapPanelArrange_PreviewMouseLeftButtonUp;
            this.PreviewMouseMove += WrapPanelArrange_PreviewMouseMove;
            this.MouseLeave += WrapPanelArrange_MouseLeave;
        }


        #region Event Handlers
        void WrapPanelArrange_PreviewMouseLeftButtonDown(object sender,
                                                         System.Windows.Input.MouseButtonEventArgs e)
        {
            this.elementCapurted = this.GetElement(e.Source as FrameworkElement);
            this.positionElementStart = this.elementCapurted.TranslatePoint(new Point(),
                                                                            this);

            this.positionElementStart.X -= this.elementCapurted.Margin.Left;
            this.positionElementStart.Y -= this.elementCapurted.Margin.Top;


            // Get the initial mouse position
            this.mousePosition = e.MouseDevice.GetPosition(this);
            e.Handled = true;
        }

        private FrameworkElement GetElement(FrameworkElement frameworkElement)
        {
            if (frameworkElement.Parent == this)
                return frameworkElement;
            else
                return this.GetElement(frameworkElement.Parent as FrameworkElement);
        }

        void WrapPanelArrange_PreviewMouseMove(object sender,
                                               System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                if (this.elementCapurted != null)
                {
                    // Get the position of the mouse relative to the mouse down position.
                    this.mouseDelta.X = e.MouseDevice.GetPosition(this).X - this.mousePosition.X;
                    this.mouseDelta.Y = e.MouseDevice.GetPosition(this).Y - this.mousePosition.Y;

                    if (this.mouseDelta.X > SystemParameters.MinimumHorizontalDragDistance
                    || this.mouseDelta.X < -SystemParameters.MinimumHorizontalDragDistance
                    || this.mouseDelta.Y > SystemParameters.MinimumVerticalDragDistance
                    || this.mouseDelta.Y < -SystemParameters.MinimumVerticalDragDistance
                    || isDragging)
                    {
                        if (!isDragging)
                        {
                            // We have just started dragging
                            isDragging = true;

                            this.elementCapurted.CaptureMouse();

                            // Get the start position of the selected UI Element
                            Point point = new Point(this.elementCapurted.Margin.Left,
                                                    this.elementCapurted.Margin.Top);

                            this.positionStart = this.elementCapurted.TranslatePoint(point,
                                                                                     this);

                            this.ShowPlaceHolder();
                        }

                        this.InvalidateVisual();
                    }
                }
            }
        }


        private void WrapPanelArrange_MouseLeave(object sender,
                                                 System.Windows.Input.MouseEventArgs e)
        {
            if (isDragging)
            {
                this.HidePlaseHolder();
                isDragging = false;
            }
        }

        void WrapPanelArrange_PreviewMouseLeftButtonUp(object sender,
                                                       System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                // End of drag

                this.HidePlaseHolder();
                isDragging = false;

                // Raise a rearranged event.
                if (this.CollectionOrderChanged != null)
                    this.CollectionOrderChanged(this, null);
            }
            else
            {
                // Run a click event
                //Console.WriteLine("Click event");

                UIElement element = this.elementCapurted as UIElement;

                switch (this.elementCapurted.DependencyObjectType.BaseType.Name)
                {
                    case "ToggleButton":
                
                        try
                        {
                            System.Windows.Controls.Primitives.ToggleButton tb = element as System.Windows.Controls.Primitives.ToggleButton;
                            tb.IsChecked = !tb.IsChecked;
                            tb.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ToggleButton.ClickEvent, element));

                            tb.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render,
                                                 EmptyDelegate);
                            tb.UpdateLayout();

                            //ViewModel.Stopwatch stopwatch = tb.DataContext as ViewModel.Stopwatch;
                            //stopwatch.OnPropertyChanged("Caption");
                            //stopwatch.OnPropertyChanged("IsOn");
                        }
                        catch { }
                        break;

                    case "Button":

                        try
                        {
                            //element.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                            element.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent, element));
                        }
                        catch { }
                        break;
                }

                // Force the element to render is content. 
                // For some reason,, on certain machine this screen does not automatically update.
                //element.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render,
                //                          EmptyDelegate);

                //Dispatcher.Invoke(new Action(() => { }), System.Windows.Threading.DispatcherPriority.ContextIdle, null);
            }
        }

        public static void ForceUIToUpdate()
        {
            System.Windows.Threading.DispatcherFrame frame = new System.Windows.Threading.DispatcherFrame();
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, 
                                                                         new System.Windows.Threading.DispatcherOperationCallback(delegate(object parameter)
                                                                             {
                                                                                 frame.Continue =false;
                                                                                 return null;
                                                                             }), null);
            System.Windows.Threading.Dispatcher.PushFrame(frame);
        }
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
        //    get { return this.orientation; }
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
            if (this.placeHolder.Visibility == System.Windows.Visibility.Collapsed
            && this.isDragging)
            {
                if (elementCapurted != null)
                {
                    // Mimic the properties of the captured element.
                    this.placeHolder.Width = this.elementCapurted.RenderSize.Width;
                    this.placeHolder.Height = this.elementCapurted.RenderSize.Height;
                    this.placeHolder.Margin = this.elementCapurted.Margin;
                    this.placeHolder.VerticalAlignment = this.elementCapurted.VerticalAlignment;
                    this.placeHolder.HorizontalAlignment = this.elementCapurted.HorizontalAlignment;


                    // Place into the place holder into the collection 
                    // and place the caputured element at the end of the collection
                    this.placeHolderIndex = base.InternalChildren.IndexOf(this.elementCapurted);
                    if (this.placeHolderIndex > -1)
                    {
                        base.InternalChildren.Insert(placeHolderIndex, placeHolder);
                        base.InternalChildren.Remove(this.elementCapurted);
                        base.InternalChildren.Add(this.elementCapurted);
                        this.placeHolder.Visibility = System.Windows.Visibility.Visible;

                        this.elementCapurted.SetValue(ZIndexProperty, 1);
                    }
                }
            }
        }





        private void HidePlaseHolder()
        {
            if (this.placeHolder.Visibility == System.Windows.Visibility.Visible
            && this.isDragging)
            {

                Point position = this.placeHolder.TranslatePoint(new Point(0, 0),
                                                                 this);

                this.elementCapurted.Arrange(new Rect(position.X,
                                                      position.Y,
                                                      this.elementCapurted.RenderSize.Width,
                                                      this.elementCapurted.RenderSize.Height));

                // Place into the collection
                int index = base.InternalChildren.IndexOf(this.placeHolder);
                base.InternalChildren.Remove(this.elementCapurted);
                base.InternalChildren.Insert(index, this.elementCapurted);
                this.elementCapurted.SetValue(ZIndexProperty, 0);
                this.elementCapurted.ReleaseMouseCapture();
                this.elementCapurted = null;

                base.InternalChildren.Remove(this.placeHolder);

                this.placeHolder.Visibility = System.Windows.Visibility.Collapsed;
            }
        }



        protected override Size MeasureOverride(Size constraint)
        {
            // Place the cildren on the panel
            foreach (UIElement child in base.InternalChildren)
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
                                    bool PositionElements)
        {
            Point positionNextChild = new Point();
            double rowHeight = 0;

            Size panelSize = new Size();

            foreach (FrameworkElement child in base.InternalChildren)
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
                    rowHeight = 0;
                }
                else
                {
                    // Set the height of the next row
                    positionNextChild.Y = rowHeight;
                }




                // Position the child
                if (PositionElements)
                    if (child != this.elementCapurted)
                    {
                        // Note: Animation could go here.
                        child.Arrange(new Rect(positionNextChild,
                                               child.DesiredSize));

                        if (child == this.placeHolder)
                            this.placeHolderIndex = base.InternalChildren.IndexOf(child);
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
            if (this.isDragging)
            {
                if (this.elementCapurted != null)
                {
                    //Has the captured element centre overlapped another element, other than the place holder?
                    Point position = new Point(this.positionElementStart.X + this.mouseDelta.X,
                                               this.positionElementStart.Y + this.mouseDelta.Y);



                    this.elementCapurted.Arrange(new Rect(position,
                                                          elementCapurted.DesiredSize));



                    // Get the centre of the captured element.
                    Point centre = new Point(position.X + elementCapurted.DesiredSize.Width / 2,
                                             position.Y + elementCapurted.DesiredSize.Height / 2);



                    FrameworkElement childOverlap = null;
                    int index = 0;
                    foreach (FrameworkElement child in base.InternalChildren)
                    {
                        if (child != this.placeHolder
                        && child != this.elementCapurted)
                        {

                            Point childPosition = child.TranslatePoint(new Point(),
                                                                       this);


                            if ((centre.X >= childPosition.X && centre.X <= childPosition.X + child.DesiredSize.Width)
                            && (centre.Y >= childPosition.Y && centre.Y <= childPosition.Y + child.DesiredSize.Height))
                            {
                                childOverlap = child;
                                break;
                            }
                            else
                                childOverlap = null;
                        }

                        index++;
                    }

                    if (childOverlap != null)
                        this.PositionPlaceHolder(childOverlap, 
                                                 this.placeHolderIndex, 
                                                 index);

                }
            }


            return panelSize;
        }


        private void PositionPlaceHolder(FrameworkElement child,
                                         int placeHolderIndex,
                                         int childIndex)
        {
            bool isGreater = placeHolderIndex < childIndex;

            base.InternalChildren.Remove(this.placeHolder);
            placeHolderIndex = base.InternalChildren.IndexOf(child);

            if (isGreater 
             || this.placeHolderIndex == placeHolderIndex)
                placeHolderIndex++;

            base.InternalChildren.Insert(placeHolderIndex,
                                         this.placeHolder);

        }
        #endregion
    }
}
