using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Common.Utilities
{
	public class WindowResizer
	{
		private readonly Window _target;

		private bool _resizeRight;
		private bool _resizeLeft;
		private bool _resizeUp;
		private bool _resizeDown;

		private readonly Dictionary<UIElement, short> _leftElements = new Dictionary<UIElement, short>();
		private readonly Dictionary<UIElement, short> _rightElements = new Dictionary<UIElement, short>();
		private readonly Dictionary<UIElement, short> _upElements = new Dictionary<UIElement, short>();
		private readonly Dictionary<UIElement, short> _downElements = new Dictionary<UIElement, short>();

		private PointAPI _resizePoint;
		private Size _resizeSize;
		private Point _resizeWindowPoint;

		private delegate void RefreshDelegate();

		public WindowResizer(Window target)
		{
			this._target = target;

			if (target == null)
			{
				throw new Exception("Invalid Window handle");
			}
		}

		#region add resize components
		private void ConnectMouseHandlers(UIElement element)
		{
			if ( element == null )
				return;

			element.MouseLeftButtonDown += element_MouseLeftButtonDown;
			element.MouseLeftButtonUp += element_MouseLeftButtonUp;
			element.MouseEnter += element_MouseEnter;
			element.MouseLeave += SetArrowCursor;
		}

		public void AddResizerRight(UIElement element)
		{
			if ( element == null )
				return;

			ConnectMouseHandlers(element);
			_rightElements.Add(element, 0);
		}

		public void AddResizerLeft(UIElement element)
		{
			if ( element == null )
				return;

			ConnectMouseHandlers(element);
			_leftElements.Add(element, 0);
		}

		public void AddResizerUp(UIElement element)
		{
			if ( element == null )
				return;

			ConnectMouseHandlers(element);
			_upElements.Add(element, 0);
		}

		public void AddResizerDown(UIElement element)
		{
			if ( element == null )
				return;

			ConnectMouseHandlers(element);
			_downElements.Add(element, 0);
		}

		public void AddResizerRightDown(UIElement element)
		{
			if ( element == null )
				return;

			ConnectMouseHandlers(element);
			_rightElements.Add(element, 0);
			_downElements.Add(element, 0);
		}

		public void AddResizerLeftDown(UIElement element)
		{
			if ( element == null )
				return;

			ConnectMouseHandlers(element);
			_leftElements.Add(element, 0);
			_downElements.Add(element, 0);
		}

		public void AddResizerRightUp(UIElement element)
		{
			if ( element == null )
				return;

			ConnectMouseHandlers(element);
			_rightElements.Add(element, 0);
			_upElements.Add(element, 0);
		}

		public void AddResizerLeftUp(UIElement element)
		{
			if ( element == null )
				return;

			ConnectMouseHandlers(element);
			_leftElements.Add(element, 0);
			_upElements.Add(element, 0);
		}
		#endregion

		#region resize handlers
		private void element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			(sender as UIElement).CaptureMouse();

			GetCursorPos(out _resizePoint);

			_resizeSize = new Size(_target.Width, _target.Height);
			_resizeWindowPoint = new Point(_target.Left, _target.Top);

			#region updateResizeDirection
			var sourceSender = (UIElement)sender;
			if (_leftElements.ContainsKey(sourceSender))
				_resizeLeft = true;
			if (_rightElements.ContainsKey(sourceSender))
				_resizeRight = true;
			if (_upElements.ContainsKey(sourceSender))
				_resizeUp = true;
			if (_downElements.ContainsKey(sourceSender))
				_resizeDown = true;
			#endregion

			var t = new Thread(UpdateSizeLoop)
            {
                Name = "Mouse Position Poll Thread"
            };
            t.Start();
		}

        private void element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
            _resizeDown = false;
            _resizeLeft = false;
            _resizeRight = false;
            _resizeUp = false;
			(sender as UIElement).ReleaseMouseCapture();
		}

		private void UpdateSizeLoop()
		{
			try
			{
				while (_resizeDown || _resizeLeft || _resizeRight || _resizeUp)
				{
					_target.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, new RefreshDelegate(UpdateSize));
					_target.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, new RefreshDelegate(UpdateMouseDown));
					Thread.Sleep(10);
				}

				_target.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, new RefreshDelegate(SetArrowCursor));
			}
            catch
            {
                // ignored
            }
        }

		#region updates
		private void UpdateSize()
		{
			PointAPI p;
			GetCursorPos(out p);

            _target.SizeToContent = SizeToContent.Manual;

			if (_resizeRight)
                _target.Width = Math.Max(0, this._resizeSize.Width - (_resizePoint.X - p.X));

            if (_resizeDown)
                _target.Height = Math.Max(0, _resizeSize.Height - (_resizePoint.Y - p.Y));

            if (_resizeLeft)
            {
                _target.Width = Math.Max(0, _resizeSize.Width + (_resizePoint.X - p.X));
                _target.Left = Math.Max(0, _resizeWindowPoint.X - (_resizePoint.X - p.X));
            }

            if (_resizeUp)
            {
                _target.Height = Math.Max(0, _resizeSize.Height + (_resizePoint.Y - p.Y));
                _target.Top = Math.Max(0, _resizeWindowPoint.Y - (_resizePoint.Y - p.Y));
            }
		}

		private void UpdateMouseDown()
        {
            if (Mouse.LeftButton != MouseButtonState.Released)
                return;

            _resizeRight = false;
            _resizeLeft = false;
            _resizeUp = false;
            _resizeDown = false;
        }
		#endregion
		#endregion

		#region cursor updates
		private void element_MouseEnter(object sender, MouseEventArgs e)
		{
			var resizeRight = false;
			var resizeLeft = false;
			var resizeUp = false;
			var resizeDown = false;

			var window = (Window)((FrameworkElement)sender).TemplatedParent;
			if ( window != null && window.WindowState == WindowState.Maximized)
			{
				return;
			}

			var sourceSender = (UIElement)sender;

			if (_leftElements.ContainsKey(sourceSender))
				resizeLeft = true;
			if (_rightElements.ContainsKey(sourceSender))
				resizeRight = true;
			if (_upElements.ContainsKey(sourceSender))
				resizeUp = true;
			if (_downElements.ContainsKey(sourceSender))
				resizeDown = true;

			if ((resizeLeft && resizeDown) || (resizeRight && resizeUp))
				SetNeswCursor(sender, e);
			else if ((resizeRight && resizeDown) || (resizeLeft && resizeUp))
				SetNwseCursor(sender, e);
			else if (resizeLeft || resizeRight)
				SetWeCursor(sender, e);
			else if (resizeUp || resizeDown)
				SetNsCursor(sender, e);
		}

		private void SetWeCursor(object sender, MouseEventArgs e)
		{
			_target.Cursor = Cursors.SizeWE;
		}

		private void SetNsCursor(object sender, MouseEventArgs e)
		{
			_target.Cursor = Cursors.SizeNS;
		}

		private void SetNeswCursor(object sender, MouseEventArgs e)
		{
			_target.Cursor = Cursors.SizeNESW;
		}

		private void SetNwseCursor(object sender, MouseEventArgs e)
		{
			_target.Cursor = Cursors.SizeNWSE;
		}

		private void SetArrowCursor(object sender, MouseEventArgs e)
		{
			if (!_resizeDown && !_resizeLeft && !_resizeRight && !_resizeUp)
				_target.Cursor = Cursors.Arrow;
		}

		private void SetArrowCursor()
		{
			_target.Cursor = Cursors.Arrow;
		}
		#endregion

		#region external call
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetCursorPos(out PointAPI lpPoint);

		private struct PointAPI
		{
			public int X;
			public int Y;
		}
		#endregion
	}
}
