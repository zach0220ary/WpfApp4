using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _2026_WpfApp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Color strokeColor = Colors.Black;
        Color fillColor = Colors.Transparent;
        int strokeThickness = 1;
        string shapeType = "Line";
        string actionType = "Draw";
        Point start = new Point { X = 0, Y = 0 };
        Point end = new Point { X = 0, Y = 0 };

        public MainWindow()
        {
            InitializeComponent();
            StrokeColorPicker.SelectedColor = strokeColor;
            FillColorPicker.SelectedColor = fillColor;
        }

        private void ShapeButton_Checked(object sender, RoutedEventArgs e)
        {
            var targetShapeButton = sender as RadioButton;
            shapeType = targetShapeButton.Tag.ToString();
            actionType = "Draw";
            DisplayStatus();
        }

        private void StrokeColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            strokeColor = (Color)StrokeColorPicker.SelectedColor;
            DisplayStatus();
        }

        private void DisplayStatus()
        {
            if (StatusLabel != null)
            {
                if (actionType == "Draw")
                {
                    StatusLabel.Content = $"圖形：{shapeType}  座標：({start.X}, {start.Y}) - ({end.X},{end.Y})";
                }
                else
                {
                    StatusLabel.Content = $"工作模式：{actionType}";
                }
            }
            if (ColorLabel != null) ColorLabel.Content = $"筆刷色彩:{strokeColor}     填充色彩:{fillColor}     筆刷粗細：{strokeThickness}";
            if (ShapeLabel != null) ShapeLabel.Content = $"形狀：{shapeType}";
        }

        private void FillColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            fillColor = (Color)FillColorPicker.SelectedColor;
            DisplayStatus();
        }

        private void ThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            strokeThickness = (int)ThicknessSlider.Value;
            DisplayStatus();
        }

        private void MyCanvas_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MyCanvas.Cursor = Cursors.Pen;
        }

        private void MyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyCanvas.Cursor = Cursors.Cross;
            start = e.GetPosition(MyCanvas);

            if (actionType == "Draw")
            {
                switch (shapeType)
                {
                    case "Line":
                        Line line = new Line
                        {
                            X1 = start.X,
                            Y1 = start.Y,
                            X2 = end.X,
                            Y2 = end.Y,
                            Stroke = Brushes.Gray,
                            StrokeThickness = 1
                        };
                        MyCanvas.Children.Add(line);
                        break;

                    case "Rectangle":
                        Rectangle rect = new Rectangle
                        {
                            Stroke = Brushes.Gray,
                            Fill = Brushes.LightGray
                        };
                        rect.SetValue(Canvas.LeftProperty, start.X);
                        rect.SetValue(Canvas.TopProperty, start.Y);
                        MyCanvas.Children.Add(rect);
                        break;

                    case "Ellipse":
                        Ellipse ellipse = new Ellipse
                        {
                            Stroke = Brushes.Gray,
                            Fill = Brushes.LightGray
                        };
                        ellipse.SetValue(Canvas.LeftProperty, start.X);
                        ellipse.SetValue(Canvas.TopProperty, start.Y);
                        MyCanvas.Children.Add(ellipse);
                        break;

                    case "Polyline":
                        Polyline polyline = new Polyline
                        {
                            Stroke = Brushes.Gray,
                            Fill = Brushes.LightGray
                        };
                        MyCanvas.Children.Add(polyline);
                        break;
                }
            }
        }

        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            end = e.GetPosition(MyCanvas);
            DisplayStatus();

            switch (actionType)
            {
                case "Draw": //繪圖模式
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Point origin;
                        origin.X = Math.Min(start.X, end.X);
                        origin.Y = Math.Min(start.Y, end.Y);
                        double width = Math.Abs(end.X - start.X);
                        double height = Math.Abs(end.Y - start.Y);

                        switch (shapeType)
                        {
                            case "Line":
                                var line = MyCanvas.Children.OfType<Line>().LastOrDefault();
                                line.X2 = end.X;
                                line.Y2 = end.Y;
                                break;

                            case "Rectangle":
                                var rect = MyCanvas.Children.OfType<Rectangle>().LastOrDefault();
                                rect.Width = width;
                                rect.Height = height;
                                rect.SetValue(Canvas.LeftProperty, origin.X);
                                rect.SetValue(Canvas.TopProperty, origin.Y);
                                break;

                            case "Ellipse":
                                var ellipse = MyCanvas.Children.OfType<Ellipse>().LastOrDefault();
                                ellipse.Width = width;
                                ellipse.Height = height;
                                ellipse.SetValue(Canvas.LeftProperty, origin.X);
                                ellipse.SetValue(Canvas.TopProperty, origin.Y);
                                break;

                            case "Polyline":
                                var polyline = MyCanvas.Children.OfType<Polyline>().LastOrDefault();
                                polyline.Points.Add(end);
                                break;

                        }
                    }
                    break;

                case "Eraser": //橡皮擦模式
                    MyCanvas.Cursor = Cursors.Hand;
                    var shape = e.OriginalSource as Shape;
                    MyCanvas.Children.Remove(shape);
                    if (MyCanvas.Children.Count == 0) MyCanvas.Cursor = Cursors.Arrow;
                    break;
            }
        }

        private void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Brush strokeBrush = new SolidColorBrush(strokeColor);
            Brush fillBrush = new SolidColorBrush(fillColor);

            switch (shapeType)
            {
                case "Line":
                    var line = MyCanvas.Children.OfType<Line>().LastOrDefault() as Line;
                    line.Stroke = strokeBrush;
                    line.StrokeThickness = strokeThickness;
                    break;

                case "Rectangle":
                    var rect = MyCanvas.Children.OfType<Rectangle>().LastOrDefault() as Rectangle;
                    rect.Stroke = strokeBrush;
                    rect.Fill = fillBrush;
                    rect.StrokeThickness = strokeThickness;
                    break;

                case "Ellipse":
                    var ellipse = MyCanvas.Children.OfType<Ellipse>().LastOrDefault() as Ellipse;
                    ellipse.Stroke = strokeBrush;
                    ellipse.Fill = fillBrush;
                    ellipse.StrokeThickness = strokeThickness;
                    break;

                case "Polyline":
                    var polyline = MyCanvas.Children.OfType<Polyline>().LastOrDefault() as Polyline;
                    polyline.Stroke = strokeBrush;
                    polyline.Fill = fillBrush;
                    polyline.StrokeThickness = strokeThickness;
                    break;
            }
        }

        private void EraserButton_Click(object sender, RoutedEventArgs e)
        {
            actionType = "Eraser";
            DisplayStatus();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            actionType = "Clear";
            MyCanvas.Children.Clear();
            DisplayStatus();
            actionType = "Draw";
        }
    }
}