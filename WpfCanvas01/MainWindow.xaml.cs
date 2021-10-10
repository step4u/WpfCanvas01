using Jeff.Controls;
using Jeff.Defines;
using Jeff.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfCanvas01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel vModel;

        public MainWindow()
        {
            InitializeComponent();

            vModel = this.DataContext as MainWindowViewModel;
        }

        double angle0 = 0;
        double curshapeAngle = 0;
        double curselectionAngle = 0;
        int polycount = 0;
        SelectionCanvas curSelection;
        double canvasLeft, canvasTop, canvasLeftSel, canvasTopSel;
        Point point, point4shape, centerpoint;
        dynamic curshape = null;
        DrawMode drawMode = DrawMode.None;
        SelectMode selectMode = SelectMode.None;
        private void canvas0_MouseDown(object sender, MouseButtonEventArgs e)
        {
            point = e.GetPosition(canvas0);

            if (e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Pressed)
            {
                if (drawMode == DrawMode.None)
                {
                    if (cmbShape.SelectedIndex == 0) return;

                    point4shape = new Point(point.X, point.Y);

                    dynamic newshape = null;

                    switch (cmbShape.SelectedIndex)
                    {
                        case 1:
                            newshape = new Line()
                            {
                                Fill = Brushes.Transparent,
                                Stroke = Brushes.Black,
                                StrokeThickness = 2,
                                X1 = 0,
                                Y1 = 0,
                                X2 = 0,
                                Y2 = 0,
                                StrokeDashArray = (DoubleCollection)cmbLineKinds.SelectedValue
                            };
                            break;
                        case 2:
                            newshape = new Rectangle()
                            {
                                Fill = Brushes.Transparent,
                                Stroke = Brushes.Black,
                                StrokeThickness = 2,
                                Width = 0,
                                Height = 0,
                                StrokeDashArray = (DoubleCollection)cmbLineKinds.SelectedValue
                            };
                            break;
                        case 3:
                            newshape = new Polyline()
                            {
                                Stroke = Brushes.Black,
                                StrokeThickness = 2,
                                StrokeDashArray = (DoubleCollection)cmbLineKinds.SelectedValue
                            };
                            newshape.Points.Add(new Point(0, 0));
                            polycount = newshape.Points.Count;
                            break;
                        case 4:
                            newshape = new Ellipse()
                            {
                                Fill = Brushes.Transparent,
                                Stroke = Brushes.Black,
                                StrokeThickness = 2,
                                StrokeDashArray = (DoubleCollection)cmbLineKinds.SelectedValue
                            };
                            break;
                    }

                    Canvas.SetLeft(newshape, point.X);
                    Canvas.SetTop(newshape, point.Y);
                    newshape.RenderTransform = CreateTransformGroup();
                    curshape = newshape;
                    AddShape(newshape);

                    drawMode = DrawMode.Draw;
                }
                else if (drawMode == DrawMode.Draw)
                {
                    if (cmbShape.SelectedIndex == 3)
                    {
                        double _x = point.X - point4shape.X;
                        double _y = point.Y - point4shape.Y;

                        curshape.Points.Add(new Point(_x, _y));
                        polycount = curshape.Points.Count;
                    }
                }
                else if (drawMode == DrawMode.Select && selectMode == SelectMode.None)
                {
                    point4shape = new Point(point.X, point.Y);

                    if (IsBelongToThis(e.OriginalSource))
                    {
                        if (e.OriginalSource is SelectionCanvas)
                        {
                            UpdateSelection(ref curSelection, ref curshape, Visibility.Collapsed);
                            return;
                        }
                        else if (e.OriginalSource is Line)
                        {
                            curshape = (Line)e.OriginalSource;
                        }
                        else if (e.OriginalSource is Rectangle)
                        {
                            curshape = (Rectangle)e.OriginalSource;
                        }
                        else if (e.OriginalSource is Polyline)
                        {
                            curshape = (Polyline)e.OriginalSource;
                        }
                        else if (e.OriginalSource is Ellipse)
                        {
                            curshape = (Ellipse)e.OriginalSource;
                        }

                        if (Keyboard.Modifiers == ModifierKeys.Control)
                        {
                            curSelection = CreateSelectionCanvas();
                            UpdateSelection(ref curSelection, ref curshape);
                        }
                        else
                        {
                            if (GetCountOfSelection() > 0)
                            {
                                UpdateSelection(ref curSelection, ref curshape, Visibility.Collapsed);

                                curSelection = CreateSelectionCanvas();
                                UpdateSelection(ref curSelection, ref curshape);
                            }
                            else
                            {
                                curSelection = CreateSelectionCanvas();
                                UpdateSelection(ref curSelection, ref curshape);
                            }
                        }
                    }
                    else
                    {
                        if (e.OriginalSource is Canvas)
                        {
                            UpdateSelection(ref curSelection, ref curshape, Visibility.Collapsed);
                            return;
                        }
                        else if (e.OriginalSource is Border)
                        {
                            if (Keyboard.Modifiers == ModifierKeys.Control)
                            {
                                Border _border = (Border)e.OriginalSource;
                                var _selection = (SelectionCanvas)_border.Parent;

                                UpdateSelection(ref _selection, ref curshape, Visibility.Collapsed);

                                //curshape = null;
                            }
                            else
                            {
                                Border _border = (Border)e.OriginalSource;
                                var _selection = (SelectionCanvas)_border.Parent;
                                curSelection = _selection;
                                curshape = _selection.Shape;
                            }
                        }
                    }

                    canvasLeft = Canvas.GetLeft(curshape);
                    canvasTop = Canvas.GetTop(curshape);

                    canvasLeftSel = Canvas.GetLeft(curSelection);
                    canvasTopSel = Canvas.GetTop(curSelection);

                    selectMode = SelectMode.Moving;
                }
                else if (drawMode == DrawMode.Select && selectMode == SelectMode.RotateOver)
                {
                    double _x = Canvas.GetLeft(curSelection) + curSelection.ActualWidth / 2;
                    double _y = Canvas.GetTop(curSelection) + curSelection.ActualHeight / 2;

                    centerpoint = new Point(_x, _y);

                    double __x = point.X - centerpoint.X;
                    double __y = point.Y - centerpoint.Y;
                    double atanval = __y / __x;
                    double radians = Math.Atan(atanval);
                    angle0 = radians * 180 / Math.PI;

                    RotateTransform transform = GetTransform<RotateTransform>(curshape);
                    curshapeAngle = transform.Angle;

                    transform = GetTransform<RotateTransform>(curSelection);
                    curselectionAngle = transform.Angle;

                    selectMode = SelectMode.Rotating;

                    //string msg = $"MouseDown -> curshapeAngle: {curshapeAngle}, curselectionAngle: {curselectionAngle}";
                    //DebugWriteLine(msg);
                }
                else if (drawMode == DrawMode.Select && selectMode == SelectMode.ResizeOver)
                {
                    selectMode = SelectMode.Resizing;
                }
            }
        }

        private void canvas0_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (curshape == null) return;

            if (e.ChangedButton == MouseButton.Left)
            {
                if (drawMode == DrawMode.Draw && selectMode == SelectMode.None)
                {
                    if (cmbShape.SelectedIndex == 1 || cmbShape.SelectedIndex == 2 || cmbShape.SelectedIndex == 4)
                    {
                        CompletDrawing();
                    }
                }
                else if (drawMode == DrawMode.Select && selectMode == SelectMode.Moving)
                {
                    selectMode = SelectMode.None;
                }
                else if (drawMode == DrawMode.Select && selectMode == SelectMode.Rotating)
                {
                    selectMode = SelectMode.RotateOver;
                }
                else if (drawMode == DrawMode.Select && selectMode == SelectMode.RotatingOutOfRange)
                {
                    canvas0.Cursor = Cursors.Arrow;
                    selectMode = SelectMode.None;
                }
                else if (drawMode == DrawMode.Select && selectMode == SelectMode.Resizing)
                {
                    selectMode = SelectMode.ResizeOver;
                }
                else if (drawMode == DrawMode.Select && selectMode == SelectMode.ResizingOutOfRange)
                {
                    canvas0.Cursor = Cursors.Arrow;
                    selectMode = SelectMode.None;
                }
            }

            if (e.ChangedButton == MouseButton.Right && drawMode == DrawMode.Draw && selectMode == SelectMode.None)
            {
                if (cmbComplete.SelectedIndex == 1)
                {
                    CompletDrawing();
                }
            }
        }

        private void canvas0_MouseMove(object sender, MouseEventArgs e)
        {
            //if (curSelection == null) return;

            point = e.GetPosition(canvas0);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (drawMode == DrawMode.Draw)
                {
                    if (curshape == null) return;

                    double left, top, _x, _y;

                    switch (cmbShape.SelectedIndex)
                    {
                        case 1:
                            _x = point.X - point4shape.X;
                            _y = point.Y - point4shape.Y;
                            curshape.X2 = _x;
                            curshape.Y2 = _y;
                            break;
                        case 2:
                        case 4:
                            left = Canvas.GetLeft(curshape);
                            top = Canvas.GetTop(curshape);
                            double _width = point.X - left;
                            double _height = point.Y - top;
                            if (cmbShape.SelectedIndex == 4 && Keyboard.Modifiers == ModifierKeys.Shift)
                            {
                                if (_width - _height < 0)
                                {
                                    _height = _width;
                                }
                                else
                                {
                                    _width = _height;
                                }
                            }
                            if (_width < 0)
                            {
                                ScaleTransform _scale = curshape.RenderTransform.Children[0];
                                _scale.ScaleX = -1;
                            }
                            else
                            {
                                ScaleTransform _scale = curshape.RenderTransform.Children[0];
                                _scale.ScaleX = 1;
                            }

                            if (_height < 0)
                            {
                                ScaleTransform _scale = curshape.RenderTransform.Children[0];
                                _scale.ScaleY = -1;
                            }
                            else
                            {
                                ScaleTransform _scale = curshape.RenderTransform.Children[0];
                                _scale.ScaleY = 1;
                            }

                            curshape.Width = Math.Abs(_width);
                            curshape.Height = Math.Abs(_height);
                            break;
                        case 3:
                            _x = point.X - point4shape.X;
                            _y = point.Y - point4shape.Y;
                            if (curshape.Points.Count == polycount)
                            {
                                curshape.Points.Add(new Point(_x, _y));
                            }
                            else
                            {
                                curshape.Points[curshape.Points.Count - 1] = new Point(_x, _y);
                            }
                            break;
                    }
                }
                else if (drawMode == DrawMode.Select)
                {
                    if (curSelection == null) return;

                    if (selectMode == SelectMode.Moving)
                    {
                        double _leftMoved = point.X - point4shape.X;
                        double _topMoved = point.Y - point4shape.Y;

                        if (_leftMoved == 0 && _topMoved == 0)
                            return;

                        Canvas.SetLeft(curSelection, canvasLeftSel + _leftMoved);
                        Canvas.SetTop(curSelection, canvasTopSel + _topMoved);

                        Canvas.SetLeft(curshape, canvasLeft + _leftMoved);
                        Canvas.SetTop(curshape, canvasTop + _topMoved);
                    }
                    else if (selectMode == SelectMode.Rotating || selectMode == SelectMode.RotatingOutOfRange)
                    {
                        RotateTransform transform = GetTransform<RotateTransform>(curSelection);
                        transform.Angle = GetAngle(point, centerpoint, angle0, curshapeAngle);
                    }
                    else if (selectMode == SelectMode.Resizing || selectMode == SelectMode.ResizingOutOfRange)
                    {

                    }
                }
            }
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (curshape == null) return;

            if (e.ChangedButton == MouseButton.Left && drawMode == DrawMode.Draw && selectMode == SelectMode.None)
            {
                //if (cmbComplete.SelectedIndex == 2)
                //{
                //    CompletDrawing();
                //}

                if (cmbShape.SelectedIndex == 3)
                {
                    CompletDrawing();
                }
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = sender as ToggleButton;

            if ((bool)btn.IsChecked)
            {
                MemoryStream _mem = new MemoryStream(Properties.Resources.ArrowSelectMove);
                canvas0.Cursor = new Cursor(_mem);
                drawMode = DrawMode.Select;
                cmbShape.SelectedIndex = 0;
            }
            else
            {
                drawMode = DrawMode.None;
                selectMode = SelectMode.None;
                canvas0.Cursor = Cursors.Arrow;
                UpdateSelection(ref curSelection, ref curshape, Visibility.Collapsed);
            }
        }

        private void cmbShape_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox editor = sender as ComboBox;

            if (!editor.IsMouseOver) return;

            btnSelect.IsChecked = false;

            drawMode = DrawMode.None;
            canvas0.Cursor = Cursors.Arrow;
            UpdateSelection(ref curSelection, ref curshape, Visibility.Collapsed);
        }

        #region Selection events whether the mouse Enter, Leave, Rotation or Resize on range
        private void Selection_MouseEnteredRotationRange(object source, MouseEventArgs e)
        {
            if (drawMode == DrawMode.Select && selectMode == SelectMode.None)
            {
                Rectangle rect = (Rectangle)e.OriginalSource;

                MemoryStream _mem = new MemoryStream(Properties.Resources.ArrowRotate);
                canvas0.Cursor = new Cursor(_mem);

                selectMode = SelectMode.RotateOver;
            }
        }

        private void Selection_MouseLeftRotationRange(object source, MouseEventArgs e)
        {
            if (drawMode == DrawMode.Select)
            {
                if (selectMode == SelectMode.RotateOver)
                {
                    Rectangle rect = (Rectangle)e.OriginalSource;

                    MemoryStream _mem = new MemoryStream(Properties.Resources.ArrowSelectMove);
                    canvas0.Cursor = new Cursor(_mem);

                    selectMode = SelectMode.None;
                }
                else if (selectMode == SelectMode.Rotating)
                {
                    selectMode = SelectMode.RotatingOutOfRange;
                }
            }
        }

        private void Selection_MouseEnteredResizeRange(object source, MouseEventArgs e)
        {
            if (drawMode == DrawMode.Select && selectMode == SelectMode.None)
            {
                Rectangle rect = (Rectangle)e.OriginalSource;

                Cursor _cursor = Cursors.ScrollNE;

                switch (rect.Name)
                {
                    case "LT":
                        _cursor = Cursors.SizeNWSE;
                        break;
                    case "MT":
                        _cursor = Cursors.SizeNS;
                        break;
                    case "RT":
                        _cursor = Cursors.SizeNESW;
                        break;
                    case "RM":
                        _cursor = Cursors.SizeWE;
                        break;
                    case "RB":
                        _cursor = Cursors.SizeNWSE;
                        break;
                    case "BM":
                        _cursor = Cursors.SizeNS;
                        break;
                    case "LB":
                        _cursor = Cursors.SizeNESW;
                        break;
                    case "LM":
                        _cursor = Cursors.SizeWE;
                        break;
                }

                canvas0.Cursor = _cursor;

                selectMode = SelectMode.ResizeOver;
            }
        }

        private void Selection_MouseLeftResizeRange(object source, MouseEventArgs e)
        {
            if (drawMode == DrawMode.Select)
            {
                if (selectMode == SelectMode.ResizeOver)
                {
                    Rectangle rect = (Rectangle)e.OriginalSource;

                    MemoryStream _mem = new MemoryStream(Properties.Resources.ArrowSelectMove);
                    canvas0.Cursor = new Cursor(_mem);

                    selectMode = SelectMode.None;
                }
                else if (selectMode == SelectMode.Resizing)
                {
                    selectMode = SelectMode.ResizingOutOfRange;
                }
            }
        }
        #endregion

        #region private Methods..
        private TransformGroup CreateTransformGroup()
        {
            TransformGroup _group = new TransformGroup();
            _group.Children.Add(new ScaleTransform());
            _group.Children.Add(new SkewTransform());
            _group.Children.Add(new RotateTransform() { CenterX = 25, CenterY = 50 });
            _group.Children.Add(new TranslateTransform());

            return _group;
        }

        private SelectionCanvas CreateSelectionCanvas()
        {
            SelectionCanvas _selection = new SelectionCanvas()
            {
                ParentWin = this,
                RenderTransform = CreateTransformGroup(),
                RenderTransformOrigin = new Point(0.5, 0.5)
            };

            _selection.MouseEnteredRotationRange += Selection_MouseEnteredRotationRange;
            _selection.MouseLeftRotationRange += Selection_MouseLeftRotationRange;
            _selection.MouseEnteredResizeRange += Selection_MouseEnteredResizeRange;
            _selection.MouseLeftResizeRange += Selection_MouseLeftResizeRange;

            return _selection;
        }

        private void DebugWriteLine(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
        }

        private T GetTransform<T>(dynamic shape) where T : class
        {
            var tfg = shape.RenderTransform as TransformGroup;
            var transform = new object();
            if (typeof(T) == typeof(ScaleTransform))
            {
                transform = tfg.Children[0];
            }
            else if (typeof(T) == typeof(SkewTransform))
            {
                transform = tfg.Children[1];
            }
            else if (typeof(T) == typeof(RotateTransform))
            {
                transform = tfg.Children[2];
            }
            else if (typeof(T) == typeof(TranslateTransform))
            {
                transform = tfg.Children[3];
            }

            return (T)transform;
        }

        private double GetAngle(Point currentPoint, Point centerPoint, double startAngle, double curshapeAngle)
        {
            double radians = Math.Atan((currentPoint.Y - centerPoint.Y) / (currentPoint.X - centerPoint.X));
            double angle = radians * 180 / Math.PI;

            angle = angle - startAngle + curshapeAngle;

            //System.Diagnostics.Debug.WriteLine($"selectMode: {selectMode}, centerpoint.X: {centerpoint.X}, centerpoint.Y: {centerpoint.Y}");
            //System.Diagnostics.Debug.WriteLine($"selectMode: {selectMode}, point.X: {point.X}, point.Y: {point.Y}");
            //System.Diagnostics.Debug.WriteLine($"selectMode: {selectMode}, x: {_x}, y: {_y}, atanval: {atanval}, radians: {radians}, angle0: {angle0}, angle1: {angle1}, angle: {angle}");
            //System.Diagnostics.Debug.WriteLine($"selectMode: {selectMode}, angle0: {angle0}, angle1: {angle1}, angle: {angle}");

            if ((currentPoint.X - centerPoint.X) < 0)
                angle += 360;

            return angle;
        }

        private void CompletDrawing()
        {
            switch (cmbShape.SelectedIndex)
            {
                case 1:
                    double _x = point.X - point4shape.X;
                    double _y = point.Y - point4shape.Y;

                    curshape.X2 = _x;
                    curshape.Y2 = _y;
                    break;
                case 2:
                    break;
                case 3:
                    if (curshape.Points.Count > 2)
                    {
                        curshape.Points.Add(curshape.Points[0]);
                    }

                    curshape.Fill = Brushes.Transparent;
                    polycount = 0;
                    break;
                case 4:
                    break;
            }

            //selectionCanvas = new SelectionCanvas() { ParentWin = this };
            //UpdateSelectShape(curshape, Visibility.Visible);

            curshape = null;
            canvas0.Cursor = Cursors.Arrow;
            drawMode = DrawMode.None;
        }

        private void UpdateSelection(ref SelectionCanvas selection, ref dynamic shape, Visibility visibility = Visibility.Visible)
        {
            if (visibility == Visibility.Collapsed)
            {
                if (selection == null) return;

                // Shape is deselected.

                RemoveShape(selection);
                selection = null;
            }
            else
            {
                // Shape is selected.

                double _width = 0;
                double _height = 0;
                double _left = 0;
                double _top = 0;

                if (shape is Line)
                {
                    _width = shape.X2 - shape.X1;
                    _height = shape.Y2 - shape.Y1;

                    if (_width == 0 && _height == 0)
                    {
                        shape = null;
                        return;
                    }

                    selection.Width = Math.Abs(_width) + 30;
                    selection.Height = Math.Abs(_height) + 30;

                    _left = Canvas.GetLeft(shape);
                    _top = Canvas.GetTop(shape);

                    if (_width < 0) _left = _left + _width;
                    if (_height < 0) _top = _top + _height;
                }
                else if (shape is Rectangle || shape is Ellipse)
                {
                    _width = shape.Width;
                    _height = shape.Height;

                    if (_width == 0 && _height == 0)
                    {
                        shape = null;
                        return;
                    }

                    selection.Width = Math.Abs(_width) + 30;
                    selection.Height = Math.Abs(_height) + 30;

                    _left = Canvas.GetLeft(shape);
                    _top = Canvas.GetTop(shape);

                    var _tfg = shape.RenderTransform as TransformGroup;
                    var _sct = _tfg.Children[0] as ScaleTransform;
                    if (_sct.ScaleX == -1)
                    {
                        _left = _left - _width;
                    }
                    if (_sct.ScaleY == -1)
                    {
                        _top = _top - _height;
                    }
                }
                else if (shape is Polyline)
                {
                    double _canvasLeft = Canvas.GetLeft(curshape);
                    double _canvasTop = Canvas.GetTop(curshape);

                    selection.Width = shape.ActualWidth + 30;
                    selection.Height = shape.ActualHeight + 30;

                    _left = Canvas.GetLeft(shape);
                    _top = Canvas.GetTop(shape);
                }

                var tfg = shape.RenderTransform as TransformGroup ?? CreateTransformGroup();
                var rt = tfg.Children[2] as RotateTransform;

                var tfgSel = selection.RenderTransform as TransformGroup;
                var rtSel = tfgSel.Children[2] as RotateTransform;

                rtSel.Angle = rt.Angle;
                rtSel.CenterX = rt.CenterX;
                rtSel.CenterY = rt.CenterY;

                Canvas.SetLeft(selection, _left - 15);
                Canvas.SetTop(selection, _top - 15);

                selection.Shape = curshape;
                AddShape(selection);

                canvasLeftSel = Canvas.GetLeft(selection);
                canvasTopSel = Canvas.GetTop(selection);
            }
        }

        private void AddShape(dynamic element)
        {
            this.canvas0.Children.Add(element);
        }

        private void RemoveShape(dynamic element)
        {
            this.canvas0.Children.Remove(element);
        }

        private void GetWidthHeightSelectRectOfPoly(Polyline shape, ref double _width, ref double _height,
            out double _maxX, out double _maxY, out double _minX, out double _minY)
        {
            _maxX = _maxY = _minX = _minY = 0;

            Point[] _points = new Point[shape.Points.Count];
            shape.Points.CopyTo(_points, 0);

            _minX = 0;
            _maxX = _points.Max(X1 => X1.X);
            _minY = 0;
            _maxY = _points.Max(Y1 => Y1.Y);

            _width = _maxX - _minX;
            _height = _maxY - _minY;
        }

        public void SetCursor(MouseArrowState _state, SelectionCanvas _selection)
        {
            if (drawMode == DrawMode.Select)
            {
                MemoryStream _mem;

                switch (_state)
                {
                    case MouseArrowState.Normal:
                        canvas0.Cursor = Cursors.Arrow;
                        break;
                    case MouseArrowState.Select:
                        _mem = new MemoryStream(Properties.Resources.ArrowSelectMove);
                        canvas0.Cursor = new Cursor(_mem, true);
                        break;
                    case MouseArrowState.SLT:
                        canvas0.Cursor = Cursors.ScrollNW;
                        break;
                    case MouseArrowState.RLT:
                        _mem = new MemoryStream(Properties.Resources.ArrowRotate);
                        canvas0.Cursor = new Cursor(_mem, true);
                        break;
                }
            }
        }

        private bool IsBelongToThis(object shape)
        {
            var element = new UIElement();

            if (shape is Line)
            {
                element = (Line)shape;
            }
            else if (shape is Rectangle)
            {
                element = (Rectangle)shape;
            }
            else if (shape is Polyline)
            {
                element = (Polyline)shape;
            }
            else if (shape is Ellipse)
            {
                element = (Ellipse)shape;
            }
            else if (shape is SelectionCanvas)
            {
                element = (SelectionCanvas)shape;
            }
            else
            {
                return false;
            }

            return canvas0.Children.Contains(element);
        }

        private int GetCountOfSelection()
        {
            return canvas0.Children.OfType<SelectionCanvas>().Count();
        }
        #endregion

        #region Canvert Path to Image
        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            RenderToDisk0();
            RenderToDisk1();
            RenderToDisk2();
        }

        private void RenderToDisk0()
        {
            System.Windows.Shapes.Path _path = (System.Windows.Shapes.Path)App.Current.FindResource("ArrowRotate");

            Rect bounds = _path.Data.GetRenderBounds(null);
            _path.Measure(bounds.Size);
            _path.Arrange(bounds);

            var bitmaprender = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, 300, 300, PixelFormats.Pbgra32);
            bitmaprender.Render(_path);

            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmaprender));
            encoder.Save(stream);

            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(stream);
            bitmap.Save("ArrowRotate0.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }


        private void btnTest0_Click(object sender, RoutedEventArgs e)
        {
            //bool IsSelected = Selector.GetIsSelected(poly02);
            //Selector.SetIsSelected(poly02, !IsSelected);
            //bool IsSelectedActive = Selector.GetIsSelectionActive(poly02);

            var _width = poly02.ActualWidth;
            var _height = poly02.ActualHeight;

            //Rectangle rect = new Rectangle() { Width = _width, Height = _height, Stroke = Brushes.Black, StrokeThickness = 1 };
            //Canvas.SetLeft(rect, Canvas.GetLeft(poly02));
            //Canvas.SetTop(rect, Canvas.GetTop(poly02));
            //canvas0.Children.Add(rect);
        }

        private void RenderToDisk1()
        {
            //var path = new System.Windows.Shapes.Path
            //{
            //    Data = new EllipseGeometry(new Point(100, 100), 100, 100),
            //    Fill = Brushes.Blue
            //};

            System.Windows.Shapes.Path path = (System.Windows.Shapes.Path)App.Current.FindResource("ArrowRotate");

            var bounds = path.Data.GetRenderBounds(null);
            path.Measure(bounds.Size);
            path.Arrange(bounds);

            var rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(path);

            MemoryStream stream = new MemoryStream();
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            encoder.Save(stream);

            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(stream);
            bitmap.Save("ArrowRotate1.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        private void RenderToDisk2()
        {
            System.Windows.Shapes.Path _path = (System.Windows.Shapes.Path)App.Current.FindResource("ArrowRotate");
            string filepath = @"ArrowRotate2.png";
            SaveImage(_path, 32, 32, filepath);
        }

        public void SaveImage(Visual visual, int width, int height, string filePath)
        {
            var rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(visual);

            //MemoryStream stream = new MemoryStream();
            //BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(rtb));
            //encoder.Save(stream);

            //System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(stream);
            //bitmap.Save("arrow2.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            var image = new PngBitmapEncoder();
            //var image = new BmpBitmapEncoder();
            image.Frames.Add(BitmapFrame.Create(rtb));
            using (Stream fs = File.Create(filePath))
            {
                image.Save(fs);
            }
        }
        #endregion
    }
}
