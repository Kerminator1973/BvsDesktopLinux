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

## Добавление базы данных SQL

Создать db-файл можно используя plug-in VSCode [SQLite](https://marketplace.visualstudio.com/items?itemName=alexcvzz.vscode-sqlite) by alexcvzz.

В палитре команд следует выбрать пункт "SQLite: New Query". В появившемся окне текстового редактора следует ввести команду создания таблицыm например:

``` sql
CREATE TABLE Banknotes (
   Id INTEGER PRIMARY KEY AUTOINCREMENT,
   Currency TEXT NOT NULL,
   Denomination TEXT NOT NULL
);
```

Затем в контекстном меню текстового редактора выбрать пункт "Run Query". При выполнении команды будет автоматически создан файл "banknotes.db" (по имени таблицы).

Для добавления нескольких записей в таблицу следует выполнить команду:

``` sql
INSERT INTO Banknotes (Currency,Denomination)
VALUES
    ('RUB','500'),
    ('RUB','1000'),
    ('RUB','2000');
```

Просматривать содержимое базы данных можно через инструмент SQLite Explorer в "Проводнике" VSCode.

## Работа с базой данных на SQLite

В соответствии с шаблоном проектирования MVVM, классы с версткой должны обладать слабыми связями с моделью. При создании главного окна может быть создан DataContext главного окна:

``` csharp
public partial class App : Application
{
    ...
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
```

Заметим, что в XAML-разметке **не должно быть** определения статической модели:

``` xml
<Design.DataContext>
    <vm:MainWindowViewModel/>
</Design.DataContext>
```

Подключить Entity Framework и SQLite можно добавив через Nuget packages `Microsoft.EntityFrameworkCore` и `Microsoft.EntityFrameworkCore.Sqlite`

Следующим этапом следует скорректировать модель, добавив в неё поле Id, чтобы соответствовать _Conventions_ Entity Framework:

``` csharp
public class Banknote
{
    public int Id { get; set; }
    public string Currency { get; set; }
    public string Denomination { get; set; }
}
```

Далее необходимо создать DbContext:

``` csharp
public class BanknotesDbContext : DbContext
{
    public DbSet<Banknote> Banknotes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=banknotes.db");
    }
}
```

После этого в классе ModelView можно использовать BanknotesDbContext, например:

``` csharp
public class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<Banknote> Banknotes { get; }

    public MainWindowViewModel()
    {
        BanknotesDbContext _dbContext = new();
        Banknotes = new ObservableCollection<Banknote>(_dbContext.Banknotes);
    }
}
```

Заметим, что приложение будет загружать данные из файла "banknotes.db" только если этот файл будет существовать.

## Работа с миграциями

После установки package `Microsoft.EntityFrameworkCore.Design` появляется возможность работать с миграциями в проекте. Например, можно добавить первую миграцию командой:

``` shell
dotnet ef migrations add InitialCreate
```

Для PowerShell используется команда **Add-Migration**:

``` powershell
Add-Migration TheBadMigration -Force
```

После того, как миграция успешно создана в коде приложения можно применить миграцию к существующей базе данных:

``` csharp
public MainWindowViewModel()
{
    BanknotesDbContext _dbContext = new();
    _dbContext.Database.Migrate();
```

Следует заметить, что применять миграцию нужно там, где создаётся сервис доступа к базе данных. Чаще всего - это StartUp-метод класса Application.

Метод EnsureCreated() похож на **Migrate**(), но он испольузется не для промышленной эксплуатации, а для [тестирования кода](https://docs.microsoft.com/ru-ru/dotnet/api/microsoft.entityframeworkcore.infrastructure.databasefacade.ensurecreated?view=efcore-6.0). Этот метод проверяет наличие базы данных и если не обнаруживает её, то создаёт актуальную схему.

## Реализация команд в Avalonia

Добавление команд в Avalonia UI осуществляется не так, как в WPF. Первое отличие состоит в том, что WPF использует специальный класс RoutedUICommand, в котором определяется статический класс, описывающий команду:

``` csharp
public static class CustomCommands
{
    public static readonly RoutedUICommand Exit = new RoutedUICommand
        (
            "Exit",
            "Exit",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.F4, ModifierKeys.Alt)
            }
        );

    // Define more commands here, just like the one above
}
```

Определённая команда может использоваться, практически, везде - в меню, кнопках, контекстном меню. В Avalonia UI команда определяется как часть ViewModel, см. [Binding to Commands](https://docs.avaloniaui.net/docs/data-binding/binding-to-commands#icommandexecute). Пример:

``` xml
<Button Margin="0,0,5,0" Command="{Binding RestoreCounts}" CommandParameter="RUB" />
```

``` csharp
public void RestoreCounts(string currency)
{
    Banknotes.Clear();
    Banknotes.Add(new Banknote { Id = 4, Currency = currency, Denomination = "200" });
    Banknotes.Add(new Banknote { Id = 5, Currency = currency, Denomination = "100" });
    Banknotes.Add(new Banknote { Id = 6, Currency = currency, Denomination = "2000" });
    Banknotes.Add(new Banknote { Id = 7, Currency = currency, Denomination = "5000" });
}
```

По другому работает механизм **CanExecute** - в Avalonia достаточно просто определить метод такой же, как и у команды, но начинающийся с Can:

``` csharp
public bool CanRestoreCounts()
```

Метод CanUpdate() можно использовать совместно с атрибцтами ViewModel, как показано в приведённом ниже примере, но, в действительности, этот механизм не достаточно универсален и, например, для отслеживания изменения SelectedItem списка GridView необходима добавлять специализированный метод вручную:

``` csharp
[DependsOn(nameof(SelectedBanknote))]
public bool CanDeleteBanknote(/* CommandParameter */object parameter)
{
    return null != SelectedBanknote;
}
```
