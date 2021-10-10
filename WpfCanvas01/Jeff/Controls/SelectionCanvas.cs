using Jeff.Converter;
using Jeff.Defines;
using Jeff.Events;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfCanvas01;

namespace Jeff.Controls
{
    public class SelectionCanvas : Canvas
    {
        public event MouseEnteredRotateRangeEventHandler MouseEnteredRotationRange;
        public event MouseLeftRotateRangeEventHandler MouseLeftRotationRange;

        public event MouseEnteredResizeRangeEventHandler MouseEnteredResizeRange;
        public event MouseLeftResizeRangeEventHandler MouseLeftResizeRange;

        public MainWindow ParentWin;
        Border border;
        Rectangle rectLM, rectLT, rectMT, rectRT, rectRM, rectRB, rectMB, rectLB;
        Rectangle rectRLT, rectRRT, rectRRB, rectRLB;
        private dynamic shape;
        string strokeColor = "#FF6998ED";
        Brush strokeBrush;

        public SelectionCanvas()
        {
            this.Loaded += SelectionCanvas_Loaded;
        }

        private void SelectionCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            InitBinding();
            InitEvents();
        }

        private void InitBinding()
        {
            strokeBrush = new BrushConverter().ConvertFromString(strokeColor) as Brush;

            this.Background = Brushes.Transparent;
            this.RenderTransform = CreateTransformGroup();
            this.RenderTransformOrigin = new Point(0.5, 0.5);

            InitBorder();
            InitRectanlges();
        }

        //Point point;
        public dynamic Shape
        {
            get { return shape; }
            set
            {
                shape = value;
                //DrawSahpe();
            }
        }

#if false
        private void DrawSahpe()
        {
            double _width = 0;
            double _height = 0;
            double _left = 15;
            double _top = 15;

            if (shape is Line)
            {
                _width = shape.X2 - shape.X1;
                _height = shape.Y2 - shape.Y1;

                if (_width < 0) _left = _left - _width;
                if (_height < 0) _top = _top - _height;
            }
            else if (shape is Rectangle || shape is Ellipse)
            {
                _width = shape.Width;
                _height = shape.Height;

                var tfg = shape.RenderTransform;
                var sct = tfg.Children[0];
                if (sct.ScaleX == -1)
                {
                    _left = _left + _width;
                }
                if (sct.ScaleY == -1)
                {
                    _top = _top + _height;
                }
            }
            else if (shape is Polyline)
            {
                ParentWin.GetWidthHeightSelectRectOfPoly(shape, ref _width, ref _height, out double _maxX, out double _maxY, out double _minX, out double _minY);

                _left = _left - _minX;
                _top = _top - _minY;
            }

            Canvas.SetLeft(shape, _left);
            Canvas.SetTop(shape, _top);
        }
#endif

        private void InitEvents()
        {
            rectRLT.MouseEnter += RectRotate_MouseEnter;
            rectRRT.MouseEnter += RectRotate_MouseEnter;
            rectRRB.MouseEnter += RectRotate_MouseEnter;
            rectRLB.MouseEnter += RectRotate_MouseEnter;

            rectRLT.MouseLeave += RectRotate_MouseLeave;
            rectRRT.MouseLeave += RectRotate_MouseLeave;
            rectRRB.MouseLeave += RectRotate_MouseLeave;
            rectRLB.MouseLeave += RectRotate_MouseLeave;

            rectLT.MouseEnter += RectResize_MouseEnter;
            rectMT.MouseEnter += RectResize_MouseEnter;
            rectRT.MouseEnter += RectResize_MouseEnter;
            rectRM.MouseEnter += RectResize_MouseEnter;
            rectRB.MouseEnter += RectResize_MouseEnter;
            rectMB.MouseEnter += RectResize_MouseEnter;
            rectLB.MouseEnter += RectResize_MouseEnter;
            rectLM.MouseEnter += RectResize_MouseEnter;

            rectLT.MouseLeave += RectResize_MouseLeave;
            rectMT.MouseLeave += RectResize_MouseLeave;
            rectRT.MouseLeave += RectResize_MouseLeave;
            rectRM.MouseLeave += RectResize_MouseLeave;
            rectRB.MouseLeave += RectResize_MouseLeave;
            rectMB.MouseLeave += RectResize_MouseLeave;
            rectLB.MouseLeave += RectResize_MouseLeave;
            rectLM.MouseLeave += RectResize_MouseLeave;
        }

        private void RectRotate_MouseEnter(object sender, MouseEventArgs e)
        {
            if (MouseEnteredRotationRange != null)
                MouseEnteredRotationRange(sender, e);
        }

        private void RectRotate_MouseLeave(object sender, MouseEventArgs e)
        {
            if (MouseLeftRotationRange != null)
                MouseLeftRotationRange(sender, e);
        }

        private void RectResize_MouseEnter(object sender, MouseEventArgs e)
        {
            if (MouseEnteredResizeRange != null)
                MouseEnteredResizeRange(sender, e);
        }

        private void RectResize_MouseLeave(object sender, MouseEventArgs e)
        {
            if (MouseLeftResizeRange != null)
                MouseLeftResizeRange(sender, e);
        }

        private void DetectMouseEnterPosition(out bool isOnPosition, out MousePosition position, Point cpoint)
        {
            isOnPosition = false;
            position = MousePosition.None;

            if (cpoint.X >= 0 && cpoint.X < 10
                && cpoint.Y >= 0 && cpoint.Y < 10)
            {
                isOnPosition = true;
                position = MousePosition.R_LT;
            }
            else if (cpoint.X > this.Width - 10 && cpoint.X <= this.Width
                && cpoint.Y >= 0 && cpoint.Y < 10)
            {
                isOnPosition = true;
                position = MousePosition.R_RT;
            }
            else if (cpoint.X > this.Width - 10 && cpoint.X <= this.Width
                && cpoint.Y > this.Height - 10 && cpoint.Y <= this.Height)
            {
                isOnPosition = true;
                position = MousePosition.R_RB;
            }
            else if (cpoint.X >= 0 && cpoint.X < 10
                && cpoint.Y > this.Height - 10 && cpoint.Y <= this.Height)
            {
                isOnPosition = true;
                position = MousePosition.R_LB;
            }
            else if (cpoint.X > 10 && cpoint.X <= 15
                && cpoint.Y > 10 && cpoint.Y <= 15)
            {
                isOnPosition = true;
                position = MousePosition.S_LT;
            }
            else if (cpoint.X > 10 && cpoint.X <= 15
                && cpoint.Y > 10 && cpoint.Y <= 15)
            {
                isOnPosition = true;
                position = MousePosition.S_LT;
            }
        }

        private void ChangeMouseCursor(bool isOnPosition, MousePosition position)
        {
            if (isOnPosition)
            {
                switch (position)
                {
                    case MousePosition.None:
                        break;
                }
            }
            else
            {
                Canvas _parent = this.Parent as Canvas;
            }
        }

        private void InitBorder()
        {
            border = new Border();
            border.Background = Brushes.LightBlue;
            border.BorderBrush = Brushes.Blue;
            border.BorderThickness = new Thickness(1);
            border.Opacity = 0.3;

            Canvas.SetLeft(border, 15);
            Canvas.SetTop(border, 15);

            Binding bind = new Binding("Width");
            bind.Source = this;
            bind.Converter = new GetActualSize();
            bind.ConverterParameter = 30;
            border.SetBinding(Border.WidthProperty, bind);

            bind = new Binding("Height");
            bind.Source = this;
            bind.Converter = new GetActualSize();
            bind.ConverterParameter = 30;
            border.SetBinding(Border.HeightProperty, bind);

            Add(border);
        }

        private void InitRectanlges()
        {
            // 좌중(LM)
            RectConfigValues config = new RectConfigValues() { name = "LM", left = 11 };
            rectLM = CreateResizeRect(config);

            // 좌상(LT)
            config = new RectConfigValues() { name = "LT", left = 11, top = 11 };
            rectLT = CreateResizeRect(config);

            // 중상(MT)
            config = new RectConfigValues() { name = "MT", top = 11 };
            rectMT = CreateResizeRect(config);

            // 우상(RT)
            config = new RectConfigValues() { name = "RT", left = 16, top = 11 };
            rectRT = CreateResizeRect(config);

            // 우중(RM)
            config = new RectConfigValues() { name = "RM", left = 16 };
            rectRM = CreateResizeRect(config);

            // 우하(RB)
            config = new RectConfigValues() { name = "RB", left = 16, top = 16 };
            rectRB = CreateResizeRect(config);

            // 하중(BM)
            config = new RectConfigValues() { name = "BM", top = 16 };
            rectMB = CreateResizeRect(config);

            // 좌하(LB)
            config = new RectConfigValues() { name = "LB", left = 11, top = 16 };
            rectLB = CreateResizeRect(config);

            // 회전 좌상(RLT)
            config = new RectConfigValues() { name = "RLT", left = 0, top = 0, width = 11, height = 11 };
            rectRLT = CreateResizeRect(config);

            // 회전 우상(RRT)
            config = new RectConfigValues() { name = "RRT", left = 11, top = 0, width = 11, height = 11 };
            rectRRT = CreateResizeRect(config);

            // 회전 우하(RRB)
            config = new RectConfigValues() { name = "RRB", left = 11, top = 11, width = 11, height = 11 };
            rectRRB = CreateResizeRect(config);

            // 회전 좌하(RLB)
            config = new RectConfigValues() { name = "RLB", left = 0, top = 11, width = 11, height = 11 };
            rectRLB = CreateResizeRect(config);
        }

        private Rectangle CreateResizeRect(RectConfigValues config)
        {
            Binding bind;

            Rectangle rect = new Rectangle()
            {
                Name = config.name,
                Width = config.width,
                Height = config.height,
                Fill = Brushes.Transparent,
                Stroke = strokeBrush,
                StrokeThickness = 1,
                Stretch = Stretch.Fill
            };

            switch (config.name)
            {
                case "LM":
                    SetLeft(rect, config.left);
                    bind = new Binding(config.bindHeight);
                    bind.Source = this;
                    bind.Converter = new GetHalf();
                    bind.ConverterParameter = 5;
                    rect.SetBinding(Canvas.TopProperty, bind);
                    break;
                case "LT":
                    SetLeft(rect, config.left);
                    SetTop(rect, config.top);
                    break;
                case "MT":
                    bind = new Binding(config.bindWidth);
                    bind.Source = this;
                    bind.Converter = new GetHalf();
                    bind.ConverterParameter = 5;
                    rect.SetBinding(Canvas.LeftProperty, bind);
                    SetTop(rect, config.top);
                    break;
                case "RT":
                    SetLeft(rect, this.Width - config.left);
                    SetTop(rect, config.top);
                    break;
                case "RM":
                    SetLeft(rect, this.Width - config.left);
                    bind = new Binding(config.bindHeight);
                    bind.Source = this;
                    bind.Converter = new GetHalf();
                    bind.ConverterParameter = 5;
                    rect.SetBinding(Canvas.TopProperty, bind);
                    break;
                case "RB":
                    SetLeft(rect, this.Width - config.left);
                    SetTop(rect, this.Height - config.top);
                    break;
                case "BM":
                    bind = new Binding(config.bindWidth);
                    bind.Source = this;
                    bind.Converter = new GetHalf();
                    bind.ConverterParameter = 5;
                    rect.SetBinding(Canvas.LeftProperty, bind);
                    SetTop(rect, this.Height - config.top);
                    break;
                case "LB":
                    SetLeft(rect, config.left);
                    SetTop(rect, this.Height - config.top);
                    break;
                case "RLT":
                    SetLeft(rect, config.left);
                    SetTop(rect, config.top);
                    break;
                case "RRT":
                    SetLeft(rect, this.Width - config.left);
                    SetTop(rect, config.top);
                    break;
                case "RRB":
                    SetLeft(rect, this.Width - config.left);
                    SetTop(rect, this.Height - config.top);
                    break;
                case "RLB":
                    SetLeft(rect, config.left);
                    SetTop(rect, this.Height - config.top);
                    break;
            }

            Add(rect);

            return rect;
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

        private void Add(UIElement element)
        {
            this.Children.Add(element);
        }

        private void Remove(UIElement element)
        {
            this.Children.Remove(element);
        }

        public void AddShape(dynamic shape)
        {
            Shape = shape;
            Add(Shape);
        }

        public void RemoveShape()
        {
            Remove(Shape);
        }

        class RectConfigValues
        {
            public string name;
            public double left, top;
            public double width = 5;
            public double height = 5;
            public string bindWidth = "Width";
            public string bindHeight = "Height";
            public double converterParameter = 5;
            public DependencyProperty dpt = Canvas.TopProperty;
            public DependencyProperty dpl = Canvas.LeftProperty;
        }
    }

}
