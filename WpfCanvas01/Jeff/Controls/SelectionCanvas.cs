using Jeff.Converter;
using Jeff.Defines;
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

        Point point;
        public dynamic Shape
        {
            get
            {
                return shape;
            }
            set
            {
                shape = value;

                double _width = shape.X2 - shape.X1;
                double _heigh = shape.Y2 - shape.Y1;

                double _left = 15;
                double _top = 15;

                if (_width < 0) _left = _left - _width;
                if (_heigh < 0) _top = _top - _heigh;

                Canvas.SetLeft(shape, _left);
                Canvas.SetTop(shape, _top);
            }
        }

        private void InitEvents()
        {
            //this.MouseEnter += (s, e) => {
            //    var _parent = this.Parent as Canvas;
            //    prevcursor = _parent.Cursor;

            //    point = e.GetPosition(this);

            //    DetectMouseEnterPosition(out bool isOnPosition, out MousePosition position, point);
            //    ChangeMouseCursor(isOnPosition, position);
            //    System.Diagnostics.Debug.WriteLine($"MouseEnter -> isOnPosition:{isOnPosition}, MousePosition:{position}, _parent.Cursor:{_parent.Cursor}");
            //    System.Diagnostics.Debug.WriteLine($"MouseEnter -> border.Width:{border.Width}, border.Height:{border.Height}");
            //};

            //this.MouseLeave += (s, e) =>
            //{
            //    DetectMouseEnterPosition(out bool isOnPosition, out MousePosition position, point);

            //    if (!isOnPosition)
            //    {
            //        Canvas _parent = this.Parent as Canvas;
            //        if (_parent != null)
            //            _parent.Cursor = prevcursor;
            //    }
            //};

            rectRLT.MouseEnter += RectRotate_MouseEnter;
            rectRRT.MouseEnter += RectRotate_MouseEnter;
            rectRRB.MouseEnter += RectRotate_MouseEnter;
            rectRLB.MouseEnter += RectRotate_MouseEnter;

            rectRLT.MouseLeave += RectRotate_MouseLeave;
            rectRRT.MouseLeave += RectRotate_MouseLeave;
            rectRRB.MouseLeave += RectRotate_MouseLeave;
            rectRLB.MouseLeave += RectRotate_MouseLeave;
        }

        private void RectRotate_MouseEnter(object sender, MouseEventArgs e)
        {
            point = e.GetPosition(ParentWin.canvas0);

            System.Diagnostics.Debug.WriteLine($"SelectionCanvas -> RectRotate_MouseEnter -> point: {point.X}/{point.Y}");

            Rectangle rect = sender as Rectangle;

            switch (rect.Name)
            {
                case "RLT":
                    //ParentWin.SetCursor(MouseArrowState.)
                    break;
                case "RRT":
                    break;
                case "RRB":
                    break;
                case "RLB":
                    break;
            }
        }

        private void RectRotate_MouseLeave(object sender, MouseEventArgs e)
        {

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
                    bind = new Binding(config.bindWidth);
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
