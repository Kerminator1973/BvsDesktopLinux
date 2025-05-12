# Как добавить собственный орган управления

Задача из реальной жизни: необходимо нарисовать на экране изображение купюроприёмника (точечная матрица), а поверх изображения отображить пять кругов, каждый из которых показывает место размещения сенсора. В зависимости от свойства (Binding) каждый кружок отображается либо зелёным, либо красным цветом.

Первый шаг - создать орган управления, задать свойство для передачи картинки из ресурсов и вывести какую-то векторную графику.

Пример реализации органа управления:

```csharp
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using System;

namespace approve.CustomControls
{
    public class D210Sensors : Control
    {
        // Свойство для установки изображения делаем доступным для привязки (expose to allow binding)
        public static readonly StyledProperty<IImage?> SourceImageProperty =
            AvaloniaProperty.Register<D210Sensors, IImage?>(nameof(SourceImage));

        public IImage? SourceImage
        {
            get => GetValue(SourceImageProperty);
            set => SetValue(SourceImageProperty, value);
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            // Если изображение указано через свойство, то отображаем его на экране
            if (SourceImage != null)
            {
                Rect destRect = new Rect(Bounds.Size);
                context.DrawImage(SourceImage, destRect);
            }

            // Отображаем векторную графику поверх картинки
            double centerX = Bounds.Width / 2;
            double centerY = Bounds.Height / 2;
            double radius = Math.Min(Bounds.Width, Bounds.Height) / 4;

            // Настраиваем параметры формирования векторной графики (Brush и Pen)
            IBrush fillBrush = Brushes.Transparent;
            IPen pen = new Pen(Brushes.Red, 3);

            // Отрисовывает эллипс (для примера)
            context.DrawEllipse(fillBrush, pen, new Point(centerX, centerY), radius, radius);
        }
    }
}
```

В XAML верстке использование такого органа управления может выглядеть следующим образом:

```csharp
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:controls="clr-namespace:approve.Views"
			 xmlns:local="clr-namespace:approve.CustomControls"
			 mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
			 xmlns:prop="clr-namespace:approve.Properties"
             x:Class="approve.Views.ScreenD210Status">

<local:D210Sensors Grid.Row="0" SourceImage="avares://approve/Assets/357x170_250BA.png" 
    HorizontalAlignment="Right" VerticalAlignment="Stretch" Height="170" Width="357" Opacity="950" />
```

## Добавление дополнительных свойств

Мы можем определить custom-ные свойства этого органа управления:

```csharp
// Регистрируем свойство типа Boolean (Sensor1)
public static readonly StyledProperty<bool> IsSensor1Property =
    AvaloniaProperty.Register<D210Sensors, bool>(
        nameof(Sensor1),
        defaultValue: false);

// Определяем свойство Sensor1
public bool Sensor1
{
    get => GetValue(IsSensor1Property);
    set => SetValue(IsSensor1Property, value);
}
```

Указать свойство можно через параметр:

```csharp
<local:D210Sensors Sensor1="true"... />
```
