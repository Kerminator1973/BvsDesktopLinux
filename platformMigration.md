# Миграция на актуальные платформы

В мае 2024 года была осуществлена попытка миграции приложений с .NET6/Avalonia 10 на .NET8/Avalonia 11. Портирование не было осуществлена в один момент - возникла целая группа проблем, которые пришлось решать.

У всех трёх проектов был установлен ".Net 8" в качестве "Target Framework", но обновить зависимости не удалось - в процессе обновления, потребовалось дополнительно установить два компонента: **Microsoft.CodeAnalysis.Common** и **Microsoft.CodeAnalysis.CSharp**. После их добавления зависимости успешно обновились.

Также потребовалось добавить зависимость Avalonia.Themes.Fluent, которая стала дополнительной, а не базовой возможностью. Важно отметить, что в "App.axaml" изменилась структура описания тем. Рабочий вариант:

```csharp
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="using:BvsDesktopLinux"
			 RequestedThemeVariant="Default"
             x:Class="BvsDesktopLinux.App">
	
    <Application.Styles>
		<FluentTheme />
        <StyleInclude Source="avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml"/>
    </Application.Styles>
</Application>
```

В Avalonia 10 не требовалось указывать атрибут **RequestedThemeVariant**, без указания которого клиентская область окна просто не отразиться будет отрисовываться.

## ViewLocator - не нужен?

Не очень понятная ситуация с ViewLocator, который, вроде бы уже не нужен в Avalonia 11. Удалив файл "ViewLocator.cs", а тауже убраз из "App.axaml" следующий код:

<Application.DataTemplates>
        <local:ViewLocator/>
</Application.DataTemplates>

Проблемы, связанные с использованием IControl и ещё нескольких интерфейсов в ViewLocator исчезли.

## Проблемы с MainWindow

При сборке приложения возникали следующие проблемы:

```output
BvsDesktopLinux.Views.MainWindow.g.cs(11,52,11,59): error CS0102: The type 'MainWindow' already contains a definition for 'textbox'
BvsDesktopLinux.Views.MainWindow.g.cs(12,50,12,55): error CS0102: The type 'MainWindow' already contains a definition for 'image'
BvsDesktopLinux.Views.MainWindow.g.cs(20,21,20,40): error CS0111: Type 'MainWindow' already defines a member called 'InitializeComponent' with the same parameter types
```

Проблемы исчезли сразу после того, как из зависимостей проекта (csproj) была удалена строка: 

```xml
<PackageReference Include="XamlNameReferenceGenerator" Version="1.6.1" />
```

## FontAwesome

Инициализация FontAwesome теперь должна выглядеть чуть по другому:

```csharp
public static AppBuilder BuildAvaloniaApp()
{
	IconProvider.Current
		.Register<FontAwesomeIconProvider>();

	return AppBuilder.Configure<App>()
		.UsePlatformDetect()
		.LogToTrace()
		.UseReactiveUI();
}
```

## DataGrid

"Вишенка на торте" - в DataGrid свойство **Items** было переименовано в **ItemsSource**.

## Прочее

После внесения перечисленных выше изменений приложения запустились и заработали. Тем не менее, осталось нерешённая проблема:

- [Binding]Error in binding to 'Avalonia.Controls.Button'.'Command': 'Could not find a matching property accessor for 'RestoreCounts' on 'BvsDesktopLinux.ViewModels.MainWindowViewModel''(Button #40311937)
