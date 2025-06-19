using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UmlEditor
{
      
        
    public partial class MainWindow : Window
    {
        private Point _startPoint;
        private bool _isDragging = false;
        private FrameworkElement _selectedElement;
        private Point _dragStartPosition;
        private DeleteAdorner _deleteAdorner;
        private ResizeAdorner _resizeAdorner;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void ToolButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string toolType = button.Tag.ToString();
                FrameworkElement element = null;

                switch (toolType)
                {
                    case "Human":
                        element = CreateHumanFigure();
                        break;
                    case "Oval":
                        element = new Ellipse { Width = 80, Height = 50, Stroke = Brushes.Black, Fill = Brushes.White };
                        break;
                    case "Rectangle":
                        element = new Rectangle { Width = 100, Height = 60, Stroke = Brushes.Black, Fill = Brushes.White };
                        break;
                    case "Diamond":
                        element = new Polygon
                        {
                            Points = new PointCollection { new Point(50, 0), new Point(100, 30), new Point(50, 60), new Point(0, 30) },
                            Stroke = Brushes.Black,
                            Fill = Brushes.White
                        };
                        break;
                    case "Arrow":
                        element = CreateArrow();
                        break;
                    case "DiamondArrow":
                        element = CreateDiamondArrow();
                        break;
                }

                if (element != null)
                {
                    element.MouseLeftButtonDown += Element_MouseLeftButtonDown;
                    Canvas.SetLeft(element, 100);
                    Canvas.SetTop(element, 100);
                    DrawingCanvas.Children.Add(element);
                }
            }
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedElement != null)
            {
                // Удаляем элемент с холста
                DrawingCanvas.Children.Remove(_selectedElement);

                // Удаляем adorners
                var layer = AdornerLayer.GetAdornerLayer(_selectedElement);
                if (layer != null)
                {
                    if (_deleteAdorner != null)
                        layer.Remove(_deleteAdorner);
                    if (_resizeAdorner != null)
                        layer.Remove(_resizeAdorner);
                }

                // Сбрасываем ссылки
                _selectedElement = null;
                _deleteAdorner = null;
                _resizeAdorner = null;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteButton_Click(sender, e);
            }
        }

        private FrameworkElement CreateHumanFigure()
        {
            Canvas humanCanvas = new Canvas { Width = 40, Height = 60 };
            humanCanvas.Children.Add(new Ellipse { Width = 10, Height = 10, Stroke = Brushes.Black, Fill = Brushes.White, Margin = new Thickness(15, 0, 0, 0) });
            humanCanvas.Children.Add(new Line { X1 = 20, Y1 = 10, X2 = 20, Y2 = 30, Stroke = Brushes.Black, StrokeThickness = 2 });
            humanCanvas.Children.Add(new Line { X1 = 5, Y1 = 20, X2 = 35, Y2 = 20, Stroke = Brushes.Black, StrokeThickness = 2 });
            humanCanvas.Children.Add(new Line { X1 = 20, Y1 = 30, X2 = 10, Y2 = 50, Stroke = Brushes.Black, StrokeThickness = 2 });
            humanCanvas.Children.Add(new Line { X1 = 20, Y1 = 30, X2 = 30, Y2 = 50, Stroke = Brushes.Black, StrokeThickness = 2 });
            return humanCanvas;
        }

        private FrameworkElement CreateArrow()
        {
            Canvas arrowCanvas = new Canvas { Width = 100, Height = 30 };
            arrowCanvas.Children.Add(new Line { X1 = 10, Y1 = 15, X2 = 90, Y2 = 15, Stroke = Brushes.Black, StrokeThickness = 2 });
            arrowCanvas.Children.Add(new Polygon { Points = new PointCollection { new Point(90, 15), new Point(80, 10), new Point(80, 20) }, Stroke = Brushes.Black, Fill = Brushes.Black });
            return arrowCanvas;
        }

        private FrameworkElement CreateDiamondArrow()
        {
            Canvas diamondArrowCanvas = new Canvas { Width = 100, Height = 30 };
            diamondArrowCanvas.Children.Add(new Line { X1 = 10, Y1 = 15, X2 = 50, Y2 = 15, Stroke = Brushes.Black, StrokeThickness = 2 });
            diamondArrowCanvas.Children.Add(new Polygon { Points = new PointCollection { new Point(50, 15), new Point(70, 5), new Point(90, 15), new Point(70, 25) }, Stroke = Brushes.Black, Fill = Brushes.White });
            return diamondArrowCanvas;
        }

        private void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                SelectElement(element);
                _dragStartPosition = e.GetPosition(DrawingCanvas);
                _startPoint = e.GetPosition(element);
                element.CaptureMouse();
                _isDragging = true;
                e.Handled = true;
            }
        }


        private void SelectElement(FrameworkElement element)
        {
            // Удаляем предыдущие adorners
            if (_selectedElement != null)
            {
                _selectedElement.Effect = null;
                RemoveAdorners();
            }

            _selectedElement = element;

            if (_selectedElement != null)
            {
                // Добавляем эффект выделения
                _selectedElement.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Blue,
                    ShadowDepth = 0,
                    BlurRadius = 10
                };

                // Добавляем adorners
                _deleteAdorner = new DeleteAdorner(_selectedElement);
                _resizeAdorner = new ResizeAdorner(_selectedElement);

                var layer = AdornerLayer.GetAdornerLayer(_selectedElement);
                layer.Add(_deleteAdorner);
                layer.Add(_resizeAdorner);
            }
        }
        private void RemoveAdorners()
        {
            if (_selectedElement != null)
            {
                var layer = AdornerLayer.GetAdornerLayer(_selectedElement);
                if (layer != null)
                {
                    if (_deleteAdorner != null)
                        layer.Remove(_deleteAdorner);
                    if (_resizeAdorner != null)
                        layer.Remove(_resizeAdorner);
                }
            }
            _deleteAdorner = null;
            _resizeAdorner = null;
        }
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Canvas)
            {
                SelectElement(null);
                _startPoint = e.GetPosition(DrawingCanvas);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _selectedElement != null && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(DrawingCanvas);
                double left = Canvas.GetLeft(_selectedElement) + (currentPosition.X - _dragStartPosition.X);
                double top = Canvas.GetTop(_selectedElement) + (currentPosition.Y - _dragStartPosition.Y);

                Canvas.SetLeft(_selectedElement, left);
                Canvas.SetTop(_selectedElement, top);

                _dragStartPosition = currentPosition;

                // Обновляем положение adorners
                AdornerLayer.GetAdornerLayer(_selectedElement)?.Update();
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging && _selectedElement != null)
            {
                _selectedElement.ReleaseMouseCapture();
                _isDragging = false;
            }
        }


        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement element && element != DrawingCanvas)
            {
                SelectElement(element);
                e.Handled = true;
            }
        }

        public class ResizeAdorner : Adorner
        {
            private readonly Thumb topLeft, topRight, bottomLeft, bottomRight;
            private readonly VisualCollection visualChildren;

            public ResizeAdorner(UIElement adornedElement) : base(adornedElement)
            {
                visualChildren = new VisualCollection(this);

                topLeft = CreateResizeThumb(Cursors.SizeNWSE);
                topRight = CreateResizeThumb(Cursors.SizeNESW);
                bottomLeft = CreateResizeThumb(Cursors.SizeNESW);
                bottomRight = CreateResizeThumb(Cursors.SizeNWSE);

                topLeft.DragDelta += HandleTopLeft;
                topRight.DragDelta += HandleTopRight;
                bottomLeft.DragDelta += HandleBottomLeft;
                bottomRight.DragDelta += HandleBottomRight;
            }

            private Thumb CreateResizeThumb(Cursor cursor)
            {
                var thumb = new Thumb
                {
                    Width = 10,
                    Height = 10,
                    Cursor = cursor,
                    Style = Application.Current.Resources["ScrollBarThumbVertical"] as Style
                };
                visualChildren.Add(thumb);
                return thumb;
            }

            private void HandleTopLeft(object sender, DragDeltaEventArgs args)
            {
                if (AdornedElement is FrameworkElement element)
                {
                    element.Width = Math.Max(0, element.Width - args.HorizontalChange);
                    element.Height = Math.Max(0, element.Height - args.VerticalChange);
                    Canvas.SetLeft(element, Canvas.GetLeft(element) + args.HorizontalChange);
                    Canvas.SetTop(element, Canvas.GetTop(element) + args.VerticalChange);
                }
            }

            private void HandleTopRight(object sender, DragDeltaEventArgs args)
            {
                if (AdornedElement is FrameworkElement element)
                {
                    element.Width = Math.Max(0, element.Width + args.HorizontalChange);
                    element.Height = Math.Max(0, element.Height - args.VerticalChange);
                    Canvas.SetTop(element, Canvas.GetTop(element) + args.VerticalChange);
                }
            }

            private void HandleBottomLeft(object sender, DragDeltaEventArgs args)
            {
                if (AdornedElement is FrameworkElement element)
                {
                    element.Width = Math.Max(0, element.Width - args.HorizontalChange);
                    element.Height = Math.Max(0, element.Height + args.VerticalChange);
                    Canvas.SetLeft(element, Canvas.GetLeft(element) + args.HorizontalChange);
                }
            }

            private void HandleBottomRight(object sender, DragDeltaEventArgs args)
            {
                if (AdornedElement is FrameworkElement element)
                {
                    element.Width = Math.Max(0, element.Width + args.HorizontalChange);
                    element.Height = Math.Max(0, element.Height + args.VerticalChange);
                }
            }

            protected override int VisualChildrenCount => visualChildren.Count;
            protected override Visual GetVisualChild(int index) => visualChildren[index];

            protected override Size ArrangeOverride(Size finalSize)
            {
                double width = AdornedElement.DesiredSize.Width;
                double height = AdornedElement.DesiredSize.Height;

                topLeft.Arrange(new Rect(-5, -5, 10, 10));
                topRight.Arrange(new Rect(width - 5, -5, 10, 10));
                bottomLeft.Arrange(new Rect(-5, height - 5, 10, 10));
                bottomRight.Arrange(new Rect(width - 5, height - 5, 10, 10));

                return finalSize;
            }

            public void Remove()
            {
                var layer = AdornerLayer.GetAdornerLayer(AdornedElement);
                layer?.Remove(this);
            }
        }
    }
    public class DeleteAdorner : Adorner
    {
        private readonly Button _deleteButton;
        private readonly VisualCollection _visualChildren;

        public DeleteAdorner(UIElement adornedElement) : base(adornedElement)
        {
            _visualChildren = new VisualCollection(this);

            _deleteButton = new Button
            {
                Content = "✕",
                Width = 20,
                Height = 20,
                Background = Brushes.Red,
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                Padding = new Thickness(0),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(-10, -10, 0, 0),
                Cursor = Cursors.Hand
            };

            _deleteButton.Click += (sender, e) =>
            {
                if (AdornedElement is FrameworkElement element && element.Parent is Panel parent)
                {
                    parent.Children.Remove(element);
                }
            };

            _visualChildren.Add(_deleteButton);
        }

        protected override int VisualChildrenCount => _visualChildren.Count;
        protected override Visual GetVisualChild(int index) => _visualChildren[index];

        protected override Size ArrangeOverride(Size finalSize)
        {
            _deleteButton.Arrange(new Rect(finalSize));
            return finalSize;
        }
    }
}



