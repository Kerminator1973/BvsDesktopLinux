# Композиция элементов пользовательского интерфейса

Естественный способ снизить сложность кода отвечающего за пользовательский интерфейс - активно использовать композицию элементов пользовательского интерфейса. Выглядеть композиция может таким образом:

``` xml
<Window xmlns="https://github.com/avaloniaui"
    ...
    xmlns:controls="clr-namespace:approve.Views"
    Title="approve">
    ...
    <StackPanel Orientation="Vertical">
        <controls:PortSelector DeviceName="Купюпроприёмник D210" />
        <controls:PortSelector DeviceName="Принтер Cashino" />
        <controls:PortSelector DeviceName="Модуль спецэлектроники WDT" />
    </StackPanel>
</Window>
```

В приведённом выше примере, в главное окно встраивается три схожих элемента пользовательского интерфейса. Верстка такого компонента может выглядеть следующим образом:

``` xml
<UserControl xmlns="https://github.com/avaloniaui"
             ...
             x:Class="approve.Views.PortSelector">
    <StackPanel Orientation="Horizontal">
        <TextBlock Text="{Binding DeviceName}" />
        <ComboBox Name="FontComboBox" />
        <Button Content="Проверить" />
    </StackPanel>
</UserControl>
```

Связанный программный код:

``` csharp
using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;

namespace approve.Views
{
    public partial class PortSelector : UserControl
    {
        public String DeviceName
        {
            get { return (String)GetValue(DeviceNameProperty); }
            set { SetValue(DeviceNameProperty, value); }
        }

        public PortSelector()
        {
            InitializeComponent();

            DataContext = this;

            var fontComboBox = this.Find<ComboBox>("FontComboBox");
            if (null != fontComboBox)
            {
                fontComboBox.Items = new List<string> { "Порт 1", "Порт 2", "Порт 3", "Порт 4" };
                fontComboBox.SelectedIndex = 0;
            }
        }

        // Регистрируем зависимое свойство, которое можно будет считать и установить в том числе, в родительском органе управления
        public static readonly StyledProperty<String> DeviceNameProperty =
            AvaloniaProperty.Register<MainWindow, String>
            (
                nameof(DeviceName)
            );
    }
}
```

Ключевой частью являются т.н. зависимые свойства, через которые можно передать данные из верстки родительского элемента дочернему элементу. В приведённом выше примере, такое зависимое свойство называется DeviceName:

``` csharp
public String DeviceName
{
    get { return (String)GetValue(DeviceNameProperty); }
    set { SetValue(DeviceNameProperty, value); }
}
```

Чтобы иметь возможность использовать это свойство его необходимо зарегистрировать особенным способом:

``` csharp
public static readonly StyledProperty<String> DeviceNameProperty =
    AvaloniaProperty.Register<MainWindow, String>
    (
        nameof(DeviceName)
    );
```

Эквивалентом **StyledProperty** в WPF является [DependencyProperty](https://github.com/Kerminator1973/BVSDesktopSupport/blob/main/ui_composition.md).

## Если в дочернем элементе не установлен DataContext?

В этом случае, при попытке доступа к DataContext в любых методах, за исключением конструктора, будет получен контекст данных родительского элемента. Такое поведение очень удобно для разработчика, т.к. достаточно определить в родительском элементе объект - контекст данных и доступ к нему из дочерних элементов будет осуществляться без каких-либо дополнительных действий.

Это фундаментально важная особенность поведения Avalonia и WPF: один контекст данных совместно используется несколькими разными объектами, которые используются в качестве композиционной группы (View + UserControls). Хотя, при необходимости, конкретный элемент пользовательского элемента может получить свой собственный контекст данных.

## Контекст данных - связывание со свойствами родительского элемента

В приведённом выше примере,  в дочернем элементе контекс данных установлен на текущий компонент: `DataContext = this;`. Это не является обязательным. Нам, например, может потребоваться использовать несколько органов управления одного типа, но связать атрибуты этих дочерних элементов со свойствами родительского органа управления. В этом случае, следует использовать контекст данных родительского элемента и не устанавливать в дочернем элементе контекст данных (удалив строку `DataContext = this;`).

Использование контекста родительского элемента позволяет обращаться к свойствам родительского элемента через специальный селектор `$parent[UserControl]`.

Допустим, что мы определили следующий родительский элемент:

``` csharp
<Window ...
    xmlns:controls="clr-namespace:ComposeUI.Views">

    <Design.DataContext>
        <vm:MainWindowViewModel/>
    </Design.DataContext>

	<StackPanel>
	    <controls:Child SomeString="{Binding PropertyA}" Background="Red" />
	    <controls:Child SomeString="{Binding PropertyB}" Background="Yellow" />
	    <controls:Child SomeString="{Binding PropertyC}" Background="Green" />
	</StackPanel>
</Window>
```

В контексте данных родительского элемента мы определяем три свойства: PropertyA, PropertyB и PropertyC:

``` csharp
private string _propertyA = "Avalonia";

public string PropertyA
{
    get => _propertyA;
    set => this.RaiseAndSetIfChanged(ref _propertyA, value);
}
```

Соответственно, в дочернем элементе мы должны определить свойство **SomeString** и использовать его в разметке:

``` csharp
public partial class Child : UserControl
{
    public Child()
    {
        InitializeComponent();
    }

    public string SomeString
    {
        get => this.GetValue(SomeStringProperty);
        set => this.SetValue(SomeStringProperty, value);
    }

    public static readonly StyledProperty<string> SomeStringProperty =
        AvaloniaProperty.Register<Child, string>(nameof(SomeString));
}
```

Пример XAML:

``` csharp
<UserControl ...
             x:Class="ComposeUI.Views.Child">
	<Border BorderBrush="Black" BorderThickness="0,0,0,3">
		<TextBlock Text="{Binding $parent[UserControl].SomeString}"
				   Background="{Binding Background}"
				   Padding="10,0,0,0" />
	</Border>
</UserControl>
```

Расшифровать селектор `$parent[UserControl]` можно так: в родительском элементе (parent) взять текущий дочерний UserControl и связать его со свойством **Background**.

## Передать сообытие родительскому элементу

При использовании композиции важно иметь возможность передать событие дочернего элемента родительскому элементу. Предположим, что у нас есть два блока разметки, которые должны сменять друг друга при нажатии некоторых управляющих элементов. Верстка родительского элемента может выглядеть следующим образом:

``` xml
<Grid>
    <controls:ConfigureView Name="ConfigureView" />
    <controls:TestsView Name="TestsView" IsVisible="False" />
</Grid>
```

Дочерний элемент с именем _TestsView_ является невидимым. Допустим, что дочерний элемент _ConfigureView_ содержит кнопку при которой нужно спрятать _ConfigureView_ и отобразить _TestsView_. Разметка дочернего элемента может выглядеть так:

``` xml
<Button Content="Дальше >>" Grid.Column="2" Margin="10" Command="{Binding ConfigurationApprove}" />
```

Обработчик нажатия кнопки проверяет, есть внешние подписчики события, определённого специально для подобной ситуации и, если такие подписчики есть, то вызывает зарегистрированный код:

``` csharp
public partial class ConfigureView : UserControl
{
    // Определяем событие, на которое может подписаться родительский элемент
    public event Action ConfigurationIsApproved;

    public ConfigureView()
    {
        InitializeComponent();

        DataContext = this;
    }

    public void ConfigurationApprove()
    {
        if (null != ConfigurationIsApproved)
        {
            ConfigurationIsApproved();
        }
    }
}
```

Соответственно, родительский элемент может подписаться на это событие и обработать его. Например так:

``` csharp
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        ConfigureView.ConfigurationIsApproved += () =>
        {
            TestsView.IsVisible = true;
            ConfigureView.IsVisible = false;
            TestsView.Focus();
        };
    }
}
```

## Передать сообщение из ViewModel во View

К сожалению, далеко не во всех случаях можно добиться нужно отображения во View лишь изменив зависимые свойства. Например, в случае, при изменении некоторого свойства изменяется композиция из точечных изображений (Bitmap), то получение события может осуществляться во ViewModel, а управлять композицией из кода можно непосредственно во View.

Поскольку получить доступ ко View из ViewModel - противоречит требованиям шаблона MVVM, рекомендуется следующий подход:

- в ModelView определяются делегаты (callback-функции) через которые рассылаются события
- При создании View он получает DataContext и подписывается на события ModelView
- При обработке события, ModelView вызывает callback-функцию и View обрабатывает его в своём методе

Определить callback-функции в ViewModel можно следующим образом:

``` csharp
public partial class MainWindowViewModel : ViewModelBase
{
    public delegate void onWdtUpdateHWVersionCallback(int major, int minor);
    public onWdtUpdateHWVersionCallback? fnWdtUpdateHWVersionCallback;
```

Установка callback-функции во View может выглядеть так:

``` csharp
public partial class ScreenWdtStatus : UserControl
{
    public ScreenWdtStatus()
    {
        InitializeComponent();

        // Подписываемся на системное сообщение об изменении контекста данных
        DataContextChanged += onDataContextChanged;
    }

    private void onDataContextChanged(object? sender, EventArgs e)
    {
        // Подписываемся на события ViewModel, которые необходимо обработать во View
        if (null != this.DataContext && this.DataContext is MainWindowViewModel)
        {
            var context = (MainWindowViewModel)this.DataContext;
            context.fnWdtUpdateHWVersionCallback = 
                new MainWindowViewModel.onWdtUpdateHWVersionCallback(onWdtUpdateHWVersionCallback);
        }
    }
```

Вызов метода подписчика через callback-функцию выполняется так:

``` csharp
if (null != this._fnWdtUpdateHWVersionCallback)
{
    Dispatcher.UIThread?.InvokeAsync(() => {
        this._fnWdtUpdateHWVersionCallback(major, minor);
    });
}
```

В приведённом выше примере, вызов осуществляется из отдельной задачи (потока исполнения), которая была запущена в ModelView. Чтобы избежать ошибок межпоточного взаимодействия, необходимо осуществить вызов, используя InvokeAsync().
