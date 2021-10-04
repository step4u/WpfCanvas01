using Jeff.Defines;
using Jeff.ViewModels;
using System;
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


        int zindex;
        //PointCollection polypoints;
        double X1, X2, Y1, Y2;
        double canvasLeft, canvasTop;
        Point point, point4shape;
        dynamic curshape;
        DrawState _drawState = DrawState.None;
        private void canvas0_MouseDown(object sender, MouseButtonEventArgs e)
        {
            point = e.GetPosition(canvas0);

            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.LeftButton == MouseButtonState.Pressed && _drawState == DrawState.None)
                {
                    if (cmbShape.SelectedIndex == 0) return;

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
                                StrokeDashArray = (DoubleCollection)cmbLineKinds.SelectedValue,
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

                    point4shape = new Point(point.X, point.Y);
                    Canvas.SetLeft(newshape, point.X);
                    Canvas.SetTop(newshape, point.Y);
                    newshape.RenderTransform = CreateTransformGroup();
                    curshape = newshape;
                    canvas0.Children.Add(newshape);

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
                    if (e.OriginalSource is Canvas)
                    {
                        UpdateSelectShape(curshape, Visibility.Collapsed);
                    }
                    else if (e.OriginalSource is Border)
                    {
                        point4shape = e.GetPosition(canvas0);
                        canvasLeft = Canvas.GetLeft(curshape);
                        canvasTop = Canvas.GetTop(curshape);
                    }
                    else
                    {
                        if (e.OriginalSource is Line)
                        {
                            curshape = (Line)e.OriginalSource;
                            //X1 = curshape.X1;
                            //X2 = curshape.X2;
                            //Y1 = curshape.Y1;
                            //Y2 = curshape.Y2;
                        }
                        else if (e.OriginalSource is Rectangle)
                        {
                            curshape = (Rectangle)e.OriginalSource;
                        }
                        else if (e.OriginalSource is Polyline)
                        {
                            curshape = (Polyline)e.OriginalSource;
                            //polypoints = curshape.Points;
                        }
                        else if (e.OriginalSource is Ellipse)
                        {
                            curshape = (Ellipse)e.OriginalSource;
                        }

                        point4shape = e.GetPosition(canvas0);
                        canvasLeft = Canvas.GetLeft(curshape);
                        canvasTop = Canvas.GetTop(curshape);

                        UpdateSelectShape(curshape);
                    }

                    
                    //zindex = Canvas.GetZIndex(curshape);
                    //Canvas.SetZIndex(curshape, 9999);
                }
            }
        }

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
            //else if (e.ChangedButton == MouseButton.Left && _drawState == DrawState.Selecting)
            //{
            //    if (curshape != null)
            //    {
            //        //point4shape = e.GetPosition(canvas0);
            //        //Canvas.SetZIndex(curshape, zindex);
            //        //curshape = null;
            //    }
            //}

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
            if (curshape == null) return;

            point = e.GetPosition(canvas0);

            if (_drawState == DrawState.Drawing)
            {
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

                //System.Diagnostics.Debug.WriteLine($"canvas0_MouseMove -> curline - {curshape.X1},{curshape.Y1}:{curshape.X2},{curshape.Y2}");
            }
            else if (e.LeftButton == MouseButtonState.Pressed && _drawState == DrawState.Selecting)
            {
                double _leftMoved = point.X - point4shape.X;
                double _topMoved = point.Y - point4shape.Y;

                //if (curshape is Line)
                //{
                //    curshape.X1 = X1 + _leftMoved;
                //    curshape.Y1 = Y1 + _topMoved;
                //    curshape.X2 = X2 + _leftMoved;
                //    curshape.Y2 = Y2 + _topMoved;
                //}
                //else if (curshape is Polyline)
                //{
                //    PointCollection _points = curshape.Points;
                //    PointCollection _newPoints = new PointCollection();
                //    for (int i = 0; i < polypoints.Count; i++)
                //    {
                //        Point _polypoint = polypoints[i];
                //        Point _shapepoint = _points[i];
                //        _shapepoint.X = _polypoint.X + _leftMoved;
                //        _shapepoint.Y = _polypoint.Y + _topMoved;
                //        _newPoints.Add(_shapepoint);
                //    }
                //    curshape.Points = _newPoints;
                //}
                //else
                //{
                    Canvas.SetLeft(curshape, canvasLeft + _leftMoved);
                    Canvas.SetTop(curshape, canvasTop + _topMoved);
                //}

                UpdateSelectShape(curshape, Visibility.Visible);
            }
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
            _group.Children.Add(new RotateTransform());
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

            curshape = null;
            canvas0.Cursor = Cursors.Arrow;

            _drawState = DrawState.None;
        }

        private void UpdateSelectShape(dynamic shape, Visibility visibility = Visibility.Visible)
        {
            if (visibility == Visibility.Collapsed)
            {
                selectionCanvas.Visibility = visibility;
            }
            else
            {
                if (shape is Line)
                {
                    selectionCanvas.Width = Math.Abs(shape.X2 - shape.X1);
                    selectionCanvas.Height = Math.Abs(shape.Y2 - shape.Y1);
                    Canvas.SetLeft(selectionCanvas, Canvas.GetLeft(shape));
                    Canvas.SetTop(selectionCanvas, Canvas.GetTop(shape));
                    selectionCanvas.Visibility = visibility;
                }
                else if (shape is Rectangle || shape is Ellipse)
                {
                    selectionCanvas.Width = shape.Width;
                    selectionCanvas.Height = shape.Height;
                    Canvas.SetLeft(selectionCanvas, Canvas.GetLeft(shape));
                    Canvas.SetTop(selectionCanvas, Canvas.GetTop(shape));
                    selectionCanvas.Visibility = visibility;
                }
                else if (shape is Polyline)
                {
                    Point[] _points = new Point[shape.Points.Count];
                    shape.Points.CopyTo(_points, 0);

                    var _minX = _points.Min(X1 => X1.X);
                    var _maxX = _points.Max(X1 => X1.X);
                    var _minY = _points.Min(Y1 => Y1.Y);
                    var _maxY = _points.Max(Y1 => Y1.Y);

                    double _width = _maxX - _minX;
                    double _height = _maxY - _minY;

                    selectionCanvas.Width = _width;
                    selectionCanvas.Height = _height;
                    Canvas.SetLeft(selectionCanvas, Canvas.GetLeft(shape) + _minX);
                    Canvas.SetTop(selectionCanvas, Canvas.GetTop(shape) + _minY);
                    selectionCanvas.Visibility = visibility;
                }
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (curshape != null)
            {
                SelectMode();
            }

            if (_drawState == DrawState.Selecting)
            {
                UpdateSelectShape(curshape, Visibility.Collapsed);
                canvas0.Cursor = Cursors.Arrow;
                _drawState = DrawState.None;

                curshape = null;
            }
            else
            {
                MemoryStream _mem = new MemoryStream(Properties.Resources.ArrowSelectMove);
                canvas0.Cursor = new Cursor(_mem);
                _drawState = DrawState.Selecting;
                cmbShape.SelectedIndex = 0;
            }
        }

        private void SelectMode()
        {
            canvas0.Children.Remove(curshape);
            polycount = 0;
            _drawState = DrawState.None;
            curshape = null;
        }

        private void cmbShape_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox editor = sender as ComboBox;

            if (!editor.IsMouseOver) return;

            _drawState = DrawState.None;
        }


        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            RenderToDisk0();
            RenderToDisk1();
            RenderToDisk2();
        }

        private void canvas0_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                //foreach (var item in canvas0.Children)
                //{
                //    string propertyName = "RenderTransform";
                //    var trg = item.GetType().GetProperty(propertyName).GetValue(item) as TransformGroup;

                //    if (trg == null)
                //        continue;

                //    var sc = trg.Children[0] as ScaleTransform;
                //    sc.ScaleX *= 2;
                //    sc.ScaleY *= 2;
                //}
            }
            else
            {
                //foreach (var item in canvas0.Children)
                //{
                //    string propertyName = "RenderTransform";
                //    var trg = item.GetType().GetProperty(propertyName).GetValue(item) as TransformGroup;

                //    if (trg == null)
                //        continue;

                //    var sc = trg.Children[0] as ScaleTransform;
                //    sc.ScaleX /= 2;
                //    sc.ScaleY /= 2;
                //}
            }
        }

        private void RenderToDisk0()
        {
            System.Windows.Shapes.Path _path = (System.Windows.Shapes.Path)App.Current.FindResource("ArrowSelectMovePath");

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
            bitmap.Save("arrow0.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        private void RenderToDisk1()
        {
            //var path = new System.Windows.Shapes.Path
            //{
            //    Data = new EllipseGeometry(new Point(100, 100), 100, 100),
            //    Fill = Brushes.Blue
            //};

            System.Windows.Shapes.Path path = (System.Windows.Shapes.Path)App.Current.FindResource("ArrowSelectMovePath");

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
            bitmap.Save("arrow1.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        private void RenderToDisk2()
        {
            System.Windows.Shapes.Path _path = (System.Windows.Shapes.Path)App.Current.FindResource("ArrowSelectMovePath");
            string filepath = @"arrow2.png";
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
    }
}
