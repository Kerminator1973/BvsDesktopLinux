# BvsDesktopLinux

Порт приложения BVS Desktop в Linux с использованием Avalonia.

## Установка .NET 6 в Ubuntu 22.04

Установить Runtime:

``` shell
sudo apt-get update && \
  sudo apt-get install -y dotnet6
```

Установить SDK: `sudo apt-get install -y dotnet-sdk-6.0`

В случае возникновения ошибок из-за конфликтов зависимостей, может потребоваться выполнить настройку канала доступа к компонентам .NET:

``` shell
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
```

Проверить список установленных SDK: `dotnet --list-sdks`

К сожалению, установка SDK не всегда работает. Я сталкивался с [проблемой потери информации о SDK](https://github.com/dotnet/sdk/issues/27129), в частности, при переходе на Ubuntu 22.10 Kinetic Kudu.

**Решение**, которое помогло:

1. Удалить предыдущие установки: `sudo apt remove --purge --autoremove *dotnet*`
2. Создать, или модифицировать файл `/etc/apt/preferences` и добавить в него, если нужно, следующие строки (нужен **sudo**):

```
Package: *net*
Pin: origin packages.microsoft.com
Pin-Priority: 1001
```

Подробнее о настройке можно почитать здесь: `man apt_preferences`

3. Загрузить .NET командой: `sudo apt install dotnet-sdk-6.0`

## Основные консольные команды

Команды создания приложения:

```
dotnet new avalonia.mvvm -o BvsDesktopLinux -n BvsDesktopLinux
cd BvsDesktopLinux
dotnet new sln
dotnet sln add BvsDesktopLinux.csproj
```

Сразу после команды new указывается имя шаболна. Список шаблонов можно посмотреть командой: `dotnet new list`. Установить шаблоны Avalonia можно командой `dotnet new install Avalonia.Templates`, см. [шаблоны Avalonia](https://github.com/AvaloniaUI/avalonia-dotnet-templates)

Если нужно указать конкретную платформу, то это можно сделать используя параметр `-F`:

``` shell
dotnet new avalonia.mvvm -F "net6.0" -o approve -n approve
```

Список параметров для конкретного шаблона можно посмотреть командой (пример): `dotnet new avalonia.mvvm -h`

Установка Git и загрузка репозитария из GitHub:

``` shell
sudo apt-get update
sudo apt-get install git
git clone https://github.com/Kerminator1973/BvsDesktopLinux.git
```

Команды сборки и запуска приложения (запуск из подкаталога с файлом .csproj):

``` shell
dotnet restore
dotnet build
dotnet run
```

Запуск Unit-тестов: 'dotnet test'

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

## Всплывающие подсказки (ToolTips)

В Avalonia UI можно легко добавить всплывающие подсказки к элементам пользовательского интерфейса, но разметка отличается от WPF.

Типовая разметка в WPF:

``` csharp
<Button Grid.Column="0" Click="buttonAdd_Click" Padding="20,0,20,0">
	<Button.ToolTip>
		<ToolTip Content="{x:Static p:Resources.TooltipAddNewCounter}" />
	</Button.ToolTip>
	<fa:ImageAwesome Icon="Solid_Plus" Height="20" Width="20" Margin="5" PrimaryColor="LightSeaGreen"/>
</Button>
```

Разметка в Avalonia UI:

``` csharp
<Button Margin="0,0,5,0" Command="{Binding RestoreCounts}" CommandParameter="RUB">
	<ToolTip.Tip>
		<ToolTip Background="Yellow" Foreground="Black">
			<TextBlock Margin="3" Text="{x:Static p:Resources.TooltipRestoreCounts}" />
		</ToolTip>
	</ToolTip.Tip>				
	<Border BorderThickness="4">
		<i:Icon Value="fas fa-redo" FontSize="20" />
	</Border>
</Button>
```

## FontAwesome

Для Avalonia следует использовать библиотеки работы с иконками отличные от применяемых в WPF. Одним из вариантов является [FontAwesome.Avalonia](https://github.com/Projektanker/Icons.Avalonia) от Projektanker.

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

В библиотеке могут быть использованы префиксы: fas (_solid_), far (_regular_) и fab (_brands_). Префиксы соответствуют группам иконок из бесплатного набора иконок [FontAwesome](https://fontawesome.com/icons). 

Полный набор иконок содержащихся в компоненте находится в [icons.json](https://github.com/Projektanker/Icons.Avalonia/blob/main/src/Projektanker.Icons.Avalonia.FontAwesome/Assets/icons.json) в папке Assets. В "icons.json" можно выполнять полнотекстовый поиск по ожидаемому названию иконки. Например, для поиска иконки принтера следует искать вхождение слова "print", как узла верхнего уровня:

```json
  "print": {
    "styles": [
      "solid"
    ],
    "label": "print",
    "svg": { ...
```

Ключевым является свойство "styles", из которого следует, какой именно префикс следует использовать для данной иконки. Например, если в списке есть стиль "solid", то следует использовать класс иконки "fas":

```xml
<i:Icon Value="fas fa-print" FontSize="20" />
```

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

В качестве альтернативного инструмента можно использовать «DB Browser for SQLite», который работает в разных операционных системах: Windows, macOS, Linux. Установить продукт под Linux можно, в частности, командой:

``` cmd
snap install sqlitebrowser
```

После установки посредством snap, приложение появляется в «Application».

## Работа с базой данных SQLite в коде на C#

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

Метод CanUpdate() можно использовать совместно с атрибутами ViewModel, как показано в приведённом ниже примере:

``` csharp
[DependsOn(nameof(SelectedBanknote))]
public bool CanDeleteBanknote(/* CommandParameter */object parameter)
{
    return null != SelectedBanknote;
}
```

В действительности, у Avalonia UI есть некоторая проблема с использованием события (event) PropertyChanged в классе производном от ViewModelBase. Заметим, что автоматически генерируемый по шаблону Avalonia проект содержит класс MainWindowViewModel, производный от ViewModelBase. Проблема состоит в том, что ViewModelBase наследуется ReactiveObject, а в ReactiveObject уже определено событие PropertyChanged, которое имеет модификато доступа private. Возникает коллизия, которая не нравится компилятору и может иметь побочные эффекты.

Чтобы обойти ограничение, следует использовать вспомогательное зависимое свойство IsBanknoteSelected:

``` csharp 
private bool isBanknoteSelected = false;
public bool IsBanknoteSelected
{
    get { return isBanknoteSelected; }
    set
    {
        this.RaiseAndSetIfChanged(ref isBanknoteSelected, value);
    }
}

private Banknote? selectedBanknote = null;
public Banknote? SelectedBanknote {
    get { return selectedBanknote; }
    set
    {
        this.RaiseAndSetIfChanged(ref selectedBanknote, value);

        // Информируем подписчиков (метод CanDeleteBanknote) об изменении состояния
        // SelectedBanknote, косвенным способом через изменение свойства IsBanknoteSelected
        IsBanknoteSelected = (null != selectedBanknote);
    } 
}
```

Соответственно, код отслеживания изменения SelectedItem в DataGrid и связывания этого изменения с состоянием командной кнопки, выглядеть так:

``` csharp
// Метод CanDeleteBanknote() будет вызываться если измениться свойство IsBanknoteSelected
[DependsOn(nameof(IsBanknoteSelected))]
public bool CanDeleteBanknote(/* CommandParameter */object parameter)
{
    return null != SelectedBanknote;
}
```

Заметим, что в Avalonia UI используется удобный helper-метод **RaiseAndSetIfChanged**().

## Локализация приложений

Механизм локализации работает точно также как и для WPF-приложения в Windows. См.: https://github.com/Kerminator1973/BVSDesktopSupport/blob/main/i18n.md

Обработка параметров командной строки с целью установки локализации должна быть выполнена до создания главного окна. Фактический код:

``` csharp
public override void OnFrameworkInitializationCompleted()
{
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
        // Обрабатываем параметры командной строки с целью явным образом
        // указать локализацию. Это особенно удобно для того, чтобы выполнять
        // тестирование и отладку локализованных сообщений
        ParseCommandLine(Environment.GetCommandLineArgs());

        // Создаём основное окно
        desktop.MainWindow = new MainWindow
        {
            DataContext = new MainWindowViewModel(),
        };
    }

    // Инициализация приложения завершена, можно визуализировать
    // пользовательский интерфейс
    base.OnFrameworkInitializationCompleted();
}
```

Заметим, что параметры командной строки можно получить в любом месте приложения, используя статический метод GetCommandLineArgs() класса Environment.

Установка локализации в коде осуществляется так:

``` csharp
var culture = new System.Globalization.CultureInfo("ru-RU");
System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
Thread.CurrentThread.CurrentCulture = culture;
```

Запуск приложения с указанием локализации может быть таким:

``` shell
dotnet run -locale en
dotnet run -locale ru-RU
```

Стоит заметить, что концептуально более правильным было бы устанавливать текущую локализацию (locale) средствами операционной системы. Возможно, что именно в этом направлении следует развивать приложение. Тем не менее, в текущем моменте, использование дополнительных параметров запуска приложения кажется достаточно удобным для инженера со среднем уровнем подготовки.

## Экспорт данных в PDF

Задача экспорта данных в PDF может быть решена посредством библиотеки [SkiaSharp](https://github.com/mono/SkiaSharp), доступной в Avalonia UI "из коробки".

Основной цикл выглядит следующим образом:

``` csharp
using SkiaSharp;
...
using var doc = SKDocument.CreatePdf("SkiaSample.pdf", metadata);

// Критичное упрощение: все записи выводятся только на одну страницу,
// размер страницы фиксированный
float pageWidth = 840.0f;
float pageHeight = 1188.0f;

// Получаем контекст вывода данных (контекст отрисовки)
using (var pdfCanvas = doc.BeginPage(pageWidth, pageHeight)) {

    // Формируем структуру, которая описывает параметры отображения элемента
    using var paint = new SKPaint
    {
        TextSize = 48.0f,
        IsAntialias = true,
        Color = SKColors.Black,
        IsStroke = true,
        StrokeWidth = 2,
        TextAlign = SKTextAlign.Left
    };

    float curPos = 0.0f;    // Текущая позиция вывода - изменяется на каждой итерации
    float margin = 24.0f;   // Отступы к каждой из сторон
    float oneThird = (pageWidth - margin * 2) / 3;  // Каждая колонка шириной в треть

    foreach (var note in Banknotes)
    {
        pdfCanvas.DrawText(note.Id.ToString(), margin, curPos + paint.TextSize, paint);
        pdfCanvas.DrawText(note.Currency, margin + oneThird, curPos + paint.TextSize, paint);
        pdfCanvas.DrawText(note.Denomination, margin + oneThird * 2, curPos + paint.TextSize, paint);
        curPos += paint.TextSize;
    }

    doc.EndPage();
}

doc.Close();
```

В данном подходе приходится самостоятельно управлять разбиением контента на отдельные страницы и вообще, подход низко-уровневый, но зато всё контролирует код и можно создать форму любого уровня сложности.

Примеры, иллюстрирующие данный подход:

- https://github.com/mono/SkiaSharp/blob/main/samples/Gallery/Shared/Samples/CreatePdfSample.cs
- https://github.com/Oaz/AvaloniaUI.PrintToPDF
