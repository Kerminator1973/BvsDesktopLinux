# Использование Converter для вычисления значений свойств

Может возникнуть потребность вычисления некоторого атрибута элемента пользовательского интерфейса на основании некоторого другого свойства. Например, цвет фона ячейки/строки должен зависить от значения некоторого свойства. Для это цели можно использовать объекты-конверторы.

Конвертор является классом, реализующим интерфейс **IValueConverter**, в котором следует реализовать два метода: Convert() и ConvertBack(). Второй метод часто остаётся не реализованным - он используется только в том случае, если применяется двух-сторонний Binding. Пример реализации класса:

``` csharp
public class StatusConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var status = value as string;

        return status switch
        {
            "Rejected" => Brushes.Red,
            _ => Brushes.Green
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

Если мы хотим использовать конвертер в пользовательском интерфейсе, нас следует создать статический экземпляр этого класса:

``` csharp
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        ...
		xmlns:converter="using:BvsDesktopLinux.Converters">

    <Window.Resources>
        <converter:StatusConverter x:Key="StatusConverter" />
    </Window.Resources>
```

Далее, мы можем использовать экземпляр Converter-а в связывании свойств:

``` csharp
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        ...
		xmlns:models="using:BvsDesktopLinux.Models">

	<Window.Styles>
        <Style Selector="DataGridCell" x:DataType="models:Banknote">
            <Setter Property="Background" Value="{Binding Status, Converter={StaticResource StatusConverter}}" />
        </Style>
	</Window.Styles>
```
