using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UmlEditor
{
    public class UmlGroupManager
    {
        private readonly Canvas _drawingCanvas;
        private readonly List<UmlElement> _allElements;
        private readonly List<UmlElement> _selectedForGroup = new List<UmlElement>();
        private UmlElement _currentSelectedElement;

        public UmlGroupManager(Canvas drawingCanvas, List<UmlElement> allElements)
        {
            _drawingCanvas = drawingCanvas;
            _allElements = allElements;
        }

        public bool CanGroup => _selectedForGroup.Count >= 2;
        public bool CanUngroup => _currentSelectedElement != null && _currentSelectedElement.IsGroup;
        public class UmlElement
        {
            public Grid Container { get; set; }
            public FrameworkElement Visual { get; set; }
            public TextBlock TextBlock { get; set; }
            public string Type { get; set; } // "Human", "Oval", "Group", etc.
            public string Text { get; set; }
            public bool IsGroup => Type == "Group";
            public List<UmlElement> Children { get; set; } = new List<UmlElement>();
        }
        public void SelectForGroup(UmlElement element)
        {
            if (!_selectedForGroup.Contains(element))
            {
                _selectedForGroup.Add(element);
                HighlightElement(element, Colors.Green);
            }
            else
            {
                _selectedForGroup.Remove(element);
                HighlightElement(element, Colors.Transparent);
            }
        }

        public void UpdateSelection(UmlElement element)
        {
            _currentSelectedElement = element;
        }

        public void CreateGroup()
        {
            if (!CanGroup) return;

            // Создаем контейнер для группы
            var groupContainer = new Grid();

            // Определяем границы группы
            Rect groupBounds = CalculateGroupBounds();

            // Устанавливаем размер и позицию группы
            groupContainer.Width = groupBounds.Width;
            groupContainer.Height = groupBounds.Height;
            Canvas.SetLeft(groupContainer, groupBounds.X);
            Canvas.SetTop(groupContainer, groupBounds.Y);

            // Переносим элементы в группу
            foreach (var element in _selectedForGroup)
            {
                _drawingCanvas.Children.Remove(element.Container);
                groupContainer.Children.Add(element.Container);

                // Обновляем позицию относительно группы
                Canvas.SetLeft(element.Container,
                    Canvas.GetLeft(element.Container) - groupBounds.X);
                Canvas.SetTop(element.Container,
                    Canvas.GetTop(element.Container) - groupBounds.Y);

                _allElements.Remove(element);
            }

            // Добавляем группу на холст
            _drawingCanvas.Children.Add(groupContainer);

            // Создаем новый UmlElement для группы
            var groupElement = new UmlElement
            {
                Container = groupContainer,
                Visual = groupContainer,
                Type = "Group",
                Text = $"Группа ({_selectedForGroup.Count} элементов)",
                Children = new List<UmlElement>(_selectedForGroup)
            };

            // Добавляем текст группы
            var groupText = new TextBlock
            {
                Text = groupElement.Text,
                Foreground = Brushes.Black,
                Background = Brushes.White,
                Padding = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top
            };
            groupContainer.Children.Add(groupText);

            _allElements.Add(groupElement);
            ResetGroupSelection();
            _currentSelectedElement = groupElement;
        }

        public void UngroupSelected()
        {
            if (!CanUngroup) return;

            var groupElement = _currentSelectedElement;
            var groupContainer = groupElement.Container;

            // Возвращаем элементы на холст
            foreach (var child in groupElement.Children)
            {
                // Восстанавливаем глобальные координаты
                double globalX = Canvas.GetLeft(groupContainer) + Canvas.GetLeft(child.Container);
                double globalY = Canvas.GetTop(groupContainer) + Canvas.GetTop(child.Container);

                groupContainer.Children.Remove(child.Container);
                _drawingCanvas.Children.Add(child.Container);

                Canvas.SetLeft(child.Container, globalX);
                Canvas.SetTop(child.Container, globalY);

                _allElements.Add(child);
            }

            // Удаляем группу
            _drawingCanvas.Children.Remove(groupContainer);
            _allElements.Remove(groupElement);
            _currentSelectedElement = null;
        }

        private Rect CalculateGroupBounds()
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (var element in _selectedForGroup)
            {
                double left = Canvas.GetLeft(element.Container);
                double top = Canvas.GetTop(element.Container);
                double right = left + element.Container.ActualWidth;
                double bottom = top + element.Container.ActualHeight;

                if (left < minX) minX = left;
                if (top < minY) minY = top;
                if (right > maxX) maxX = right;
                if (bottom > maxY) maxY = bottom;
            }

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        private void HighlightElement(UmlElement element, Color color)
        {
            var border = new Border
            {
                BorderBrush = new SolidColorBrush(color),
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(3),
                IsHitTestVisible = false
            };

            if (element.Container.Children.Count > 0 &&
                element.Container.Children[0] is Border existingBorder)
            {
                element.Container.Children.RemoveAt(0);
            }

            if (color != Colors.Transparent)
            {
                element.Container.Children.Insert(0, border);
            }
        }

        public void ResetGroupSelection()
        {
            foreach (var element in _selectedForGroup)
            {
                HighlightElement(element, Colors.Transparent);
            }
            _selectedForGroup.Clear();
        }
    }
}

