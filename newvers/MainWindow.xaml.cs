using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<UmlElement> _umlElements = new List<UmlElement>();
        private FrameworkElement _selectedElement;
        private DeleteAdorner _deleteAdorner;
        private ResizeAdorner _resizeAdorner;
        private Point _startPoint;
        private Point _dragStartPosition;
        private bool _isDragging;

        public MainWindow()
        {
            InitializeComponent();

            // Подключаем обработчики событий
            DrawingCanvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            DrawingCanvas.MouseMove += Canvas_MouseMove;
            DrawingCanvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
        }

        private void ToolButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string toolType)
            {
                var element = CreateUmlElement(toolType);
                if (element != null)
                {
                    element.Container.MouseLeftButtonDown += Element_MouseLeftButtonDown;
                    element.TextBlock.MouseLeftButtonDown += TextBlock_MouseLeftButtonDown;

                    Canvas.SetLeft(element.Container, 100);
                    Canvas.SetTop(element.Container, 100);
                    DrawingCanvas.Children.Add(element.Container);

                    _umlElements.Add(element);
                    SelectElement(element.Container);
                }
            }
        }

        private UmlElement CreateUmlElement(string toolType)
        {
            switch (toolType)
            {
                case "Human": return CreateHumanFigure();
                case "Oval": return CreateOval();
                case "Rectangle": return CreateRectangle();
                case "Diamond": return CreateDiamond();
                case "Arrow": return CreateArrow();
                case "DiamondArrow": return CreateDiamondArrow();
                default: return null;
            }
        }

        private UmlElement CreateHumanFigure()
        {
            var canvas = new Canvas { Width = 40, Height = 60 };

            // Голова
            canvas.Children.Add(new Ellipse
            {
                Width = 10,
                Height = 10,
                Stroke = Brushes.Black,
                Fill = Brushes.White,
                Margin = new Thickness(15, 0, 0, 0)
            });

            // Тело
            canvas.Children.Add(new Line
            {
                X1 = 20,
                Y1 = 10,
                X2 = 20,
                Y2 = 30,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            });

            // Руки
            canvas.Children.Add(new Line
            {
                X1 = 5,
                Y1 = 20,
                X2 = 35,
                Y2 = 20,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            });

            // Ноги
            canvas.Children.Add(new Line
            {
                X1 = 20,
                Y1 = 30,
                X2 = 10,
                Y2 = 50,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            });
            canvas.Children.Add(new Line
            {
                X1 = 20,
                Y1 = 30,
                X2 = 30,
                Y2 = 50,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            });

            var container = new Grid();
            container.Children.Add(canvas);

            var textBlock = new TextBlock
            {
                Text = "Актор",
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, -20),
                FontSize = 10,
                IsHitTestVisible = true
            };
            container.Children.Add(textBlock);

            return new UmlElement
            {
                Container = container,
                Visual = canvas,
                TextBlock = textBlock,
                Type = "Human",
                Text = "Актор"
            };
        }

        private UmlElement CreateOval()
        {
            var ellipse = new Ellipse
            {
                Width = 120,
                Height = 80,
                Stroke = Brushes.Black,
                Fill = Brushes.White
            };

            var container = new Grid();
            container.Children.Add(ellipse);

            var textBlock = new TextBlock
            {
                Text = "Вариант использования",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 12,
                IsHitTestVisible = true
            };
            container.Children.Add(textBlock);

            return new UmlElement
            {
                Container = container,
                Visual = ellipse,
                TextBlock = textBlock,
                Type = "Oval",
                Text = "Вариант использования"
            };
        }

        private UmlElement CreateRectangle()
        {
            var rectangle = new Rectangle
            {
                Width = 120,
                Height = 80,
                Stroke = Brushes.Black,
                Fill = Brushes.White
            };

            var container = new Grid();
            container.Children.Add(rectangle);

            var textBlock = new TextBlock
            {
                Text = "Класс",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 12,
                IsHitTestVisible = true
            };
            container.Children.Add(textBlock);

            return new UmlElement
            {
                Container = container,
                Visual = rectangle,
                TextBlock = textBlock,
                Type = "Rectangle",
                Text = "Класс"
            };
        }

        private UmlElement CreateDiamond()
        {
            var diamond = new Polygon
            {
                Points = new PointCollection
                {
                    new Point(60, 0),
                    new Point(120, 40),
                    new Point(60, 80),
                    new Point(0, 40)
                },
                Stroke = Brushes.Black,
                Fill = Brushes.White
            };

            var container = new Grid();
            container.Children.Add(diamond);

            var textBlock = new TextBlock
            {
                Text = "Решение",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 12,
                IsHitTestVisible = true
            };
            container.Children.Add(textBlock);

            return new UmlElement
            {
                Container = container,
                Visual = diamond,
                TextBlock = textBlock,
                Type = "Diamond",
                Text = "Решение"
            };
        }

        private UmlElement CreateArrow()
        {
            var canvas = new Canvas { Width = 100, Height = 30 };

            var line = new Line
            {
                X1 = 10,
                Y1 = 15,
                X2 = 90,
                Y2 = 15,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            var arrowHead = new Polygon
            {
                Points = new PointCollection
                {
                    new Point(90, 15),
                    new Point(80, 10),
                    new Point(80, 20)
                },
                Stroke = Brushes.Black,
                Fill = Brushes.Black
            };

            canvas.Children.Add(line);
            canvas.Children.Add(arrowHead);

            var container = new Grid();
            container.Children.Add(canvas);

            var textBlock = new TextBlock
            {
                Text = "Связь",
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, -20, 0, 0),
                FontSize = 10,
                IsHitTestVisible = true
            };
            container.Children.Add(textBlock);

            return new UmlElement
            {
                Container = container,
                Visual = canvas,
                TextBlock = textBlock,
                Type = "Arrow",
                Text = "Связь"
            };
        }

        private UmlElement CreateDiamondArrow()
        {
            var canvas = new Canvas { Width = 100, Height = 30 };

            var line = new Line
            {
                X1 = 10,
                Y1 = 15,
                X2 = 50,
                Y2 = 15,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            var diamondHead = new Polygon
            {
                Points = new PointCollection
                {
                    new Point(50, 15),
                    new Point(70, 5),
                    new Point(90, 15),
                    new Point(70, 25)
                },
                Stroke = Brushes.Black,
                Fill = Brushes.White
            };

            canvas.Children.Add(line);
            canvas.Children.Add(diamondHead);

            var container = new Grid();
            container.Children.Add(canvas);

            var textBlock = new TextBlock
            {
                Text = "Агрегация",
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, -20, 0, 0),
                FontSize = 10,
                IsHitTestVisible = true
            };
            container.Children.Add(textBlock);

            return new UmlElement
            {
                Container = container,
                Visual = canvas,
                TextBlock = textBlock,
                Type = "DiamondArrow",
                Text = "Агрегация"
            };
        }

        private void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid container)
            {
                var umlElement = _umlElements.FirstOrDefault(x => x.Container == container);
                if (umlElement == null) return;

                if (e.ClickCount == 2 && umlElement.TextBlock.IsMouseOver)
                {
                    StartTextEditing(umlElement);
                    e.Handled = true;
                    return;
                }

                SelectElement(container);
                StartDrag(container, e);
                e.Handled = true;
            }
        }

        private void StartDrag(FrameworkElement element, MouseButtonEventArgs e)
        {
            _dragStartPosition = e.GetPosition(DrawingCanvas);
            _startPoint = e.GetPosition(element);
            element.CaptureMouse();
            _isDragging = true;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && sender is TextBlock textBlock)
            {
                var umlElement = _umlElements.FirstOrDefault(x => x.TextBlock == textBlock);
                if (umlElement != null)
                {
                    StartTextEditing(umlElement);
                    e.Handled = true;
                }
            }
        }

        private void SelectElement(FrameworkElement element)
        {
            if (_selectedElement != null)
            {
                _selectedElement.Effect = null;
                RemoveAdorners();
            }

            _selectedElement = element;

            if (_selectedElement != null)
            {
                _selectedElement.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Blue,
                    ShadowDepth = 0,
                    BlurRadius = 10
                };

                _deleteAdorner = new DeleteAdorner(_selectedElement);
                _resizeAdorner = new ResizeAdorner(_selectedElement);

                var layer = AdornerLayer.GetAdornerLayer(_selectedElement);
                layer?.Add(_deleteAdorner);
                layer?.Add(_resizeAdorner);
            }
        }

        private void RemoveAdorners()
        {
            if (_selectedElement != null)
            {
                var layer = AdornerLayer.GetAdornerLayer(_selectedElement);
                if (layer != null)
                {
                    layer.Remove(_deleteAdorner);
                    layer.Remove(_resizeAdorner);
                }
            }
            _deleteAdorner = null;
            _resizeAdorner = null;
        }

        private void StartTextEditing(UmlElement umlElement)
        {
            var textBox = new TextBox
            {
                Text = umlElement.Text,
                FontSize = umlElement.TextBlock.FontSize,
                Foreground = umlElement.TextBlock.Foreground,
                Background = Brushes.White,
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = umlElement.TextBlock.VerticalAlignment,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = umlElement.TextBlock.Margin,
                AcceptsReturn = true,
                MinWidth = 50
            };

            umlElement.Container.Children.Remove(umlElement.TextBlock);
            umlElement.Container.Children.Add(textBox);

            textBox.Focus();
            textBox.SelectAll();

            textBox.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter && Keyboard.Modifiers != ModifierKeys.Shift)
                {
                    EndTextEditing(umlElement, textBox);
                    e.Handled = true;
                }
            };

            textBox.LostFocus += (s, e) => EndTextEditing(umlElement, textBox);
        }

        private void EndTextEditing(UmlElement umlElement, TextBox textBox)
        {
            umlElement.Text = textBox.Text;
            umlElement.TextBlock.Text = textBox.Text;

            umlElement.Container.Children.Remove(textBox);
            umlElement.Container.Children.Add(umlElement.TextBlock);
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Canvas)
            {
                SelectElement(null);
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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Delete && _selectedElement != null)
            {
                DrawingCanvas.Children.Remove(_selectedElement);
                _umlElements.RemoveAll(x => x.Container == _selectedElement);
                _selectedElement = null;
                RemoveAdorners();
            }
        }
    }

    public class UmlElement
    {
        public Grid Container { get; set; }
        public FrameworkElement Visual { get; set; }
        public TextBlock TextBlock { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
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
                Cursor = Cursors.Hand,
                Margin = new Thickness(-10, -10, 0, 0)
            };

            _deleteButton.Click += (s, e) =>
            {
                if (AdornedElement is FrameworkElement element && element.Parent is Panel parent)
                {
                    parent.Children.Remove(element);
                }
                e.Handled = true;
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

    public class ResizeAdorner : Adorner
    {
        private readonly Thumb _topLeft, _topRight, _bottomLeft, _bottomRight;
        private readonly VisualCollection _visualChildren;

        public ResizeAdorner(UIElement adornedElement) : base(adornedElement)
        {
            _visualChildren = new VisualCollection(this);

            _topLeft = CreateResizeThumb(Cursors.SizeNWSE);
            _topRight = CreateResizeThumb(Cursors.SizeNESW);
            _bottomLeft = CreateResizeThumb(Cursors.SizeNESW);
            _bottomRight = CreateResizeThumb(Cursors.SizeNWSE);

            _topLeft.DragDelta += HandleTopLeft;
            _topRight.DragDelta += HandleTopRight;
            _bottomLeft.DragDelta += HandleBottomLeft;
            _bottomRight.DragDelta += HandleBottomRight;
        }

        private Thumb CreateResizeThumb(Cursor cursor)
        {
            var thumb = new Thumb
            {
                Width = 10,
                Height = 10,
                Background = Brushes.White,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Cursor = cursor
            };
            _visualChildren.Add(thumb);
            return thumb;
        }

        private void HandleTopLeft(object sender, DragDeltaEventArgs args)
        {
            if (AdornedElement is FrameworkElement element)
            {
                element.Width = Math.Max(20, element.Width - args.HorizontalChange);
                element.Height = Math.Max(20, element.Height - args.VerticalChange);
                Canvas.SetLeft(element, Canvas.GetLeft(element) + args.HorizontalChange);
                Canvas.SetTop(element, Canvas.GetTop(element) + args.VerticalChange);
            }
        }

        private void HandleTopRight(object sender, DragDeltaEventArgs args)
        {
            if (AdornedElement is FrameworkElement element)
            {
                element.Width = Math.Max(20, element.Width + args.HorizontalChange);
                element.Height = Math.Max(20, element.Height - args.VerticalChange);
                Canvas.SetTop(element, Canvas.GetTop(element) + args.VerticalChange);
            }
        }

        private void HandleBottomLeft(object sender, DragDeltaEventArgs args)
        {
            if (AdornedElement is FrameworkElement element)
            {
                element.Width = Math.Max(20, element.Width - args.HorizontalChange);
                element.Height = Math.Max(20, element.Height + args.VerticalChange);
                Canvas.SetLeft(element, Canvas.GetLeft(element) + args.HorizontalChange);
            }
        }

        private void HandleBottomRight(object sender, DragDeltaEventArgs args)
        {
            if (AdornedElement is FrameworkElement element)
            {
                element.Width = Math.Max(20, element.Width + args.HorizontalChange);
                element.Height = Math.Max(20, element.Height + args.VerticalChange);
            }
        }

        protected override int VisualChildrenCount => _visualChildren.Count;
        protected override Visual GetVisualChild(int index) => _visualChildren[index];

        protected override Size ArrangeOverride(Size finalSize)
        {
            double width = AdornedElement.RenderSize.Width;
            double height = AdornedElement.RenderSize.Height;

            _topLeft.Arrange(new Rect(-5, -5, 10, 10));
            _topRight.Arrange(new Rect(width - 5, -5, 10, 10));
            _bottomLeft.Arrange(new Rect(-5, height - 5, 10, 10));
            _bottomRight.Arrange(new Rect(width - 5, height - 5, 10, 10));

            return finalSize;
        }
    }
}