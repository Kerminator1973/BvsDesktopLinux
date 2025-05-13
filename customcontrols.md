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

## Избежать дублирования свойств

Предположим, что нам нужно отобразить на картинке пять разных сенсоров, используя общий код для их отображения. При этом координаты и состояние сенсора необходимо задавать в XAML.

Сначала следует определить блок свойств системы:

```csharp
public class Sensor : AvaloniaObject
{
    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<Sensor, bool>(nameof(IsActive), defaultValue: false);

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public static readonly StyledProperty<int> XProperty =
        AvaloniaProperty.Register<Sensor, int>(nameof(X), defaultValue: 10);

    public int X
    {
        get => GetValue(XProperty);
        set => SetValue(XProperty, value);
    }

    public static readonly StyledProperty<int> YProperty =
        AvaloniaProperty.Register<Sensor, int>(nameof(Y), defaultValue: 10);

    public int Y
    {
        get => GetValue(YProperty);
        set => SetValue(YProperty, value);
    }
}
```

Затем следует определить необходимое нам количество сенсоров в custom-ном органе управления:

```csharp
public class D210Sensors : TemplatedControl
{
    public Sensor Sensor1 { get; set; } = new Sensor();
    public Sensor Sensor2 { get; set; } = new Sensor();
    public Sensor Sensor3 { get; set; } = new Sensor();
    public Sensor Sensor4 { get; set; } = new Sensor();
    public Sensor Sensor5 { get; set; } = new Sensor();
```

Задать свойства каждого из сенсоров в XAML верстке можно следующим образом:

```csharp
<local:D210Sensors Grid.Row="0" SourceImage="avares://approve/Assets/357x170_250BA.png"
    HorizontalAlignment="Right" VerticalAlignment="Stretch" Height="170" Width="357" Opacity="950">
    <local:D210Sensors.Sensor1>
        <local:Sensor IsActive="True" X="50" Y="70"/>
    </local:D210Sensors.Sensor1>
    <local:D210Sensors.Sensor2>
        <local:Sensor IsActive="False" X="10" Y="50"/>
    </local:D210Sensors.Sensor2>
    <local:D210Sensors.Sensor3>
        <local:Sensor IsActive="True" X="80" Y="30"/>
    </local:D210Sensors.Sensor3>
    <local:D210Sensors.Sensor4>
        <local:Sensor IsActive="True" X="80" Y="70"/>
    </local:D210Sensors.Sensor4>
    <local:D210Sensors.Sensor5>
        <local:Sensor IsActive="False" X="30" Y="30"/>
    </local:D210Sensors.Sensor5>
</local:D210Sensors>
```

Отрисовка всех сенсоров может выглядеть следующим образом:

```csharp
public override void Render(DrawingContext context)
{
    base.Render(context);

    // Если изображение указано через свойство, то отображаем его на экране
    if (SourceImage != null)
    {
        Rect destRect = new Rect(Bounds.Size);
        context.DrawImage(SourceImage, destRect);
    }

    // TODO: если прибор не подключен, то не отображаем точки привязки сенсоров

    DrawSensor(context, Sensor1.IsActive, Sensor1.X, Sensor1.Y);
    DrawSensor(context, Sensor2.IsActive, Sensor2.X, Sensor2.Y);
    DrawSensor(context, Sensor3.IsActive, Sensor3.X, Sensor3.Y);
    DrawSensor(context, Sensor4.IsActive, Sensor4.X, Sensor4.Y);
    DrawSensor(context, Sensor5.IsActive, Sensor5.X, Sensor5.Y);
}

private void DrawSensor(DrawingContext context, bool state, int x, int y)
{
    IBrush fillBrush = state ? Brushes.Green : Brushes.Red;
    IPen pen = new Pen(Brushes.Black, 3);

    double radius = Math.Min(Bounds.Width, Bounds.Height) / 10;
    double centerX = Bounds.Width * x / 100;
    double centerY = Bounds.Height * y / 100;

    // Отрисовываем эллипс (для примера)
    context.DrawEllipse(fillBrush, pen, new Point(centerX, centerY), radius, radius);
}
```
