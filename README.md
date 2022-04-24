# BvsDesktopLinux

Порт приложения BVS Desktop в Linux с использованием Avalonia.

## Основные консольные команды

Команды создания приложения:

```
dotnet new avalonia.mvvm -o BvsDesktopLinux -n BvsDesktopLinux
cd BvsDesktopLinux
dotnet new sln
dotnet sln add BvsDesktopLinux.csproj
```

Установка Git и загрузка репозитария из GitHub:

```
sudo apt-get update
sudo apt-get install git
git clone https://github.com/Kerminator1973/BvsDesktopLinux.git
```

Команды сборки и запуска приложения:

```
dotnet restore
dotnet build
dotnet run
```

## Различия в пространствах имён

В проектах WPF и Avalonia используются разные пространства имен. Типовой заголовок компонента Avalonia:

``` xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="BVSDesktopA.App">
```

Заголовок XAML:

``` xml
<Application xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="BVSDesktop.App" StartupUri="MainWindow.xaml">
```

## Различия в наборе компонентов пользовательского интерфейса и их атрибутах

XAML и Avalonia также очень сильно расходятся в наборе атрибутов органов управления; фактически - это два разных набора органов управления.

### Grid

Grid в Avalonia не имеет свойства **ColSpan**, но есть **ColumnSpan**. Правила использования те же самые.

### Label/TextBlock

В Avalonia нет органа управления Label, вместо него следует использовать **Border** и **TextBlock**.

Пример портированного Label в Avalonia:

``` xml
<Border Background="LightSkyBlue" BorderBrush="LightSkyBlue" BorderThickness="2" CornerRadius="8" 
            Grid.Row="0" Grid.Column="1" Margin="0,10,0,0" >
    <TextBlock Text="Bill Report" Foreground="White" Background="LightSkyBlue" 
            FontSize="16" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Border>
```

Свойства отвечающие за размещение текста внутри TextBlock имеют другое название. Например, в WPF используется HorizontalContentAlignment, тогда как в Avalonia следует использовать **HorizontalAlignment**.

В Avalonia нет атрибута Padding, который приходится реализовывать через Border, либо через вложенный Grid. Вариант добавления _padding_ через **Border**:

``` xml
<Button  Margin="0,0,5,0">
    <Border BorderThickness="4">
        <i:Icon Value="fas fa-redo" FontSize="20" />
    </Border>
</Button>
```

## FontAwesome

Для Avalonia следует использовать библиотеки работы с иконками отличные от применяемых в WPF. Одним из вариантов является [FontAwesome.Avaloia](https://github.com/Projektanker/Icons.Avalonia) от Projektanker.

Чтобы начать использовать библиотеку необходимо в Program.cs добавить специализированный провайдер иконок:

``` csharp
class Program
{
    ...
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI()
            .WithIcons(container => container
                .Register<FontAwesomeIconProvider>());
}
```

В XAML следует сделать видимым пространство имён:

``` xml
<Window xmlns="https://github.com/avaloniaui" ...
		xmlns:i="clr-namespace:Projektanker.Icons.Avalonia;assembly=Projektanker.Icons.Avalonia">
```

Включить иконку в состав кнопки можно так:

``` xml
<Button i:Attached.Icon="fas fa-redo" FontSize="20" />
```

В библиотеке могут быть использованы префиксы: fas (_solid_), far (_regular_) и fab (_brands_). Префиксы соответствуют шруппам иконок из бесплатного набора иконок [FontAwesome](https://fontawesome.com/icons). Полный набор иконок содержащихся в компоненте находится в [icons.json](https://github.com/Projektanker/Icons.Avalonia/blob/main/src/Projektanker.Icons.Avalonia.FontAwesome/Assets/icons.json) в папке Assets.
