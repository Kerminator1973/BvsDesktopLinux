# BvsDesktopLinux

Порт приложения BVS Desktop в Linux с использованием Avalonia.

Инструкция по установке Microsoft.NET на платформе Linux доступна [здесь](./installdotnet.md).

Обзорная статья [Avalonia .NET UI Framework Picks Up Steam as Milestone Release Nears](https://visualstudiomagazine.com/articles/2023/04/04/avalonia.aspx) из Visual Studio Magazine.

Для Visual Studio Code выпущен plug-in "Avalonia for VSCode", который содержит визуальный редактор пользовательского интерфейса и реализует IntelliSense для XAML-кода.

## Просмотр дерева элементов пользовательского интерфейса

При сборке Avalonia в Debug-режиме, автоматически добавляется Developer Tools - инструмент, который позволяет просматривать Logical Tree, а также Visual Tree приложения Avalonia. Доступ к этому инструменту предоставляется при нажатии кнопки F12.

## Компилируемые привязки (CompileBindings)

В Avalonia можно использовать [скомпилированные Bindings](https://docs.avaloniaui.net/docs/data-binding/compiledbindings). Это даёт два преимущества:

- проверка связей на этапе компиляции
- скомпилированное связывание не использует Reflections и работает быстрее

Чтобы активировать этот механизм нужно явным образом указать, где находятся данные, а также флаг компиляции привязок:

``` csharp
<Window xmlns="https://github.com/avaloniaui" ...
		x:DataType="vm:MainWindowViewModel"
		x:CompileBindings="True">
```

Устанавливать компилируемую привязку можно и не в корне документа, а в конкретных элементах, например:

``` csharp
<!-- Set DataType inside the Binding-markup -->
<TextBox Text="{Binding MailAddress, DataType={x:Type vm:MyViewModel}}" />
```

Заметим, что компилируемые привязки не работают для Command и конкретно для них этот механизм нужно отключать:

``` csharp
<Button Command="{Binding DeleteBanknote}" x:CompileBindings="False">
```

Также в командах мы можем явно указать другой тип привязок:

``` csharp
<!-- We cannot use compiled bindings to bind to methods, so we use ReflectionBinding instead -->
<Button Content="Send an E-Mail"
        Command="{ReflectionBinding SendEmailCommand}" />
```

В использовании компилируемых привязок есть ограничения.

### Применение компилируемых привязок в DataGrid

Мы можем использовать CompiledBindings и совместно с DataGrid:

``` csharp
<Window xmlns="https://github.com/avaloniaui" ...
		xmlns:local="using:BvsDesktopLinux.Models"
		x:DataType="vm:MainWindowViewModel"
		x:CompileBindings="True">
...
    <DataGrid AutoGenerateColumns="False" Items="{CompiledBinding Banknotes}"
            SelectedItem="{Binding SelectedBanknote}">
        <DataGrid.Columns>
            <DataGridTextColumn Header="{x:Static p:Resources.NoteId}" 
                                Binding="{CompiledBinding Id}" x:DataType="local:Banknote" />
            <DataGridTextColumn Header="{x:Static p:Resources.NoteCurrency}" 
                                Binding="{CompiledBinding Currency}" x:DataType="local:Banknote" />
            <DataGridTextColumn Header="{x:Static p:Resources.NoteDenomination}" 
                                Binding="{CompiledBinding Denomination}" x:DataType="local:Banknote" />
            <DataGridTextColumn Header="{x:Static p:Resources.Status}" 
                                Binding="{CompiledBinding Status}" x:DataType="local:Banknote" 
                                CellStyleClasses="statusColumn" />
        </DataGrid.Columns>
    </DataGrid>
```

В определении DataGridTextColumn следует заменить слово "Binding" на "CompiledBinding", а также указать класс, в котором определен используемый в привязке атрибут класса: `x:DataType="local:Banknote"`

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

## Вызов потока пользовательского интерфейса

В Avalonia, вызывать некоторый метод в потоке пользовательского интферейса (основной поток) можно следующим вызовом:

``` csharp
Dispatcher.UIThread?.InvokeAsync(() => { 
    this.UpdateWdtStatus(_rollOutDoor); 
});
```

При этом, сам метод абсоютно типовой:

``` csharp
private void UpdateWdtStatus(long rollOutDoor)
{
    if (null != DataContext && DataContext is TestsViewModel)
    {
        switch(rollOutDoor) {
            case 0:
                ((TestsViewModel)DataContext).IsRollOutDoorOpen = false;
                RollOutDoor.Source = DoorClosed;
                break;
            case 1:
                ((TestsViewModel)DataContext).IsRollOutDoorOpen = true;
                RollOutDoor.Source = DoorOpened;
                break;
        }
    }
}
```

Вызов метода главного потока в WPF выглядит схожим образом:

``` csharp
Dispatcher.BeginInvoke((Action)(() => appTabControl.SelectedIndex = 0));
```

Приведённый выше пример функционального эквивалентен более многословному, но более формальному варианту:

``` csharp
onDownloadTaskCompletedCallback fnDownloadCallback;
...
private void InitializeDelegates()
{
    fnDownloadCallback = new onDownloadTaskCompletedCallback(onDownloadTaskCompleted);
}
...
private void onDownloadTaskCompleted(App app, string _ip, string _uin, string _status)
{
    ...
}
...
Application.Current.Dispatcher.BeginInvoke(
    fnDownloadCallback, new object[] { app, IPAddress, UIN, Status });
```

## Различия в наборе компонентов пользовательского интерфейса и их атрибутах

XAML и Avalonia также очень сильно расходятся в наборе атрибутов органов управления; фактически - это два разных набора органов управления.

### Grid

А Avalonia является допустимой сокращённая форма определения количества строк/колонок и их высоты/ширины:

```csharp
<Grid RowDefinitions="Auto,*" ColumnDefinitions="Auto,*,Auto">
```

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

### Всплывающие подсказки (ToolTips)

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

### FontAwesome

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

Добавить эффект "крутилочки" можно благодаря свойству Animation, которое принимает одно из двух значений (Spin, или Pulse):

```xml
<i:Icon Value="fas fa-spinner" FontSize="72" Animation="Spin" />
```

## Стилистическое оформление

В Avalonia используется подход, максимально близкий к CSS из HTML: определяется некоторый стиль, который затем может быть назначен органу управления.

Определить стили (в частности: h1 и h5 для блоков текста) можно, например, так:

``` csharp
<Window.Styles>
    <Style Selector="TextBlock.h1">
        <Setter Property="FontSize" Value="30"/>
        <Setter Property="FontWeight" Value="Bold"/>
        <Setter Property="FontFamily" Value="Arial"/>
    </Style>
    <Style Selector="TextBlock.h5">
        <Setter Property="FontSize" Value="18"/>
        <Setter Property="FontFamily" Value="Arial"/>
    </Style>
    
    <Style Selector="Button">
        <Setter Property="Background" Value="#e6e6e6"/>
    </Style>
    <Style Selector="Grid">
        <Setter Property="Background" Value="#e6e6e6"/>
    </Style>
</Window.Styles>
```

Затем можно применить стили к конкретным органам управления через атрибут **Classes**:

``` csharp
<TextBlock Text="{x:Static prop:Resource.WdtStatus}" Classes="h2" />
```

Следует заметить, что можно определить стили в родительском элементе (**Window**), а использовать - во вложенных дочерних элементах (**UserControl**).

Так же следует заметить, что механизм гораздо более глубокий, чем может показаться на первый взгляд. В частности, он позволяет управлять не только фиксированными свойствами элементов, но и их отображением в определённых состояниях. Например, при наведении мышкой на кнопку, или нажатием пользователем на экранную кнопку. Примеры стилей:

``` csharp
<Style Selector="Button.back:not(:pointerover) /template/ ContentPresenter#PART_ContentPresenter">
    <Setter Property="Background" Value="White"/>
</Style>
```

Можно настраивать стили и отдельным элементам DataGrid:

``` csharp
<Window.Styles>
	<Style Selector="DataGridCell.statusColumn">
		<Setter Property="FontSize" Value="24"/>
		<Setter Property="FontWeight" Value="Bold"/>
		<Setter Property="Background" Value="Green"/>
	</Style>
</Window.Styles>	
...
<DataGrid>
	<DataGrid.Columns>
		<DataGridTextColumn Header="{x:Static p:Resources.Status}" Binding="{Binding Status}" CellStyleClasses="statusColumn"></DataGridTextColumn>
	</DataGrid.Columns>
```

В приведённом выше пример, у всех ячеек колонки статуса будет установлен зелёный фон и bold-текст размером в 24 пункта.

## Убрать Caption / Title bar

Для того, чтобы убрать Caption / Title bar необходимо установить атрибуты главного окна:

```csharp
<Window xmlns="https://github.com/avaloniaui"
        CanResize="False"
        ExtendClientAreaToDecorationsHint="True"
        ExtendClientAreaChromeHints="NoChrome"
        ExtendClientAreaTitleBarHeightHint="0">
```

Но этот вариант не работает в Linux, т.к. Caption - это [декоратор Gnome](https://stackoverflow.com/questions/67474194/avalonia-hiding-titlebar-window-on-linux-does-not-work)

Чтобы работало под Linux, нужно к приведённым выше параметрам выполнить ещё и следующий код:

``` csharp
public MainWindow()
{
    WindowState = WindowState.FullScreen;

    InitializeComponent();
```

## Получить ссылку на главное окно из ViewModel

Сделать это можно следующим образом:

``` csharp
public class MainWindowViewModel : ViewModelBase
{
    public void OnCloseApp()
    {
        // Получаем доступ к главному окну приложения и закрываем приложение через окно
        if (Application.Current != null)
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow.Close();
            }
        }
    }
}
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

## Работа с базой данных SQLite в коде на C\#

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

Определённая команда может использоваться, практически, везде - в меню, кнопках, контекстном меню.

В Avalonia UI команда определяется как часть ViewModel, см. [Binding to Commands](https://docs.avaloniaui.net/docs/data-binding/binding-to-commands#icommandexecute). Пример:

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

В действительности, у Avalonia UI есть некоторая проблема с использованием события (event) PropertyChanged в классе производном от ViewModelBase. Заметим, что автоматически генерируемый по шаблону Avalonia проект содержит класс MainWindowViewModel, производный от ViewModelBase. Проблема состоит в том, что ViewModelBase наследуется ReactiveObject, а в ReactiveObject уже определено событие PropertyChanged, которое имеет модификатор доступа private. Возникает коллизия, которая не нравится компилятору и может иметь побочные эффекты.

Чтобы обойти ограничение, следует использовать вспомогательное зависимое свойство IsBanknoteSelected:

``` csharp
private bool isBanknoteSelected = false;
public bool IsBanknoteSelected {
    get => return isBanknoteSelected;   // Новый стрелочный синтаксис
    set => this.RaiseAndSetIfChanged(ref isBanknoteSelected, value); }
}

private Banknote? selectedBanknote = null;
public Banknote? SelectedBanknote {
    get { return selectedBanknote; }    // Традиционный синтаксис определения setter/getter
    set {
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

[Механизм локализации](https://github.com/Kerminator1973/BVSDesktopSupport/blob/main/i18n.md) работает точно также как и для WPF-приложения в Windows.

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

Печать документов на принтере может быть осуществлена посредством CUPS-драйвера, доступ к которому возможен либо через [команды shell](./printing.md), либо посредством API на C++ (см. `#include <cups/cups.h>`).

## Создание инсталлятора для Linux

Для создания deb-файла использовался package: https://www.nuget.org/packages/dotnet-deb. Официальный сайт проекта: https://github.com/quamotion/dotnet-packaging

Заметим, что проект уже больше года никто не трогал. Он не очень похож на актуальный и живой. Тем не менее, его можно использовать, как минимум, для изучения процесса развертывания приложений в Linux.

Установка инструмента: `dotnet tool install --global dotnet-deb`

Далее, в папке проекта необходимо добавить инструмент в проект (замечу, что при этом csproj-файл не изменился): `dotnet deb install`

Сборка deb-файла осуществляется командой: `dotnet deb`. Путь, по которому сохраняется deb-файл, выводится в консоли. Приблизительный размер deb-файла в Debug-сборке составляет 48.8 Мб, который разворачиваются в ~160 Мб рабочего приложения.

У команды есть несколько опций, описанных на сайте package. Наиболее важные: платформа (например, `--runtime ubuntu.22.04-x64`) и конфигурация (например, `--configuration Release`).

Следует заметить, что в конце 2022 года, Avalonia ещё не поддерживала TrimMode, т.е. удаление из состава дистрибутива не используемых компонентов.

Попытка установки deb-файла может быть выполнена, приложение отображается в списке установленные в "Ubuntu Software", но не появляется в доступных для запуска приложениях. Вероятно, это связано с тем, что ОС трактует его как полученное из недоверенного источника и не использующее Sandbox. Проект установливается в папку `/usr/share/BvsDesktopLinux` и приложение можно запустить, хотя при этом на экран выводится сообщение о том, что не установлен .NET 6.0

Заметим, что установить deb-файл можно используя команду apt-get, например:

``` shell
apt-get install my-app.1.0.0.deb
```

Как вариант, можно рассмотреть возможность модификации deb-файла специализированным скриптом. Современный deb-файл является ar-архивом, в котором может находится файл "control", который может находится внутри другого архива "control.tar" В частности, файл "control", в котором можно указать различную информацию о пакете.
