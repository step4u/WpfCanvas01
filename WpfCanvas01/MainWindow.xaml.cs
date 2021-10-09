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
            //this.Loaded += MainWindow_Loaded;

            vModel = this.DataContext as MainWindowViewModel;
        }

        //private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        //{
        //    SelectionCanvas selCanvas = new SelectionCanvas() { ParentWin = this, Width = 100, Height = 100 };
        //    Canvas.SetLeft(selCanvas, 300);
        //    Canvas.SetTop(selCanvas, 80);
        //    this.canvas0.Children.Add(selCanvas);
        //}

        SelectionCanvas curSelection;
        double canvasLeft, canvasTop;
        Point point, point4shape, centerpoint;
        dynamic curshape = null;
        DrawState _drawState = DrawState.None;
        private void canvas0_MouseDown(object sender, MouseButtonEventArgs e)
        {
            point = e.GetPosition(canvas0);

            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.LeftButton == MouseButtonState.Pressed && _drawState == DrawState.None)
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

                    _drawState = DrawState.Drawing;
                }
                else if (e.LeftButton == MouseButtonState.Pressed && _drawState == DrawState.Drawing)
                {
                    if (cmbShape.SelectedIndex == 3)
                    {
                        double _x = point.X - point4shape.X;
                        double _y = point.Y - point4shape.Y;

                        curshape.Points.Add(new Point(_x, _y));
                        polycount = curshape.Points.Count;
                    }
                }
                else if (e.LeftButton == MouseButtonState.Pressed && _drawState == DrawState.Selecting)
                {
                    point4shape = e.GetPosition(canvas0);

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

                        //canvasLeft = Canvas.GetLeft(curshape);
                        //canvasTop = Canvas.GetTop(curshape);
                        System.Diagnostics.Debug.WriteLine($"MouseDown -> Rectangle canvasLeft: {canvasLeft}, canvasTop: {canvasTop}");

                        if (Keyboard.Modifiers == ModifierKeys.Control)
                        {
                            curSelection = new SelectionCanvas() { ParentWin = this, RenderTransform = CreateTransformGroup(), RenderTransformOrigin = new Point(0.5, 0.5) };
                            curSelection.MouseEnteredRotationRange += Selection_MouseEnteredRotationRange;
                            curSelection.MouseLeftRotationRange += Selection_MouseLeftRotationRange;
                            curSelection.MouseEnteredResizeRange += Selection_MouseEnteredResizeRange;
                            curSelection.MouseLeftResizeRange += Selection_MouseLeftResizeRange;

                            UpdateSelection(ref curSelection, ref curshape);
                        }
                        else
                        {
                            if (GetCountOfSelection() > 0)
                            {
                                UpdateSelection(ref curSelection, ref curshape, Visibility.Collapsed);

                                curSelection = new SelectionCanvas() { ParentWin = this, RenderTransform = CreateTransformGroup(), RenderTransformOrigin = new Point(0.5, 0.5) };
                                curSelection.MouseEnteredRotationRange += Selection_MouseEnteredRotationRange;
                                curSelection.MouseLeftRotationRange += Selection_MouseLeftRotationRange;
                                curSelection.MouseEnteredResizeRange += Selection_MouseEnteredResizeRange;
                                curSelection.MouseLeftResizeRange += Selection_MouseLeftResizeRange;

                                UpdateSelection(ref curSelection, ref curshape);
                            }
                            else
                            {
                                curSelection = new SelectionCanvas() { ParentWin = this, RenderTransform = CreateTransformGroup(), RenderTransformOrigin = new Point(0.5, 0.5) };
                                curSelection.MouseEnteredRotationRange += Selection_MouseEnteredRotationRange;
                                curSelection.MouseLeftRotationRange += Selection_MouseLeftRotationRange;
                                curSelection.MouseEnteredResizeRange += Selection_MouseEnteredResizeRange;
                                curSelection.MouseLeftResizeRange += Selection_MouseLeftResizeRange;

                                UpdateSelection(ref curSelection, ref curshape);
                            }
                        }
                    }
                    else
                    {
                        if (e.OriginalSource is Canvas)
                        {
                            UpdateSelection(ref curSelection, ref curshape, Visibility.Collapsed);
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

                                //point4shape = e.GetPosition(canvas0);
                                canvasLeft = Canvas.GetLeft(curSelection);
                                canvasTop = Canvas.GetTop(curSelection);

                                System.Diagnostics.Debug.WriteLine($"canvas0_MouseDown -> Border -> canvasLeft: {canvasLeft}, canvasTop: {canvasTop}");
                            }
                        }
                    }
                }
                else if (e.LeftButton == MouseButtonState.Pressed && _drawState == DrawState.RotateOver)
                {
                    double _x = Canvas.GetLeft(curSelection) + curSelection.Width / 2;
                    double _y = Canvas.GetTop(curSelection) + curSelection.Height / 2;

                    centerpoint = new Point(_x, _y);
                    
                    _drawState = DrawState.Rotating;
                    System.Diagnostics.Debug.WriteLine($"canvas0_MouseDown -> _drawState.RotateOver -> _drawState: {_drawState}");
                }
                else if (e.LeftButton == MouseButtonState.Pressed && _drawState == DrawState.ResizeOver)
                {
                    _drawState = DrawState.Resizing;
                    System.Diagnostics.Debug.WriteLine($"canvas0_MouseDown -> _drawState.ResizeOver -> _drawState: {_drawState}");
                }
            }
        }

        #region Event from Selection whether the mouse Enter or Leave on Rotation or Resize range
        private void Selection_MouseEnteredRotationRange(object source, MouseEventArgs e)
        {
            if ((bool)btnSelect.IsChecked && _drawState == DrawState.Selecting)
            {
                Rectangle rect = (Rectangle)e.OriginalSource;
                System.Diagnostics.Debug.WriteLine($"Selection_MouseEnteredRotationRange -> Name:{rect.Name}");

                MemoryStream _mem = new MemoryStream(Properties.Resources.ArrowRotate);
                canvas0.Cursor = new Cursor(_mem);

                _drawState = DrawState.RotateOver;
            }
        }

        private void Selection_MouseLeftRotationRange(object source, MouseEventArgs e)
        {
            if ((bool)btnSelect.IsChecked && _drawState == DrawState.RotateOver)
            {
                Rectangle rect = (Rectangle)e.OriginalSource;
                System.Diagnostics.Debug.WriteLine($"Selection_MouseLeftRotationRange -> Name:{rect.Name} ");

                MemoryStream _mem = new MemoryStream(Properties.Resources.ArrowSelectMove);
                canvas0.Cursor = new Cursor(_mem);

                _drawState = DrawState.Selecting;
            }
        }

        private void Selection_MouseEnteredResizeRange(object source, MouseEventArgs e)
        {
            if ((bool)btnSelect.IsChecked && _drawState == DrawState.Selecting)
            {
                Rectangle rect = (Rectangle)e.OriginalSource;
                System.Diagnostics.Debug.WriteLine($"Selection_MouseEnteredResizeRange -> Name:{rect.Name} ");

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

                _drawState = DrawState.ResizeOver;
            }
        }

        private void Selection_MouseLeftResizeRange(object source, MouseEventArgs e)
        {
            if ((bool)btnSelect.IsChecked && _drawState == DrawState.ResizeOver)
            {
                Rectangle rect = (Rectangle)e.OriginalSource;
                System.Diagnostics.Debug.WriteLine($"Selection_MouseLeftResizeRange -> Name:{rect.Name} ");

                MemoryStream _mem = new MemoryStream(Properties.Resources.ArrowSelectMove);
                canvas0.Cursor = new Cursor(_mem);

                _drawState = DrawState.Selecting;
            }
        }
        #endregion

        int polycount = 0;
        private void canvas0_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (curshape == null) return;

            if (e.ChangedButton == MouseButton.Left && _drawState == DrawState.Drawing)
            {
                if (cmbShape.SelectedIndex == 1 || cmbShape.SelectedIndex == 2 || cmbShape.SelectedIndex == 4)
                {
                    CompletDrawing();
                }
            }
            else if (e.ChangedButton == MouseButton.Left && _drawState == DrawState.Rotating)
            {
                if ((bool)btnSelect.IsChecked)
                {
                    _drawState = DrawState.RotateOver;
                    System.Diagnostics.Debug.WriteLine($"canvas0_MouseUp -> _drawState:{_drawState}");
                }
            }
            else if (e.ChangedButton == MouseButton.Left && _drawState == DrawState.Resizing)
            {
                if ((bool)btnSelect.IsChecked)
                {
                    _drawState = DrawState.ResizeOver;
                    System.Diagnostics.Debug.WriteLine($"canvas0_MouseUp -> _drawState:{_drawState}");
                }
            }

            if (e.ChangedButton == MouseButton.Right && _drawState == DrawState.Drawing)
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

            //System.Diagnostics.Debug.WriteLine($"canvas0_MouseMove -> e.LeftButton: {e.LeftButton}, _drawState: {_drawState}");

            point = e.GetPosition(canvas0);

            if (_drawState == DrawState.Drawing)
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
            else if (e.LeftButton == MouseButtonState.Pressed && _drawState == DrawState.Selecting)
            {
                if (curSelection == null) return;

                double _leftMoved = point.X - point4shape.X;
                double _topMoved = point.Y - point4shape.Y;

                Canvas.SetLeft(curSelection, canvasLeft + _leftMoved);
                Canvas.SetTop(curSelection, canvasTop + _topMoved);

                //UpdateSelection(ref curSelection, ref curshape, Visibility.Visible);

                //System.Diagnostics.Debug.WriteLine($"canvas0_MouseMove -> e.LeftButton: {e.LeftButton}, _drawState: {_drawState}");
                System.Diagnostics.Debug.WriteLine($"canvas0_MouseMove -> Selecting -> _leftMoved: {_leftMoved}, _topMoved: {_topMoved}");
            }
            else if (e.LeftButton == MouseButtonState.Pressed && _drawState == DrawState.Rotating)
            {
                if (curSelection == null) return;

                double radians = Math.Atan((point.Y - centerpoint.Y) / (point.X - centerpoint.X));
                double angle = radians * 180 / Math.PI;

                if ((point.X - centerpoint.X) < 0)
                {
                    angle += 180;
                }

                if (angle >= 360)
                    angle = 0;
                
                TransformGroup tfg = curSelection.RenderTransform as TransformGroup;
                RotateTransform rt = tfg.Children[2] as RotateTransform;
                rt.Angle = angle;

                System.Diagnostics.Debug.WriteLine($"Rotate_Shape_MouseMove -> Rotating: {_drawState}");
            }
            else if (e.LeftButton == MouseButtonState.Pressed && _drawState == DrawState.Resizing)
            {
                if (curSelection == null) return;

                System.Diagnostics.Debug.WriteLine($"Rotate_Shape_MouseMove -> Resizing: {_drawState}");

                //var source = e.OriginalSource as Rectangle;
                //if (source.Parent is Canvas)
                //{
                //    var canvas = source.Parent as Canvas;
                //    if (canvas.Name == "canvasLT")
                //    {
                //        GetWidthHeightSelectRectOfPoly(curPolyline, out double _width, out double _height, out double _maxX, out double _maxY, out double _minX, out double _minY);

                //        double _x = Canvas.GetLeft(curshape) + _width / 2;
                //        double _y = Canvas.GetTop(curshape) + _height / 2;

                //        point4shape = e.GetPosition(canvas0);
                //        centerpoint = new Point(_x, _y);
                //        _drawState = DrawState.Rotating;
                //    }
                //}
            }

            //System.Diagnostics.Debug.WriteLine($"canvas0_MouseMove -> e.LeftButton: {e.LeftButton}, _drawState: {_drawState}");
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (curshape == null) return;

            if (e.ChangedButton == MouseButton.Left && _drawState == DrawState.Drawing)
            {
                if (cmbComplete.SelectedIndex == 2)
                {
                    CompletDrawing();
                }

                if (cmbShape.SelectedIndex == 3)
                {
                    CompletDrawing();
                }
            }
        }

        private TransformGroup CreateTransformGroup()
        {
            TransformGroup _group = new TransformGroup();
            _group.Children.Add(new ScaleTransform());
            _group.Children.Add(new SkewTransform());
            _group.Children.Add(new RotateTransform() { CenterX = 25, CenterY = 50 });
            _group.Children.Add(new TranslateTransform());

            return _group;
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
            _drawState = DrawState.None;
        }

        private void UpdateSelection(ref SelectionCanvas selection, ref dynamic shape, Visibility visibility = Visibility.Visible)
        {
            if (visibility == Visibility.Collapsed)
            {
                if (selection == null) return;

                // Shape is deselected.

                double _width = 0;
                double _height = 0;
                double _left = 0;
                double _top = 0;

                dynamic _shape = GetShapeFromSelection(ref selection);

                if (_shape is Line)
                {
                    _width = _shape.X2 - _shape.X1;
                    _height = _shape.Y2 - _shape.Y1;

                    _left = Canvas.GetLeft(selection);
                    _top = Canvas.GetTop(selection);

                    if (_width < 0) _left = _left - _width;
                    if (_height < 0) _top = _top - _height;

                    TransformGroup tfg = _shape.RenderTransform as TransformGroup;
                    RotateTransform rt = tfg.Children[2] as RotateTransform;

                    TransformGroup tfg1 = selection.RenderTransform as TransformGroup;
                    RotateTransform rt1 = tfg1.Children[2] as RotateTransform;
                    rt.Angle = rt1.Angle;
                }
                else if (_shape is Rectangle || _shape is Ellipse)
                {
                    _width = _shape.Width;
                    _height = _shape.Height;

                    _left = Canvas.GetLeft(selection);
                    _top = Canvas.GetTop(selection);

                    var tfg = _shape.RenderTransform as TransformGroup;
                    var sct = tfg.Children[0] as ScaleTransform;
                    if (sct.ScaleX == -1)
                    {
                        _left = _left + _width;
                    }
                    if (sct.ScaleY == -1)
                    {
                        _top = _top + _height;
                    }

                    //TransformGroup tfg = _shape.RenderTransform as TransformGroup;
                    var rt = tfg.Children[2] as RotateTransform;

                    var tfg1 = selection.RenderTransform as TransformGroup;
                    var rt1 = tfg1.Children[2] as RotateTransform;
                    rt.Angle = rt1.Angle;
                }
                else if (_shape is Polyline)
                {
                    GetWidthHeightSelectRectOfPoly(_shape, ref _width, ref _height, out double _maxX, out double _maxY, out double _minX, out double _minY);

                    TransformGroup tfg = _shape.RenderTransform as TransformGroup;
                    var rt = tfg.Children[2] as RotateTransform;

                    var tfg1 = selection.RenderTransform as TransformGroup;
                    var rt1 = tfg1.Children[2] as RotateTransform;
                    rt.Angle = rt1.Angle;

                    _left = Canvas.GetLeft(selection) - _minX;
                    _top = Canvas.GetTop(selection) - _minY;
                }
                
                Canvas.SetLeft(_shape, _left + 15);
                Canvas.SetTop(_shape, _top + 15);

                RemoveShape(selection);
                selection.Visibility = Visibility.Collapsed;
                selection = null;

                AddShape(_shape);
                shape = null;
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
                    GetWidthHeightSelectRectOfPoly(shape, ref _width, ref _height, out double _maxX, out double _maxY, out double _minX, out double _minY);

                    selection.Width = _width + 30;
                    selection.Height = _height + 30;

                    double __left = Canvas.GetLeft(shape);
                    double __top = Canvas.GetTop(shape);

                    _left = __left + _minX;
                    _top = __top + _minY;
                }
                System.Diagnostics.Debug.WriteLine($"canvas0_MouseDown -> UpdateSelection -> canvasLeft: {Canvas.GetLeft(selection)}, canvasTop: {Canvas.GetTop(selection)}");

                var tfg = shape.RenderTransform as TransformGroup;
                var rt = tfg.Children[2] as RotateTransform;

                var tfg1 = selection.RenderTransform as TransformGroup;
                var rt1 = tfg1.Children[2] as RotateTransform;

                rt1.Angle = rt.Angle;

                Canvas.SetLeft(selection, _left - 15);
                Canvas.SetTop(selection, _top - 15);
                selection.Visibility = Visibility.Visible;

                RemoveShape(shape);
                MoveShape2Selection(selection, shape);
                //shape = null;
                AddShape(selection);
                //curSelection = selection;

                canvasLeft = Canvas.GetLeft(selection);
                canvasTop = Canvas.GetTop(selection);

                System.Diagnostics.Debug.WriteLine($"canvas0_MouseDown -> UpdateSelection -> canvasLeft: {Canvas.GetLeft(curSelection)}, canvasTop: {Canvas.GetTop(curSelection)}");
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

        public void GetWidthHeightSelectRectOfPoly(Polyline shape, ref double _width, ref double _height,
            out double _maxX, out double _maxY, out double _minX, out double _minY)
        {
            _maxX = _maxY = _minX = _minY = 0;

            Point[] _points = new Point[shape.Points.Count];
            shape.Points.CopyTo(_points, 0);

            _minX = _points.Min(X1 => X1.X);
            _maxX = _points.Max(X1 => X1.X);
            _minY = _points.Min(Y1 => Y1.Y);
            _maxY = _points.Max(Y1 => Y1.Y);

            _width = _maxX - _minX;
            _height = _maxY - _minY;
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = sender as ToggleButton;

            if ((bool)btn.IsChecked)
            {
                SelectModeOnOff(SelectMode.ON);
                //UpdateSelection(ref curSelection, ref curshape, Visibility.Visible);
            }
            else
            {
                SelectModeOnOff(SelectMode.OFF);
                UpdateSelection(ref curSelection, ref curshape, Visibility.Collapsed);
            }
        }

        private void SelectModeOnOff(SelectMode mode)
        {
            switch (mode)
            {
                case SelectMode.ON:
                    MemoryStream _mem = new MemoryStream(Properties.Resources.ArrowSelectMove);
                    canvas0.Cursor = new Cursor(_mem);
                    _drawState = DrawState.Selecting;
                    cmbShape.SelectedIndex = 0;
                    break;
                case SelectMode.OFF:
                    //if (curSelection == null) return;

                    //curshape = curSelection.Shape;
                    //curSelection.RemoveShape();
                    //RemoveShape(curSelection);
                    //AddShape(curshape);
                    _drawState = DrawState.None;
                    canvas0.Cursor = Cursors.Arrow;
                    break;
            }
        }

        private void cmbShape_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox editor = sender as ComboBox;

            if (!editor.IsMouseOver) return;

            btnSelect.IsChecked = false;

            SelectModeOnOff(SelectMode.OFF);
            UpdateSelection(ref curSelection, ref curshape, Visibility.Collapsed);
        }

        public void SetCursor(MouseArrowState _state, SelectionCanvas _selection)
        {
            if (_drawState == DrawState.Selecting)
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

        private void MoveShape2Selection(SelectionCanvas selection, dynamic shape)
        {
            selection.AddShape(shape);
        }

        private dynamic GetShapeFromSelection(ref SelectionCanvas selection)
        {
            dynamic shape = selection.Shape;
            selection.RemoveShape();
            return shape;
        }

        private int GetCountOfSelection()
        {
            return canvas0.Children.OfType<SelectionCanvas>().Count();
        }

        private SelectionCanvas Get1stSelectionCanvas()
        {
            return canvas0.Children.OfType<SelectionCanvas>().Single();
        }

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
